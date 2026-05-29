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
public class TaiKhoanController : Controller
{
    private readonly AppDbContext _context;

    public TaiKhoanController(AppDbContext context) => _context = context;

    // GET: /TaiKhoan
    public async Task<IActionResult> Index(string? search, string? vaiTro, string? userName)
    {
        // NhanVien trước đây quản lý riêng, giờ đã gộp chung vào bảng TaiKhoan
        var query = _context.TaiKhoans.AsQueryable();

        if (!string.IsNullOrWhiteSpace(userName))
            query = query.Where(t => t.TenDangNhap == userName);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(t => t.TenDangNhap.Contains(search) ||
                                     (t.HoTen != null && t.HoTen.Contains(search)));
        if (!string.IsNullOrWhiteSpace(vaiTro))
            query = query.Where(t => t.VaiTro == vaiTro);

        if (!User.IsInRole("Admin"))
        {
            query = query.Where(t => t.VaiTro == "KhachHang");
            vaiTro = "KhachHang";
            ViewBag.VaiTroList = new SelectList(new[] { "KhachHang" });
        }
        else
        {
            ViewBag.VaiTroList = new SelectList(new[] { "Admin", "NhanVien", "BacSi", "KhachHang" });
        }

        ViewBag.Search    = search;
        ViewBag.VaiTro    = vaiTro;
        ViewBag.UserName  = userName;

        if (User.IsInRole("Admin"))
        {
            ViewBag.AllTaiKhoans = await _context.TaiKhoans.OrderBy(t => t.TenDangNhap).ToListAsync();
        }
        else
        {
            ViewBag.AllTaiKhoans = await _context.TaiKhoans.Where(t => t.VaiTro == "KhachHang").OrderBy(t => t.TenDangNhap).ToListAsync();
        }

        return View(await query.OrderBy(t => t.VaiTro).ThenBy(t => t.TenDangNhap).ToListAsync());
    }

    // GET: /TaiKhoan/DoiMatKhau/username
    public async Task<IActionResult> DoiMatKhau(string id)
    {
        var tk = await _context.TaiKhoans.FindAsync(id);
        if (tk is null) return NotFound();
        if (!User.IsInRole("Admin") && tk.VaiTro != "KhachHang") return Forbid();
        return View(new DoiMatKhauViewModel { TenDangNhap = id, HoTen = tk.HoTen });
    }

    // POST: /TaiKhoan/DoiMatKhau
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DoiMatKhau(DoiMatKhauViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var tk = await _context.TaiKhoans.FindAsync(vm.TenDangNhap);
        if (tk is null) return NotFound();
        if (!User.IsInRole("Admin") && tk.VaiTro != "KhachHang") return Forbid();

        tk.MatKhauHash = PasswordHelper.Hash(vm.MatKhauMoi);
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Đã đổi mật khẩu cho \"{tk.TenDangNhap}\".";
        return RedirectToAction(nameof(Index));
    }

    // POST: /TaiKhoan/Delete/username
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        var tk = await _context.TaiKhoans.FindAsync(id);
        if (tk is null) return NotFound();
        if (!User.IsInRole("Admin") && tk.VaiTro != "KhachHang") return Forbid();

        // Không xóa Admin cuối cùng
        if (tk.VaiTro == "Admin")
        {
            var soAdmin = await _context.TaiKhoans.CountAsync(t => t.VaiTro == "Admin");
            if (soAdmin <= 1)
            {
                TempData["Error"] = "Không thể xóa tài khoản Admin duy nhất.";
                return RedirectToAction(nameof(Index));
            }
        }

        _context.TaiKhoans.Remove(tk);
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Đã xóa tài khoản \"{id}\".";
        return RedirectToAction(nameof(Index));
    }

    // GET: /TaiKhoan/CreateKhachHang
    public IActionResult CreateKhachHang() => View();

    // POST: /TaiKhoan/CreateKhachHang
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateKhachHang(RegisterViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        if (await _context.TaiKhoans.AnyAsync(t => t.TenDangNhap == vm.TenDangNhap))
        {
            ModelState.AddModelError("TenDangNhap", "Tên đăng nhập đã tồn tại.");
            return View(vm);
        }

        var khach = new TaiKhoan
        {
            TenDangNhap = vm.TenDangNhap,
            HoTen = vm.HoTen,
            Email = vm.Email,
            SoDienThoai = vm.SoDienThoai,
            DiaChi = vm.DiaChi,
            MatKhauHash = PasswordHelper.Hash(vm.MatKhau),
            VaiTro = "KhachHang"
        };

        _context.TaiKhoans.Add(khach);
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Đã tạo khách hàng \"{vm.HoTen}\".";
        return RedirectToAction(nameof(Index), new { vaiTro = "KhachHang" });
    }
}
