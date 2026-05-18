using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace do_an.Models;

public class LoThuoc
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    [Display(Name = "Mã Lô")]
    public string MaLo { get; set; } = "";

    [Required]
    [Display(Name = "Sản phẩm / Thuốc")]
    public int SanPhamId { get; set; }
    
    [ForeignKey("SanPhamId")]
    public SanPham SanPham { get; set; } = null!;

    [Display(Name = "Số Lượng Ban Đầu")]
    [Range(1, int.MaxValue)]
    public int SoLuongBanDau { get; set; }

    [Display(Name = "Số Lượng Khả Dụng")]
    [Range(0, int.MaxValue)]
    [ConcurrencyCheck]
    public int SoLuongKhaDung { get; set; }

    [Display(Name = "Ngày Sản Xuất")]
    [DataType(DataType.Date)]
    public DateTime NgaySX { get; set; }

    [Display(Name = "Hạn Sử Dụng")]
    [DataType(DataType.Date)]
    public DateTime HanSuDung { get; set; }

    [Display(Name = "Trạng Thái")]
    public bool TrangThai { get; set; } = true;
}
