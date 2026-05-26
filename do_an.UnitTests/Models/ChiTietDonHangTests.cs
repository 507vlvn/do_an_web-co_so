using Xunit;
using do_an.Models;

namespace do_an.UnitTests.Models;

public class ChiTietDonHangTests
{
    [Fact]
    public void ThanhTien_TinhDung()
    {
        var ct = new ChiTietDonHang { GiaBan = 15000, SoLuong = 3 };
        Assert.Equal(45000, ct.ThanhTien);
    }

    [Fact]
    public void ThanhTien_KhiSoLuongLa1()
    {
        var ct = new ChiTietDonHang { GiaBan = 25000, SoLuong = 1 };
        Assert.Equal(25000, ct.ThanhTien);
    }

    [Fact]
    public void ThanhTien_KhiGiaBanLa0()
    {
        var ct = new ChiTietDonHang { GiaBan = 0, SoLuong = 5 };
        Assert.Equal(0, ct.ThanhTien);
    }
}
