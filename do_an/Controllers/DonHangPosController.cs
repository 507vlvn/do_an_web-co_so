using do_an.Data;
using do_an.Helpers;
using do_an.Models;
using do_an.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace do_an.Controllers;

[Authorize(Roles = "Admin,NhanVien,BacSi")]
public class DonHangPosController : Controller
{
    private readonly AppDbContext _context;

    public DonHangPosController(AppDbContext context)
    {
        _context = context;
    }

    // GET: /HoaDon
    public async Task<IActionResult> Index(string? search)
    {
        var query = _context.DonHangs
            .Include(d => d.NhanVienBan)
            .Where(d => d.LoaiDon == LoaiDonHang.POS_TaiQuay);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(d => d.MaDonHang.Contains(search) || 
                (d.GhiChu != null && d.GhiChu.Contains(search)));
        }

        ViewBag.Search = search;
        var list = await query.OrderByDescending(d => d.NgayDat).ToListAsync();
        return View(list);
    }

    // GET: /HoaDon/Details/{id}
    public async Task<IActionResult> Details(int id)
    {
        var hoaDon = await _context.DonHangs
            .Include(d => d.NhanVienBan)
            .Include(d => d.ChiTietDonHangs)
                .ThenInclude(c => c.SanPham)
            .FirstOrDefaultAsync(d => d.Id == id && d.LoaiDon == LoaiDonHang.POS_TaiQuay);
            
        if (hoaDon is null) return NotFound();
        return View(hoaDon);
    }

    // GET: /HoaDon/Create
    public async Task<IActionResult> Create()
    {
        await LoadDropdowns();
        return View(new CreateHoaDonViewModel());
    }

    // POST: /HoaDon/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateHoaDonViewModel vm)
    {
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        List<ChiTietInputViewModel> items;
        try
        {
            items = JsonSerializer.Deserialize<List<ChiTietInputViewModel>>(vm.ItemsJson, options) ?? new List<ChiTietInputViewModel>();
        }
        catch
        {
            items = new List<ChiTietInputViewModel>();
        }

        if (items.Count == 0)
        {
            ModelState.AddModelError(string.Empty, "Vui lòng thêm ít nhất một sản phẩm vào hóa đơn.");
            await LoadDropdowns(vm.TaiKhoanId);
            return View(vm);
        }

        var donHangPos = new DonHang
        {
            MaDonHang = $"POS{DateTime.Now:yyyyMMddHHmmss}{Random.Shared.Next(100, 999)}",
            LoaiDon = LoaiDonHang.POS_TaiQuay,
            // Nếu không chọn thì người đang login phụ trách
            NhanVienPhuTrachId = string.IsNullOrEmpty(vm.TaiKhoanId) 
                ? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                : vm.TaiKhoanId,
                
            TrangThai = TrangThaiDonHang.DaGiao, // Đã xong do bán tại quầy
            NgayDat = DateTime.Now,
            NgayThanhToan = DateTime.Now,
            NgayGiao = DateTime.Now,
            PhuongThucThanhToan = PhuongThucThanhToan.ThanhToanTaiQuay,
            HoTenNguoiNhan = "Khách Hàng Lẻ"
        };

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            foreach (var item in items)
            {
                var sp = await _context.SanPhams.FindAsync(item.SanPhamId);
                if (sp == null || sp.SoLuong < item.SoLuong)
                    throw new DbUpdateException($"\"{item.TenSanPham}\" không đủ hàng.");
                
                var truResult = await KhoHelper.TruKhoFEFOAsync(_context, sp.Id, item.SoLuong);
                if (!truResult.Success)
                    throw new DbUpdateException($"\"{item.TenSanPham}\" không đủ kho thực tế.");

                foreach (var split in truResult.CacLoDaTru)
                {
                    donHangPos.ChiTietDonHangs.Add(new ChiTietDonHang
                    {
                        SanPhamId = item.SanPhamId,
                        TenSanPhamSnapshot = string.IsNullOrEmpty(split.MaLo) 
                            ? sp.TenSanPham 
                            : $"{sp.TenSanPham} (Lô: {split.MaLo})",
                        SoVienMoiNgay = item.SoVienMoiNgay,
                        SoNgayUong = item.SoNgayUong,
                        SoLuong = split.SoLuong,
                        GiaBan = split.GiaBan,
                        ThoiDiemUong = item.ThoiDiemUong,
                        CachDung = item.CachDung,
                        GhiChuLieuDung = item.GhiChuLieuDung
                    });
                }
            }

            donHangPos.TongTienHang = donHangPos.ChiTietDonHangs.Sum(c => c.SoLuong * c.GiaBan);
            _context.DonHangs.Add(donHangPos);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            TempData["Success"] = $"Đã tạo hóa đơn POS #{donHangPos.MaDonHang} thành công.";
            return RedirectToAction(nameof(Details), new { id = donHangPos.Id });
        }
        catch (DbUpdateException ex)
        {
            await transaction.RollbackAsync();
            ModelState.AddModelError(string.Empty, ex.Message);
            await LoadDropdowns(vm.TaiKhoanId);
            return View(vm);
        }
    }

    // GET: /HoaDon/TimKiemThuoc?tuKhoa=xxx  (AJAX tìm kiếm thông minh cho POS)
    public async Task<IActionResult> TimKiemThuoc(string tuKhoa)
    {
        if (string.IsNullOrWhiteSpace(tuKhoa))
            return Json(new List<object>());
        
        tuKhoa = tuKhoa.Trim().ToLower();
        var results = await _context.SanPhams
            .Include(s => s.DanhMuc)
            .Where(s => s.SoLuong > 0 && s.TrenKe &&
                        (s.TenSanPham.ToLower().Contains(tuKhoa) || 
                         s.Id.ToString().Contains(tuKhoa)))
            .Take(10)
            .ToListAsync();

        var top10 = results.Select(t => new
        {
            t.Id,
            TenThuoc = t.TenSanPham,
            t.GiaBan,
            t.SoLuong,
            LoaiThuoc = t.DanhMuc?.TenDanhMuc ?? "",
            t.CongDung,
            t.ThanhPhan
        });
        return Json(top10);
    }

    // GET: /HoaDon/GetThuocInfo?id=xxx  (JSON API cho JavaScript)
    public async Task<IActionResult> GetThuocInfo(int id)
    {
        var thuoc = await _context.SanPhams.FindAsync(id);
        if (thuoc is null || !thuoc.TrenKe) return Json(new { error = "Không tìm thấy sản phẩm" });
        return Json(new
        {
            Id = thuoc.Id,
            TenThuoc = thuoc.TenSanPham,
            thuoc.GiaBan,
            thuoc.SoLuong
        });
    }

    // GET: /HoaDon/Delete/{id}
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var donHang = await _context.DonHangs
            .Include(d => d.ChiTietDonHangs)
            .Include(d => d.NhanVienBan)
            .FirstOrDefaultAsync(m => m.Id == id && m.LoaiDon == LoaiDonHang.POS_TaiQuay);
        
        if (donHang == null) return NotFound();

        return View(donHang);
    }

    // POST: /HoaDon/Delete/{id}
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, bool hoanKho = true)
    {
        var donHangPos = await _context.DonHangs
            .Include(d => d.ChiTietDonHangs)
            .FirstOrDefaultAsync(d => d.Id == id && d.LoaiDon == LoaiDonHang.POS_TaiQuay);

        if (donHangPos == null) return NotFound();

        if (hoanKho)
        {
            // Hoàn kho
            foreach (var ct in donHangPos.ChiTietDonHangs)
            {
                if (ct.SanPhamId.HasValue)
                {
                    await KhoHelper.HoanKhoFEFOAsync(_context, ct.SanPhamId.Value, ct.SoLuong);
                }
            }
        }
        
        // Xóa vĩnh viễn khỏi CSDL
        _context.DonHangs.Remove(donHangPos);
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Đã xóa hóa đơn POS #{donHangPos.MaDonHang} thành công {(hoanKho ? "(Có hoàn kho)" : "(Không hoàn kho)")}.";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadDropdowns(string? selectedTaiKhoanId = null)
    {
        ViewBag.NhanViens = await _context.TaiKhoans
            .Where(t => t.VaiTro == "Admin" || t.VaiTro == "NhanVien" || t.VaiTro == "BacSi")
            .ToListAsync();

        ViewBag.SanPhams = await _context.SanPhams
            .Where(s => s.SoLuong > 0)
            .OrderBy(s => s.TenSanPham)
            .ToListAsync();

        ViewBag.SelectedTaiKhoanId = selectedTaiKhoanId;
    }
}
