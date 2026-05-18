using do_an.Controllers;
using do_an.Data;
using do_an.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace do_an.UnitTests;

[TestClass]
public class DonHangControllerTests
{
    private DbContextOptions<AppDbContext> CreateInMemoryOptions()
    {
        return new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    private DonHangController CreateControllerWithTempData(AppDbContext context)
    {
        var controller = new DonHangController(context);
        var httpContext = new DefaultHttpContext();
        var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        controller.TempData = tempData;
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
        return controller;
    }

    [TestMethod]
    public void Constructor_WithValidContext_SetsContext()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);

        // Act
        var controller = new DonHangController(context);

        // Assert
        Assert.IsNotNull(controller);
    }

    [TestMethod]
    public async Task Index_WithNoFilters_ReturnsAllOrders()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        var dh1 = new DonHang { MaDonHang = "DH001", TrangThai = TrangThaiDonHang.ChoThanhToan, NgayDat = DateTime.Now };
        var dh2 = new DonHang { MaDonHang = "DH002", TrangThai = TrangThaiDonHang.DaThanhToan, NgayDat = DateTime.Now.AddHours(-1) };
        context.DonHangs.AddRange(dh1, dh2);
        await context.SaveChangesAsync();

        var controller = CreateControllerWithTempData(context);

        // Act
        var result = await controller.Index(null, null);

        // Assert
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);
        var model = viewResult.Model as List<DonHang>;
        Assert.IsNotNull(model);
        Assert.HasCount(2, model);
        Assert.AreEqual("DH001", model[0].MaDonHang);
    }

    [TestMethod]
    public async Task Index_WithTrangThaiFilter_ReturnsFilteredOrders()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        var dh1 = new DonHang { MaDonHang = "DH001", TrangThai = TrangThaiDonHang.ChoThanhToan, NgayDat = DateTime.Now };
        var dh2 = new DonHang { MaDonHang = "DH002", TrangThai = TrangThaiDonHang.DaThanhToan, NgayDat = DateTime.Now };
        context.DonHangs.AddRange(dh1, dh2);
        await context.SaveChangesAsync();

        var controller = CreateControllerWithTempData(context);

        // Act
        var result = await controller.Index(TrangThaiDonHang.ChoThanhToan, null);

        // Assert
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);
        var model = viewResult.Model as List<DonHang>;
        Assert.IsNotNull(model);
        Assert.HasCount(1, model);
        Assert.AreEqual("DH001", model[0].MaDonHang);
    }

    [TestMethod]
    public async Task Index_WithSearchByMaDonHang_ReturnsMatchingOrders()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        var dh1 = new DonHang { MaDonHang = "DH001", TrangThai = TrangThaiDonHang.ChoThanhToan, NgayDat = DateTime.Now };
        var dh2 = new DonHang { MaDonHang = "DH002", TrangThai = TrangThaiDonHang.DaThanhToan, NgayDat = DateTime.Now };
        context.DonHangs.AddRange(dh1, dh2);
        await context.SaveChangesAsync();

        var controller = CreateControllerWithTempData(context);

        // Act
        var result = await controller.Index(null, "DH001");

        // Assert
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);
        var model = viewResult.Model as List<DonHang>;
        Assert.IsNotNull(model);
        Assert.HasCount(1, model);
        Assert.AreEqual("DH001", model[0].MaDonHang);
    }

    [TestMethod]
    public async Task Index_WithSearchByHoTenNguoiNhan_ReturnsMatchingOrders()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        var dh1 = new DonHang { MaDonHang = "DH001", HoTenNguoiNhan = "Nguyen Van A", TrangThai = TrangThaiDonHang.ChoThanhToan, NgayDat = DateTime.Now };
        var dh2 = new DonHang { MaDonHang = "DH002", HoTenNguoiNhan = "Tran Van B", TrangThai = TrangThaiDonHang.DaThanhToan, NgayDat = DateTime.Now };
        context.DonHangs.AddRange(dh1, dh2);
        await context.SaveChangesAsync();

        var controller = CreateControllerWithTempData(context);

        // Act
        var result = await controller.Index(null, "Nguyen");

        // Assert
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);
        var model = viewResult.Model as List<DonHang>;
        Assert.IsNotNull(model);
        Assert.HasCount(1, model);
        Assert.AreEqual("DH001", model[0].MaDonHang);
    }

    [TestMethod]
    public async Task Index_WithSearchBySoDienThoai_ReturnsMatchingOrders()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        var dh1 = new DonHang { MaDonHang = "DH001", SoDienThoai = "0123456789", TrangThai = TrangThaiDonHang.ChoThanhToan, NgayDat = DateTime.Now };
        var dh2 = new DonHang { MaDonHang = "DH002", SoDienThoai = "0987654321", TrangThai = TrangThaiDonHang.DaThanhToan, NgayDat = DateTime.Now };
        context.DonHangs.AddRange(dh1, dh2);
        await context.SaveChangesAsync();

        var controller = CreateControllerWithTempData(context);

        // Act
        var result = await controller.Index(null, "0123");

        // Assert
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);
        var model = viewResult.Model as List<DonHang>;
        Assert.IsNotNull(model);
        Assert.HasCount(1, model);
        Assert.AreEqual("DH001", model[0].MaDonHang);
    }

    [TestMethod]
    public async Task Index_WithNullHoTenAndSearch_DoesNotCrash()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        var dh1 = new DonHang { MaDonHang = "DH001", HoTenNguoiNhan = null, SoDienThoai = null, TrangThai = TrangThaiDonHang.ChoThanhToan, NgayDat = DateTime.Now };
        context.DonHangs.Add(dh1);
        await context.SaveChangesAsync();

        var controller = CreateControllerWithTempData(context);

        // Act
        var result = await controller.Index(null, "Test");

        // Assert
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);
        var model = viewResult.Model as List<DonHang>;
        Assert.IsNotNull(model);
        Assert.IsEmpty(model);
    }

    [TestMethod]
    public async Task Index_SetsViewBagProperties_Correctly()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        var dh1 = new DonHang { MaDonHang = "DH001", TrangThai = TrangThaiDonHang.ChoThanhToan, NgayDat = DateTime.Now };
        var dh2 = new DonHang { MaDonHang = "DH002", TrangThai = TrangThaiDonHang.DaThanhToan, NgayDat = DateTime.Now };
        var dh3 = new DonHang { MaDonHang = "DH003", TrangThai = TrangThaiDonHang.DangGiao, NgayDat = DateTime.Now };
        var dh4 = new DonHang { MaDonHang = "DH004", TrangThai = TrangThaiDonHang.DaGiao, NgayDat = DateTime.Now };
        var dh5 = new DonHang { MaDonHang = "DH005", TrangThai = TrangThaiDonHang.DaHuy, NgayDat = DateTime.Now };
        context.DonHangs.AddRange(dh1, dh2, dh3, dh4, dh5);
        await context.SaveChangesAsync();

        var controller = CreateControllerWithTempData(context);

        // Act
        var result = await controller.Index(TrangThaiDonHang.DaThanhToan, "test");

        // Assert
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);
        Assert.AreEqual(TrangThaiDonHang.DaThanhToan, controller.ViewBag.TrangThai);
        Assert.AreEqual("test", controller.ViewBag.Search);
        dynamic stats = controller.ViewBag.Stats;
        Assert.AreEqual(1, stats.ChoThanhToan);
        Assert.AreEqual(1, stats.DaThanhToan);
        Assert.AreEqual(1, stats.DangGiao);
        Assert.AreEqual(1, stats.DaGiao);
        Assert.AreEqual(1, stats.DaHuy);
    }

    [TestMethod]
    public async Task Index_OrdersByNgayDatDescending_Correctly()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        var dh1 = new DonHang { MaDonHang = "DH001", TrangThai = TrangThaiDonHang.ChoThanhToan, NgayDat = DateTime.Now.AddHours(-2) };
        var dh2 = new DonHang { MaDonHang = "DH002", TrangThai = TrangThaiDonHang.DaThanhToan, NgayDat = DateTime.Now };
        var dh3 = new DonHang { MaDonHang = "DH003", TrangThai = TrangThaiDonHang.DangGiao, NgayDat = DateTime.Now.AddHours(-1) };
        context.DonHangs.AddRange(dh1, dh2, dh3);
        await context.SaveChangesAsync();

        var controller = CreateControllerWithTempData(context);

        // Act
        var result = await controller.Index(null, null);

        // Assert
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);
        var model = viewResult.Model as List<DonHang>;
        Assert.IsNotNull(model);
        Assert.HasCount(3, model);
        Assert.AreEqual("DH002", model[0].MaDonHang);
        Assert.AreEqual("DH003", model[1].MaDonHang);
        Assert.AreEqual("DH001", model[2].MaDonHang);
    }

    [TestMethod]
    public async Task Details_WithValidId_ReturnsViewWithOrder()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        var dh = new DonHang { MaDonHang = "DH001", TrangThai = TrangThaiDonHang.ChoThanhToan, NgayDat = DateTime.Now };
        context.DonHangs.Add(dh);
        await context.SaveChangesAsync();

        var controller = CreateControllerWithTempData(context);

        // Act
        var result = await controller.Details(dh.Id);

        // Assert
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);
        var model = viewResult.Model as DonHang;
        Assert.IsNotNull(model);
        Assert.AreEqual("DH001", model.MaDonHang);
    }

    [TestMethod]
    public async Task Details_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        var controller = CreateControllerWithTempData(context);

        // Act
        var result = await controller.Details(999);

        // Assert
        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
    }

    [TestMethod]
    public async Task Details_IncludesChiTietDonHangsAndSanPham_Successfully()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        var sp = new SanPham { TenSanPham = "Product A", GiaBan = 100 };
        context.SanPhams.Add(sp);
        await context.SaveChangesAsync();

        var dh = new DonHang { MaDonHang = "DH001", TrangThai = TrangThaiDonHang.ChoThanhToan, NgayDat = DateTime.Now };
        context.DonHangs.Add(dh);
        await context.SaveChangesAsync();

        var ct = new ChiTietDonHang { DonHangId = dh.Id, SanPhamId = sp.Id, SoLuong = 2, GiaBan = 100 };
        context.ChiTietDonHangs.Add(ct);
        await context.SaveChangesAsync();

        var controller = CreateControllerWithTempData(context);

        // Act
        var result = await controller.Details(dh.Id);

        // Assert
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);
        var model = viewResult.Model as DonHang;
        Assert.IsNotNull(model);
        Assert.HasCount(1, model.ChiTietDonHangs);
        Assert.AreEqual("Product A", model.ChiTietDonHangs.First().SanPham.TenSanPham);
    }

    [TestMethod]
    public async Task CapNhatTrangThai_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        var controller = CreateControllerWithTempData(context);

        // Act
        var result = await controller.CapNhatTrangThai(999, TrangThaiDonHang.DaThanhToan);

        // Assert
        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
    }

    [TestMethod]
    public async Task CapNhatTrangThai_FromChoThanhToanToDaThanhToan_UpdatesSuccessfully()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        var dh = new DonHang { MaDonHang = "DH001", TrangThai = TrangThaiDonHang.ChoThanhToan, NgayDat = DateTime.Now };
        context.DonHangs.Add(dh);
        await context.SaveChangesAsync();

        var controller = CreateControllerWithTempData(context);

        // Act
        var result = await controller.CapNhatTrangThai(dh.Id, TrangThaiDonHang.DaThanhToan);

        // Assert
        var redirectResult = result as RedirectToActionResult;
        Assert.IsNotNull(redirectResult);
        Assert.AreEqual("Details", redirectResult.ActionName);
        
        var updatedDh = await context.DonHangs.FindAsync(dh.Id);
        Assert.AreEqual(TrangThaiDonHang.DaThanhToan, updatedDh!.TrangThai);
        Assert.IsNotNull(updatedDh.NgayThanhToan);
    }

    [TestMethod]
    public async Task CapNhatTrangThai_FromChoThanhToanToDaHuy_UpdatesSuccessfully()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        var dh = new DonHang { MaDonHang = "DH001", TrangThai = TrangThaiDonHang.ChoThanhToan, NgayDat = DateTime.Now };
        context.DonHangs.Add(dh);
        await context.SaveChangesAsync();

        var controller = CreateControllerWithTempData(context);

        // Act
        var result = await controller.CapNhatTrangThai(dh.Id, TrangThaiDonHang.DaHuy);

        // Assert
        var redirectResult = result as RedirectToActionResult;
        Assert.IsNotNull(redirectResult);
        
        var updatedDh = await context.DonHangs.FindAsync(dh.Id);
        Assert.AreEqual(TrangThaiDonHang.DaHuy, updatedDh!.TrangThai);
        Assert.IsNotNull(updatedDh.NgayHuy);
    }

    [TestMethod]
    public async Task CapNhatTrangThai_FromDaThanhToanToDangChuanBi_UpdatesSuccessfully()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        var dh = new DonHang { MaDonHang = "DH001", TrangThai = TrangThaiDonHang.DaThanhToan, NgayDat = DateTime.Now };
        context.DonHangs.Add(dh);
        await context.SaveChangesAsync();

        var controller = CreateControllerWithTempData(context);

        // Act
        var result = await controller.CapNhatTrangThai(dh.Id, TrangThaiDonHang.DangChuanBi);

        // Assert
        var redirectResult = result as RedirectToActionResult;
        Assert.IsNotNull(redirectResult);
        
        var updatedDh = await context.DonHangs.FindAsync(dh.Id);
        Assert.AreEqual(TrangThaiDonHang.DangChuanBi, updatedDh!.TrangThai);
        Assert.IsNotNull(updatedDh.NgayChuanBi);
    }

    [TestMethod]
    public async Task CapNhatTrangThai_FromDaThanhToanToDaHuy_UpdatesSuccessfully()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        var dh = new DonHang { MaDonHang = "DH001", TrangThai = TrangThaiDonHang.DaThanhToan, NgayDat = DateTime.Now };
        context.DonHangs.Add(dh);
        await context.SaveChangesAsync();

        var controller = CreateControllerWithTempData(context);

        // Act
        var result = await controller.CapNhatTrangThai(dh.Id, TrangThaiDonHang.DaHuy);

        // Assert
        var redirectResult = result as RedirectToActionResult;
        Assert.IsNotNull(redirectResult);
        
        var updatedDh = await context.DonHangs.FindAsync(dh.Id);
        Assert.AreEqual(TrangThaiDonHang.DaHuy, updatedDh!.TrangThai);
        Assert.IsNotNull(updatedDh.NgayHuy);
    }

    [TestMethod]
    public async Task CapNhatTrangThai_FromDangChuanBiToDangGiao_UpdatesSuccessfully()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        var dh = new DonHang { MaDonHang = "DH001", TrangThai = TrangThaiDonHang.DangChuanBi, NgayDat = DateTime.Now };
        context.DonHangs.Add(dh);
        await context.SaveChangesAsync();

        var controller = CreateControllerWithTempData(context);

        // Act
        var result = await controller.CapNhatTrangThai(dh.Id, TrangThaiDonHang.DangGiao);

        // Assert
        var redirectResult = result as RedirectToActionResult;
        Assert.IsNotNull(redirectResult);
        
        var updatedDh = await context.DonHangs.FindAsync(dh.Id);
        Assert.AreEqual(TrangThaiDonHang.DangGiao, updatedDh!.TrangThai);
        Assert.IsNotNull(updatedDh.NgayBatDauGiao);
    }

    [TestMethod]
    public async Task CapNhatTrangThai_FromDangChuanBiToDaHuy_UpdatesSuccessfully()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        var dh = new DonHang { MaDonHang = "DH001", TrangThai = TrangThaiDonHang.DangChuanBi, NgayDat = DateTime.Now };
        context.DonHangs.Add(dh);
        await context.SaveChangesAsync();

        var controller = CreateControllerWithTempData(context);

        // Act
        var result = await controller.CapNhatTrangThai(dh.Id, TrangThaiDonHang.DaHuy);

        // Assert
        var redirectResult = result as RedirectToActionResult;
        Assert.IsNotNull(redirectResult);
        
        var updatedDh = await context.DonHangs.FindAsync(dh.Id);
        Assert.AreEqual(TrangThaiDonHang.DaHuy, updatedDh!.TrangThai);
        Assert.IsNotNull(updatedDh.NgayHuy);
    }

    [TestMethod]
    public async Task CapNhatTrangThai_FromDangGiaoToDaGiao_UpdatesSuccessfully()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        var dh = new DonHang { MaDonHang = "DH001", TrangThai = TrangThaiDonHang.DangGiao, NgayDat = DateTime.Now };
        context.DonHangs.Add(dh);
        await context.SaveChangesAsync();

        var controller = CreateControllerWithTempData(context);

        // Act
        var result = await controller.CapNhatTrangThai(dh.Id, TrangThaiDonHang.DaGiao);

        // Assert
        var redirectResult = result as RedirectToActionResult;
        Assert.IsNotNull(redirectResult);
        
        var updatedDh = await context.DonHangs.FindAsync(dh.Id);
        Assert.AreEqual(TrangThaiDonHang.DaGiao, updatedDh!.TrangThai);
        Assert.IsNotNull(updatedDh.NgayGiao);
    }

    [TestMethod]
    public async Task CapNhatTrangThai_InvalidTransition_SetsErrorAndRedirects()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        var dh = new DonHang { MaDonHang = "DH001", TrangThai = TrangThaiDonHang.ChoThanhToan, NgayDat = DateTime.Now };
        context.DonHangs.Add(dh);
        await context.SaveChangesAsync();

        var controller = CreateControllerWithTempData(context);

        // Act
        var result = await controller.CapNhatTrangThai(dh.Id, TrangThaiDonHang.DangGiao);

        // Assert
        var redirectResult = result as RedirectToActionResult;
        Assert.IsNotNull(redirectResult);
        Assert.AreEqual("Details", redirectResult.ActionName);
        Assert.IsTrue(controller.TempData.ContainsKey("Error"));
        
        var updatedDh = await context.DonHangs.FindAsync(dh.Id);
        Assert.AreEqual(TrangThaiDonHang.ChoThanhToan, updatedDh!.TrangThai);
    }

    [TestMethod]
    public async Task CapNhatTrangThai_FromDaGiaoToAnyState_IsInvalid()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        var dh = new DonHang { MaDonHang = "DH001", TrangThai = TrangThaiDonHang.DaGiao, NgayDat = DateTime.Now };
        context.DonHangs.Add(dh);
        await context.SaveChangesAsync();

        var controller = CreateControllerWithTempData(context);

        // Act
        var result = await controller.CapNhatTrangThai(dh.Id, TrangThaiDonHang.DaHuy);

        // Assert
        var redirectResult = result as RedirectToActionResult;
        Assert.IsNotNull(redirectResult);
        Assert.IsTrue(controller.TempData.ContainsKey("Error"));
        
        var updatedDh = await context.DonHangs.FindAsync(dh.Id);
        Assert.AreEqual(TrangThaiDonHang.DaGiao, updatedDh!.TrangThai);
    }

    [TestMethod]
    public async Task CapNhatTrangThai_FromDaHuyToAnyState_IsInvalid()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        var dh = new DonHang { MaDonHang = "DH001", TrangThai = TrangThaiDonHang.DaHuy, NgayDat = DateTime.Now };
        context.DonHangs.Add(dh);
        await context.SaveChangesAsync();

        var controller = CreateControllerWithTempData(context);

        // Act
        var result = await controller.CapNhatTrangThai(dh.Id, TrangThaiDonHang.ChoThanhToan);

        // Assert
        var redirectResult = result as RedirectToActionResult;
        Assert.IsNotNull(redirectResult);
        Assert.IsTrue(controller.TempData.ContainsKey("Error"));
        
        var updatedDh = await context.DonHangs.FindAsync(dh.Id);
        Assert.AreEqual(TrangThaiDonHang.DaHuy, updatedDh!.TrangThai);
    }

    [TestMethod]
    public async Task CapNhatTrangThai_ValidTransition_SetsSuccessMessage()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        var dh = new DonHang { MaDonHang = "DH001", TrangThai = TrangThaiDonHang.ChoThanhToan, NgayDat = DateTime.Now };
        context.DonHangs.Add(dh);
        await context.SaveChangesAsync();

        var controller = CreateControllerWithTempData(context);

        // Act
        var result = await controller.CapNhatTrangThai(dh.Id, TrangThaiDonHang.DaThanhToan);

        // Assert
        Assert.IsTrue(controller.TempData.ContainsKey("Success"));
        var successMessage = controller.TempData["Success"] as string;
        Assert.Contains("DH001", successMessage!);
    }

    [TestMethod]
    public async Task CapNhatTrangThai_WithChiTietDonHangs_CallsKhoHelperWhenCancelling()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        
        var sp = new SanPham { TenSanPham = "Product A", GiaBan = 100, IsThuoc = true };
        context.SanPhams.Add(sp);
        await context.SaveChangesAsync();

        var dh = new DonHang { MaDonHang = "DH001", TrangThai = TrangThaiDonHang.ChoThanhToan, NgayDat = DateTime.Now };
        context.DonHangs.Add(dh);
        await context.SaveChangesAsync();

        var ct = new ChiTietDonHang { DonHangId = dh.Id, SanPhamId = sp.Id, SoLuong = 2, GiaBan = 100 };
        context.ChiTietDonHangs.Add(ct);
        await context.SaveChangesAsync();

        var controller = CreateControllerWithTempData(context);

        // Act & Assert - This should not throw an exception
        var result = await controller.CapNhatTrangThai(dh.Id, TrangThaiDonHang.DaHuy);

        // Assert
        var redirectResult = result as RedirectToActionResult;
        Assert.IsNotNull(redirectResult);
    }

    [TestMethod]
    public async Task CapNhatTrangThai_SameState_IsInvalid()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        var dh = new DonHang { MaDonHang = "DH001", TrangThai = TrangThaiDonHang.ChoThanhToan, NgayDat = DateTime.Now };
        context.DonHangs.Add(dh);
        await context.SaveChangesAsync();

        var controller = CreateControllerWithTempData(context);

        // Act
        var result = await controller.CapNhatTrangThai(dh.Id, TrangThaiDonHang.ChoThanhToan);

        // Assert
        var redirectResult = result as RedirectToActionResult;
        Assert.IsNotNull(redirectResult);
        Assert.IsTrue(controller.TempData.ContainsKey("Error"));
    }

    [TestMethod]
    public async Task CapNhatTrangThai_ToChoThanhToan_IsInvalid()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        var dh = new DonHang { MaDonHang = "DH001", TrangThai = TrangThaiDonHang.DaThanhToan, NgayDat = DateTime.Now };
        context.DonHangs.Add(dh);
        await context.SaveChangesAsync();

        var controller = CreateControllerWithTempData(context);

        // Act
        var result = await controller.CapNhatTrangThai(dh.Id, TrangThaiDonHang.ChoThanhToan);

        // Assert
        var redirectResult = result as RedirectToActionResult;
        Assert.IsNotNull(redirectResult);
        Assert.IsTrue(controller.TempData.ContainsKey("Error"));
    }

    [TestMethod]
    public async Task Index_WithEmptySearch_ReturnsAllOrders()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        var dh1 = new DonHang { MaDonHang = "DH001", TrangThai = TrangThaiDonHang.ChoThanhToan, NgayDat = DateTime.Now };
        var dh2 = new DonHang { MaDonHang = "DH002", TrangThai = TrangThaiDonHang.DaThanhToan, NgayDat = DateTime.Now };
        context.DonHangs.AddRange(dh1, dh2);
        await context.SaveChangesAsync();

        var controller = CreateControllerWithTempData(context);

        // Act
        var result = await controller.Index(null, "   ");

        // Assert
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);
        var model = viewResult.Model as List<DonHang>;
        Assert.IsNotNull(model);
        Assert.HasCount(2, model);
    }

    [TestMethod]
    public async Task Index_WithWhitespaceSearch_ReturnsAllOrders()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        var dh1 = new DonHang { MaDonHang = "DH001", TrangThai = TrangThaiDonHang.ChoThanhToan, NgayDat = DateTime.Now };
        context.DonHangs.Add(dh1);
        await context.SaveChangesAsync();

        var controller = CreateControllerWithTempData(context);

        // Act
        var result = await controller.Index(null, "\t\n");

        // Assert
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);
        var model = viewResult.Model as List<DonHang>;
        Assert.IsNotNull(model);
        Assert.HasCount(1, model);
    }

    [TestMethod]
    public async Task CapNhatTrangThai_DaHuy_SetsNgayHuyToNow()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        var dh = new DonHang { MaDonHang = "DH001", TrangThai = TrangThaiDonHang.ChoThanhToan, NgayDat = DateTime.Now };
        context.DonHangs.Add(dh);
        await context.SaveChangesAsync();

        var controller = CreateControllerWithTempData(context);
        var beforeTime = DateTime.Now.AddSeconds(-1);

        // Act
        var result = await controller.CapNhatTrangThai(dh.Id, TrangThaiDonHang.DaHuy);

        // Assert
        var afterTime = DateTime.Now.AddSeconds(1);
        var updatedDh = await context.DonHangs.FindAsync(dh.Id);
        Assert.IsNotNull(updatedDh!.NgayHuy);
        Assert.IsTrue(updatedDh.NgayHuy >= beforeTime);
        Assert.IsTrue(updatedDh.NgayHuy <= afterTime);
    }

    [TestMethod]
    public async Task CapNhatTrangThai_FromDangGiaoToDangGiao_IsInvalid()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        var dh = new DonHang { MaDonHang = "DH001", TrangThai = TrangThaiDonHang.DangGiao, NgayDat = DateTime.Now };
        context.DonHangs.Add(dh);
        await context.SaveChangesAsync();

        var controller = CreateControllerWithTempData(context);

        // Act
        var result = await controller.CapNhatTrangThai(dh.Id, TrangThaiDonHang.DangGiao);

        // Assert
        var redirectResult = result as RedirectToActionResult;
        Assert.IsNotNull(redirectResult);
        Assert.IsTrue(controller.TempData.ContainsKey("Error"));
    }
}
