using do_an.Data;
using do_an.Helpers;
using do_an.Models;
using do_an.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace do_an.Controllers;

[Authorize(Roles = "Admin,NhanVien,BacSi")]
public class LoThuocController : Controller
{
    private readonly AppDbContext _context;

    public LoThuocController(AppDbContext context) => _context = context;

    // GET: /LoThuoc?sanPhamId=5
    public async Task<IActionResult> Index(int? sanPhamId, string? search)
    {
        var query = _context.LoThuocs
            .Include(l => l.SanPham)
            .AsQueryable();

        if (sanPhamId.HasValue)
            query = query.Where(l => l.SanPhamId == sanPhamId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();
            query = query.Where(l => l.MaLo.Contains(search) || l.SanPham.TenSanPham.Contains(search));
        }

        ViewBag.Search    = search;
        ViewBag.SanPhamId = sanPhamId;
        ViewBag.SanPhams  = await _context.SanPhams
            .OrderBy(s => s.TenSanPham)
            .ToListAsync();

        var list = await query
            .OrderBy(l => l.SanPham.TenSanPham)
            .ThenBy(l => l.HanSuDung)
            .ToListAsync();

        return View(list);
    }

    // GET: /LoThuoc/Edit/5
    [Authorize(Roles = "Admin,NhanVien,BacSi")]
    public async Task<IActionResult> Edit(int id)
    {
        var lo = await _context.LoThuocs
            .Include(l => l.SanPham)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (lo is null) return NotFound();
        return View(lo);
    }

    // POST: /LoThuoc/Edit/5
    [Authorize(Roles = "Admin,NhanVien,BacSi")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, LoThuoc model)
    {
        if (id != model.Id) return BadRequest();

        ModelState.Remove(nameof(model.SanPham));

        if (model.HanSuDung <= model.NgaySX)
            ModelState.AddModelError(nameof(model.HanSuDung), "Hạn sử dụng phải sau ngày sản xuất.");

        if (model.HanSuDung <= DateTime.Today)
            ModelState.AddModelError(nameof(model.HanSuDung), "Hạn sử dụng phải trong tương lai.");

        if (model.SoLuongKhaDung > model.SoLuongBanDau)
            ModelState.AddModelError(nameof(model.SoLuongKhaDung), "Số lượng khả dụng không thể vượt số lượng ban đầu.");

        if (!ModelState.IsValid)
        {
            model.SanPham = (await _context.SanPhams.FindAsync(model.SanPhamId))!;
            return View(model);
        }

        var lo = await _context.LoThuocs.FindAsync(id);
        if (lo is null) return NotFound();

        // Điều chỉnh tổng tồn kho theo delta
        int delta = model.SoLuongKhaDung - lo.SoLuongKhaDung;
        var sp = await _context.SanPhams.FindAsync(lo.SanPhamId);
        if (delta != 0 && sp is not null)
        {
            sp.SoLuong += delta;
        }

        lo.MaLo           = model.MaLo;
        lo.SoLuongBanDau  = model.SoLuongBanDau;
        lo.SoLuongKhaDung = model.SoLuongKhaDung;
        lo.NgaySX         = model.NgaySX;
        lo.HanSuDung      = model.HanSuDung;
        lo.GiaNhap        = model.GiaNhap;
        lo.GiaBan         = model.GiaBan;
        lo.TrangThai      = model.TrangThai;

        await _context.SaveChangesAsync();

        // Đồng bộ HSD/NgaySX sản phẩm theo lô FEFO hiện tại
        if (sp is not null)
        {
            await KhoHelper.CapNhatHanSuDungTheoLoAsync(_context, sp);
            await _context.SaveChangesAsync();
        }

        TempData["Success"] = $"Đã cập nhật lô \"{lo.MaLo}\".";
        return RedirectToAction(nameof(Index), new { sanPhamId = lo.SanPhamId });
    }

    // POST: /LoThuoc/VoHieu/5  (Admin only — vô hiệu/kích hoạt lại lô)
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleTrangThai(int id)
    {
        var lo = await _context.LoThuocs.FindAsync(id);
        if (lo is null) return NotFound();

        lo.TrangThai = !lo.TrangThai;

        // Điều chỉnh tồn kho sản phẩm khi vô hiệu/kích hoạt lô
        var sp = await _context.SanPhams.FindAsync(lo.SanPhamId);
        if (sp is not null)
        {
            if (!lo.TrangThai) sp.SoLuong -= lo.SoLuongKhaDung; // vô hiệu → bớt
            else               sp.SoLuong += lo.SoLuongKhaDung; // kích hoạt lại → thêm
        }

        await _context.SaveChangesAsync();
        TempData["Success"] = lo.TrangThai
            ? $"Lô \"{lo.MaLo}\" đã được kích hoạt lại."
            : $"Lô \"{lo.MaLo}\" đã bị vô hiệu hóa.";

        return RedirectToAction(nameof(Index), new { sanPhamId = lo.SanPhamId });
    }

    // POST: /LoThuoc/TieuHuy/5 — Tiêu hủy lô thuốc (hết hạn hoặc kém chất lượng)
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TieuHuy(int id)
    {
        var lo = await _context.LoThuocs.FindAsync(id);
        if (lo is null) return NotFound();

        var sp = await _context.SanPhams.FindAsync(lo.SanPhamId);

        // Trừ số lượng khả dụng khỏi tổng tồn kho
        if (sp is not null && lo.SoLuongKhaDung > 0)
        {
            sp.SoLuong -= lo.SoLuongKhaDung;
            if (sp.SoLuong < 0) sp.SoLuong = 0;
        }

        int soLuongTieuHuy = lo.SoLuongKhaDung;
        lo.SoLuongKhaDung = 0;
        lo.TrangThai = false;

        await _context.SaveChangesAsync();

        // Đồng bộ HSD sản phẩm
        if (sp is not null)
        {
            await KhoHelper.CapNhatHanSuDungTheoLoAsync(_context, sp);
            await _context.SaveChangesAsync();
        }

        TempData["Success"] = $"Đã tiêu hủy lô \"{lo.MaLo}\" ({soLuongTieuHuy} sản phẩm). Tồn kho đã được điều chỉnh.";
        return RedirectToAction(nameof(Index), new { sanPhamId = lo.SanPhamId });
    }

    // GET: /LoThuoc/GetThuocInfo?sanPhamId=5  (AJAX)
    public async Task<IActionResult> GetThuocInfo(int sanPhamId)
    {
        var sp = await _context.SanPhams.FindAsync(sanPhamId);
        if (sp is null) return Json(null);

        var loThuocs = await _context.LoThuocs
            .Where(l => l.SanPhamId == sanPhamId && l.TrangThai)
            .OrderBy(l => l.HanSuDung)
            .Select(l => new { l.MaLo, l.SoLuongKhaDung, HanSuDung = l.HanSuDung.ToString("dd/MM/yyyy") })
            .ToListAsync();

        return Json(new
        {
            sp.Id,
            sp.TenSanPham,
            sp.SoLuong,
            sp.DonViTinh,
            LoThuocs = loThuocs
        });
    }

    // POST: /LoThuoc/DongBoHanSuDung — Đồng bộ HSD tất cả sản phẩm theo lô FEFO hiện tại
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DongBoHanSuDung()
    {
        int soLuongCapNhat = await Helpers.KhoHelper.DongBoTatCaHanSuDungAsync(_context);
        TempData["Success"] = $"Đã đồng bộ hạn sử dụng cho {soLuongCapNhat} sản phẩm theo lô FEFO hiện tại.";
        return RedirectToAction(nameof(Index));
    }


    // ── Helpers ──────────────────────────────────────────────────
}
