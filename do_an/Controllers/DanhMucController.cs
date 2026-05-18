using do_an.Data;
using do_an.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace do_an.Controllers;

[Authorize(Roles = "Admin")]
public class DanhMucController : Controller
{
    private readonly AppDbContext _context;

    public DanhMucController(AppDbContext context)
    {
        _context = context;
    }

    // GET: /DanhMuc
    public async Task<IActionResult> Index()
    {
        var list = await _context.DanhMucSanPhams
            .Include(d => d.SanPhams)
            .OrderBy(d => d.TenDanhMuc)
            .ToListAsync();

        ViewBag.DtCountPerDm = new Dictionary<int, int>();

        return View(list);
    }

    // GET: /DanhMuc/Create
    public IActionResult Create()
    {
        return View(new DanhMucSanPham());
    }

    // POST: /DanhMuc/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DanhMucSanPham danhMuc)
    {
        if (!ModelState.IsValid)
            return View(danhMuc);

        var exists = await _context.DanhMucSanPhams
            .AnyAsync(d => d.TenDanhMuc == danhMuc.TenDanhMuc);
        if (exists)
        {
            ModelState.AddModelError(nameof(DanhMucSanPham.TenDanhMuc), "Danh mục này đã tồn tại.");
            return View(danhMuc);
        }

        _context.DanhMucSanPhams.Add(danhMuc);
        await _context.SaveChangesAsync();
        TempData["Success"] = $"Đã thêm danh mục '{danhMuc.TenDanhMuc}' thành công.";
        return RedirectToAction(nameof(Index));
    }

    // GET: /DanhMuc/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var danhMuc = await _context.DanhMucSanPhams.FindAsync(id);
        if (danhMuc is null)
            return NotFound();

        return View(danhMuc);
    }

    // POST: /DanhMuc/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, DanhMucSanPham danhMuc)
    {
        if (id != danhMuc.Id)
            return BadRequest();

        if (!ModelState.IsValid)
            return View(danhMuc);

        var exists = await _context.DanhMucSanPhams
            .AnyAsync(d => d.TenDanhMuc == danhMuc.TenDanhMuc && d.Id != id);
        if (exists)
        {
            ModelState.AddModelError(nameof(DanhMucSanPham.TenDanhMuc), "Danh mục này đã tồn tại.");
            return View(danhMuc);
        }

        _context.Update(danhMuc);
        await _context.SaveChangesAsync();
        TempData["Success"] = $"Đã cập nhật danh mục '{danhMuc.TenDanhMuc}'.";
        return RedirectToAction(nameof(Index));
    }

    // POST: /DanhMuc/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var danhMuc = await _context.DanhMucSanPhams
            .Include(d => d.SanPhams)
            .FirstOrDefaultAsync(d => d.Id == id);
        if (danhMuc is null)
            return NotFound();

        if (danhMuc.SanPhams.Count > 0)
        {
            TempData["Error"] = $"Không thể xóa '{danhMuc.TenDanhMuc}' vì đang có {danhMuc.SanPhams.Count} sản phẩm thuộc danh mục này.";
            return RedirectToAction(nameof(Index));
        }

        _context.DanhMucSanPhams.Remove(danhMuc);
        await _context.SaveChangesAsync();
        TempData["Success"] = $"Đã xóa danh mục '{danhMuc.TenDanhMuc}'.";
        return RedirectToAction(nameof(Index));
    }
}
