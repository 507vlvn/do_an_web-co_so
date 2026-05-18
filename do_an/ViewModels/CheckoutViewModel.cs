using do_an.Helpers;
using do_an.Models;
using System.ComponentModel.DataAnnotations;

namespace do_an.ViewModels;

public class CheckoutViewModel
{
    public List<CartItem> Items { get; set; } = new List<CartItem>();

    [Required(ErrorMessage = "Vui lòng nhập họ tên")]
    [MaxLength(100)]
    [Display(Name = "Họ và tên người nhận")]
    public string HoTenNguoiNhan { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    [MaxLength(15)]
    [Display(Name = "Số điện thoại")]
    public string SoDienThoai { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
    [MaxLength(250)]
    [Display(Name = "Địa chỉ giao hàng")]
    public string DiaChi { get; set; } = "";

    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    [MaxLength(100)]
    [Display(Name = "Email (nhận thông báo)")]
    public string? Email { get; set; }

    [MaxLength(500)]
    [Display(Name = "Ghi chú")]
    public string? GhiChu { get; set; }

    [MaxLength(20)]
    [Display(Name = "Mã voucher")]
    public string? MaVoucher { get; set; }

    [Display(Name = "Phương thức thanh toán")]
    public PhuongThucThanhToan PhuongThucThanhToan { get; set; } = PhuongThucThanhToan.ThanhToanOnline;

    // Computed
    public decimal TongTienHang => Items.Sum(i => i.ThanhTien);
    public decimal TienGiam     { get; set; }
    public decimal TongThanhToan => TongTienHang - TienGiam;
}