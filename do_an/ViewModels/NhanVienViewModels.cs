using System.ComponentModel.DataAnnotations;

namespace do_an.ViewModels;

public class NhanVienCreateViewModel
{
    // ── Thông tin nhân viên ───────────────────────────────────
    [Required(ErrorMessage = "Mã nhân viên không được để trống")]
    [MaxLength(20)]
    [RegularExpression(@"^[A-Za-z0-9]+$", ErrorMessage = "Mã chỉ gồm chữ và số")]
    [Display(Name = "Mã nhân viên")]
    public string MaNV { get; set; } = "";

    [Required(ErrorMessage = "Họ tên không được để trống")]
    [MaxLength(100)]
    [Display(Name = "Họ và tên")]
    public string HoTen { get; set; } = "";

    [MaxLength(100)]
    [Display(Name = "Bộ phận")]
    public string? BoPhan { get; set; }

    [Required(ErrorMessage = "Vai trò không được để trống")]
    [Display(Name = "Vai trò")]
    public string VaiTro { get; set; } = "NhanVien";

    [Range(0, 99, ErrorMessage = "Hệ số lương không hợp lệ")]
    [Display(Name = "Hệ số lương")]
    public decimal HeSoLuong { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Lương theo giờ không hợp lệ")]
    [Display(Name = "Lương theo giờ (đ/h)")]
    public decimal LuongTheoGio { get; set; }

    // ── Tài khoản đăng nhập ───────────────────────────────────
    [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
    [MaxLength(50)]
    [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Chỉ chứa chữ, số và dấu gạch dưới")]
    [Display(Name = "Tên đăng nhập")]
    public string TenDangNhap { get; set; } = "";

    [EmailAddress]
    [MaxLength(100)]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Mật khẩu không được để trống")]
    [MinLength(6, ErrorMessage = "Tối thiểu 6 ký tự")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu")]
    public string MatKhau { get; set; } = "";

    [Required(ErrorMessage = "Xác nhận mật khẩu không được để trống")]
    [Compare(nameof(MatKhau), ErrorMessage = "Mật khẩu xác nhận không khớp")]
    [DataType(DataType.Password)]
    [Display(Name = "Xác nhận mật khẩu")]
    public string XacNhanMatKhau { get; set; } = "";
}

public class NhanVienDoiMatKhauViewModel
{
    public string MaNV     { get; set; } = "";
    public string HoTen    { get; set; } = "";
    public string TenDangNhap { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới")]
    [MinLength(6, ErrorMessage = "Tối thiểu 6 ký tự")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu mới")]
    public string MatKhauMoi { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu")]
    [Compare(nameof(MatKhauMoi), ErrorMessage = "Mật khẩu xác nhận không khớp")]
    [DataType(DataType.Password)]
    [Display(Name = "Xác nhận mật khẩu")]
    public string XacNhan { get; set; } = "";
}
