using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace do_an.ViewModels;

public class CreateHoaDonViewModel
{
    [Display(Name = "Nhân Viên Phụ Trách")]
    public string? TaiKhoanId { get; set; }

    // JSON string của danh sách chi tiết, serialize từ phía client
    public string ItemsJson { get; set; } = "[]";
}

public class ChiTietInputViewModel
{
    [JsonPropertyName("maThuoc")]
    public int SanPhamId { get; set; }

    [JsonPropertyName("tenThuoc")]
    public string TenSanPham { get; set; } = string.Empty;

    [JsonPropertyName("soVienMoiNgay")]
    public int? SoVienMoiNgay { get; set; }

    [JsonPropertyName("soNgayUong")]
    public int? SoNgayUong { get; set; }

    [JsonPropertyName("tongSoVien")]
    public int SoLuong { get; set; }

    [JsonPropertyName("giaBan")]
    public decimal GiaBan { get; set; }

    [JsonPropertyName("thanhTien")]
    public decimal ThanhTien { get; set; }

    [JsonPropertyName("thoiDiemUong")]
    public string? ThoiDiemUong { get; set; }

    [JsonPropertyName("cachDung")]
    public string? CachDung { get; set; }

    [JsonPropertyName("ghiChuLieuDung")]
    public string? GhiChuLieuDung { get; set; }
}
