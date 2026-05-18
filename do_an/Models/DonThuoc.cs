using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace do_an.Models;

public enum TrangThaiDonThuoc
{
    MoiTao = 0,
    DaDuyet = 1,
    DaMua = 2,
    HetHan = 3,
    DaHuy = 4
}

public class DonThuoc
{
    public int Id { get; set; }

    [MaxLength(20)]
    [Display(Name = "Mã đơn thuốc")]
    [Required]
    public string MaDonThuoc { get; set; } = "";

    [MaxLength(1000)]
    [Display(Name = "Tên đơn thuốc / toa thuốc")]
    public string TenDonThuoc { get; set; } = "";

    [MaxLength(1000)]
    [Display(Name = "Ghi chú / Lời dặn chung")]
    public string? GhiChu { get; set; }

    [Display(Name = "Trạng thái")]
    public TrangThaiDonThuoc TrangThai { get; set; } = TrangThaiDonThuoc.MoiTao;

    [MaxLength(500)]
    [Display(Name = "Hình ảnh đơn thuốc")]
    public string? HinhAnh { get; set; }

    [Display(Name = "Đã đăng lên Store")]
    public bool DaDangStore { get; set; } = false;

    public ICollection<ChiTietDonThuoc> ChiTietDonThuocs { get; set; } = new List<ChiTietDonThuoc>();
}

public class ChiTietDonThuoc
{
    public int Id { get; set; }

    [Required]
    public int DonThuocId { get; set; }

    [ForeignKey(nameof(DonThuocId))]
    public DonThuoc DonThuoc { get; set; } = null!;

    [Display(Name = "Sản phẩm / Thuốc (ID)")]
    public int? SanPhamId { get; set; }

    [ForeignKey(nameof(SanPhamId))]
    public SanPham? SanPham { get; set; }
    
    [MaxLength(150)]
    public string? TenSanPhamSnapshot { get; set; }

    [Display(Name = "Số viên mỗi ngày")]
    [Range(1, int.MaxValue)]
    public int SoVienMoiNgay { get; set; }

    [Display(Name = "Số ngày uống")]
    [Range(1, int.MaxValue)]
    public int SoNgayUong { get; set; }

    [Display(Name = "Tổng số viên")]
    public int TongSoVien => SoVienMoiNgay * SoNgayUong;

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