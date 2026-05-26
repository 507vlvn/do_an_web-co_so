using do_an.Models;

namespace do_an.ViewModels;

public class BaoCaoViewModel
{
    // Doanh thu theo ngày
    public List<DoanhThuTheoNgay> DoanhThu7Ngay { get; set; } = new();
    public List<DoanhThuTheoNgay> DoanhThu30Ngay { get; set; } = new();

    // Top sản phẩm bán chạy
    public List<SanPhamBanChay> TopSanPhamBanChay { get; set; } = new();

    // Doanh thu theo danh mục
    public List<DoanhThuTheoDanhMuc> DoanhThuTheoDanhMuc { get; set; } = new();

    // Lợi nhuận
    public decimal LoiNhuanThang { get; set; }
    public decimal LoiNhuanHomNay { get; set; }

    // Tổng quan
    public int TongDonHangThang { get; set; }
    public decimal DoanhThuThang { get; set; }
    public decimal DoanhThuHomNay { get; set; }
}

public class DoanhThuTheoNgay
{
    public DateTime Ngay { get; set; }
    public decimal DoanhThu { get; set; }
    public int SoDonHang { get; set; }
}

public class SanPhamBanChay
{
    public int SanPhamId { get; set; }
    public string TenSanPham { get; set; } = "";
    public int SoLuongBan { get; set; }
    public decimal DoanhThu { get; set; }
}

public class DoanhThuTheoDanhMuc
{
    public string TenDanhMuc { get; set; } = "";
    public decimal DoanhThu { get; set; }
    public int SoLuongBan { get; set; }
}
