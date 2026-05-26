using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace do_an.Models;

public enum TrangThaiPhieuNhap
{
    MoiTao = 0,
    DaNhap = 1,
    DaHuy = 2
}

public class PhieuNhapHang
{
    public int Id { get; set; }

    [Required]
    [MaxLength(30)]
    [Display(Name = "Mã phiếu nhập")]
    public string MaPhieu { get; set; } = "";

    [Display(Name = "Nhà cung cấp")]
    public int NhaCungCapId { get; set; }

    [ForeignKey(nameof(NhaCungCapId))]
    public NhaCungCap NhaCungCap { get; set; } = null!;

    [MaxLength(50)]
    [Display(Name = "Nhân viên nhập")]
    public string? NhanVienNhapId { get; set; }

    [ForeignKey(nameof(NhanVienNhapId))]
    public TaiKhoan? NhanVienNhap { get; set; }

    [Display(Name = "Ngày nhập")]
    public DateTime NgayNhap { get; set; } = DateTime.Now;

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Tổng tiền")]
    public decimal TongTien { get; set; }

    [Display(Name = "Trạng thái")]
    public TrangThaiPhieuNhap TrangThai { get; set; } = TrangThaiPhieuNhap.MoiTao;

    [MaxLength(500)]
    [Display(Name = "Ghi chú")]
    public string? GhiChu { get; set; }

    public ICollection<ChiTietPhieuNhap> ChiTietPhieuNhaps { get; set; } = new List<ChiTietPhieuNhap>();
}

public class ChiTietPhieuNhap
{
    public int Id { get; set; }

    public int PhieuNhapHangId { get; set; }

    [ForeignKey(nameof(PhieuNhapHangId))]
    public PhieuNhapHang PhieuNhapHang { get; set; } = null!;

    [Display(Name = "Sản phẩm")]
    public int SanPhamId { get; set; }

    [ForeignKey(nameof(SanPhamId))]
    public SanPham SanPham { get; set; } = null!;

    [Display(Name = "Số lượng")]
    [Range(1, int.MaxValue)]
    public int SoLuong { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Giá nhập")]
    public decimal GiaNhap { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Thành tiền")]
    public decimal ThanhTien => GiaNhap * SoLuong;

    // Cho thuốc: tự động tạo lô
    [MaxLength(50)]
    [Display(Name = "Mã lô (nếu là thuốc)")]
    public string? MaLo { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "NSX (nếu là thuốc)")]
    public DateTime? NgaySX { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "HSD (nếu là thuốc)")]
    public DateTime? HanSuDung { get; set; }
}
