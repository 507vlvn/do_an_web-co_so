using System.ComponentModel.DataAnnotations;

namespace do_an.Models;

public class ChiTietDonHang
{
    public int Id { get; set; }
    public int DonHangId { get; set; }
    public DonHang DonHang { get; set; } = null!;
    public int? SanPhamId { get; set; }
    public SanPham? SanPham { get; set; }
    
    [MaxLength(150)]
    public string? TenSanPhamSnapshot { get; set; }

    public int SoLuong { get; set; }
    public decimal GiaBan { get; set; }
    public decimal ThanhTien => GiaBan * SoLuong;

    [Display(Name = "Số viên mỗi ngày")]
    public int? SoVienMoiNgay { get; set; }

    [Display(Name = "Số ngày uống")]
    public int? SoNgayUong { get; set; }

    [MaxLength(100)]
    [Display(Name = "Thời điểm uống")]
    public string? ThoiDiemUong { get; set; }

    [MaxLength(100)]
    [Display(Name = "Cách dùng")]
    public string? CachDung { get; set; }

    [MaxLength(200)]
    [Display(Name = "Ghi chú liều dùng")]
    public string? GhiChuLieuDung { get; set; }
}