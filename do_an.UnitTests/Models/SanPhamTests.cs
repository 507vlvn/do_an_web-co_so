using Xunit;
using do_an.Models;

namespace do_an.UnitTests.Models;

public class SanPhamTests
{
    [Fact]
    public void PhanTramGiam_KhiCoGiaGocVaGiaBanThapHon_TinhDung()
    {
        var sp = new SanPham { GiaBan = 70000, GiaGoc = 100000 };
        Assert.Equal(30, sp.PhanTramGiam);
    }

    [Fact]
    public void PhanTramGiam_KhiGiaBanBangGiaGoc_ReturnsNull()
    {
        var sp = new SanPham { GiaBan = 100000, GiaGoc = 100000 };
        Assert.Null(sp.PhanTramGiam);
    }

    [Fact]
    public void PhanTramGiam_KhiKhongCoGiaGoc_ReturnsNull()
    {
        var sp = new SanPham { GiaBan = 50000, GiaGoc = null };
        Assert.Null(sp.PhanTramGiam);
    }

    [Fact]
    public void PhanTramGiam_KhiGiaGocLa0_ReturnsNull()
    {
        var sp = new SanPham { GiaBan = 50000, GiaGoc = 0 };
        Assert.Null(sp.PhanTramGiam);
    }

    [Fact]
    public void PhanTramGiam_KhiGiaBanCaoHonGiaGoc_ReturnsNull()
    {
        var sp = new SanPham { GiaBan = 120000, GiaGoc = 100000 };
        Assert.Null(sp.PhanTramGiam);
    }

    [Fact]
    public void HinhAnhDaiDien_KhiCoHinhAnhFile_ReturnsFile()
    {
        var sp = new SanPham { HinhAnhFile = "/images/sanpham/test.jpg" };
        Assert.Equal("/images/sanpham/test.jpg", sp.HinhAnhDaiDien);
    }

    [Fact]
    public void HinhAnhDaiDien_KhiKhongCoFile_ReturnsUrl()
    {
        var sp = new SanPham { HinhAnhUrl = "https://example.com/img.jpg" };
        Assert.Equal("https://example.com/img.jpg", sp.HinhAnhDaiDien);
    }

    [Fact]
    public void HinhAnhDaiDien_KhiCoThuVienHinhAnh_ReturnsDauTien()
    {
        var sp = new SanPham { ThuVienHinhAnh = "/img1.jpg;/img2.jpg;/img3.jpg" };
        Assert.Equal("/img1.jpg", sp.HinhAnhDaiDien);
    }

    [Fact]
    public void HinhAnhDaiDien_KhiKhongCoGi_ReturnsNoImage()
    {
        var sp = new SanPham();
        Assert.Equal("/images/no-image.svg", sp.HinhAnhDaiDien);
    }

    [Fact]
    public void PhanTramGiam_LamTronDung()
    {
        // 33.33% -> lam tron thanh 33
        var sp = new SanPham { GiaBan = 66670, GiaGoc = 100000 };
        Assert.Equal(33, sp.PhanTramGiam);
    }
}
