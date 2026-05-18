using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace do_an.Models;

public class TaiKhoan
{
    [Key]
    [MaxLength(50)]
    public string TenDangNhap { get; set; } = "";

    [MaxLength(100)]
    [Required(ErrorMessage = "Họ tên không được để trống")]
    public string HoTen { get; set; } = "";

    public string MatKhauHash { get; set; } = "";

    [MaxLength(20)]
    public string VaiTro { get; set; } = "KhachHang"; // Admin, NhanVien, KhachHang

    // --- Thông tin liên hệ (Dùng cho cả Khách Hàng và Nhân Viên) ---
    [MaxLength(100)]
    public string? Email { get; set; }

    [MaxLength(15)]
    public string? SoDienThoai { get; set; }

    [MaxLength(250)]
    public string? DiaChi { get; set; }

    // --- Thông tin Dành riêng cho Nhân viên ---
    [MaxLength(100)]
    [Display(Name = "Bộ Phận")]
    public string? BoPhan { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Hệ Số Lương")]
    [Range(0, double.MaxValue)]
    public decimal? HeSoLuong { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Lương Theo Giờ")]
    [Range(0, double.MaxValue)]
    public decimal? LuongTheoGio { get; set; }

    // --- Navigation ---
    // Đơn hàng do khách tự đặt (VaiTro == KhachHang)
    [InverseProperty(nameof(DonHang.KhachHang))]
    public ICollection<DonHang> DonHangsDat { get; set; } = new List<DonHang>();

    // Đơn hàng được nhân viên xử lý tại quầy (VaiTro == NhanVien)
    [InverseProperty(nameof(DonHang.NhanVienBan))]
    public ICollection<DonHang> DonHangsXuLy { get; set; } = new List<DonHang>();

    public GioHang? GioHang { get; set; }
}
