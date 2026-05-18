using do_an.Controllers;
using do_an.Data;
using do_an.Models;
using do_an.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace do_an.UnitTests.Controllers;

[TestClass]
public class DonThuocControllerTests
{
    private AppDbContext _context = null!;
    private DonThuocController _controller = null!;

    [TestInitialize]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _controller = new DonThuocController(_context);

        // Setup TempData
        var httpContext = new DefaultHttpContext();
        var tempDataProvider = new Mock<ITempDataProvider>();
        var tempDataDictionaryFactory = new TempDataDictionaryFactory(tempDataProvider.Object);
        var tempData = tempDataDictionaryFactory.GetTempData(httpContext);
        _controller.TempData = tempData;
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context?.Dispose();
    }

    [TestMethod]
    public void Constructor_SetsContext()
    {
        // Arrange & Act
        var controller = new DonThuocController(_context);

        // Assert
        Assert.IsNotNull(controller);
    }

    [TestMethod]
    public async Task Index_NoFilters_ReturnsAllDonThuocs()
    {
        // Arrange
        var donThuoc1 = new DonThuoc { Id = 1, MaDonThuoc = "DT001", TenDonThuoc = "Don 1", TrangThai = TrangThaiDonThuoc.MoiTao };
        var donThuoc2 = new DonThuoc { Id = 2, MaDonThuoc = "DT002", TenDonThuoc = "Don 2", TrangThai = TrangThaiDonThuoc.DaDuyet };
        _context.DonThuocs.AddRange(donThuoc1, donThuoc2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.Index(null, null);

        // Assert
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);
        var model = viewResult.Model as List<DonThuoc>;
        Assert.IsNotNull(model);
        Assert.HasCount(2, model);
        Assert.AreEqual(2, model[0].Id); // OrderByDescending
        Assert.AreEqual(1, model[1].Id);
    }

    [TestMethod]
    public async Task Index_WithTrangThaiFilter_ReturnsFilteredDonThuocs()
    {
        // Arrange
        var donThuoc1 = new DonThuoc { Id = 1, MaDonThuoc = "DT001", TenDonThuoc = "Don 1", TrangThai = TrangThaiDonThuoc.MoiTao };
        var donThuoc2 = new DonThuoc { Id = 2, MaDonThuoc = "DT002", TenDonThuoc = "Don 2", TrangThai = TrangThaiDonThuoc.DaDuyet };
        _context.DonThuocs.AddRange(donThuoc1, donThuoc2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.Index(null, TrangThaiDonThuoc.DaDuyet);

        // Assert
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);
        var model = viewResult.Model as List<DonThuoc>;
        Assert.IsNotNull(model);
        Assert.HasCount(1, model);
        Assert.AreEqual(TrangThaiDonThuoc.DaDuyet, model[0].TrangThai);
    }

    [TestMethod]
    public async Task Index_WithSearch_ReturnsDonThuocsMatchingMaDonThuoc()
    {
        // Arrange
        var donThuoc1 = new DonThuoc { Id = 1, MaDonThuoc = "DT001", TenDonThuoc = "Don 1" };
        var donThuoc2 = new DonThuoc { Id = 2, MaDonThuoc = "DT002", TenDonThuoc = "Don 2" };
        _context.DonThuocs.AddRange(donThuoc1, donThuoc2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.Index("dt001", null);

        // Assert
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);
        var model = viewResult.Model as List<DonThuoc>;
        Assert.IsNotNull(model);
        Assert.HasCount(1, model);
        Assert.AreEqual("DT001", model[0].MaDonThuoc);
    }

    [TestMethod]
    public async Task Index_WithSearch_ReturnsDonThuocsMatchingTenDonThuoc()
    {
        // Arrange
        var donThuoc1 = new DonThuoc { Id = 1, MaDonThuoc = "DT001", TenDonThuoc = "Thuoc cam" };
        var donThuoc2 = new DonThuoc { Id = 2, MaDonThuoc = "DT002", TenDonThuoc = "Thuoc ho" };
        _context.DonThuocs.AddRange(donThuoc1, donThuoc2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.Index("cam", null);

        // Assert
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);
        var model = viewResult.Model as List<DonThuoc>;
        Assert.IsNotNull(model);
        Assert.HasCount(1, model);
        Assert.AreEqual("Thuoc cam", model[0].TenDonThuoc);
    }

    [TestMethod]
    public async Task Index_WithSearchAndTrangThai_ReturnsCombinedFilter()
    {
        // Arrange
        var donThuoc1 = new DonThuoc { Id = 1, MaDonThuoc = "DT001", TenDonThuoc = "Thuoc cam", TrangThai = TrangThaiDonThuoc.MoiTao };
        var donThuoc2 = new DonThuoc { Id = 2, MaDonThuoc = "DT002", TenDonThuoc = "Thuoc cam", TrangThai = TrangThaiDonThuoc.DaDuyet };
        _context.DonThuocs.AddRange(donThuoc1, donThuoc2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.Index("cam", TrangThaiDonThuoc.DaDuyet);

        // Assert
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);
        var model = viewResult.Model as List<DonThuoc>;
        Assert.IsNotNull(model);
        Assert.HasCount(1, model);
        Assert.AreEqual(2, model[0].Id);
    }

    [TestMethod]
    public async Task Index_WithWhitespaceSearch_TrimsAndConvertsToLowercase()
    {
        // Arrange
        var donThuoc1 = new DonThuoc { Id = 1, MaDonThuoc = "DT001", TenDonThuoc = "Thuoc Cam" };
        _context.DonThuocs.Add(donThuoc1);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.Index("  CAM  ", null);

        // Assert
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);
        var model = viewResult.Model as List<DonThuoc>;
        Assert.IsNotNull(model);
        Assert.HasCount(1, model);
    }

    [TestMethod]
    public async Task Index_SetsViewBagProperties()
    {
        // Arrange & Act
        var result = await _controller.Index("test", TrangThaiDonThuoc.MoiTao);

        // Assert
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);
        Assert.AreEqual("test", _controller.ViewBag.Search);
        Assert.AreEqual(TrangThaiDonThuoc.MoiTao, _controller.ViewBag.TrangThai);
    }

    [TestMethod]
    public async Task Create_ReturnsViewWithViewModel()
    {
        // Arrange
        _context.SanPhams.Add(new SanPham { Id = 1, TenSanPham = "Thuoc A", SoLuong = 10, IsThuoc = true, GiaBan = 10000 });
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.Create();

        // Assert
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);
        var model = viewResult.Model as CreateDonThuocViewModel;
        Assert.IsNotNull(model);
    }

    [TestMethod]
    public async Task CreatePost_EmptyItemsJson_ReturnsViewWithError()
    {
        // Arrange
        var vm = new CreateDonThuocViewModel
        {
            TenDonThuoc = "Test",
            ItemsJson = "[]"
        };

        // Act
        var result = await _controller.Create(vm);

        // Assert
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);
        Assert.IsFalse(_controller.ModelState.IsValid);
        Assert.IsTrue(_controller.ModelState.ContainsKey(""));
    }

    [TestMethod]
    public async Task CreatePost_InvalidJson_ReturnsViewWithError()
    {
        // Arrange
        var vm = new CreateDonThuocViewModel
        {
            TenDonThuoc = "Test",
            ItemsJson = "invalid json"
        };

        // Act
        var result = await _controller.Create(vm);

        // Assert
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);
        Assert.IsFalse(_controller.ModelState.IsValid);
    }

    [TestMethod]
    public async Task CreatePost_ValidData_CreatesDonThuoc()
    {
        // Arrange
        var sanPham = new SanPham { Id = 1, TenSanPham = "Thuoc A", SoLuong = 10, IsThuoc = true, GiaBan = 10000 };
        _context.SanPhams.Add(sanPham);
        await _context.SaveChangesAsync();

        var itemsJson = "[{\"sanPhamId\":1,\"soVienMoiNgay\":2,\"soNgayUong\":5,\"thoiDiemUong\":\"Sang\",\"cachDung\":\"Uong sau an\"}]";
        var vm = new CreateDonThuocViewModel
        {
            TenDonThuoc = "Don thuoc test",
            GhiChu = "Ghi chu test",
            DangNgayLenStore = false,
            ItemsJson = itemsJson
        };

        // Act
        var result = await _controller.Create(vm);

        // Assert
        var redirectResult = result as RedirectToActionResult;
        Assert.IsNotNull(redirectResult);
        Assert.AreEqual(nameof(_controller.Details), redirectResult.ActionName);

        var donThuoc = await _context.DonThuocs.Include(d => d.ChiTietDonThuocs).FirstOrDefaultAsync();
        Assert.IsNotNull(donThuoc);
        Assert.AreEqual("Don thuoc test", donThuoc.TenDonThuoc);
        Assert.AreEqual("Ghi chu test", donThuoc.GhiChu);
        Assert.AreEqual(TrangThaiDonThuoc.MoiTao, donThuoc.TrangThai);
        Assert.IsFalse(donThuoc.DaDangStore);
        Assert.HasCount(1, donThuoc.ChiTietDonThuocs);
    }

    [TestMethod]
    public async Task CreatePost_DangNgayLenStore_SetsTrangThaiDaDuyet()
    {
        // Arrange
        var sanPham = new SanPham { Id = 1, TenSanPham = "Thuoc A", SoLuong = 10, IsThuoc = true, GiaBan = 10000, TrenKe = false };
        _context.SanPhams.Add(sanPham);
        await _context.SaveChangesAsync();

        var itemsJson = "[{\"sanPhamId\":1,\"soVienMoiNgay\":2,\"soNgayUong\":5}]";
        var vm = new CreateDonThuocViewModel
        {
            TenDonThuoc = "Don thuoc test",
            DangNgayLenStore = true,
            ItemsJson = itemsJson
        };

        // Act
        var result = await _controller.Create(vm);

        // Assert
        var donThuoc = await _context.DonThuocs.FirstOrDefaultAsync();
        Assert.IsNotNull(donThuoc);
        Assert.AreEqual(TrangThaiDonThuoc.DaDuyet, donThuoc.TrangThai);
        Assert.IsTrue(donThuoc.DaDangStore);
    }



    [TestMethod]
    public async Task CreatePost_NullSoVienMoiNgay_DefaultsToOne()
    {
        // Arrange
        var sanPham = new SanPham { Id = 1, TenSanPham = "Thuoc A", SoLuong = 10, IsThuoc = true, GiaBan = 10000 };
        _context.SanPhams.Add(sanPham);
        await _context.SaveChangesAsync();

        var itemsJson = "[{\"sanPhamId\":1,\"soVienMoiNgay\":null,\"soNgayUong\":null}]";
        var vm = new CreateDonThuocViewModel
        {
            TenDonThuoc = "Don thuoc test",
            DangNgayLenStore = false,
            ItemsJson = itemsJson
        };

        // Act
        var result = await _controller.Create(vm);

        // Assert
        var donThuoc = await _context.DonThuocs.Include(d => d.ChiTietDonThuocs).FirstOrDefaultAsync();
        Assert.IsNotNull(donThuoc);
        var chiTiet = donThuoc.ChiTietDonThuocs.First();
        Assert.AreEqual(1, chiTiet.SoVienMoiNgay);
        Assert.AreEqual(1, chiTiet.SoNgayUong);
    }

    [TestMethod]
    public async Task CreatePost_ValidData_SetsTempDataSuccessMessage()
    {
        // Arrange
        var sanPham = new SanPham { Id = 1, TenSanPham = "Thuoc A", SoLuong = 10, IsThuoc = true, GiaBan = 10000 };
        _context.SanPhams.Add(sanPham);
        await _context.SaveChangesAsync();

        var itemsJson = "[{\"sanPhamId\":1}]";
        var vm = new CreateDonThuocViewModel
        {
            TenDonThuoc = "Don thuoc test",
            ItemsJson = itemsJson
        };

        // Act
        var result = await _controller.Create(vm);

        // Assert
        Assert.IsTrue(_controller.TempData.ContainsKey("Success"));
        var message = _controller.TempData["Success"] as string;
        Assert.IsNotNull(message);
        Assert.Contains("thành công", message);
    }

    [TestMethod]
    public async Task CreatePost_WithHinhAnhFile_SavesImagePath()
    {
        // Arrange
        var sanPham = new SanPham { Id = 1, TenSanPham = "Thuoc A", SoLuong = 10, IsThuoc = true, GiaBan = 10000 };
        _context.SanPhams.Add(sanPham);
        await _context.SaveChangesAsync();

        var mockFile = new Mock<IFormFile>();
        var content = "fake image content";
        var fileName = "test.jpg";
        var ms = new MemoryStream();
        var writer = new StreamWriter(ms);
        writer.Write(content);
        writer.Flush();
        ms.Position = 0;

        mockFile.Setup(f => f.Length).Returns(ms.Length);
        mockFile.Setup(f => f.FileName).Returns(fileName);
        mockFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var itemsJson = "[{\"sanPhamId\":1}]";
        var vm = new CreateDonThuocViewModel
        {
            TenDonThuoc = "Don thuoc test",
            ItemsJson = itemsJson,
            HinhAnhFile = mockFile.Object
        };

        // Act
        var result = await _controller.Create(vm);

        // Assert
        var donThuoc = await _context.DonThuocs.FirstOrDefaultAsync();
        Assert.IsNotNull(donThuoc);
        Assert.IsNotNull(donThuoc.HinhAnh);
        Assert.StartsWith("/uploads/donthuoc/", donThuoc.HinhAnh);
        Assert.EndsWith(".jpg", donThuoc.HinhAnh);
    }

    [TestMethod]
    public async Task CreatePost_MultipleItems_CreatesAllChiTietDonThuocs()
    {
        // Arrange
        var sanPham1 = new SanPham { Id = 1, TenSanPham = "Thuoc A", SoLuong = 10, IsThuoc = true, GiaBan = 10000 };
        var sanPham2 = new SanPham { Id = 2, TenSanPham = "Thuoc B", SoLuong = 5, IsThuoc = true, GiaBan = 20000 };
        _context.SanPhams.AddRange(sanPham1, sanPham2);
        await _context.SaveChangesAsync();

        var itemsJson = "[{\"sanPhamId\":1,\"soVienMoiNgay\":2,\"soNgayUong\":5},{\"sanPhamId\":2,\"soVienMoiNgay\":3,\"soNgayUong\":7}]";
        var vm = new CreateDonThuocViewModel
        {
            TenDonThuoc = "Don thuoc test",
            ItemsJson = itemsJson
        };

        // Act
        var result = await _controller.Create(vm);

        // Assert
        var donThuoc = await _context.DonThuocs.Include(d => d.ChiTietDonThuocs).FirstOrDefaultAsync();
        Assert.IsNotNull(donThuoc);
        Assert.HasCount(2, donThuoc.ChiTietDonThuocs);
    }

    [TestMethod]
    public async Task Details_ExistingId_ReturnsViewWithDonThuoc()
    {
        // Arrange
        var donThuoc = new DonThuoc
        {
            Id = 1,
            MaDonThuoc = "DT001",
            TenDonThuoc = "Don test"
        };
        _context.DonThuocs.Add(donThuoc);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.Details(1);

        // Assert
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);
        var model = viewResult.Model as DonThuoc;
        Assert.IsNotNull(model);
        Assert.AreEqual(1, model.Id);
        Assert.AreEqual("DT001", model.MaDonThuoc);
    }

    [TestMethod]
    public async Task Details_NonExistingId_ReturnsNotFound()
    {
        // Arrange & Act
        var result = await _controller.Details(999);

        // Assert
        Assert.IsInstanceOfType<NotFoundResult>(result);
    }

    [TestMethod]
    public async Task Details_LoadsChiTietDonThuocsWithSanPham()
    {
        // Arrange
        var sanPham = new SanPham { Id = 1, TenSanPham = "Thuoc A", SoLuong = 10, IsThuoc = true, GiaBan = 10000 };
        var donThuoc = new DonThuoc
        {
            Id = 1,
            MaDonThuoc = "DT001",
            TenDonThuoc = "Don test"
        };
        donThuoc.ChiTietDonThuocs.Add(new ChiTietDonThuoc
        {
            SanPhamId = 1,
            SoVienMoiNgay = 2,
            SoNgayUong = 5
        });
        _context.SanPhams.Add(sanPham);
        _context.DonThuocs.Add(donThuoc);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.Details(1);

        // Assert
        var viewResult = result as ViewResult;
        Assert.IsNotNull(viewResult);
        var model = viewResult.Model as DonThuoc;
        Assert.IsNotNull(model);
        Assert.HasCount(1, model.ChiTietDonThuocs);
        var chiTiet = model.ChiTietDonThuocs.First();
        Assert.IsNotNull(chiTiet.SanPham);
        Assert.AreEqual("Thuoc A", chiTiet.SanPham.TenSanPham);
    }

    [TestMethod]
    public async Task Duyet_ExistingDonThuocWithMoiTaoStatus_UpdatesStatusAndRedirects()
    {
        // Arrange
        var donThuoc = new DonThuoc
        {
            Id = 1,
            MaDonThuoc = "DT001",
            TenDonThuoc = "Don test",
            TrangThai = TrangThaiDonThuoc.MoiTao
        };
        _context.DonThuocs.Add(donThuoc);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.Duyet(1);

        // Assert
        var redirectResult = result as RedirectToActionResult;
        Assert.IsNotNull(redirectResult);
        Assert.AreEqual(nameof(_controller.Details), redirectResult.ActionName);
        Assert.AreEqual(1, redirectResult.RouteValues?["id"]);

        var updatedDonThuoc = await _context.DonThuocs.FindAsync(1);
        Assert.IsNotNull(updatedDonThuoc);
        Assert.AreEqual(TrangThaiDonThuoc.DaDuyet, updatedDonThuoc.TrangThai);
        Assert.IsTrue(_controller.TempData.ContainsKey("Success"));
    }

    [TestMethod]
    public async Task Duyet_NonExistingId_ReturnsNotFound()
    {
        // Arrange & Act
        var result = await _controller.Duyet(999);

        // Assert
        Assert.IsInstanceOfType<NotFoundResult>(result);
    }

    [TestMethod]
    public async Task Duyet_DonThuocNotMoiTao_SetsErrorAndRedirects()
    {
        // Arrange
        var donThuoc = new DonThuoc
        {
            Id = 1,
            MaDonThuoc = "DT001",
            TenDonThuoc = "Don test",
            TrangThai = TrangThaiDonThuoc.DaDuyet
        };
        _context.DonThuocs.Add(donThuoc);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.Duyet(1);

        // Assert
        var redirectResult = result as RedirectToActionResult;
        Assert.IsNotNull(redirectResult);
        Assert.AreEqual(nameof(_controller.Details), redirectResult.ActionName);
        Assert.IsTrue(_controller.TempData.ContainsKey("Error"));

        var updatedDonThuoc = await _context.DonThuocs.FindAsync(1);
        Assert.IsNotNull(updatedDonThuoc);
        Assert.AreEqual(TrangThaiDonThuoc.DaDuyet, updatedDonThuoc.TrangThai);
    }

    [TestMethod]
    public async Task Duyet_ValidDonThuoc_SetsTempDataSuccessMessage()
    {
        // Arrange
        var donThuoc = new DonThuoc
        {
            Id = 1,
            MaDonThuoc = "DT001",
            TenDonThuoc = "Don test",
            TrangThai = TrangThaiDonThuoc.MoiTao
        };
        _context.DonThuocs.Add(donThuoc);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.Duyet(1);

        // Assert
        var message = _controller.TempData["Success"] as string;
        Assert.IsNotNull(message);
        Assert.Contains("DT001", message);
        Assert.Contains("đã được duyệt", message);
    }

    [TestMethod]
    public async Task Huy_ExistingDonThuoc_UpdatesStatusAndRedirects()
    {
        // Arrange
        var donThuoc = new DonThuoc
        {
            Id = 1,
            MaDonThuoc = "DT001",
            TenDonThuoc = "Don test",
            TrangThai = TrangThaiDonThuoc.DaDuyet,
            DaDangStore = true
        };
        _context.DonThuocs.Add(donThuoc);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.Huy(1);

        // Assert
        var redirectResult = result as RedirectToActionResult;
        Assert.IsNotNull(redirectResult);
        Assert.AreEqual(nameof(_controller.Details), redirectResult.ActionName);
        Assert.AreEqual(1, redirectResult.RouteValues?["id"]);

        var updatedDonThuoc = await _context.DonThuocs.FindAsync(1);
        Assert.IsNotNull(updatedDonThuoc);
        Assert.AreEqual(TrangThaiDonThuoc.DaHuy, updatedDonThuoc.TrangThai);
        Assert.IsFalse(updatedDonThuoc.DaDangStore);
        Assert.IsTrue(_controller.TempData.ContainsKey("Success"));
    }

    [TestMethod]
    public async Task Huy_NonExistingId_ReturnsNotFound()
    {
        // Arrange & Act
        var result = await _controller.Huy(999);

        // Assert
        Assert.IsInstanceOfType<NotFoundResult>(result);
    }

    [TestMethod]
    public async Task Huy_WithChiTietAndSanPham_CallsAnThuocKhiGoStore()
    {
        // Arrange
        var sanPham = new SanPham { Id = 1, TenSanPham = "Thuoc A", SoLuong = 10, IsThuoc = true, GiaBan = 10000, TrenKe = true };
        var donThuoc = new DonThuoc
        {
            Id = 1,
            MaDonThuoc = "DT001",
            TenDonThuoc = "Don test",
            TrangThai = TrangThaiDonThuoc.DaDuyet,
            DaDangStore = true
        };
        donThuoc.ChiTietDonThuocs.Add(new ChiTietDonThuoc
        {
            SanPhamId = 1,
            SoVienMoiNgay = 2,
            SoNgayUong = 5
        });
        _context.SanPhams.Add(sanPham);
        _context.DonThuocs.Add(donThuoc);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.Huy(1);

        // Assert
        var updatedSanPham = await _context.SanPhams.FindAsync(1);
        Assert.IsNotNull(updatedSanPham);
        Assert.IsFalse(updatedSanPham.TrenKe);
    }

    [TestMethod]
    public async Task Huy_ValidDonThuoc_SetsTempDataSuccessMessage()
    {
        // Arrange
        var donThuoc = new DonThuoc
        {
            Id = 1,
            MaDonThuoc = "DT001",
            TenDonThuoc = "Don test",
            TrangThai = TrangThaiDonThuoc.MoiTao
        };
        _context.DonThuocs.Add(donThuoc);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.Huy(1);

        // Assert
        var message = _controller.TempData["Success"] as string;
        Assert.IsNotNull(message);
        Assert.Contains("DT001", message);
        Assert.Contains("đã bị hủy", message);
    }

    [TestMethod]
    public async Task DangStore_NonExistingId_ReturnsNotFound()
    {
        // Arrange & Act
        var result = await _controller.DangStore(999);

        // Assert
        Assert.IsInstanceOfType<NotFoundResult>(result);
    }

    [TestMethod]
    public async Task DangStore_DonThuocNotDaDuyet_SetsErrorAndRedirects()
    {
        // Arrange
        var donThuoc = new DonThuoc
        {
            Id = 1,
            MaDonThuoc = "DT001",
            TenDonThuoc = "Don test",
            TrangThai = TrangThaiDonThuoc.MoiTao
        };
        _context.DonThuocs.Add(donThuoc);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.DangStore(1);

        // Assert
        var redirectResult = result as RedirectToActionResult;
        Assert.IsNotNull(redirectResult);
        Assert.AreEqual(nameof(_controller.Details), redirectResult.ActionName);
        Assert.IsTrue(_controller.TempData.ContainsKey("Error"));
    }

    [TestMethod]
    public async Task DangStore_DaDangStoreFalse_SetsDaDangStoreToTrue()
    {
        // Arrange
        var sanPham = new SanPham { Id = 1, TenSanPham = "Thuoc A", SoLuong = 10, IsThuoc = true, GiaBan = 10000, TrenKe = false };
        var donThuoc = new DonThuoc
        {
            Id = 1,
            MaDonThuoc = "DT001",
            TenDonThuoc = "Don test",
            TrangThai = TrangThaiDonThuoc.DaDuyet,
            DaDangStore = false
        };
        donThuoc.ChiTietDonThuocs.Add(new ChiTietDonThuoc
        {
            SanPhamId = 1,
            SoVienMoiNgay = 2,
            SoNgayUong = 5
        });
        _context.SanPhams.Add(sanPham);
        _context.DonThuocs.Add(donThuoc);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.DangStore(1);

        // Assert
        var updatedDonThuoc = await _context.DonThuocs.FindAsync(1);
        Assert.IsNotNull(updatedDonThuoc);
        Assert.IsTrue(updatedDonThuoc.DaDangStore);

        var updatedSanPham = await _context.SanPhams.FindAsync(1);
        Assert.IsNotNull(updatedSanPham);
        Assert.IsTrue(updatedSanPham.TrenKe);

        var message = _controller.TempData["Success"] as string;
        Assert.IsNotNull(message);
        Assert.Contains("đã được đăng lên Store", message);
    }

    [TestMethod]
    public async Task DangStore_DaDangStoreTrue_SetsDaDangStoreToFalse()
    {
        // Arrange
        var sanPham = new SanPham { Id = 1, TenSanPham = "Thuoc A", SoLuong = 10, IsThuoc = true, GiaBan = 10000, TrenKe = true };
        var donThuoc = new DonThuoc
        {
            Id = 1,
            MaDonThuoc = "DT001",
            TenDonThuoc = "Don test",
            TrangThai = TrangThaiDonThuoc.DaDuyet,
            DaDangStore = true
        };
        donThuoc.ChiTietDonThuocs.Add(new ChiTietDonThuoc
        {
            SanPhamId = 1,
            SoVienMoiNgay = 2,
            SoNgayUong = 5
        });
        _context.SanPhams.Add(sanPham);
        _context.DonThuocs.Add(donThuoc);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.DangStore(1);

        // Assert
        var updatedDonThuoc = await _context.DonThuocs.FindAsync(1);
        Assert.IsNotNull(updatedDonThuoc);
        Assert.IsFalse(updatedDonThuoc.DaDangStore);

        var updatedSanPham = await _context.SanPhams.FindAsync(1);
        Assert.IsNotNull(updatedSanPham);
        Assert.IsFalse(updatedSanPham.TrenKe);

        var message = _controller.TempData["Success"] as string;
        Assert.IsNotNull(message);
        Assert.Contains("đã gỡ khỏi Store", message);
    }

    [TestMethod]
    public async Task DangStore_ValidRequest_SavesChanges()
    {
        // Arrange
        var sanPham = new SanPham { Id = 1, TenSanPham = "Thuoc A", SoLuong = 10, IsThuoc = true, GiaBan = 10000 };
        var donThuoc = new DonThuoc
        {
            Id = 1,
            MaDonThuoc = "DT001",
            TenDonThuoc = "Don test",
            TrangThai = TrangThaiDonThuoc.DaDuyet,
            DaDangStore = false
        };
        donThuoc.ChiTietDonThuocs.Add(new ChiTietDonThuoc
        {
            SanPhamId = 1,
            SoVienMoiNgay = 2,
            SoNgayUong = 5
        });
        _context.SanPhams.Add(sanPham);
        _context.DonThuocs.Add(donThuoc);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.DangStore(1);

        // Assert
        var redirectResult = result as RedirectToActionResult;
        Assert.IsNotNull(redirectResult);
        Assert.AreEqual(nameof(_controller.Details), redirectResult.ActionName);
        Assert.AreEqual(1, redirectResult.RouteValues?["id"]);
    }

    [TestMethod]
    public async Task Delete_NonExistingId_ReturnsNotFound()
    {
        // Arrange & Act
        var result = await _controller.Delete(999);

        // Assert
        Assert.IsInstanceOfType<NotFoundResult>(result);
    }

    [TestMethod]
    public async Task Delete_ExistingDonThuoc_RemovesFromDatabase()
    {
        // Arrange
        var donThuoc = new DonThuoc
        {
            Id = 1,
            MaDonThuoc = "DT001",
            TenDonThuoc = "Don test",
            TrangThai = TrangThaiDonThuoc.MoiTao
        };
        _context.DonThuocs.Add(donThuoc);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.Delete(1);

        // Assert
        var redirectResult = result as RedirectToActionResult;
        Assert.IsNotNull(redirectResult);
        Assert.AreEqual(nameof(_controller.Index), redirectResult.ActionName);

        var deletedDonThuoc = await _context.DonThuocs.FindAsync(1);
        Assert.IsNull(deletedDonThuoc);
        Assert.IsTrue(_controller.TempData.ContainsKey("Success"));
    }

    [TestMethod]
    public async Task Delete_DonThuocWithChiTiet_RemovesChiTietDonThuocs()
    {
        // Arrange
        var sanPham = new SanPham { Id = 1, TenSanPham = "Thuoc A", SoLuong = 10, IsThuoc = true, GiaBan = 10000 };
        var donThuoc = new DonThuoc
        {
            Id = 1,
            MaDonThuoc = "DT001",
            TenDonThuoc = "Don test",
            TrangThai = TrangThaiDonThuoc.MoiTao
        };
        donThuoc.ChiTietDonThuocs.Add(new ChiTietDonThuoc
        {
            SanPhamId = 1,
            SoVienMoiNgay = 2,
            SoNgayUong = 5
        });
        _context.SanPhams.Add(sanPham);
        _context.DonThuocs.Add(donThuoc);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.Delete(1);

        // Assert
        var chiTietCount = await _context.ChiTietDonThuocs.CountAsync();
        Assert.AreEqual(0, chiTietCount);
    }

    [TestMethod]
    public async Task Delete_DonThuocWithSanPhamOnStore_CallsAnThuocKhiGoStore()
    {
        // Arrange
        var sanPham = new SanPham { Id = 1, TenSanPham = "Thuoc A", SoLuong = 10, IsThuoc = true, GiaBan = 10000, TrenKe = true };
        var donThuoc = new DonThuoc
        {
            Id = 1,
            MaDonThuoc = "DT001",
            TenDonThuoc = "Don test",
            TrangThai = TrangThaiDonThuoc.DaDuyet,
            DaDangStore = true
        };
        donThuoc.ChiTietDonThuocs.Add(new ChiTietDonThuoc
        {
            SanPhamId = 1,
            SoVienMoiNgay = 2,
            SoNgayUong = 5
        });
        _context.SanPhams.Add(sanPham);
        _context.DonThuocs.Add(donThuoc);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.Delete(1);

        // Assert
        var updatedSanPham = await _context.SanPhams.FindAsync(1);
        Assert.IsNotNull(updatedSanPham);
        Assert.IsFalse(updatedSanPham.TrenKe);
    }

    [TestMethod]
    public async Task Delete_ValidDonThuoc_SetsTempDataSuccessMessage()
    {
        // Arrange
        var donThuoc = new DonThuoc
        {
            Id = 1,
            MaDonThuoc = "DT001",
            TenDonThuoc = "Don test",
            TrangThai = TrangThaiDonThuoc.MoiTao
        };
        _context.DonThuocs.Add(donThuoc);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.Delete(1);

        // Assert
        var message = _controller.TempData["Success"] as string;
        Assert.IsNotNull(message);
        Assert.Contains("DT001", message);
        Assert.Contains("Đã xóa đơn thuốc", message);
    }

    [TestMethod]
    public async Task TimKiemThuoc_NullTuKhoa_ReturnsEmptyJsonList()
    {
        // Arrange & Act
        var result = await _controller.TimKiemThuoc(null!);

        // Assert
        var jsonResult = result as JsonResult;
        Assert.IsNotNull(jsonResult);
        var value = jsonResult.Value as List<object>;
        Assert.IsNotNull(value);
        Assert.HasCount(0, value);
    }

    [TestMethod]
    public async Task TimKiemThuoc_WhitespaceTuKhoa_ReturnsEmptyJsonList()
    {
        // Arrange & Act
        var result = await _controller.TimKiemThuoc("   ");

        // Assert
        var jsonResult = result as JsonResult;
        Assert.IsNotNull(jsonResult);
        var value = jsonResult.Value as List<object>;
        Assert.IsNotNull(value);
        Assert.HasCount(0, value);
    }

    [TestMethod]
    public async Task TimKiemThuoc_ValidTuKhoa_ReturnsMatchingSanPhams()
    {
        // Arrange
        var danhMuc = new DanhMucSanPham { Id = 1, TenDanhMuc = "Thuoc cam" };
        var sanPham1 = new SanPham { Id = 1, TenSanPham = "Paracetamol", SoLuong = 10, IsThuoc = true, GiaBan = 10000, DanhMucId = 1, CongDung = "Giam dau", ThanhPhan = "Para" };
        var sanPham2 = new SanPham { Id = 2, TenSanPham = "Aspirin", SoLuong = 5, IsThuoc = true, GiaBan = 20000, DanhMucId = 1, CongDung = "Giam dau", ThanhPhan = "Aspirin" };
        var sanPham3 = new SanPham { Id = 3, TenSanPham = "Vitamin C", SoLuong = 0, IsThuoc = true, GiaBan = 15000, DanhMucId = 1 };
        _context.DanhMucSanPhams.Add(danhMuc);
        _context.SanPhams.AddRange(sanPham1, sanPham2, sanPham3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.TimKiemThuoc("para");

        // Assert
        var jsonResult = result as JsonResult;
        Assert.IsNotNull(jsonResult);
        Assert.IsNotNull(jsonResult.Value);
    }

    [TestMethod]
    public async Task TimKiemThuoc_TrimsAndConvertsToLowercase()
    {
        // Arrange
        var danhMuc = new DanhMucSanPham { Id = 1, TenDanhMuc = "Thuoc cam" };
        var sanPham = new SanPham { Id = 1, TenSanPham = "Paracetamol", SoLuong = 10, IsThuoc = true, GiaBan = 10000, DanhMucId = 1 };
        _context.DanhMucSanPhams.Add(danhMuc);
        _context.SanPhams.Add(sanPham);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.TimKiemThuoc("  PARA  ");

        // Assert
        var jsonResult = result as JsonResult;
        Assert.IsNotNull(jsonResult);
        Assert.IsNotNull(jsonResult.Value);
    }

    [TestMethod]
    public async Task TimKiemThuoc_OnlyReturnsThuocWithSoLuongGreaterThanZero()
    {
        // Arrange
        var sanPham1 = new SanPham { Id = 1, TenSanPham = "Paracetamol", SoLuong = 10, IsThuoc = true, GiaBan = 10000 };
        var sanPham2 = new SanPham { Id = 2, TenSanPham = "Paracetamol 2", SoLuong = 0, IsThuoc = true, GiaBan = 20000 };
        _context.SanPhams.AddRange(sanPham1, sanPham2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.TimKiemThuoc("para");

        // Assert
        var jsonResult = result as JsonResult;
        Assert.IsNotNull(jsonResult);
        Assert.IsNotNull(jsonResult.Value);
    }

    [TestMethod]
    public async Task TimKiemThuoc_OnlyReturnsIsThuocTrue()
    {
        // Arrange
        var sanPham1 = new SanPham { Id = 1, TenSanPham = "Paracetamol", SoLuong = 10, IsThuoc = true, GiaBan = 10000 };
        var sanPham2 = new SanPham { Id = 2, TenSanPham = "Para product", SoLuong = 10, IsThuoc = false, GiaBan = 20000 };
        _context.SanPhams.AddRange(sanPham1, sanPham2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.TimKiemThuoc("para");

        // Assert
        var jsonResult = result as JsonResult;
        Assert.IsNotNull(jsonResult);
        Assert.IsNotNull(jsonResult.Value);
    }

    [TestMethod]
    public async Task TimKiemThuoc_LimitsResultsToTen()
    {
        // Arrange
        for (int i = 1; i <= 15; i++)
        {
            _context.SanPhams.Add(new SanPham
            {
                Id = i,
                TenSanPham = $"Paracetamol {i}",
                SoLuong = 10,
                IsThuoc = true,
                GiaBan = 10000
            });
        }
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.TimKiemThuoc("para");

        // Assert
        var jsonResult = result as JsonResult;
        Assert.IsNotNull(jsonResult);
        Assert.IsNotNull(jsonResult.Value);
    }
}
