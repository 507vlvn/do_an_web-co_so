using Xunit;
using do_an.Models;

namespace do_an.UnitTests.Models;

public class DonHangTests
{
    [Fact]
    public void TongThanhToan_TinhDung()
    {
        var dh = new DonHang { TongTienHang = 100000, TienGiam = 20000 };
        Assert.Equal(80000, dh.TongThanhToan);
    }

    [Fact]
    public void TongThanhToan_KhiKhongGiamGia()
    {
        var dh = new DonHang { TongTienHang = 100000, TienGiam = 0 };
        Assert.Equal(100000, dh.TongThanhToan);
    }

    [Fact]
    public void TrangThaiMacDinh_LaChoThanhToan()
    {
        var dh = new DonHang();
        Assert.Equal(TrangThaiDonHang.ChoThanhToan, dh.TrangThai);
    }

    [Fact]
    public void LoaiDonMacDinh_LaOnline()
    {
        var dh = new DonHang();
        Assert.Equal(LoaiDonHang.Online, dh.LoaiDon);
    }

    [Fact]
    public void NgayDatMacDinh_LaNgayHienTai()
    {
        var before = DateTime.Now.AddSeconds(-1);
        var dh = new DonHang();
        var after = DateTime.Now.AddSeconds(1);

        Assert.InRange(dh.NgayDat, before, after);
    }

    [Fact]
    public void ChiTietDonHangs_KhoiTaoLaListRong()
    {
        var dh = new DonHang();
        Assert.NotNull(dh.ChiTietDonHangs);
        Assert.Empty(dh.ChiTietDonHangs);
    }
}
