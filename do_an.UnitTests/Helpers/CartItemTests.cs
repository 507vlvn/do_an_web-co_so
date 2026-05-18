using System.Text;
using System.Text.Json;
using do_an.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace do_an.UnitTests;

[TestClass]
public class CartItemTests
{
    [TestMethod]
    public void ThanhTien_WithPositiveValues_ReturnsProduct()
    {
        // Arrange
        var cartItem = new CartItem
        {
            GiaBan = 100.5m,
            SoLuong = 3
        };

        // Act
        var result = cartItem.ThanhTien;

        // Assert
        Assert.AreEqual(301.5m, result);
    }

    [TestMethod]
    public void ThanhTien_WithZeroQuantity_ReturnsZero()
    {
        // Arrange
        var cartItem = new CartItem
        {
            GiaBan = 100m,
            SoLuong = 0
        };

        // Act
        var result = cartItem.ThanhTien;

        // Assert
        Assert.AreEqual(0m, result);
    }

    [TestMethod]
    public void ThanhTien_WithZeroPrice_ReturnsZero()
    {
        // Arrange
        var cartItem = new CartItem
        {
            GiaBan = 0m,
            SoLuong = 10
        };

        // Act
        var result = cartItem.ThanhTien;

        // Assert
        Assert.AreEqual(0m, result);
    }

    [TestMethod]
    public void ThanhTien_WithDecimalValues_ReturnsCorrectProduct()
    {
        // Arrange
        var cartItem = new CartItem
        {
            GiaBan = 10.99m,
            SoLuong = 5
        };

        // Act
        var result = cartItem.ThanhTien;

        // Assert
        Assert.AreEqual(54.95m, result);
    }

    [TestMethod]
    public void GetCart_WithNullSession_ReturnsEmptyList()
    {
        // Arrange
        var mockSession = new Mock<ISession>();
        mockSession.Setup(s => s.GetString("GioHang")).Returns((string?)null);

        // Act
        var result = CartHelper.GetCart(mockSession.Object);

        // Assert
        Assert.IsNotNull(result);
        Assert.HasCount(0, result);
    }

    [TestMethod]
    public void GetCart_WithEmptyCart_ReturnsEmptyList()
    {
        // Arrange
        var mockSession = new Mock<ISession>();
        var emptyCart = new List<CartItem>();
        var json = JsonSerializer.Serialize(emptyCart);
        mockSession.Setup(s => s.GetString("GioHang")).Returns(json);

        // Act
        var result = CartHelper.GetCart(mockSession.Object);

        // Assert
        Assert.IsNotNull(result);
        Assert.HasCount(0, result);
    }

    [TestMethod]
    public void GetCart_WithCartItems_ReturnsListWithItems()
    {
        // Arrange
        var mockSession = new Mock<ISession>();
        var cart = new List<CartItem>
        {
            new() { SanPhamId = 1, TenSanPham = "Product 1", GiaBan = 100m, SoLuong = 2 },
            new() { SanPhamId = 2, TenSanPham = "Product 2", GiaBan = 200m, SoLuong = 1 }
        };
        var json = JsonSerializer.Serialize(cart);
        mockSession.Setup(s => s.GetString("GioHang")).Returns(json);

        // Act
        var result = CartHelper.GetCart(mockSession.Object);

        // Assert
        Assert.IsNotNull(result);
        Assert.HasCount(2, result);
        Assert.AreEqual(1, result[0].SanPhamId);
        Assert.AreEqual("Product 1", result[0].TenSanPham);
        Assert.AreEqual(2, result[1].SanPhamId);
    }

    [TestMethod]
    public void SaveCart_WithEmptyCart_SavesEmptyJson()
    {
        // Arrange
        var mockSession = new Mock<ISession>();
        var cart = new List<CartItem>();
        string? capturedJson = null;
        mockSession.Setup(s => s.SetString("GioHang", It.IsAny<string>()))
            .Callback<string, string>((key, value) => capturedJson = value);

        // Act
        CartHelper.SaveCart(mockSession.Object, cart);

        // Assert
        mockSession.Verify(s => s.SetString("GioHang", It.IsAny<string>()), Times.Once);
        Assert.IsNotNull(capturedJson);
        Assert.AreEqual("[]", capturedJson);
    }

    [TestMethod]
    public void SaveCart_WithCartItems_SavesSerializedJson()
    {
        // Arrange
        var mockSession = new Mock<ISession>();
        var cart = new List<CartItem>
        {
            new() { SanPhamId = 1, TenSanPham = "Product 1", GiaBan = 100m, SoLuong = 2 }
        };
        string? capturedJson = null;
        mockSession.Setup(s => s.SetString("GioHang", It.IsAny<string>()))
            .Callback<string, string>((key, value) => capturedJson = value);

        // Act
        CartHelper.SaveCart(mockSession.Object, cart);

        // Assert
        mockSession.Verify(s => s.SetString("GioHang", It.IsAny<string>()), Times.Once);
        Assert.IsNotNull(capturedJson);
        var deserializedCart = JsonSerializer.Deserialize<List<CartItem>>(capturedJson);
        Assert.IsNotNull(deserializedCart);
        Assert.HasCount(1, deserializedCart);
        Assert.AreEqual(1, deserializedCart[0].SanPhamId);
    }

    [TestMethod]
    public void AddOrUpdate_WithEmptyCart_AddsNewItem()
    {
        // Arrange
        var mockSession = new Mock<ISession>();
        mockSession.Setup(s => s.GetString("GioHang")).Returns((string?)null);
        List<CartItem>? savedCart = null;
        mockSession.Setup(s => s.SetString("GioHang", It.IsAny<string>()))
            .Callback<string, string>((key, value) => savedCart = JsonSerializer.Deserialize<List<CartItem>>(value));

        var newItem = new CartItem { SanPhamId = 1, TenSanPham = "Product 1", GiaBan = 100m, SoLuong = 2 };

        // Act
        CartHelper.AddOrUpdate(mockSession.Object, newItem);

        // Assert
        Assert.IsNotNull(savedCart);
        Assert.HasCount(1, savedCart);
        Assert.AreEqual(1, savedCart[0].SanPhamId);
        Assert.AreEqual(2, savedCart[0].SoLuong);
    }

    [TestMethod]
    public void AddOrUpdate_WithExistingItem_UpdatesQuantity()
    {
        // Arrange
        var mockSession = new Mock<ISession>();
        var existingCart = new List<CartItem>
        {
            new() { SanPhamId = 1, TenSanPham = "Product 1", GiaBan = 100m, SoLuong = 2, DonThuocId = null }
        };
        mockSession.Setup(s => s.GetString("GioHang")).Returns(JsonSerializer.Serialize(existingCart));
        List<CartItem>? savedCart = null;
        mockSession.Setup(s => s.SetString("GioHang", It.IsAny<string>()))
            .Callback<string, string>((key, value) => savedCart = JsonSerializer.Deserialize<List<CartItem>>(value));

        var updateItem = new CartItem { SanPhamId = 1, TenSanPham = "Product 1", GiaBan = 100m, SoLuong = 3, DonThuocId = null };

        // Act
        CartHelper.AddOrUpdate(mockSession.Object, updateItem);

        // Assert
        Assert.IsNotNull(savedCart);
        Assert.HasCount(1, savedCart);
        Assert.AreEqual(1, savedCart[0].SanPhamId);
        Assert.AreEqual(5, savedCart[0].SoLuong);
    }

    [TestMethod]
    public void AddOrUpdate_WithDifferentProduct_AddsNewItem()
    {
        // Arrange
        var mockSession = new Mock<ISession>();
        var existingCart = new List<CartItem>
        {
            new() { SanPhamId = 1, TenSanPham = "Product 1", GiaBan = 100m, SoLuong = 2 }
        };
        mockSession.Setup(s => s.GetString("GioHang")).Returns(JsonSerializer.Serialize(existingCart));
        List<CartItem>? savedCart = null;
        mockSession.Setup(s => s.SetString("GioHang", It.IsAny<string>()))
            .Callback<string, string>((key, value) => savedCart = JsonSerializer.Deserialize<List<CartItem>>(value));

        var newItem = new CartItem { SanPhamId = 2, TenSanPham = "Product 2", GiaBan = 200m, SoLuong = 1 };

        // Act
        CartHelper.AddOrUpdate(mockSession.Object, newItem);

        // Assert
        Assert.IsNotNull(savedCart);
        Assert.HasCount(2, savedCart);
        Assert.AreEqual(1, savedCart[0].SanPhamId);
        Assert.AreEqual(2, savedCart[1].SanPhamId);
    }

    [TestMethod]
    public void AddOrUpdate_WithSameProductDifferentDonThuocId_AddsNewItem()
    {
        // Arrange
        var mockSession = new Mock<ISession>();
        var existingCart = new List<CartItem>
        {
            new() { SanPhamId = 1, TenSanPham = "Product 1", GiaBan = 100m, SoLuong = 2, DonThuocId = 1 }
        };
        mockSession.Setup(s => s.GetString("GioHang")).Returns(JsonSerializer.Serialize(existingCart));
        List<CartItem>? savedCart = null;
        mockSession.Setup(s => s.SetString("GioHang", It.IsAny<string>()))
            .Callback<string, string>((key, value) => savedCart = JsonSerializer.Deserialize<List<CartItem>>(value));

        var newItem = new CartItem { SanPhamId = 1, TenSanPham = "Product 1", GiaBan = 100m, SoLuong = 3, DonThuocId = 2 };

        // Act
        CartHelper.AddOrUpdate(mockSession.Object, newItem);

        // Assert
        Assert.IsNotNull(savedCart);
        Assert.HasCount(2, savedCart);
        Assert.AreEqual(1, savedCart[0].DonThuocId);
        Assert.AreEqual(2, savedCart[1].DonThuocId);
    }

    [TestMethod]
    public void AddOrUpdate_WithSameProductAndDonThuocId_UpdatesQuantity()
    {
        // Arrange
        var mockSession = new Mock<ISession>();
        var existingCart = new List<CartItem>
        {
            new() { SanPhamId = 1, TenSanPham = "Product 1", GiaBan = 100m, SoLuong = 2, DonThuocId = 1 }
        };
        mockSession.Setup(s => s.GetString("GioHang")).Returns(JsonSerializer.Serialize(existingCart));
        List<CartItem>? savedCart = null;
        mockSession.Setup(s => s.SetString("GioHang", It.IsAny<string>()))
            .Callback<string, string>((key, value) => savedCart = JsonSerializer.Deserialize<List<CartItem>>(value));

        var updateItem = new CartItem { SanPhamId = 1, TenSanPham = "Product 1", GiaBan = 100m, SoLuong = 3, DonThuocId = 1 };

        // Act
        CartHelper.AddOrUpdate(mockSession.Object, updateItem);

        // Assert
        Assert.IsNotNull(savedCart);
        Assert.HasCount(1, savedCart);
        Assert.AreEqual(5, savedCart[0].SoLuong);
    }

    [TestMethod]
    public void UpdateQuantity_WithNonExistentItem_DoesNothing()
    {
        // Arrange
        var mockSession = new Mock<ISession>();
        var existingCart = new List<CartItem>
        {
            new() { SanPhamId = 1, TenSanPham = "Product 1", GiaBan = 100m, SoLuong = 2 }
        };
        mockSession.Setup(s => s.GetString("GioHang")).Returns(JsonSerializer.Serialize(existingCart));
        List<CartItem>? savedCart = null;
        mockSession.Setup(s => s.SetString("GioHang", It.IsAny<string>()))
            .Callback<string, string>((key, value) => savedCart = JsonSerializer.Deserialize<List<CartItem>>(value));

        // Act
        CartHelper.UpdateQuantity(mockSession.Object, 999, 5);

        // Assert
        mockSession.Verify(s => s.SetString(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [TestMethod]
    public void UpdateQuantity_WithPositiveQuantity_UpdatesItemQuantity()
    {
        // Arrange
        var mockSession = new Mock<ISession>();
        var existingCart = new List<CartItem>
        {
            new() { SanPhamId = 1, TenSanPham = "Product 1", GiaBan = 100m, SoLuong = 2 }
        };
        mockSession.Setup(s => s.GetString("GioHang")).Returns(JsonSerializer.Serialize(existingCart));
        List<CartItem>? savedCart = null;
        mockSession.Setup(s => s.SetString("GioHang", It.IsAny<string>()))
            .Callback<string, string>((key, value) => savedCart = JsonSerializer.Deserialize<List<CartItem>>(value));

        // Act
        CartHelper.UpdateQuantity(mockSession.Object, 1, 5);

        // Assert
        Assert.IsNotNull(savedCart);
        Assert.HasCount(1, savedCart);
        Assert.AreEqual(5, savedCart[0].SoLuong);
    }

    [TestMethod]
    public void UpdateQuantity_WithZeroQuantity_RemovesItem()
    {
        // Arrange
        var mockSession = new Mock<ISession>();
        var existingCart = new List<CartItem>
        {
            new() { SanPhamId = 1, TenSanPham = "Product 1", GiaBan = 100m, SoLuong = 2 },
            new() { SanPhamId = 2, TenSanPham = "Product 2", GiaBan = 200m, SoLuong = 1 }
        };
        mockSession.Setup(s => s.GetString("GioHang")).Returns(JsonSerializer.Serialize(existingCart));
        List<CartItem>? savedCart = null;
        mockSession.Setup(s => s.SetString("GioHang", It.IsAny<string>()))
            .Callback<string, string>((key, value) => savedCart = JsonSerializer.Deserialize<List<CartItem>>(value));

        // Act
        CartHelper.UpdateQuantity(mockSession.Object, 1, 0);

        // Assert
        Assert.IsNotNull(savedCart);
        Assert.HasCount(1, savedCart);
        Assert.AreEqual(2, savedCart[0].SanPhamId);
    }

    [TestMethod]
    public void UpdateQuantity_WithNegativeQuantity_RemovesItem()
    {
        // Arrange
        var mockSession = new Mock<ISession>();
        var existingCart = new List<CartItem>
        {
            new() { SanPhamId = 1, TenSanPham = "Product 1", GiaBan = 100m, SoLuong = 2 },
            new() { SanPhamId = 2, TenSanPham = "Product 2", GiaBan = 200m, SoLuong = 1 }
        };
        mockSession.Setup(s => s.GetString("GioHang")).Returns(JsonSerializer.Serialize(existingCart));
        List<CartItem>? savedCart = null;
        mockSession.Setup(s => s.SetString("GioHang", It.IsAny<string>()))
            .Callback<string, string>((key, value) => savedCart = JsonSerializer.Deserialize<List<CartItem>>(value));

        // Act
        CartHelper.UpdateQuantity(mockSession.Object, 1, -5);

        // Assert
        Assert.IsNotNull(savedCart);
        Assert.HasCount(1, savedCart);
        Assert.AreEqual(2, savedCart[0].SanPhamId);
    }

    [TestMethod]
    public void UpdateQuantity_WithDonThuocIdNull_UpdatesCorrectItem()
    {
        // Arrange
        var mockSession = new Mock<ISession>();
        var existingCart = new List<CartItem>
        {
            new() { SanPhamId = 1, TenSanPham = "Product 1", GiaBan = 100m, SoLuong = 2, DonThuocId = null },
            new() { SanPhamId = 1, TenSanPham = "Product 1", GiaBan = 100m, SoLuong = 3, DonThuocId = 5 }
        };
        mockSession.Setup(s => s.GetString("GioHang")).Returns(JsonSerializer.Serialize(existingCart));
        List<CartItem>? savedCart = null;
        mockSession.Setup(s => s.SetString("GioHang", It.IsAny<string>()))
            .Callback<string, string>((key, value) => savedCart = JsonSerializer.Deserialize<List<CartItem>>(value));

        // Act
        CartHelper.UpdateQuantity(mockSession.Object, 1, 10, null);

        // Assert
        Assert.IsNotNull(savedCart);
        Assert.HasCount(2, savedCart);
        Assert.AreEqual(10, savedCart[0].SoLuong);
        Assert.AreEqual(3, savedCart[1].SoLuong);
    }

    [TestMethod]
    public void UpdateQuantity_WithSpecificDonThuocId_UpdatesCorrectItem()
    {
        // Arrange
        var mockSession = new Mock<ISession>();
        var existingCart = new List<CartItem>
        {
            new() { SanPhamId = 1, TenSanPham = "Product 1", GiaBan = 100m, SoLuong = 2, DonThuocId = null },
            new() { SanPhamId = 1, TenSanPham = "Product 1", GiaBan = 100m, SoLuong = 3, DonThuocId = 5 }
        };
        mockSession.Setup(s => s.GetString("GioHang")).Returns(JsonSerializer.Serialize(existingCart));
        List<CartItem>? savedCart = null;
        mockSession.Setup(s => s.SetString("GioHang", It.IsAny<string>()))
            .Callback<string, string>((key, value) => savedCart = JsonSerializer.Deserialize<List<CartItem>>(value));

        // Act
        CartHelper.UpdateQuantity(mockSession.Object, 1, 10, 5);

        // Assert
        Assert.IsNotNull(savedCart);
        Assert.HasCount(2, savedCart);
        Assert.AreEqual(2, savedCart[0].SoLuong);
        Assert.AreEqual(10, savedCart[1].SoLuong);
    }

    [TestMethod]
    public void UpdateQuantity_RemovingLastItem_SavesEmptyCart()
    {
        // Arrange
        var mockSession = new Mock<ISession>();
        var existingCart = new List<CartItem>
        {
            new() { SanPhamId = 1, TenSanPham = "Product 1", GiaBan = 100m, SoLuong = 2 }
        };
        mockSession.Setup(s => s.GetString("GioHang")).Returns(JsonSerializer.Serialize(existingCart));
        List<CartItem>? savedCart = null;
        mockSession.Setup(s => s.SetString("GioHang", It.IsAny<string>()))
            .Callback<string, string>((key, value) => savedCart = JsonSerializer.Deserialize<List<CartItem>>(value));

        // Act
        CartHelper.UpdateQuantity(mockSession.Object, 1, 0);

        // Assert
        Assert.IsNotNull(savedCart);
        Assert.HasCount(0, savedCart);
    }
}
