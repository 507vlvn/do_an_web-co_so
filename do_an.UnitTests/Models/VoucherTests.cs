using Xunit;
using do_an.Models;

namespace do_an.UnitTests.Models;

public class VoucherTests
{
    [Fact]
    public void ConHieuLuc_KhiHopLe_ReturnsTrue()
    {
        var v = new Voucher
        {
            KichHoat = true,
            DaSuDung = 0,
            SoLanSuDung = 10,
            NgayBatDau = DateTime.Today.AddDays(-1),
            NgayHetHan = DateTime.Today.AddDays(30)
        };
        Assert.True(v.ConHieuLuc);
    }

    [Fact]
    public void ConHieuLuc_KhiKhongKichHoat_ReturnsFalse()
    {
        var v = new Voucher
        {
            KichHoat = false,
            DaSuDung = 0,
            SoLanSuDung = 10,
            NgayBatDau = DateTime.Today.AddDays(-1),
            NgayHetHan = DateTime.Today.AddDays(30)
        };
        Assert.False(v.ConHieuLuc);
    }

    [Fact]
    public void ConHieuLuc_KhiHetLuong_ReturnsFalse()
    {
        var v = new Voucher
        {
            KichHoat = true,
            DaSuDung = 10,
            SoLanSuDung = 10,
            NgayBatDau = DateTime.Today.AddDays(-1),
            NgayHetHan = DateTime.Today.AddDays(30)
        };
        Assert.False(v.ConHieuLuc);
    }

    [Fact]
    public void ConHieuLuc_KhiChuaBatDau_ReturnsFalse()
    {
        var v = new Voucher
        {
            KichHoat = true,
            DaSuDung = 0,
            SoLanSuDung = 10,
            NgayBatDau = DateTime.Today.AddDays(5),
            NgayHetHan = DateTime.Today.AddDays(30)
        };
        Assert.False(v.ConHieuLuc);
    }

    [Fact]
    public void ConHieuLuc_KhiDaHetHan_ReturnsFalse()
    {
        var v = new Voucher
        {
            KichHoat = true,
            DaSuDung = 0,
            SoLanSuDung = 10,
            NgayBatDau = DateTime.Today.AddDays(-30),
            NgayHetHan = DateTime.Today.AddDays(-1)
        };
        Assert.False(v.ConHieuLuc);
    }
}
