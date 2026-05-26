using do_an.Data;
using do_an.Models;
using do_an.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace do_an.Controllers;

[Authorize(Roles = "Admin,NhanVien,BacSi")]
public class DonThuocController : Controller
{
    private readonly AppDbContext _context;

    public DonThuocController(AppDbContext context)
    {
        _context = context;
    }

    // GET: /DonThuoc
    public async Task<IActionResult> Index(string? search, TrangThaiDonThuoc? trangThai, int? sanPhamId)
    {
        var query = _context.DonThuocs
            .Include(d => d.ChiTietDonThuocs)
            .AsQueryable();

        if (trangThai.HasValue)
            query = query.Where(d => d.TrangThai == trangThai);

        if (sanPhamId.HasValue)
            query = query.Where(d => d.ChiTietDonThuocs.Any(c => c.SanPhamId == sanPhamId));

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim().ToLower();
            query = query.Where(d =>
                d.MaDonThuoc.ToLower().Contains(search) ||
                d.TenDonThuoc.ToLower().Contains(search));
        }

        ViewBag.Search = search;
        ViewBag.TrangThai = trangThai;
        ViewBag.SanPhamId = sanPhamId;
        ViewBag.SanPhams = await _context.SanPhams
            .Where(s => s.IsThuoc)
            .OrderBy(s => s.TenSanPham)
            .ToListAsync();

        return View(await query.OrderByDescending(d => d.Id).ToListAsync());
    }

    // GET: /DonThuoc/Create
    public async Task<IActionResult> Create()
    {
        var vm = new CreateDonThuocViewModel();
        await LoadDropdowns();
        return View(vm);
    }

    // POST: /DonThuoc/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateDonThuocViewModel vm)
    {
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        List<ChiTietInputViewModel> items;
        try
        {
            items = JsonSerializer.Deserialize<List<ChiTietInputViewModel>>(vm.ItemsJson, options) ?? [];
        }
        catch { items = []; }

        if (items.Count == 0)
        {
            ModelState.AddModelError("", "Vui lòng thêm ít nhất một thuốc vào đơn.");
            await LoadDropdowns();
            return View(vm);
        }

        var donThuoc = new DonThuoc
        {
            MaDonThuoc = $"DT{DateTime.Now:yyyyMMddHHmmss}",
            TenDonThuoc = vm.TenDonThuoc,
            GhiChu = vm.GhiChu,
            TrangThai = vm.DangNgayLenStore ? TrangThaiDonThuoc.DaDuyet : TrangThaiDonThuoc.MoiTao,
            DaDangStore = vm.DangNgayLenStore
        };

        // Upload hình ảnh
        if (vm.HinhAnhFile is { Length: > 0 })
        {
            var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "donthuoc");
            Directory.CreateDirectory(uploadsDir);

            var ext = Path.GetExtension(vm.HinhAnhFile.FileName);
            var fileName = $"{donThuoc.MaDonThuoc}{ext}";
            var filePath = Path.Combine(uploadsDir, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await vm.HinhAnhFile.CopyToAsync(stream);

            donThuoc.HinhAnh = $"/uploads/donthuoc/{fileName}";
        }

        foreach (var item in items)
        {
            var sp = await _context.SanPhams.FindAsync(item.SanPhamId);
            donThuoc.ChiTietDonThuocs.Add(new ChiTietDonThuoc
            {
                SanPhamId = item.SanPhamId,
                TenSanPhamSnapshot = sp?.TenSanPham,
                SoVienMoiNgay = item.SoVienMoiNgay ?? 1,
                SoNgayUong = item.SoNgayUong ?? 1,
                ThoiDiemUong = item.ThoiDiemUong,
                CachDung = item.CachDung
            });
        }

        _context.DonThuocs.Add(donThuoc);
        await _context.SaveChangesAsync();

        if (donThuoc.DaDangStore)
        {
            await HienThiThuocLenStoreAsync(donThuoc);
        }

        TempData["Success"] = $"Đã tạo đơn thuốc {donThuoc.MaDonThuoc} thành công.";
        return RedirectToAction(nameof(Details), new { id = donThuoc.Id });
    }

    // GET: /DonThuoc/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var dt = await _context.DonThuocs
            .Include(d => d.ChiTietDonThuocs)
                .ThenInclude(c => c.SanPham)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (dt is null) return NotFound();

        return View(dt);
    }

    // POST: /DonThuoc/Duyet/5
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Duyet(int id)
    {
        var dt = await _context.DonThuocs.FindAsync(id);
        if (dt is null) return NotFound();

        if (dt.TrangThai != TrangThaiDonThuoc.MoiTao)
        {
            TempData["Error"] = "Chỉ đơn thuốc 'Mới tạo' mới được duyệt.";
            return RedirectToAction(nameof(Details), new { id });
        }

        dt.TrangThai = TrangThaiDonThuoc.DaDuyet;
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Đơn thuốc {dt.MaDonThuoc} đã được duyệt.";
        return RedirectToAction(nameof(Details), new { id });
    }

    // POST: /DonThuoc/Huy/5
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Huy(int id)
    {
        var dt = await _context.DonThuocs
            .Include(d => d.ChiTietDonThuocs)
            .FirstOrDefaultAsync(d => d.Id == id);
        if (dt is null) return NotFound();

        dt.TrangThai = TrangThaiDonThuoc.DaHuy;
        dt.DaDangStore = false;

        await AnThuocKhiGoStoreAsync(dt);
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Đơn thuốc {dt.MaDonThuoc} đã bị hủy.";
        return RedirectToAction(nameof(Details), new { id });
    }

    // POST: /DonThuoc/DangStore/5
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DangStore(int id)
    {
        var dt = await _context.DonThuocs
            .Include(d => d.ChiTietDonThuocs)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (dt is null) return NotFound();

        if (dt.TrangThai != TrangThaiDonThuoc.DaDuyet)
        {
            TempData["Error"] = "Chỉ đơn thuốc đã duyệt mới được đăng Store.";
            return RedirectToAction(nameof(Details), new { id });
        }

        if (!dt.DaDangStore)
        {
            dt.DaDangStore = true;
            await HienThiThuocLenStoreAsync(dt);
            TempData["Success"] = $"Đơn thuốc {dt.MaDonThuoc} đã được đăng lên Store.";
        }
        else
        {
            dt.DaDangStore = false;
            await AnThuocKhiGoStoreAsync(dt);
            TempData["Success"] = $"Đơn thuốc {dt.MaDonThuoc} đã gỡ khỏi Store.";
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id });
    }

    // GET: /DonThuoc/Delete/5
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var dt = await _context.DonThuocs
            .Include(d => d.ChiTietDonThuocs)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (dt is null) return NotFound();

        await AnThuocKhiGoStoreAsync(dt);

        _context.ChiTietDonThuocs.RemoveRange(dt.ChiTietDonThuocs);
        _context.DonThuocs.Remove(dt);
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Đã xóa đơn thuốc {dt.MaDonThuoc}.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> TimKiemThuoc(string tuKhoa)
    {
        if (string.IsNullOrWhiteSpace(tuKhoa))
            return Json(new List<object>());

        tuKhoa = tuKhoa.Trim().ToLower();
        var results = await _context.SanPhams
            .Include(s => s.DanhMuc)
            .Where(s => s.SoLuong > 0 && s.IsThuoc &&
                        s.TenSanPham.ToLower().Contains(tuKhoa))
            .Take(10)
            .ToListAsync();

        return Json(results.Select(t => new
        {
            Id = t.Id,
            TenThuoc = t.TenSanPham,
            t.GiaBan,
            t.SoLuong,
            LoaiThuoc = t.DanhMuc?.TenDanhMuc ?? "",
            t.CongDung,
            t.ThanhPhan
        }));
    }

    private async Task LoadDropdowns()
    {
        ViewBag.SanPhams = await _context.SanPhams
            .Where(s => s.SoLuong > 0 && s.IsThuoc)
            .OrderBy(s => s.TenSanPham)
            .ToListAsync();

        ViewBag.DanhMucs = await _context.DanhMucSanPhams
            .OrderBy(d => d.TenDanhMuc)
            .ToListAsync();
    }

    private async Task HienThiThuocLenStoreAsync(DonThuoc donThuoc)
    {
        foreach (var ct in donThuoc.ChiTietDonThuocs)
        {
            var existingSp = await _context.SanPhams.FindAsync(ct.SanPhamId);
            if (existingSp is not null && !existingSp.TrenKe)
            {
                existingSp.TrenKe = true;
            }
        }
        await _context.SaveChangesAsync();
    }

    private async Task AnThuocKhiGoStoreAsync(DonThuoc donThuoc)
    {
        var spIds = donThuoc.ChiTietDonThuocs.Select(c => c.SanPhamId).ToList();

        foreach (var spId in spIds)
        {
            bool conDonKhacDung = await _context.ChiTietDonThuocs
                .AnyAsync(ct => ct.SanPhamId == spId
                    && ct.DonThuocId != donThuoc.Id
                    && ct.DonThuoc.DaDangStore
                    && ct.DonThuoc.TrangThai == TrangThaiDonThuoc.DaDuyet);

            if (conDonKhacDung) continue;

            var sp = await _context.SanPhams.FirstOrDefaultAsync(s => s.Id == spId && s.TrenKe);
            if (sp is not null)
            {
                sp.TrenKe = false;
            }
        }
    }
}