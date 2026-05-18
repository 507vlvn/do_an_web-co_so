using do_an.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace do_an.UnitTests.Models;

[TestClass]
public class SanPhamTests
{
    [TestMethod]
    public void PhanTramGiam_WithGiaGocNull_ReturnsNull()
    {
        // Arrange
        var sanPham = new SanPham
        {
            GiaBan = 100m,
            GiaGoc = null
        };

        // Act
        var result = sanPham.PhanTramGiam;

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void PhanTramGiam_WithGiaGocZero_ReturnsNull()
    {
        // Arrange
        var sanPham = new SanPham
        {
            GiaBan = 100m,
            GiaGoc = 0m
        };

        // Act
        var result = sanPham.PhanTramGiam;

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void PhanTramGiam_WithGiaGocNegative_ReturnsNull()
    {
        // Arrange
        var sanPham = new SanPham
        {
            GiaBan = 100m,
            GiaGoc = -50m
        };

        // Act
        var result = sanPham.PhanTramGiam;

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void PhanTramGiam_WithGiaGocEqualToGiaBan_ReturnsNull()
    {
        // Arrange
        var sanPham = new SanPham
        {
            GiaBan = 100m,
            GiaGoc = 100m
        };

        // Act
        var result = sanPham.PhanTramGiam;

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void PhanTramGiam_WithGiaGocLessThanGiaBan_ReturnsNull()
    {
        // Arrange
        var sanPham = new SanPham
        {
            GiaBan = 150m,
            GiaGoc = 100m
        };

        // Act
        var result = sanPham.PhanTramGiam;

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void PhanTramGiam_WithValidDiscount_ReturnsCorrectPercentage()
    {
        // Arrange
        var sanPham = new SanPham
        {
            GiaBan = 80m,
            GiaGoc = 100m
        };

        // Act
        var result = sanPham.PhanTramGiam;

        // Assert
        Assert.AreEqual(20, result);
    }

    [TestMethod]
    public void PhanTramGiam_WithFiftyPercentDiscount_ReturnsCorrectPercentage()
    {
        // Arrange
        var sanPham = new SanPham
        {
            GiaBan = 50m,
            GiaGoc = 100m
        };

        // Act
        var result = sanPham.PhanTramGiam;

        // Assert
        Assert.AreEqual(50, result);
    }

    [TestMethod]
    public void PhanTramGiam_WithRoundingDown_ReturnsRoundedPercentage()
    {
        // Arrange
        var sanPham = new SanPham
        {
            GiaBan = 66.67m,
            GiaGoc = 100m
        };

        // Act
        var result = sanPham.PhanTramGiam;

        // Assert
        Assert.AreEqual(33, result);
    }

    [TestMethod]
    public void PhanTramGiam_WithRoundingUp_ReturnsRoundedPercentage()
    {
        // Arrange
        var sanPham = new SanPham
        {
            GiaBan = 66m,
            GiaGoc = 100m
        };

        // Act
        var result = sanPham.PhanTramGiam;

        // Assert
        Assert.AreEqual(34, result);
    }

    [TestMethod]
    public void PhanTramGiam_WithExactHalfRounding_ReturnsRoundedPercentage()
    {
        // Arrange
        var sanPham = new SanPham
        {
            GiaBan = 75m,
            GiaGoc = 100m
        };

        // Act
        var result = sanPham.PhanTramGiam;

        // Assert
        Assert.AreEqual(25, result);
    }

    [TestMethod]
    public void PhanTramGiam_WithGiaBanZero_ReturnsOneHundredPercent()
    {
        // Arrange
        var sanPham = new SanPham
        {
            GiaBan = 0m,
            GiaGoc = 100m
        };

        // Act
        var result = sanPham.PhanTramGiam;

        // Assert
        Assert.AreEqual(100, result);
    }

    [TestMethod]
    public void PhanTramGiam_WithVerySmallDiscount_ReturnsCorrectPercentage()
    {
        // Arrange
        var sanPham = new SanPham
        {
            GiaBan = 99.5m,
            GiaGoc = 100m
        };

        // Act
        var result = sanPham.PhanTramGiam;

        // Assert
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void PhanTramGiam_WithVerySmallDiscountRoundingUp_ReturnsCorrectPercentage()
    {
        // Arrange
        var sanPham = new SanPham
        {
            GiaBan = 99.4m,
            GiaGoc = 100m
        };

        // Act
        var result = sanPham.PhanTramGiam;

        // Assert
        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void PhanTramGiam_WithLargeValues_ReturnsCorrectPercentage()
    {
        // Arrange
        var sanPham = new SanPham
        {
            GiaBan = 8000m,
            GiaGoc = 10000m
        };

        // Act
        var result = sanPham.PhanTramGiam;

        // Assert
        Assert.AreEqual(20, result);
    }

    [TestMethod]
    public void PhanTramGiam_WithDecimalValues_ReturnsCorrectPercentage()
    {
        // Arrange
        var sanPham = new SanPham
        {
            GiaBan = 123.45m,
            GiaGoc = 200m
        };

        // Act
        var result = sanPham.PhanTramGiam;

        // Assert
        Assert.AreEqual(38, result);
    }
}
