using System.ComponentModel.DataAnnotations;

namespace do_an.ViewModels;

public class ProfileViewModel
{
    public string TenDangNhap { get; set; } = "";
    public string VaiTro      { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập họ tên")]
    [MaxLength(100)]
    [Display(Name = "Họ và tên")]
    public string HoTen { get; set; } = "";

    [EmailAddress]
    [MaxLength(100)]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [Phone]
    [MaxLength(15)]
    [Display(Name = "Số điện thoại")]
    public string? SoDienThoai { get; set; }

    [MaxLength(250)]
    [Display(Name = "Địa chỉ")]
    public string? DiaChi { get; set; }

    // ── Đổi mật khẩu (optional) ──────────────────────────────
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu hiện tại")]
    public string? MatKhauHienTai { get; set; }

    [MinLength(6, ErrorMessage = "Mật khẩu mới tối thiểu 6 ký tự")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu mới")]
    public string? MatKhauMoi { get; set; }

    [Compare(nameof(MatKhauMoi), ErrorMessage = "Mật khẩu xác nhận không khớp")]
    [DataType(DataType.Password)]
    [Display(Name = "Xác nhận mật khẩu mới")]
    public string? XacNhanMatKhauMoi { get; set; }
}
