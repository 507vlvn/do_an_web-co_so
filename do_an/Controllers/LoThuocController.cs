using do_an.Data;
using do_an.Helpers;
using do_an.Models;
using do_an.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace do_an.Controllers;

[Authorize(Roles = "Admin,NhanVien")]
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

    // GET: /LoThuoc/NhapLo
    public async Task<IActionResult> NhapLo(int? sanPhamId)
    {
        await LoadSanPhamList(sanPhamId);

        var vm = new NhapLoThuocViewModel();
        if (sanPhamId.HasValue)
            vm.SanPhamId = sanPhamId.Value;

        // Gợi ý mã lô tự động
        vm.MaLo = $"LO-{DateTime.Now:yyyyMMdd}-{Random.Shared.Next(100, 999)}";

        return View(vm);
    }

    // POST: /LoThuoc/NhapLo
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> NhapLo(NhapLoThuocViewModel vm)
    {
        if (vm.HanSuDung <= vm.NgaySX)
            ModelState.AddModelError(nameof(vm.HanSuDung), "Hạn sử dụng phải sau ngày sản xuất.");

        if (vm.HanSuDung <= DateTime.Today)
            ModelState.AddModelError(nameof(vm.HanSuDung), "Hạn sử dụng phải trong tương lai.");

        // Kiểm tra mã lô đã tồn tại cho thuốc này chưa
        bool trungMaLo = await _context.LoThuocs
            .AnyAsync(l => l.SanPhamId == vm.SanPhamId && l.MaLo == vm.MaLo);
        if (trungMaLo)
            ModelState.AddModelError(nameof(vm.MaLo), "Mã lô này đã tồn tại cho thuốc đã chọn.");

        if (!ModelState.IsValid)
        {
            await LoadSanPhamList(vm.SanPhamId);
            return View(vm);
        }

        var sp = await _context.SanPhams.FindAsync(vm.SanPhamId);
        if (sp is null) return NotFound();

        // Tạo lô mới
        var loMoi = new LoThuoc
        {
            MaLo           = vm.MaLo,
            SanPhamId      = vm.SanPhamId,
            SoLuongBanDau  = vm.SoLuongNhap,
            SoLuongKhaDung = vm.SoLuongNhap,
            NgaySX         = vm.NgaySX,
            HanSuDung      = vm.HanSuDung,
            TrangThai      = true
        };

        // Cộng thêm vào tổng tồn kho của sản phẩm
        sp.SoLuong += vm.SoLuongNhap;

        _context.LoThuocs.Add(loMoi);
        await _context.SaveChangesAsync();

        // Đồng bộ HSD/NgaySX theo lô FEFO hiện tại (sau khi lô mới đã được lưu vào DB)
        await KhoHelper.CapNhatHanSuDungTheoLoAsync(_context, sp);
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Đã nhập lô \"{vm.MaLo}\" ({vm.SoLuongNhap} {sp.DonViTinh}) cho thuốc \"{sp.TenSanPham}\".";
        return RedirectToAction(nameof(Index), new { sanPhamId = vm.SanPhamId });
    }

    // GET: /LoThuoc/Edit/5
    [Authorize(Roles = "Admin,NhanVien")]
    public async Task<IActionResult> Edit(int id)
    {
        var lo = await _context.LoThuocs
            .Include(l => l.SanPham)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (lo is null) return NotFound();
        return View(lo);
    }

    // POST: /LoThuoc/Edit/5
    [Authorize(Roles = "Admin,NhanVien")]
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
    private async Task LoadSanPhamList(int? selected = null)
    {
        var list = await _context.SanPhams
            .Include(s => s.DanhMuc)
            .OrderBy(s => s.DanhMuc.TenDanhMuc)
            .ThenBy(s => s.TenSanPham)
            .ToListAsync();

        // Group theo danh mục cho đẹp
        var grouped = list
            .GroupBy(s => s.DanhMuc?.TenDanhMuc ?? "Khác")
            .Select(g => new SelectListGroup { Name = g.Key })
            .ToList();

        var items = list.Select(s => new SelectListItem
        {
            Value = s.Id.ToString(),
            Text  = $"{s.TenSanPham} (Tồn: {s.SoLuong} {s.DonViTinh})",
            Group = new SelectListGroup { Name = s.DanhMuc?.TenDanhMuc ?? "Khác" },
            Selected = s.Id.ToString() == selected?.ToString()
        }).ToList();

        ViewBag.SanPhamList = items;
    }
}
