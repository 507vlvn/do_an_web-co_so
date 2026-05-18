using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace do_an.Models;

public class ChiTietGioHang
{
    public int Id { get; set; }

    [Required]
    public int GioHangId { get; set; }

    [ForeignKey("GioHangId")]
    public GioHang GioHang { get; set; } = null!;

    [Required]
    public int SanPhamId { get; set; }

    [ForeignKey("SanPhamId")]
    public SanPham SanPham { get; set; } = null!;

    [Required]
    [Range(1, int.MaxValue)]
    public int SoLuong { get; set; }
}
