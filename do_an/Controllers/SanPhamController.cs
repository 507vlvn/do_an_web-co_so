using do_an.Data;
using do_an.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace do_an.Controllers;

[Authorize(Roles = "Admin,NhanVien")]
public class SanPhamController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;

    public SanPhamController(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    // GET: /SanPham
    public async Task<IActionResult> Index(string? search, int? danhMucId)
    {
        var query = _context.SanPhams
            .Include(s => s.DanhMuc)
            .Include(s => s.DanhSachLo)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();
            query = query.Where(s => s.TenSanPham.Contains(search));
        }
        if (danhMucId.HasValue)
            query = query.Where(s => s.DanhMucId == danhMucId);

        ViewBag.Search      = search;
        ViewBag.DanhMucId   = danhMucId;
        ViewBag.DanhMucs    = await _context.DanhMucSanPhams.OrderBy(d => d.TenDanhMuc).ToListAsync();
        ViewBag.IsAdmin     = User.IsInRole("Admin");

        return View(await query.OrderByDescending(s => s.NgayThem).ToListAsync());
    }

    // GET: /SanPham/Create
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create()
    {
        await LoadDanhMucSelectList();
        return View(new SanPham());
    }

    // POST: /SanPham/Create
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SanPham model, IFormFile? anhFile, List<IFormFile>? thuVienFiles)
    {
        ModelState.Remove(nameof(model.DanhMuc));
        ModelState.Remove(nameof(model.HinhAnhFile));
        ModelState.Remove(nameof(model.HinhAnhUrl));

        // Validation HSD cho sản phẩm không phải thuốc
        if (!model.IsThuoc && model.HanSuDung.HasValue)
        {
            if (model.HanSuDung.Value <= DateTime.Today)
                ModelState.AddModelError(nameof(model.HanSuDung), "Hạn sử dụng phải trong tương lai.");
            if (model.NgaySX.HasValue && model.HanSuDung.Value <= model.NgaySX.Value)
                ModelState.AddModelError(nameof(model.HanSuDung), "Hạn sử dụng phải sau ngày sản xuất.");
        }

        if (!ModelState.IsValid)
        {
            await LoadDanhMucSelectList(model.DanhMucId);
            return View(model);
        }

        try
        {
            model.HinhAnhFile = await LuuAnh(anhFile);
            if (thuVienFiles != null && thuVienFiles.Count > 0)
            {
                var thuVienPaths = new List<string>();
                foreach (var file in thuVienFiles)
                {
                    var p = await LuuAnh(file);
                    if (p != null) thuVienPaths.Add(p);
                }
                if (thuVienPaths.Count > 0)
                {
                    model.ThuVienHinhAnh = string.Join(";", thuVienPaths);
                }
            }
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("HinhAnhFile", ex.Message);
            await LoadDanhMucSelectList(model.DanhMucId);
            return View(model);
        }
        
        model.NgayThem = DateTime.Now;

        // Nếu là thuốc → HSD/NgaySX do Lô Thuốc quản lý, không nhập thủ công
        if (model.IsThuoc)
        {
            model.NgaySX = null;
            model.HanSuDung = null;
        }

        _context.SanPhams.Add(model);
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Đã thêm sản phẩm \"{model.TenSanPham}\"";
        return RedirectToAction(nameof(Index));
    }

    // GET: /SanPham/Edit/5
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var sp = await _context.SanPhams.FindAsync(id);
        if (sp is null) return NotFound();
        await LoadDanhMucSelectList(sp.DanhMucId);
        return View(sp);
    }

    // POST: /SanPham/Edit/5
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SanPham model, IFormFile? anhFile, List<IFormFile>? thuVienFiles)
    {
        if (id != model.Id) return BadRequest();

        ModelState.Remove(nameof(model.DanhMuc));
        ModelState.Remove(nameof(model.HinhAnhFile));

        // Validation HSD cho sản phẩm không phải thuốc
        if (!model.IsThuoc && model.HanSuDung.HasValue)
        {
            if (model.HanSuDung.Value <= DateTime.Today)
                ModelState.AddModelError(nameof(model.HanSuDung), "Hạn sử dụng phải trong tương lai.");
            if (model.NgaySX.HasValue && model.HanSuDung.Value <= model.NgaySX.Value)
                ModelState.AddModelError(nameof(model.HanSuDung), "Hạn sử dụng phải sau ngày sản xuất.");
        }

        if (!ModelState.IsValid)
        {
            await LoadDanhMucSelectList(model.DanhMucId);
            return View(model);
        }

        var existing = await _context.SanPhams.FindAsync(id);
        if (existing is null) return NotFound();

        existing.TenSanPham  = model.TenSanPham;
        existing.DanhMucId   = model.DanhMucId;
        existing.IsThuoc     = model.IsThuoc;
        existing.GiaNhap     = model.GiaNhap;
        existing.GiaBan      = model.GiaBan;
        existing.GiaGoc      = model.GiaGoc;
        existing.DonViTinh   = model.DonViTinh;
        existing.MoTa        = model.MoTa;
        existing.CongDung    = model.CongDung;
        existing.ThanhPhan   = model.ThanhPhan;
        existing.TacDungPhu  = model.TacDungPhu;
        existing.NhaSanXuat  = model.NhaSanXuat;

        // Nếu là thuốc → HSD/NgaySX do Lô Thuốc quản lý, không ghi đè
        if (!model.IsThuoc)
        {
            existing.NgaySX      = model.NgaySX;
            existing.HanSuDung   = model.HanSuDung;
        }
        
        existing.SoLuong     = model.SoLuong;
        existing.TrenKe      = model.TrenKe;
        existing.NoiBat      = model.NoiBat;
        existing.HinhAnhUrl  = model.HinhAnhUrl;

        try
        {
            var newFile = await LuuAnh(anhFile);
            if (newFile is not null) existing.HinhAnhFile = newFile;

            if (thuVienFiles != null && thuVienFiles.Count > 0)
            {
                var thuVienPaths = new List<string>();
                if (!string.IsNullOrEmpty(existing.ThuVienHinhAnh))
                {
                    thuVienPaths.AddRange(existing.ThuVienHinhAnh.Split(';', StringSplitOptions.RemoveEmptyEntries));
                }
                foreach (var file in thuVienFiles)
                {
                    var p = await LuuAnh(file);
                    if (p != null) thuVienPaths.Add(p);
                }
                existing.ThuVienHinhAnh = string.Join(";", thuVienPaths);
            }
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("HinhAnhFile", ex.Message);
            await LoadDanhMucSelectList(model.DanhMucId);
            return View(model);
        }

        await _context.SaveChangesAsync();
        TempData["Success"] = "Đã cập nhật sản phẩm.";
        return RedirectToAction(nameof(Index));
    }

    // POST: /SanPham/Delete/5
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var sp = await _context.SanPhams.FindAsync(id);
        if (sp is not null)
        {
            _context.SanPhams.Remove(sp);
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Đã xóa vĩnh viễn sản phẩm \"{sp.TenSanPham}\".";
        }
        return RedirectToAction(nameof(Index));
    }

    // POST: /SanPham/NgungSanXuat/5 — Ngưng sản xuất: ẩn khỏi cửa hàng, giữ trong danh sách quản lý
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> NgungSanXuat(int id)
    {
        var sp = await _context.SanPhams.FindAsync(id);
        if (sp is null) return NotFound();

        // Hạ kệ
        sp.TrenKe = false;
        sp.SoLuong = 0;

        // Vô hiệu tất cả lô thuốc
        var cacLo = await _context.LoThuocs.Where(l => l.SanPhamId == id && l.TrangThai).ToListAsync();
        foreach (var lo in cacLo)
        {
            lo.SoLuongKhaDung = 0;
            lo.TrangThai = false;
        }

        await _context.SaveChangesAsync();
        TempData["Success"] = $"Đã chuyển trạng thái \"{sp.TenSanPham}\" sang Ngưng sản xuất.";
        return RedirectToAction(nameof(Index));
    }

    // POST: /SanPham/ToggleTrenKe/5
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleTrenKe(int id)
    {
        var sp = await _context.SanPhams.FindAsync(id);
        if (sp is not null)
        {
            sp.TrenKe = !sp.TrenKe;
            await _context.SaveChangesAsync();
            TempData["Success"] = sp.TrenKe
                ? $"\"{sp.TenSanPham}\" đã được đưa lên kệ."
                : $"\"{sp.TenSanPham}\" đã được hạ xuống.";
        }
        return RedirectToAction(nameof(Index));
    }

    // ── Helpers ──────────────────────────────────────────────────
    private async Task LoadDanhMucSelectList(int? selected = null)
    {
        var list = await _context.DanhMucSanPhams
            .Where(d => d.KichHoat)
            .OrderBy(d => d.TenDanhMuc)
            .ToListAsync();
        ViewBag.DanhMucList = new SelectList(list, "Id", "TenDanhMuc", selected);
    }

    private async Task<string?> LuuAnh(IFormFile? file)
    {
        if (file is null || file.Length == 0) return null;

        var maxSize = 5 * 1024 * 1024;
        if (file.Length > maxSize)
            throw new InvalidOperationException("Dung lượng ảnh vượt quá 5MB.");

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(ext))
            throw new InvalidOperationException("Chỉ được phép tải lên file ảnh (.jpg, .jpeg, .png, .gif, .webp).");

        var folder = Path.Combine(_env.WebRootPath, "images", "sanpham");
        Directory.CreateDirectory(folder);

        var fileName = $"{Guid.NewGuid()}{ext}";
        var path = Path.Combine(folder, fileName);

        await using var stream = System.IO.File.Create(path);
        await file.CopyToAsync(stream);

        return $"/images/sanpham/{fileName}";
    }
}