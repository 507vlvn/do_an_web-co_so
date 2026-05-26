using do_an.Data;
using do_an.Helpers;
using do_an.Models;
using do_an.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;

namespace do_an.Controllers;

[Authorize(Roles = "Admin,NhanVien")]
public class PhieuNhapHangController : Controller
{
    private readonly AppDbContext _context;

    public PhieuNhapHangController(AppDbContext context) => _context = context;

    // GET: /PhieuNhapHang
    public async Task<IActionResult> Index(string? search, TrangThaiPhieuNhap? trangThai)
    {
        var query = _context.PhieuNhapHangs
            .Include(p => p.NhaCungCap)
            .Include(p => p.NhanVienNhap)
            .AsQueryable();

        if (trangThai.HasValue)
            query = query.Where(p => p.TrangThai == trangThai);

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();
            query = query.Where(p => p.MaPhieu.Contains(search) ||
                                     p.NhaCungCap.TenNCC.Contains(search));
        }

        ViewBag.Search = search;
        ViewBag.TrangThai = trangThai;
        return View(await query.OrderByDescending(p => p.NgayNhap).ToListAsync());
    }

    // GET: /PhieuNhapHang/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var phieu = await _context.PhieuNhapHangs
            .Include(p => p.NhaCungCap)
            .Include(p => p.NhanVienNhap)
            .Include(p => p.ChiTietPhieuNhaps)
                .ThenInclude(ct => ct.SanPham)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (phieu is null) return NotFound();
        return View(phieu);
    }

    // GET: /PhieuNhapHang/Create
    public async Task<IActionResult> Create()
    {
        await LoadDropdowns();
        return View(new CreatePhieuNhapViewModel
        {
            MaPhieu = $"PN-{DateTime.Now:yyyyMMdd}-{Random.Shared.Next(100, 999)}"
        });
    }

    // POST: /PhieuNhapHang/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreatePhieuNhapViewModel vm)
    {
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        List<ChiTietPhieuNhapInput> items;
        try
        {
            items = JsonSerializer.Deserialize<List<ChiTietPhieuNhapInput>>(vm.ItemsJson, options) ?? [];
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Dữ liệu chi tiết không hợp lệ. Lỗi: {ex.Message}");
            await LoadDropdowns();
            return View(vm);
        }

        if (items.Count == 0)
        {
            ModelState.AddModelError("", "Vui lòng thêm ít nhất 1 sản phẩm vào phiếu nhập.");
            await LoadDropdowns();
            return View(vm);
        }

        // Loại bỏ item chưa chọn sản phẩm (sanPhamId = 0)
        items = items.Where(i => i.SanPhamId > 0).ToList();
        if (items.Count == 0)
        {
            ModelState.AddModelError("", "Vui lòng chọn sản phẩm cho từng dòng.");
            await LoadDropdowns();
            return View(vm);
        }

        // Kiem tra ma phieu trung
        if (await _context.PhieuNhapHangs.AnyAsync(p => p.MaPhieu == vm.MaPhieu))
        {
            ModelState.AddModelError(nameof(vm.MaPhieu), "Mã phiếu đã tồn tại, vui lòng dùng mã khác.");
            await LoadDropdowns();
            return View(vm);
        }

        var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var phieu = new PhieuNhapHang
            {
                MaPhieu = vm.MaPhieu,
                NhaCungCapId = vm.NhaCungCapId,
                NhanVienNhapId = username,
                NgayNhap = DateTime.Now,
                GhiChu = vm.GhiChu,
                TrangThai = TrangThaiPhieuNhap.DaNhap
            };

            foreach (var item in items)
            {
                var sp = await _context.SanPhams.FindAsync(item.SanPhamId);
                if (sp is null)
                {
                    TempData["Error"] = $"San pham ID {item.SanPhamId} khong ton tai.";
                    await transaction.RollbackAsync();
                    await LoadDropdowns();
                    return View(vm);
                }

                // Bắt buộc nhập Ngày SX & Hạn sử dụng đối với thuốc để tạo Lô Thuốc
                if (sp.IsThuoc)
                {
                    if (!item.NgaySX.HasValue || !item.HanSuDung.HasValue)
                    {
                        TempData["Error"] = $"Sản phẩm '{sp.TenSanPham}' là thuốc, bắt buộc phải nhập đầy đủ Ngày sản xuất và Hạn sử dụng.";
                        await transaction.RollbackAsync();
                        await LoadDropdowns();
                        return View(vm);
                    }
                    if (item.HanSuDung.Value <= item.NgaySX.Value)
                    {
                        TempData["Error"] = $"Sản phẩm '{sp.TenSanPham}' có Hạn sử dụng phải sau Ngày sản xuất.";
                        await transaction.RollbackAsync();
                        await LoadDropdowns();
                        return View(vm);
                    }
                }

                // Cập nhật giá trực tiếp đối với sản phẩm không phải thuốc (không quản lý theo lô)
                // Còn đối với thuốc, giá nhập/bán mới sẽ được lưu vào LoThuoc và tự động đồng bộ theo lô đang bán hiện tại
                if (!sp.IsThuoc)
                {
                    sp.GiaNhap = item.GiaNhap;
                    if (item.GiaBan > 0)
                    {
                        sp.GiaBan = item.GiaBan;
                    }
                }

                // Cong ton kho
                sp.SoLuong += item.SoLuong;

                phieu.ChiTietPhieuNhaps.Add(new ChiTietPhieuNhap
                {
                    SanPhamId = item.SanPhamId,
                    SoLuong = item.SoLuong,
                    GiaNhap = item.GiaNhap,
                    MaLo = phieu.MaPhieu, // Tự động lấy mã phiếu làm mã lô
                    NgaySX = item.NgaySX,
                    HanSuDung = item.HanSuDung
                });

                phieu.TongTien += item.GiaNhap * item.SoLuong;
                // Neu la thuoc va co thong tin lo -> tao lo thuoc
                if (sp.IsThuoc && item.NgaySX.HasValue && item.HanSuDung.HasValue)
                {
                    var loThuoc = new LoThuoc
                    {
                        MaLo = phieu.MaPhieu, // Tự động lấy mã phiếu làm mã lô
                        SanPhamId = item.SanPhamId,
                        SoLuongBanDau = item.SoLuong,
                        SoLuongKhaDung = item.SoLuong,
                        NgaySX = item.NgaySX.Value,
                        HanSuDung = item.HanSuDung.Value,
                        GiaNhap = item.GiaNhap, // Lưu giá nhập của lô này
                        GiaBan = item.GiaBan,   // Lưu giá bán của lô này
                        TrangThai = true
                    };
                    _context.LoThuocs.Add(loThuoc);
                }
            }

            _context.PhieuNhapHangs.Add(phieu);
            await _context.SaveChangesAsync();

            // Đồng bộ HSD, NSX, GiaNhap, GiaBan cho san pham theo lo dang ban FEFO (sau khi lô đã được lưu vào DB)
            foreach (var item in items)
            {
                var sp = await _context.SanPhams.FindAsync(item.SanPhamId);
                if (sp is not null && sp.IsThuoc)
                {
                    await KhoHelper.CapNhatHanSuDungTheoLoAsync(_context, sp);
                }
            }
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            TempData["Success"] = $"Tao phieu nhap {phieu.MaPhieu} thanh cong!";
            return RedirectToAction(nameof(Details), new { id = phieu.Id });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            TempData["Error"] = $"Loi khi tao phieu nhap: {ex.Message}";
            await LoadDropdowns();
            return View(vm);
        }
    }

    // POST: /PhieuNhapHang/Huy/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Huy(int id)
    {
        var phieu = await _context.PhieuNhapHangs
            .Include(p => p.ChiTietPhieuNhaps)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (phieu is null) return NotFound();
        if (phieu.TrangThai != TrangThaiPhieuNhap.DaNhap)
        {
            TempData["Error"] = "Chi co phieu da nhap moi co the huy.";
            return RedirectToAction(nameof(Index));
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            foreach (var ct in phieu.ChiTietPhieuNhaps)
            {
                var sp = await _context.SanPhams.FindAsync(ct.SanPhamId);
                if (sp is not null)
                {
                    sp.SoLuong = Math.Max(0, sp.SoLuong - ct.SoLuong);
                }

                // Xoa lo thuoc neu co
                if (!string.IsNullOrWhiteSpace(ct.MaLo))
                {
                    var lo = await _context.LoThuocs
                        .FirstOrDefaultAsync(l => l.SanPhamId == ct.SanPhamId && l.MaLo == ct.MaLo);
                    if (lo is not null)
                    {
                        _context.LoThuocs.Remove(lo);
                    }
                }
            }

            phieu.TrangThai = TrangThaiPhieuNhap.DaHuy;
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            TempData["Success"] = "Da huy phieu nhap va hoan ton kho.";
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            TempData["Error"] = $"Loi khi huy phieu: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task LoadDropdowns()
    {
        ViewBag.NhaCungCaps = await _context.NhaCungCaps
            .Where(n => n.KichHoat)
            .OrderBy(n => n.TenNCC)
            .Select(n => new SelectListItem { Value = n.Id.ToString(), Text = n.TenNCC })
            .ToListAsync();

        ViewBag.SanPhams = await _context.SanPhams
            .Where(s => s.TrenKe || true)
            .OrderBy(s => s.TenSanPham)
            .Select(s => new
            {
                id       = s.Id.ToString(),
                name     = s.TenSanPham,
                giaNhap  = s.GiaNhap,
                giaBan   = s.GiaBan,
                donViTinh = s.DonViTinh,
                isThuoc  = s.IsThuoc
            })
            .ToListAsync();
    }
}
