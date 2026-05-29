using do_an.Data;
using do_an.Helpers;
using do_an.Models;
using do_an.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace do_an.Controllers;

[Authorize(Roles = "Admin")]
// logic : add nhan vien chi co Admin moi co the add 
public class NhanVienController : Controller
{
    private readonly AppDbContext _context;

    public NhanVienController(AppDbContext context) => _context = context;

    // GET: /NhanVien/Create
    public IActionResult Create() => View(new NhanVienCreateViewModel());

    // POST: /NhanVien/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(NhanVienCreateViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        // Kiểm tra trùng tên đăng nhập
        if (await _context.TaiKhoans.AnyAsync(t => t.TenDangNhap == vm.TenDangNhap))
        {
            ModelState.AddModelError(nameof(vm.TenDangNhap), "Tên đăng nhập đã tồn tại.");
            return View(vm);
        }

        var taiKhoan = new TaiKhoan
        {
            TenDangNhap = vm.TenDangNhap,
            HoTen = vm.HoTen,
            Email = vm.Email,
            MatKhauHash = PasswordHelper.Hash(vm.MatKhau),
            VaiTro = vm.VaiTro,
            BoPhan = vm.BoPhan,
            HeSoLuong = vm.HeSoLuong,
            LuongTheoGio = vm.LuongTheoGio
        };

        _context.TaiKhoans.Add(taiKhoan);
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Đã thêm nhân viên \"{vm.HoTen}\" thành công.";
        return RedirectToAction("Index", "TaiKhoan", new { vaiTro = "NhanVien" });
    }
}
