using Xunit;
using do_an.Helpers;
using do_an.ViewModels;

namespace do_an.UnitTests.ViewModels;

public class CheckoutViewModelTests
{
    [Fact]
    public void TongTienHang_TinhTongTatCaItems()
    {
        var vm = new CheckoutViewModel
        {
            Items = new List<CartItem>
            {
                new() { GiaBan = 10000, SoLuong = 2 },
                new() { GiaBan = 20000, SoLuong = 1 },
                new() { GiaBan = 5000, SoLuong = 3 }
            }
        };
        Assert.Equal(55000, vm.TongTienHang); // 20000 + 20000 + 15000
    }

    [Fact]
    public void TongThanhToan_SauKhiGiamGia()
    {
        var vm = new CheckoutViewModel
        {
            Items = new List<CartItem>
            {
                new() { GiaBan = 100000, SoLuong = 1 }
            },
            TienGiam = 15000
        };
        Assert.Equal(85000, vm.TongThanhToan);
    }

    [Fact]
    public void TongTienHang_KhiKhongCoItem_ReturnsZero()
    {
        var vm = new CheckoutViewModel();
        Assert.Equal(0, vm.TongTienHang);
    }
}
