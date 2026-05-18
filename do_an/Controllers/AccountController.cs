using do_an.Data;
using do_an.Helpers;
using do_an.Models;
using do_an.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace do_an.Controllers;

public class AccountController : Controller
{
    private readonly AppDbContext _context;

    public AccountController(AppDbContext context) => _context = context;

    // GET: /Account/Login
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectByRole(User.FindFirst(ClaimTypes.Role)?.Value);

        ViewBag.ReturnUrl = returnUrl;
        return View(new LoginViewModel());
    }

    // POST: /Account/Login
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel vm, string? returnUrl = null)
    {
        if (!ModelState.IsValid) return View(vm);

        var taiKhoan = await _context.TaiKhoans
            .FirstOrDefaultAsync(t => t.TenDangNhap == vm.TenDangNhap);

        if (taiKhoan is null || !PasswordHelper.Verify(vm.MatKhau, taiKhoan.MatKhauHash))
        {
            ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không đúng.");
            return View(vm);
        }

        // Auto-upgrade: nếu hash cũ là SHA256
        if (taiKhoan.MatKhauHash.Length == 64 && !taiKhoan.MatKhauHash.StartsWith("$2"))
        {
            taiKhoan.MatKhauHash = PasswordHelper.Hash(vm.MatKhau);
            await _context.SaveChangesAsync();
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, taiKhoan.HoTen ?? taiKhoan.TenDangNhap),
            new(ClaimTypes.NameIdentifier, taiKhoan.TenDangNhap),
            new(ClaimTypes.Role, taiKhoan.VaiTro)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        var properties = new AuthenticationProperties
        {
            IsPersistent = vm.GhiNho,
            ExpiresUtc = vm.GhiNho
                ? DateTimeOffset.UtcNow.AddDays(7)
                : DateTimeOffset.UtcNow.AddHours(8)
        };

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);

        // ── Merge giỏ hàng Session ──> Database khi đăng nhập ──
        var sessionCart = CartHelper.GetCart(HttpContext.Session);
        if (sessionCart.Count > 0)
        {
            var dbCart = await _context.GioHangs
                .Include(g => g.ChiTietGioHangs)
                .FirstOrDefaultAsync(g => g.TenDangNhap == taiKhoan.TenDangNhap);
            
            if (dbCart == null)
            {
                dbCart = new Models.GioHang { TenDangNhap = taiKhoan.TenDangNhap };
                _context.GioHangs.Add(dbCart);
                await _context.SaveChangesAsync();
            }

            foreach (var item in sessionCart)
            {
                var existing = dbCart.ChiTietGioHangs.FirstOrDefault(c => c.SanPhamId == item.SanPhamId);
                if (existing != null)
                {
                    existing.SoLuong = item.SoLuong;
                }
                else
                {
                    dbCart.ChiTietGioHangs.Add(new Models.ChiTietGioHang
                    {
                        SanPhamId = item.SanPhamId,
                        SoLuong = item.SoLuong
                    });
                }
            }
            dbCart.CapNhatCuoi = DateTime.Now;
            await _context.SaveChangesAsync();
        }

        // ── Load giỏ hàng từ DB vào Session (đồng bộ giá mới nhất) ──
        var userDbCart = await _context.GioHangs
            .Include(g => g.ChiTietGioHangs)
            .FirstOrDefaultAsync(g => g.TenDangNhap == taiKhoan.TenDangNhap);

        if (userDbCart != null && userDbCart.ChiTietGioHangs.Count > 0)
        {
            var loadedItems = new List<CartItem>();
            foreach (var ct in userDbCart.ChiTietGioHangs)
            {
                var sp = await _context.SanPhams.FindAsync(ct.SanPhamId);
                if (sp != null && sp.TrenKe && sp.SoLuong > 0)
                {
                    loadedItems.Add(new CartItem
                    {
                        SanPhamId  = ct.SanPhamId,
                        TenSanPham = sp.TenSanPham,
                        GiaBan     = sp.GiaBan,
                        SoLuong    = Math.Min(ct.SoLuong, sp.SoLuong),
                        HinhAnh    = sp.HinhAnhDaiDien
                    });
                }
            }
            CartHelper.SaveCart(HttpContext.Session, loadedItems);
        }
        else
        {
            CartHelper.Clear(HttpContext.Session);
        }

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectByRole(taiKhoan.VaiTro);
    }

    // GET: /Account/Register
    [AllowAnonymous]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectByRole(User.FindFirst(ClaimTypes.Role)?.Value);

        return View(new RegisterViewModel());
    }

    // POST: /Account/Register
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var exists = await _context.TaiKhoans
            .AnyAsync(t => t.TenDangNhap == vm.TenDangNhap);

        if (exists)
        {
            ModelState.AddModelError(nameof(vm.TenDangNhap), "Tên đăng nhập đã tồn tại.");
            return View(vm);
        }

        var taiKhoan = new TaiKhoan
        {
            TenDangNhap = vm.TenDangNhap,
            HoTen = vm.HoTen,
            Email = vm.Email,
            SoDienThoai = vm.SoDienThoai,
            MatKhauHash = PasswordHelper.Hash(vm.MatKhau),
            VaiTro = "KhachHang"
        };

        _context.TaiKhoans.Add(taiKhoan);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Đăng ký thành công! Vui lòng đăng nhập.";
        return RedirectToAction(nameof(Login));
    }

    // POST: /Account/Logout
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        CartHelper.Clear(HttpContext.Session);
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }

    // GET: /Account/AccessDenied
    [AllowAnonymous]
    public IActionResult AccessDenied() => View();

    // GET: /Account/Profile
    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var tk = await _context.TaiKhoans.FindAsync(username);
        if (tk is null) return NotFound();

        var vm = new ProfileViewModel
        {
            TenDangNhap = tk.TenDangNhap,
            VaiTro      = tk.VaiTro,
            HoTen       = tk.HoTen ?? "",
            Email       = tk.Email,
            SoDienThoai = tk.SoDienThoai,
            DiaChi      = tk.DiaChi
        };
        return View(vm);
    }

    // POST: /Account/Profile
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(ProfileViewModel vm)
    {
        // Validate mật khẩu chỉ khi người dùng muốn đổi
        if (!string.IsNullOrEmpty(vm.MatKhauMoi) && string.IsNullOrEmpty(vm.MatKhauHienTai))
            ModelState.AddModelError(nameof(vm.MatKhauHienTai), "Vui lòng nhập mật khẩu hiện tại.");

        if (!ModelState.IsValid) return View(vm);

        var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var tk = await _context.TaiKhoans.FindAsync(username);
        if (tk is null) return NotFound();

        tk.HoTen      = vm.HoTen;
        tk.Email      = vm.Email;
        tk.SoDienThoai = vm.SoDienThoai;
        tk.DiaChi     = vm.DiaChi;

        // Đổi mật khẩu nếu yêu cầu
        if (!string.IsNullOrEmpty(vm.MatKhauMoi))
        {
            if (!PasswordHelper.Verify(vm.MatKhauHienTai!, tk.MatKhauHash))
            {
                ModelState.AddModelError(nameof(vm.MatKhauHienTai), "Mật khẩu hiện tại không đúng.");
                return View(vm);
            }
            tk.MatKhauHash = PasswordHelper.Hash(vm.MatKhauMoi);
        }

        await _context.SaveChangesAsync();
        TempData["Success"] = "Đã cập nhật thông tin cá nhân.";

        return tk.VaiTro == "KhachHang"
            ? RedirectToAction("Index", "Store")
            : RedirectToAction(nameof(Profile));
    }

    // ── Helpers ──────────────────────────────────────────────────────
    private IActionResult RedirectByRole(string? role) => role switch
    {
        "Admin"     => RedirectToAction("Index", "Home"),
        "NhanVien"  => RedirectToAction("Create", "DonHangPos"),
        "KhachHang" => RedirectToAction("Index", "Store"),
        _           => RedirectToAction("Index", "Home")
    };
}
