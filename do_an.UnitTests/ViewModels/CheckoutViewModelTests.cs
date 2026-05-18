using do_an.Helpers;
using do_an.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace do_an.UnitTests.ViewModels;

[TestClass]
public class CheckoutViewModelTests
{
    [TestMethod]
    public void TongTienHang_EmptyItems_ReturnsZero()
    {
        // Arrange
        var viewModel = new CheckoutViewModel
        {
            Items = new List<CartItem>()
        };

        // Act
        var result = viewModel.TongTienHang;

        // Assert
        Assert.AreEqual(0m, result);
    }

    [TestMethod]
    public void TongTienHang_SingleItem_ReturnsSingleItemThanhTien()
    {
        // Arrange
        var viewModel = new CheckoutViewModel
        {
            Items = new List<CartItem>
            {
                new CartItem { GiaBan = 100m, SoLuong = 2 }
            }
        };

        // Act
        var result = viewModel.TongTienHang;

        // Assert
        Assert.AreEqual(200m, result);
    }

    [TestMethod]
    public void TongTienHang_MultipleItems_ReturnsSumOfAllThanhTien()
    {
        // Arrange
        var viewModel = new CheckoutViewModel
        {
            Items = new List<CartItem>
            {
                new CartItem { GiaBan = 100m, SoLuong = 2 },
                new CartItem { GiaBan = 50m, SoLuong = 3 },
                new CartItem { GiaBan = 200m, SoLuong = 1 }
            }
        };

        // Act
        var result = viewModel.TongTienHang;

        // Assert
        Assert.AreEqual(550m, result);
    }

    [TestMethod]
    public void TongTienHang_ItemsWithZeroQuantity_ReturnsCorrectSum()
    {
        // Arrange
        var viewModel = new CheckoutViewModel
        {
            Items = new List<CartItem>
            {
                new CartItem { GiaBan = 100m, SoLuong = 0 },
                new CartItem { GiaBan = 50m, SoLuong = 2 }
            }
        };

        // Act
        var result = viewModel.TongTienHang;

        // Assert
        Assert.AreEqual(100m, result);
    }

    [TestMethod]
    public void TongTienHang_ItemsWithZeroPrice_ReturnsCorrectSum()
    {
        // Arrange
        var viewModel = new CheckoutViewModel
        {
            Items = new List<CartItem>
            {
                new CartItem { GiaBan = 0m, SoLuong = 5 },
                new CartItem { GiaBan = 100m, SoLuong = 1 }
            }
        };

        // Act
        var result = viewModel.TongTienHang;

        // Assert
        Assert.AreEqual(100m, result);
    }

    [TestMethod]
    public void TongTienHang_AllItemsZero_ReturnsZero()
    {
        // Arrange
        var viewModel = new CheckoutViewModel
        {
            Items = new List<CartItem>
            {
                new CartItem { GiaBan = 0m, SoLuong = 0 },
                new CartItem { GiaBan = 0m, SoLuong = 5 },
                new CartItem { GiaBan = 100m, SoLuong = 0 }
            }
        };

        // Act
        var result = viewModel.TongTienHang;

        // Assert
        Assert.AreEqual(0m, result);
    }

    [TestMethod]
    public void TongTienHang_DecimalPrecision_ReturnsCorrectSum()
    {
        // Arrange
        var viewModel = new CheckoutViewModel
        {
            Items = new List<CartItem>
            {
                new CartItem { GiaBan = 10.50m, SoLuong = 3 },
                new CartItem { GiaBan = 25.75m, SoLuong = 2 }
            }
        };

        // Act
        var result = viewModel.TongTienHang;

        // Assert
        Assert.AreEqual(83m, result);
    }

    [TestMethod]
    public void TongTienHang_LargeQuantity_ReturnsCorrectSum()
    {
        // Arrange
        var viewModel = new CheckoutViewModel
        {
            Items = new List<CartItem>
            {
                new CartItem { GiaBan = 15m, SoLuong = 1000 }
            }
        };

        // Act
        var result = viewModel.TongTienHang;

        // Assert
        Assert.AreEqual(15000m, result);
    }

    [TestMethod]
    public void TongThanhToan_NoDiscount_ReturnsTongTienHang()
    {
        // Arrange
        var viewModel = new CheckoutViewModel
        {
            Items = new List<CartItem>
            {
                new CartItem { GiaBan = 100m, SoLuong = 2 }
            },
            TienGiam = 0m
        };

        // Act
        var result = viewModel.TongThanhToan;

        // Assert
        Assert.AreEqual(200m, result);
    }

    [TestMethod]
    public void TongThanhToan_WithDiscount_ReturnsCorrectAmount()
    {
        // Arrange
        var viewModel = new CheckoutViewModel
        {
            Items = new List<CartItem>
            {
                new CartItem { GiaBan = 100m, SoLuong = 2 }
            },
            TienGiam = 50m
        };

        // Act
        var result = viewModel.TongThanhToan;

        // Assert
        Assert.AreEqual(150m, result);
    }

    [TestMethod]
    public void TongThanhToan_DiscountEqualsTotal_ReturnsZero()
    {
        // Arrange
        var viewModel = new CheckoutViewModel
        {
            Items = new List<CartItem>
            {
                new CartItem { GiaBan = 100m, SoLuong = 2 }
            },
            TienGiam = 200m
        };

        // Act
        var result = viewModel.TongThanhToan;

        // Assert
        Assert.AreEqual(0m, result);
    }

    [TestMethod]
    public void TongThanhToan_DiscountGreaterThanTotal_ReturnsNegative()
    {
        // Arrange
        var viewModel = new CheckoutViewModel
        {
            Items = new List<CartItem>
            {
                new CartItem { GiaBan = 100m, SoLuong = 2 }
            },
            TienGiam = 300m
        };

        // Act
        var result = viewModel.TongThanhToan;

        // Assert
        Assert.AreEqual(-100m, result);
    }

    [TestMethod]
    public void TongThanhToan_EmptyItemsWithDiscount_ReturnsNegativeDiscount()
    {
        // Arrange
        var viewModel = new CheckoutViewModel
        {
            Items = new List<CartItem>(),
            TienGiam = 50m
        };

        // Act
        var result = viewModel.TongThanhToan;

        // Assert
        Assert.AreEqual(-50m, result);
    }

    [TestMethod]
    public void TongThanhToan_EmptyItemsNoDiscount_ReturnsZero()
    {
        // Arrange
        var viewModel = new CheckoutViewModel
        {
            Items = new List<CartItem>(),
            TienGiam = 0m
        };

        // Act
        var result = viewModel.TongThanhToan;

        // Assert
        Assert.AreEqual(0m, result);
    }

    [TestMethod]
    public void TongThanhToan_MultipleItemsWithDiscount_ReturnsCorrectAmount()
    {
        // Arrange
        var viewModel = new CheckoutViewModel
        {
            Items = new List<CartItem>
            {
                new CartItem { GiaBan = 100m, SoLuong = 2 },
                new CartItem { GiaBan = 50m, SoLuong = 3 },
                new CartItem { GiaBan = 200m, SoLuong = 1 }
            },
            TienGiam = 100m
        };

        // Act
        var result = viewModel.TongThanhToan;

        // Assert
        Assert.AreEqual(450m, result);
    }

    [TestMethod]
    public void TongThanhToan_DecimalPrecision_ReturnsCorrectAmount()
    {
        // Arrange
        var viewModel = new CheckoutViewModel
        {
            Items = new List<CartItem>
            {
                new CartItem { GiaBan = 10.50m, SoLuong = 3 }
            },
            TienGiam = 5.25m
        };

        // Act
        var result = viewModel.TongThanhToan;

        // Assert
        Assert.AreEqual(26.25m, result);
    }
}
