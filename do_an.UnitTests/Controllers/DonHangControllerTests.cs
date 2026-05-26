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

public class DonHangOnlineControllerTests : IDisposable
{
    private readonly AppDbContext _context;

    public DonHangOnlineControllerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
        _context.Database.EnsureCreated();
    }

    public void Dispose() => _context.Dispose();

    private DonHangOnlineController CreateController(string role = "NhanVien")
    {
        var controller = new DonHangOnlineController(_context);
        var httpContext = new DefaultHttpContext();
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, "Staff"),
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
    public async Task Index_ReturnsViewWithDonHangs()
    {
        _context.DonHangs.Add(new DonHang { MaDonHang = "DH-001", LoaiDon = LoaiDonHang.Online, TrangThai = TrangThaiDonHang.ChoThanhToan });
        _context.DonHangs.Add(new DonHang { MaDonHang = "DH-002", LoaiDon = LoaiDonHang.Online, TrangThai = TrangThaiDonHang.DaGiao });
        _context.DonHangs.Add(new DonHang { MaDonHang = "DH-POS", LoaiDon = LoaiDonHang.POS_TaiQuay, TrangThai = TrangThaiDonHang.DaThanhToan });
        await _context.SaveChangesAsync();

        var controller = CreateController();
        var result = await controller.Index(null, null);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<DonHang>>(viewResult.Model);
        Assert.Equal(2, model.Count); // Chi lay don online
    }

    [Fact]
    public async Task Index_LocTheoTrangThai()
    {
        _context.DonHangs.Add(new DonHang { MaDonHang = "DH-001", LoaiDon = LoaiDonHang.Online, TrangThai = TrangThaiDonHang.ChoThanhToan });
        _context.DonHangs.Add(new DonHang { MaDonHang = "DH-002", LoaiDon = LoaiDonHang.Online, TrangThai = TrangThaiDonHang.DaGiao });
        await _context.SaveChangesAsync();

        var controller = CreateController();
        var result = await controller.Index(TrangThaiDonHang.ChoThanhToan, null);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<DonHang>>(viewResult.Model);
        Assert.Single(model);
        Assert.Equal("DH-001", model[0].MaDonHang);
    }

    [Fact]
    public async Task Index_TimKiemTheoMa()
    {
        _context.DonHangs.Add(new DonHang { MaDonHang = "DH-ABC-001", LoaiDon = LoaiDonHang.Online });
        _context.DonHangs.Add(new DonHang { MaDonHang = "DH-XYZ-002", LoaiDon = LoaiDonHang.Online });
        await _context.SaveChangesAsync();

        var controller = CreateController();
        var result = await controller.Index(null, "ABC");

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<DonHang>>(viewResult.Model);
        Assert.Single(model);
    }

    // ── Details ───────────────────────────────────────────────

    [Fact]
    public async Task Details_HopLe_ReturnsView()
    {
        var dh = new DonHang { MaDonHang = "DH-001", LoaiDon = LoaiDonHang.Online };
        _context.DonHangs.Add(dh);
        await _context.SaveChangesAsync();

        var controller = CreateController();
        var result = await controller.Details(dh.Id);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.IsType<DonHang>(viewResult.Model);
    }

    [Fact]
    public async Task Details_KhongTonTai_ReturnsNotFound()
    {
        var controller = CreateController();
        var result = await controller.Details(99999);
        Assert.IsType<NotFoundResult>(result);
    }

    // ── CapNhatTrangThai ──────────────────────────────────────

    [Fact]
    public async Task CapNhatTrangThai_ChuyenHopLe_ThanhCong()
    {
        var dh = new DonHang { MaDonHang = "DH-001", TrangThai = TrangThaiDonHang.ChoThanhToan };
        _context.DonHangs.Add(dh);
        await _context.SaveChangesAsync();

        var controller = CreateController();
        var result = await controller.CapNhatTrangThai(dh.Id, TrangThaiDonHang.DaThanhToan);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        var updated = await _context.DonHangs.FindAsync(dh.Id);
        Assert.Equal(TrangThaiDonHang.DaThanhToan, updated!.TrangThai);
    }

    [Fact]
    public async Task CapNhatTrangThai_ChuyenKhongHopLe_BaoLoi()
    {
        var dh = new DonHang { MaDonHang = "DH-001", TrangThai = TrangThaiDonHang.DaGiao };
        _context.DonHangs.Add(dh);
        await _context.SaveChangesAsync();

        var controller = CreateController();
        var result = await controller.CapNhatTrangThai(dh.Id, TrangThaiDonHang.ChoThanhToan);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.NotNull(controller.TempData["Error"]);
    }

    [Fact]
    public async Task CapNhatTrangThai_HuyDon_HoanKho()
    {
        var dm = new DanhMucSanPham { TenDanhMuc = "Test", KichHoat = true };
        _context.DanhMucSanPhams.Add(dm);
        await _context.SaveChangesAsync();

        var sp = new SanPham { TenSanPham = "SP", DanhMucId = dm.Id, GiaBan = 10000, SoLuong = 70 };
        _context.SanPhams.Add(sp);
        await _context.SaveChangesAsync();

        var dh = new DonHang { MaDonHang = "DH-001", TrangThai = TrangThaiDonHang.ChoThanhToan };
        dh.ChiTietDonHangs.Add(new ChiTietDonHang { SanPhamId = sp.Id, SoLuong = 30, GiaBan = 10000 });
        _context.DonHangs.Add(dh);
        await _context.SaveChangesAsync();

        var controller = CreateController();
        await controller.CapNhatTrangThai(dh.Id, TrangThaiDonHang.DaHuy);

        var updatedSp = await _context.SanPhams.FindAsync(sp.Id);
        Assert.Equal(100, updatedSp!.SoLuong); // Hoan kho: 70 + 30
    }

    // ── Delete ────────────────────────────────────────────────

    [Fact]
    public async Task Delete_HopLe_ReturnsView()
    {
        var dh = new DonHang { MaDonHang = "DH-001", LoaiDon = LoaiDonHang.Online };
        _context.DonHangs.Add(dh);
        await _context.SaveChangesAsync();

        var controller = CreateController();
        var result = await controller.Delete(dh.Id);

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task DeleteConfirmed_XoaVinhVien()
    {
        var dh = new DonHang { MaDonHang = "DH-001", LoaiDon = LoaiDonHang.Online };
        _context.DonHangs.Add(dh);
        await _context.SaveChangesAsync();

        var controller = CreateController();
        var result = await controller.DeleteConfirmed(dh.Id, false);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Null(await _context.DonHangs.FindAsync(dh.Id));
    }
}
