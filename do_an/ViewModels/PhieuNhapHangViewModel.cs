using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace do_an.ViewModels;

public class CreatePhieuNhapViewModel
{
    [Required(ErrorMessage = "Vui long nhap ma phieu")]
    [MaxLength(30)]
    [Display(Name = "Ma phieu nhap")]
    public string MaPhieu { get; set; } = "";

    [Required(ErrorMessage = "Vui long chon nha cung cap")]
    [Display(Name = "Nha cung cap")]
    public int NhaCungCapId { get; set; }

    [MaxLength(500)]
    [Display(Name = "Ghi chu")]
    public string? GhiChu { get; set; }

    public string ItemsJson { get; set; } = "[]";
}

public class ChiTietPhieuNhapInput
{
    [JsonPropertyName("sanPhamId")]
    public int SanPhamId { get; set; }

    [JsonPropertyName("tenSanPham")]
    public string TenSanPham { get; set; } = "";

    [JsonPropertyName("soLuong")]
    public int SoLuong { get; set; }

    [JsonPropertyName("giaNhap")]
    public decimal GiaNhap { get; set; }

    [JsonPropertyName("giaBan")]
    public decimal GiaBan { get; set; }

    [JsonPropertyName("maLo")]
    public string? MaLo { get; set; }

    [JsonPropertyName("ngaySX")]
    public DateTime? NgaySX { get; set; }

    [JsonPropertyName("hanSuDung")]
    public DateTime? HanSuDung { get; set; }
}
