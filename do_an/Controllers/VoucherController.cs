using do_an.Data;
using do_an.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace do_an.Controllers;

[Authorize(Roles = "Admin")]
public class VoucherController : Controller
{
    private readonly AppDbContext _context;

    public VoucherController(AppDbContext context) => _context = context;

    public async Task<IActionResult> Index()
        => View(await _context.Vouchers.OrderByDescending(v => v.NgayHetHan).ToListAsync());

    public IActionResult Create() => View(new Voucher
    {
        NgayBatDau = DateTime.Today,
        NgayHetHan = DateTime.Today.AddMonths(1),
        SoLanSuDung = 100,
        DonHangToiThieu = 0
    });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Voucher model)
    {
        ModelState.Remove(nameof(model.ConHieuLuc));
        if (!ModelState.IsValid) return View(model);

        if (model.NgayHetHan <= model.NgayBatDau)
        {
            ModelState.AddModelError(nameof(model.NgayHetHan), "Ngày hết hạn phải sau ngày bắt đầu.");
            return View(model);
        }

        if (model.NgayHetHan < DateTime.Today)
        {
            ModelState.AddModelError(nameof(model.NgayHetHan), "Ngày hết hạn không được ở quá khứ.");
            return View(model);
        }

        model.MaVoucher = model.MaVoucher.ToUpper().Trim();
        _context.Vouchers.Add(model);
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Đã tạo voucher \"{model.MaVoucher}\".";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleKichHoat(int id)
    {
        var v = await _context.Vouchers.FindAsync(id);
        if (v is not null)
        {
            v.KichHoat = !v.KichHoat;
            await _context.SaveChangesAsync();
            TempData["Success"] = v.KichHoat
                ? $"Voucher \"{v.MaVoucher}\" đã được kích hoạt."
                : $"Voucher \"{v.MaVoucher}\" đã bị tắt.";
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var v = await _context.Vouchers.FindAsync(id);
        if (v is not null)
        {
            _context.Vouchers.Remove(v);
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Đã xóa voucher \"{v.MaVoucher}\".";
        }
        return RedirectToAction(nameof(Index));
    }
}
