using Xunit;
using do_an.Controllers;
using do_an.Data;
using do_an.Models;
using do_an.Services;
using do_an.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;

namespace do_an.UnitTests.Controllers;

public class AccountControllerTests : IDisposable
{
    private readonly AppDbContext _context;

    public AccountControllerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
        _context.Database.EnsureCreated();
    }

    public void Dispose() => _context.Dispose();

    private AccountController CreateController(string? role = null)
    {
        var controller = new AccountController(_context);
        var httpContext = new DefaultHttpContext();
        httpContext.Session = new FakeSession();

        if (role != null)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, "TestUser"),
                new(ClaimTypes.NameIdentifier, "testuser"),
                new(ClaimTypes.Role, role)
            };
            var identity = new ClaimsIdentity(claims, "Test");
            httpContext.User = new ClaimsPrincipal(identity);
        }

        var authServiceMock = new Mock<IAuthenticationService>();
        authServiceMock
            .Setup(a => a.SignOutAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<AuthenticationProperties>()))
            .Returns(Task.CompletedTask);
        authServiceMock
            .Setup(a => a.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
            .Returns(Task.CompletedTask);

        var urlHelperFactoryMock = new Mock<Microsoft.AspNetCore.Mvc.Routing.IUrlHelperFactory>();
        urlHelperFactoryMock
            .Setup(f => f.GetUrlHelper(It.IsAny<ActionContext>()))
            .Returns(new Mock<IUrlHelper>().Object);

        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock
            .Setup(s => s.GetService(typeof(IAuthenticationService)))
            .Returns(authServiceMock.Object);
        serviceProviderMock
            .Setup(s => s.GetService(typeof(Microsoft.AspNetCore.Mvc.Routing.IUrlHelperFactory)))
            .Returns(urlHelperFactoryMock.Object);

        httpContext.RequestServices = serviceProviderMock.Object;

        controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        controller.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        return controller;
    }

    private class FakeSession : ISession
    {
        private readonly Dictionary<string, byte[]> _storage = new();

        public bool IsAvailable => true;
        public string Id => "FakeSessionId";
        public IEnumerable<string> Keys => _storage.Keys;

        public void Clear() => _storage.Clear();
        public Task CommitAsync(System.Threading.CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task LoadAsync(System.Threading.CancellationToken cancellationToken = default) => Task.CompletedTask;
        public void Remove(string key) => _storage.Remove(key);
        public void Set(string key, byte[] value) => _storage[key] = value;
        public bool TryGetValue(string key, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] out byte[]? value) => _storage.TryGetValue(key, out value);
    }

    // ── Login GET ─────────────────────────────────────────────

    [Fact]
    public void Login_GET_ReturnsView()
    {
        var controller = CreateController();
        var result = controller.Login();
        Assert.IsType<ViewResult>(result);
    }

    // ── Login POST ────────────────────────────────────────────

    [Fact]
    public async Task Login_POST_SaiMatKhau_ReturnsViewWithError()
    {
        // Tao tai khoan
        var tk = new TaiKhoan
        {
            TenDangNhap = "testuser",
            HoTen = "Test User",
            MatKhauHash = do_an.Helpers.PasswordHelper.Hash("correctpass"),
            VaiTro = "KhachHang"
        };
        _context.TaiKhoans.Add(tk);
        await _context.SaveChangesAsync();

        var controller = CreateController();
        var vm = new LoginViewModel { TenDangNhap = "testuser", MatKhau = "wrongpass" };

        var result = await controller.Login(vm);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(controller.ModelState.IsValid);
    }

    [Fact]
    public async Task Login_POST_TenDangNhapKhongTonTai_ReturnsViewWithError()
    {
        var controller = CreateController();
        var vm = new LoginViewModel { TenDangNhap = "nonexistent", MatKhau = "password" };

        var result = await controller.Login(vm);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(controller.ModelState.IsValid);
    }

    // ── Register GET ──────────────────────────────────────────

    [Fact]
    public void Register_GET_ReturnsView()
    {
        var controller = CreateController();
        var result = controller.Register();
        Assert.IsType<ViewResult>(result);
    }

    // ── Register POST ─────────────────────────────────────────

    [Fact]
    public async Task Register_POST_TenDangNhapDaTonTai_ReturnsViewWithError()
    {
        var tk = new TaiKhoan
        {
            TenDangNhap = "existing",
            HoTen = "Existing User",
            MatKhauHash = do_an.Helpers.PasswordHelper.Hash("pass"),
            VaiTro = "KhachHang"
        };
        _context.TaiKhoans.Add(tk);
        await _context.SaveChangesAsync();

        var controller = CreateController();
        var vm = new RegisterViewModel
        {
            TenDangNhap = "existing",
            HoTen = "New User",
            Email = "test@test.com",
            MatKhau = "password123",
            XacNhanMatKhau = "password123"
        };

        var result = await controller.Register(vm);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(controller.ModelState.IsValid);
    }

    // ── Logout ────────────────────────────────────────────────

    [Fact]
    public async Task Logout_RedirectToLogin()
    {
        var controller = CreateController("KhachHang");
        var result = await controller.Logout();
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Login", redirect.ActionName);
    }
}
