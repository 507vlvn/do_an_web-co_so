using System.ComponentModel.DataAnnotations;

namespace do_an.Models;

public class NhaCungCap
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    [Display(Name = "Tên nhà cung cấp")]
    public string TenNCC { get; set; } = "";

    [MaxLength(300)]
    [Display(Name = "Địa chỉ")]
    public string? DiaChi { get; set; }

    [MaxLength(15)]
    [Display(Name = "Số điện thoại")]
    public string? SoDienThoai { get; set; }

    [MaxLength(100)]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [MaxLength(100)]
    [Display(Name = "Người liên hệ")]
    public string? NguoiLienHe { get; set; }

    [MaxLength(500)]
    [Display(Name = "Ghi chú")]
    public string? GhiChu { get; set; }

    [Display(Name = "Kích hoạt")]
    public bool KichHoat { get; set; } = true;

    public DateTime NgayTao { get; set; } = DateTime.Now;

    public ICollection<PhieuNhapHang> PhieuNhapHangs { get; set; } = new List<PhieuNhapHang>();
}
