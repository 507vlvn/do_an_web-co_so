using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace do_an.Models;

public class GioHang
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string TenDangNhap { get; set; } = "";

    [ForeignKey("TenDangNhap")]
    public TaiKhoan TaiKhoan { get; set; } = null!;

    public DateTime NgayTao { get; set; } = DateTime.Now;
    
    public DateTime CapNhatCuoi { get; set; } = DateTime.Now;

    public ICollection<ChiTietGioHang> ChiTietGioHangs { get; set; } = new List<ChiTietGioHang>();
}
