using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace do_an.ViewModels;

public class CreateDonThuocViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập tên đơn thuốc.")]
    [MaxLength(1000)]
    [Display(Name = "Tên đơn thuốc")]
    public string TenDonThuoc { get; set; } = "";

    [MaxLength(1000)]
    [Display(Name = "Ghi chú / Lời dặn")]
    public string? GhiChu { get; set; }

    [Display(Name = "Danh mục trên Store")]
    public int? DanhMucId { get; set; } = 8;

    [Display(Name = "Đăng lên Store ngay")]
    public bool DangNgayLenStore { get; set; } = true;

    [Display(Name = "Hình ảnh đơn thuốc")]
    public IFormFile? HinhAnhFile { get; set; }

    public string ItemsJson { get; set; } = "[]";
}