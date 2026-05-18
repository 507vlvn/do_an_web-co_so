using do_an.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace do_an.UnitTests.Models;

[TestClass]
public class DonHangTests
{
    [TestMethod]
    public void TongThanhToan_WithPositiveValues_ReturnsCorrectTotal()
    {
        // Arrange
        var donHang = new DonHang
        {
            TongTienHang = 100.50m,
            TienGiam = 20.25m
        };

        // Act
        var result = donHang.TongThanhToan;

        // Assert
        Assert.AreEqual(80.25m, result);
    }

    [TestMethod]
    public void TongThanhToan_WithZeroTongTienHang_ReturnsNegativeTienGiam()
    {
        // Arrange
        var donHang = new DonHang
        {
            TongTienHang = 0m,
            TienGiam = 50.00m
        };

        // Act
        var result = donHang.TongThanhToan;

        // Assert
        Assert.AreEqual(-50.00m, result);
    }

    [TestMethod]
    public void TongThanhToan_WithZeroTienGiam_ReturnsTongTienHang()
    {
        // Arrange
        var donHang = new DonHang
        {
            TongTienHang = 150.75m,
            TienGiam = 0m
        };

        // Act
        var result = donHang.TongThanhToan;

        // Assert
        Assert.AreEqual(150.75m, result);
    }

    [TestMethod]
    public void TongThanhToan_WithBothZero_ReturnsZero()
    {
        // Arrange
        var donHang = new DonHang
        {
            TongTienHang = 0m,
            TienGiam = 0m
        };

        // Act
        var result = donHang.TongThanhToan;

        // Assert
        Assert.AreEqual(0m, result);
    }

    [TestMethod]
    public void TongThanhToan_WithTienGiamGreaterThanTongTienHang_ReturnsNegativeValue()
    {
        // Arrange
        var donHang = new DonHang
        {
            TongTienHang = 50.00m,
            TienGiam = 75.00m
        };

        // Act
        var result = donHang.TongThanhToan;

        // Assert
        Assert.AreEqual(-25.00m, result);
    }

    [TestMethod]
    public void TongThanhToan_WithLargeValues_ReturnsCorrectTotal()
    {
        // Arrange
        var donHang = new DonHang
        {
            TongTienHang = 999999.99m,
            TienGiam = 100000.00m
        };

        // Act
        var result = donHang.TongThanhToan;

        // Assert
        Assert.AreEqual(899999.99m, result);
    }

    [TestMethod]
    public void TongThanhToan_WithDecimalPrecision_ReturnsCorrectTotal()
    {
        // Arrange
        var donHang = new DonHang
        {
            TongTienHang = 123.45m,
            TienGiam = 23.45m
        };

        // Act
        var result = donHang.TongThanhToan;

        // Assert
        Assert.AreEqual(100.00m, result);
    }
}
