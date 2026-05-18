using System.ComponentModel.DataAnnotations;

namespace do_an.Models;

public class DanhMucSanPham
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    [Display(Name = "Tên danh mục")]
    public string TenDanhMuc { get; set; } = "";

    [MaxLength(50)]
    [Display(Name = "Phân loại nhóm")]
    public string PhanLoai { get; set; } = "Chung"; // "Thuoc", "ThucPhamChucNang", "MyPham", "ThietBiYTe"

    [MaxLength(500)]
    [Display(Name = "Mô tả")]
    public string? MoTa { get; set; }

    [Display(Name = "Kích hoạt")]
    public bool KichHoat { get; set; } = true;

    public ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}