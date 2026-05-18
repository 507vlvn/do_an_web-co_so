using System.ComponentModel.DataAnnotations;

namespace do_an.ViewModels;

public class NhapLoThuocViewModel
{
    // Chọn thuốc cần nhập lô
    [Required(ErrorMessage = "Vui lòng chọn thuốc")]
    [Display(Name = "Thuốc")]
    public int SanPhamId { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập mã lô")]
    [MaxLength(50)]
    [Display(Name = "Mã lô")]
    public string MaLo { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập số lượng")]
    [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
    [Display(Name = "Số lượng nhập")]
    public int SoLuongNhap { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập ngày sản xuất")]
    [DataType(DataType.Date)]
    [Display(Name = "Ngày sản xuất")]
    public DateTime NgaySX { get; set; } = DateTime.Today.AddMonths(-1);

    [Required(ErrorMessage = "Vui lòng nhập hạn sử dụng")]
    [DataType(DataType.Date)]
    [Display(Name = "Hạn sử dụng")]
    public DateTime HanSuDung { get; set; } = DateTime.Today.AddYears(2);
}
