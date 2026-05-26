using do_an.Data;
using do_an.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace do_an.Controllers;

[Authorize(Roles = "Admin")]
public class NhaCungCapController : Controller
{
    private readonly AppDbContext _context;

    public NhaCungCapController(AppDbContext context) => _context = context;

    // GET: /NhaCungCap
    public async Task<IActionResult> Index(string? search)
    {
        var query = _context.NhaCungCaps.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();
            query = query.Where(n => n.TenNCC.Contains(search) ||
                                     (n.SoDienThoai != null && n.SoDienThoai.Contains(search)));
        }

        ViewBag.Search = search;
        return View(await query.OrderBy(n => n.TenNCC).ToListAsync());
    }

    // GET: /NhaCungCap/Create
    public IActionResult Create() => View(new NhaCungCap());

    // POST: /NhaCungCap/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(NhaCungCap model)
    {
        if (!ModelState.IsValid) return View(model);

        _context.NhaCungCaps.Add(model);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Them nha cung cap thanh cong!";
        return RedirectToAction(nameof(Index));
    }

    // GET: /NhaCungCap/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var ncc = await _context.NhaCungCaps.FindAsync(id);
        if (ncc is null) return NotFound();
        return View(ncc);
    }

    // POST: /NhaCungCap/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, NhaCungCap model)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid) return View(model);

        _context.Entry(model).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        TempData["Success"] = "Cap nhat nha cung cap thanh cong!";
        return RedirectToAction(nameof(Index));
    }

    // POST: /NhaCungCap/ToggleKichHoat/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleKichHoat(int id)
    {
        var ncc = await _context.NhaCungCaps.FindAsync(id);
        if (ncc is null) return NotFound();

        ncc.KichHoat = !ncc.KichHoat;
        await _context.SaveChangesAsync();
        TempData["Success"] = ncc.KichHoat ? "Da kich hoat NCC." : "Da vo hieu hoa NCC.";
        return RedirectToAction(nameof(Index));
    }

    // POST: /NhaCungCap/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var ncc = await _context.NhaCungCaps
            .Include(n => n.PhieuNhapHangs)
            .FirstOrDefaultAsync(n => n.Id == id);
        if (ncc is null) return NotFound();

        if (ncc.PhieuNhapHangs.Any())
        {
            TempData["Error"] = "Khong the xoa nha cung cap da co phieu nhap hang!";
            return RedirectToAction(nameof(Index));
        }

        _context.NhaCungCaps.Remove(ncc);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Da xoa nha cung cap.";
        return RedirectToAction(nameof(Index));
    }
}
