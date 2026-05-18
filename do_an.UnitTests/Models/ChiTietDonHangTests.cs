using do_an.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace do_an.UnitTests.Models;

[TestClass]
public class ChiTietDonHangTests
{
    [TestMethod]
    public void ThanhTien_WithPositiveValues_ReturnsCorrectTotal()
    {
        // Arrange
        var chiTietDonHang = new ChiTietDonHang
        {
            GiaBan = 100.50m,
            SoLuong = 3
        };

        // Act
        var result = chiTietDonHang.ThanhTien;

        // Assert
        Assert.AreEqual(301.50m, result);
    }

    [TestMethod]
    public void ThanhTien_WithZeroSoLuong_ReturnsZero()
    {
        // Arrange
        var chiTietDonHang = new ChiTietDonHang
        {
            GiaBan = 100.50m,
            SoLuong = 0
        };

        // Act
        var result = chiTietDonHang.ThanhTien;

        // Assert
        Assert.AreEqual(0m, result);
    }

    [TestMethod]
    public void ThanhTien_WithZeroGiaBan_ReturnsZero()
    {
        // Arrange
        var chiTietDonHang = new ChiTietDonHang
        {
            GiaBan = 0m,
            SoLuong = 5
        };

        // Act
        var result = chiTietDonHang.ThanhTien;

        // Assert
        Assert.AreEqual(0m, result);
    }

    [TestMethod]
    public void ThanhTien_WithBothZero_ReturnsZero()
    {
        // Arrange
        var chiTietDonHang = new ChiTietDonHang
        {
            GiaBan = 0m,
            SoLuong = 0
        };

        // Act
        var result = chiTietDonHang.ThanhTien;

        // Assert
        Assert.AreEqual(0m, result);
    }

    [TestMethod]
    public void ThanhTien_WithLargeValues_ReturnsCorrectTotal()
    {
        // Arrange
        var chiTietDonHang = new ChiTietDonHang
        {
            GiaBan = 999999.99m,
            SoLuong = 100
        };

        // Act
        var result = chiTietDonHang.ThanhTien;

        // Assert
        Assert.AreEqual(99999999.00m, result);
    }

    [TestMethod]
    public void ThanhTien_WithDecimalPrecision_ReturnsCorrectTotal()
    {
        // Arrange
        var chiTietDonHang = new ChiTietDonHang
        {
            GiaBan = 10.99m,
            SoLuong = 7
        };

        // Act
        var result = chiTietDonHang.ThanhTien;

        // Assert
        Assert.AreEqual(76.93m, result);
    }

    [TestMethod]
    public void ThanhTien_WithOneSoLuong_ReturnsGiaBan()
    {
        // Arrange
        var chiTietDonHang = new ChiTietDonHang
        {
            GiaBan = 250.75m,
            SoLuong = 1
        };

        // Act
        var result = chiTietDonHang.ThanhTien;

        // Assert
        Assert.AreEqual(250.75m, result);
    }

    [TestMethod]
    public void ThanhTien_WithSmallDecimalValues_ReturnsCorrectTotal()
    {
        // Arrange
        var chiTietDonHang = new ChiTietDonHang
        {
            GiaBan = 0.01m,
            SoLuong = 10
        };

        // Act
        var result = chiTietDonHang.ThanhTien;

        // Assert
        Assert.AreEqual(0.10m, result);
    }

    [TestMethod]
    public void ThanhTien_WithDifferentValues_ReturnsCorrectTotal()
    {
        // Arrange
        var chiTietDonHang = new ChiTietDonHang
        {
            GiaBan = 45.25m,
            SoLuong = 12
        };

        // Act
        var result = chiTietDonHang.ThanhTien;

        // Assert
        Assert.AreEqual(543.00m, result);
    }

    [TestMethod]
    public void ThanhTien_WithHighQuantity_ReturnsCorrectTotal()
    {
        // Arrange
        var chiTietDonHang = new ChiTietDonHang
        {
            GiaBan = 15.50m,
            SoLuong = 1000
        };

        // Act
        var result = chiTietDonHang.ThanhTien;

        // Assert
        Assert.AreEqual(15500.00m, result);
    }
}
