using do_an.Controllers;
using do_an.Data;
using do_an.Models;
using do_an.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace do_an.UnitTests;

[TestClass]
public class TaiKhoanControllerTests
{
    private DbContextOptions<AppDbContext> CreateInMemoryOptions()
    {
        return new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    private TaiKhoanController CreateControllerWithTempData(AppDbContext context)
    {
        var controller = new TaiKhoanController(context);
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
        var controller = new TaiKhoanController(context);

        // Assert
        Assert.IsNotNull(controller);
    }

    [TestMethod]
    public async Task Index_WithNoFilters_ReturnsAllTaiKhoans()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        context.TaiKhoans.AddRange(
            new TaiKhoan { TenDangNhap = "admin1", HoTen = "Admin User", VaiTro = "Admin" },
            new TaiKhoan { TenDangNhap = "nv1", HoTen = "Nhan Vien 1", VaiTro = "NhanVien" },
            new TaiKhoan { TenDangNhap = "kh1", HoTen = "Khach Hang 1", VaiTro = "KhachHang" }
        );
        await context.SaveChangesAsync();
        var controller = CreateControllerWithTempData(context);

        // Act
        var result = await controller.Index(null, null);

        // Assert
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);
        var model = viewResult.Model as List<TaiKhoan>;
        Assert.IsNotNull(model);
        Assert.HasCount(3, model);
    }

    [TestMethod]
    public async Task Index_WithSearchByTenDangNhap_ReturnsMatchingTaiKhoans()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        context.TaiKhoans.AddRange(
            new TaiKhoan { TenDangNhap = "admin1", HoTen = "Admin User", VaiTro = "Admin" },
            new TaiKhoan { TenDangNhap = "nv1", HoTen = "Nhan Vien 1", VaiTro = "NhanVien" },
            new TaiKhoan { TenDangNhap = "kh1", HoTen = "Khach Hang 1", VaiTro = "KhachHang" }
        );
        await context.SaveChangesAsync();
        var controller = CreateControllerWithTempData(context);

        // Act
        var result = await controller.Index("admin", null);

        // Assert
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);
        var model = viewResult.Model as List<TaiKhoan>;
        Assert.IsNotNull(model);
        Assert.HasCount(1, model);
        Assert.AreEqual("admin1", model[0].TenDangNhap);
    }

    [TestMethod]
    public async Task Index_WithSearchByHoTen_ReturnsMatchingTaiKhoans()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        context.TaiKhoans.AddRange(
            new TaiKhoan { TenDangNhap = "admin1", HoTen = "Admin User", VaiTro = "Admin" },
            new TaiKhoan { TenDangNhap = "nv1", HoTen = "Nhan Vien 1", VaiTro = "NhanVien" },
            new TaiKhoan { TenDangNhap = "kh1", HoTen = "Khach Hang 1", VaiTro = "KhachHang" }
        );
        await context.SaveChangesAsync();
        var controller = CreateControllerWithTempData(context);

        // Act
        var result = await controller.Index("Nhan Vien", null);

        // Assert
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);
        var model = viewResult.Model as List<TaiKhoan>;
        Assert.IsNotNull(model);
        Assert.HasCount(1, model);
        Assert.AreEqual("nv1", model[0].TenDangNhap);
    }

    [TestMethod]
    public async Task Index_WithVaiTroFilter_ReturnsMatchingTaiKhoans()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        context.TaiKhoans.AddRange(
            new TaiKhoan { TenDangNhap = "admin1", HoTen = "Admin User", VaiTro = "Admin" },
            new TaiKhoan { TenDangNhap = "nv1", HoTen = "Nhan Vien 1", VaiTro = "NhanVien" },
            new TaiKhoan { TenDangNhap = "kh1", HoTen = "Khach Hang 1", VaiTro = "KhachHang" }
        );
        await context.SaveChangesAsync();
        var controller = CreateControllerWithTempData(context);

        // Act
        var result = await controller.Index(null, "NhanVien");

        // Assert
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);
        var model = viewResult.Model as List<TaiKhoan>;
        Assert.IsNotNull(model);
        Assert.HasCount(1, model);
        Assert.AreEqual("nv1", model[0].TenDangNhap);
    }

    [TestMethod]
    public async Task Index_WithSearchAndVaiTroFilters_ReturnsMatchingTaiKhoans()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        context.TaiKhoans.AddRange(
            new TaiKhoan { TenDangNhap = "admin1", HoTen = "Test Admin", VaiTro = "Admin" },
            new TaiKhoan { TenDangNhap = "admin2", HoTen = "Another Admin", VaiTro = "Admin" },
            new TaiKhoan { TenDangNhap = "nv1", HoTen = "Test Nhan Vien", VaiTro = "NhanVien" }
        );
        await context.SaveChangesAsync();
        var controller = CreateControllerWithTempData(context);

        // Act
        var result = await controller.Index("Test", "Admin");

        // Assert
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);
        var model = viewResult.Model as List<TaiKhoan>;
        Assert.IsNotNull(model);
        Assert.HasCount(1, model);
        Assert.AreEqual("admin1", model[0].TenDangNhap);
    }

    [TestMethod]
    public async Task Index_WithWhitespaceSearch_IgnoresSearch()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        context.TaiKhoans.AddRange(
            new TaiKhoan { TenDangNhap = "admin1", HoTen = "Admin User", VaiTro = "Admin" },
            new TaiKhoan { TenDangNhap = "nv1", HoTen = "Nhan Vien 1", VaiTro = "NhanVien" }
        );
        await context.SaveChangesAsync();
        var controller = CreateControllerWithTempData(context);

        // Act
        var result = await controller.Index("   ", null);

        // Assert
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);
        var model = viewResult.Model as List<TaiKhoan>;
        Assert.IsNotNull(model);
        Assert.HasCount(2, model);
    }

    [TestMethod]
    public async Task Index_WithWhitespaceVaiTro_IgnoresVaiTroFilter()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        context.TaiKhoans.AddRange(
            new TaiKhoan { TenDangNhap = "admin1", HoTen = "Admin User", VaiTro = "Admin" },
            new TaiKhoan { TenDangNhap = "nv1", HoTen = "Nhan Vien 1", VaiTro = "NhanVien" }
        );
        await context.SaveChangesAsync();
        var controller = CreateControllerWithTempData(context);

        // Act
        var result = await controller.Index(null, "   ");

        // Assert
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);
        var model = viewResult.Model as List<TaiKhoan>;
        Assert.IsNotNull(model);
        Assert.HasCount(2, model);
    }

    [TestMethod]
    public async Task Index_SetsViewBagProperties()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        var controller = CreateControllerWithTempData(context);

        // Act
        var result = await controller.Index("test", "Admin");

        // Assert
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);
        Assert.AreEqual("test", controller.ViewBag.Search);
        Assert.AreEqual("Admin", controller.ViewBag.VaiTro);
        Assert.IsNotNull(controller.ViewBag.VaiTroList);
    }

    [TestMethod]
    public async Task DoiMatKhauGet_WithExistingId_ReturnsViewWithViewModel()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        context.TaiKhoans.Add(new TaiKhoan { TenDangNhap = "testuser", HoTen = "Test User" });
        await context.SaveChangesAsync();
        var controller = CreateControllerWithTempData(context);

        // Act
        var result = await controller.DoiMatKhau("testuser");

        // Assert
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);
        var model = viewResult.Model as DoiMatKhauViewModel;
        Assert.IsNotNull(model);
        Assert.AreEqual("testuser", model.TenDangNhap);
        Assert.AreEqual("Test User", model.HoTen);
    }

    [TestMethod]
    public async Task DoiMatKhauGet_WithNonExistingId_ReturnsNotFound()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        var controller = CreateControllerWithTempData(context);

        // Act
        var result = await controller.DoiMatKhau("nonexistent");

        // Assert
        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
    }

    [TestMethod]
    public async Task DoiMatKhauPost_WithInvalidModelState_ReturnsViewWithViewModel()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        var controller = CreateControllerWithTempData(context);
        controller.ModelState.AddModelError("MatKhauMoi", "Required");
        var vm = new DoiMatKhauViewModel { TenDangNhap = "testuser" };

        // Act
        var result = await controller.DoiMatKhau(vm);

        // Assert
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);
        Assert.AreEqual(vm, viewResult.Model);
    }

    [TestMethod]
    public async Task DoiMatKhauPost_WithNonExistingUser_ReturnsNotFound()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        var controller = CreateControllerWithTempData(context);
        var vm = new DoiMatKhauViewModel { TenDangNhap = "nonexistent", MatKhauMoi = "newpass123" };

        // Act
        var result = await controller.DoiMatKhau(vm);

        // Assert
        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
    }

    [TestMethod]
    public async Task DoiMatKhauPost_WithValidData_UpdatesPasswordAndRedirects()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        var taiKhoan = new TaiKhoan { TenDangNhap = "testuser", HoTen = "Test User", MatKhauHash = "oldhash" };
        context.TaiKhoans.Add(taiKhoan);
        await context.SaveChangesAsync();
        var controller = CreateControllerWithTempData(context);
        var vm = new DoiMatKhauViewModel { TenDangNhap = "testuser", MatKhauMoi = "newpass123" };

        // Act
        var result = await controller.DoiMatKhau(vm);

        // Assert
        var redirectResult = result as RedirectToActionResult;
        Assert.IsNotNull(redirectResult);
        Assert.AreEqual(nameof(TaiKhoanController.Index), redirectResult.ActionName);
        
        var updatedTaiKhoan = await context.TaiKhoans.FindAsync("testuser");
        Assert.IsNotNull(updatedTaiKhoan);
        Assert.AreNotEqual("oldhash", updatedTaiKhoan.MatKhauHash);
        
        Assert.IsTrue(controller.TempData.ContainsKey("Success"));
    }

    [TestMethod]
    public async Task Delete_WithNonExistingId_ReturnsNotFound()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        var controller = CreateControllerWithTempData(context);

        // Act
        var result = await controller.Delete("nonexistent");

        // Assert
        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
    }

    [TestMethod]
    public async Task Delete_LastAdmin_SetsErrorAndRedirects()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        context.TaiKhoans.Add(new TaiKhoan { TenDangNhap = "admin1", HoTen = "Admin User", VaiTro = "Admin" });
        await context.SaveChangesAsync();
        var controller = CreateControllerWithTempData(context);

        // Act
        var result = await controller.Delete("admin1");

        // Assert
        var redirectResult = result as RedirectToActionResult;
        Assert.IsNotNull(redirectResult);
        Assert.AreEqual(nameof(TaiKhoanController.Index), redirectResult.ActionName);
        Assert.IsTrue(controller.TempData.ContainsKey("Error"));
        
        var stillExists = await context.TaiKhoans.FindAsync("admin1");
        Assert.IsNotNull(stillExists);
    }

    [TestMethod]
    public async Task Delete_AdminWithOtherAdmins_DeletesSuccessfully()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        context.TaiKhoans.AddRange(
            new TaiKhoan { TenDangNhap = "admin1", HoTen = "Admin 1", VaiTro = "Admin" },
            new TaiKhoan { TenDangNhap = "admin2", HoTen = "Admin 2", VaiTro = "Admin" }
        );
        await context.SaveChangesAsync();
        var controller = CreateControllerWithTempData(context);

        // Act
        var result = await controller.Delete("admin1");

        // Assert
        var redirectResult = result as RedirectToActionResult;
        Assert.IsNotNull(redirectResult);
        Assert.AreEqual(nameof(TaiKhoanController.Index), redirectResult.ActionName);
        Assert.IsTrue(controller.TempData.ContainsKey("Success"));
        
        var deleted = await context.TaiKhoans.FindAsync("admin1");
        Assert.IsNull(deleted);
    }

    [TestMethod]
    public async Task Delete_NonAdminUser_DeletesSuccessfully()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        context.TaiKhoans.Add(new TaiKhoan { TenDangNhap = "nv1", HoTen = "Nhan Vien 1", VaiTro = "NhanVien" });
        await context.SaveChangesAsync();
        var controller = CreateControllerWithTempData(context);

        // Act
        var result = await controller.Delete("nv1");

        // Assert
        var redirectResult = result as RedirectToActionResult;
        Assert.IsNotNull(redirectResult);
        Assert.AreEqual(nameof(TaiKhoanController.Index), redirectResult.ActionName);
        Assert.IsTrue(controller.TempData.ContainsKey("Success"));
        
        var deleted = await context.TaiKhoans.FindAsync("nv1");
        Assert.IsNull(deleted);
    }
}
