using Xunit;
using do_an.Controllers;
using do_an.Data;
using do_an.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;

namespace do_an.UnitTests.Controllers;

public class SanPhamControllerTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _env;

    public SanPhamControllerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
        _context.Database.EnsureCreated();

        // Mock IWebHostEnvironment
        _env = Mock.Of<Microsoft.AspNetCore.Hosting.IWebHostEnvironment>(
            e => e.WebRootPath == Path.GetTempPath());
    }

    public void Dispose() => _context.Dispose();

    private SanPhamController CreateController(string role = "Admin")
    {
        var controller = new SanPhamController(_context, _env);
        var httpContext = new DefaultHttpContext();
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, "Admin"),
            new(ClaimTypes.Role, role)
        };
        var identity = new ClaimsIdentity(claims, "Test");
        httpContext.User = new ClaimsPrincipal(identity);
        controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
        controller.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        return controller;
    }

    // ── Index ─────────────────────────────────────────────────

    [Fact]
    public async Task Index_ReturnsViewWithSanPhams()
    {
        var dm = new DanhMucSanPham { TenDanhMuc = "Test", KichHoat = true };
        _context.DanhMucSanPhams.Add(dm);
        await _context.SaveChangesAsync();

        _context.SanPhams.Add(new SanPham { TenSanPham = "SP1", DanhMucId = dm.Id, GiaBan = 10000 });
        _context.SanPhams.Add(new SanPham { TenSanPham = "SP2", DanhMucId = dm.Id, GiaBan = 20000 });
        await _context.SaveChangesAsync();

        var controller = CreateController();
        var result = await controller.Index(null, null, null);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<SanPham>>(viewResult.Model);
        Assert.Equal(2, model.Count);
    }

    [Fact]
    public async Task Index_TimKiemTheoTen()
    {
        var dm = new DanhMucSanPham { TenDanhMuc = "Test", KichHoat = true };
        _context.DanhMucSanPhams.Add(dm);
        await _context.SaveChangesAsync();

        _context.SanPhams.Add(new SanPham { TenSanPham = "Paracetamol", DanhMucId = dm.Id, GiaBan = 10000 });
        _context.SanPhams.Add(new SanPham { TenSanPham = "Aspirin", DanhMucId = dm.Id, GiaBan = 20000 });
        await _context.SaveChangesAsync();

        var controller = CreateController();
        var result = await controller.Index("Para", null, null);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<SanPham>>(viewResult.Model);
        Assert.Single(model);
        Assert.Contains("Para", model[0].TenSanPham);
    }

    [Fact]
    public async Task Index_LocTheoDanhMuc()
    {
        var dm1 = new DanhMucSanPham { TenDanhMuc = "Thuoc", KichHoat = true };
        var dm2 = new DanhMucSanPham { TenDanhMuc = "MyPham", KichHoat = true };
        _context.DanhMucSanPhams.AddRange(dm1, dm2);
        await _context.SaveChangesAsync();

        _context.SanPhams.Add(new SanPham { TenSanPham = "SP1", DanhMucId = dm1.Id, GiaBan = 10000 });
        _context.SanPhams.Add(new SanPham { TenSanPham = "SP2", DanhMucId = dm2.Id, GiaBan = 20000 });
        await _context.SaveChangesAsync();

        var controller = CreateController();
        var result = await controller.Index(null, dm1.Id, null);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<SanPham>>(viewResult.Model);
        Assert.Single(model);
    }

    // ── Create ────────────────────────────────────────────────

    [Fact]
    public async Task Create_GET_ReturnsView()
    {
        // Tao danh muc de co SelectList
        var dm = new DanhMucSanPham { TenDanhMuc = "Test", KichHoat = true };
        _context.DanhMucSanPhams.Add(dm);
        await _context.SaveChangesAsync();

        var controller = CreateController();
        var result = await controller.Create();
        Assert.IsType<ViewResult>(result);
    }

    // ── Edit ──────────────────────────────────────────────────

    [Fact]
    public async Task Edit_GET_HopLe_ReturnsView()
    {
        var dm = new DanhMucSanPham { TenDanhMuc = "Test", KichHoat = true };
        _context.DanhMucSanPhams.Add(dm);
        await _context.SaveChangesAsync();

        var sp = new SanPham { TenSanPham = "SP1", DanhMucId = dm.Id, GiaBan = 10000 };
        _context.SanPhams.Add(sp);
        await _context.SaveChangesAsync();

        var controller = CreateController();
        var result = await controller.Edit(sp.Id);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.IsType<SanPham>(viewResult.Model);
    }

    [Fact]
    public async Task Edit_GET_KhongTonTai_ReturnsNotFound()
    {
        var controller = CreateController();
        var result = await controller.Edit(99999);
        Assert.IsType<NotFoundResult>(result);
    }

    // ── ToggleTrenKe ──────────────────────────────────────────

    [Fact]
    public async Task ToggleTrenKe_DoiTrangThai()
    {
        var dm = new DanhMucSanPham { TenDanhMuc = "Test", KichHoat = true };
        _context.DanhMucSanPhams.Add(dm);
        await _context.SaveChangesAsync();

        var sp = new SanPham { TenSanPham = "SP1", DanhMucId = dm.Id, GiaBan = 10000, TrenKe = true };
        _context.SanPhams.Add(sp);
        await _context.SaveChangesAsync();

        var controller = CreateController();
        var result = await controller.ToggleTrenKe(sp.Id);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        var updated = await _context.SanPhams.FindAsync(sp.Id);
        Assert.False(updated!.TrenKe);
    }
}
