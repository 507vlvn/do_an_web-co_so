using System.ComponentModel.DataAnnotations;

namespace do_an.Models;

public class Voucher
{
    public int Id { get; set; }

    [Required]
    [MaxLength(20)]
    [Display(Name = "Mã voucher")]
    public string MaVoucher { get; set; } = "";

    [MaxLength(200)]
    [Display(Name = "Mô tả")]
    public string? MoTa { get; set; }

    [Required]
    [Range(1, 100)]
    [Display(Name = "% giảm giá")]
    public decimal PhanTramGiam { get; set; }

    [Display(Name = "Giảm tối đa (VNĐ)")]
    public decimal? GiamToiDa { get; set; }

    [Display(Name = "Đơn hàng tối thiểu (VNĐ)")]
    public decimal DonHangToiThieu { get; set; }

    [Display(Name = "Ngày bắt đầu")]
    public DateTime NgayBatDau { get; set; }

    [Display(Name = "Ngày hết hạn")]
    public DateTime NgayHetHan { get; set; }

    [Display(Name = "Số lần sử dụng tối đa")]
    public int SoLanSuDung { get; set; } = 1;

    [Display(Name = "Đã sử dụng")]
    [ConcurrencyCheck]
    public int DaSuDung { get; set; }

    [Display(Name = "Kích hoạt")]
    public bool KichHoat { get; set; } = true;

    public bool ConHieuLuc =>
        KichHoat &&
        DaSuDung < SoLanSuDung &&
        DateTime.Now >= NgayBatDau &&
        DateTime.Now <= NgayHetHan;
}