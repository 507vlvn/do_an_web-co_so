using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace do_an.Models;

public class SanPham
{
    public int Id { get; set; }

    [Required]
    [MaxLength(150)]
    [Display(Name = "Tên sản phẩm")]
    public string TenSanPham { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Danh mục")]
    public int DanhMucId { get; set; }
    public DanhMucSanPham DanhMuc { get; set; } = null!;

    [Display(Name = "Là thuốc kê đơn?")]
    public bool IsThuoc { get; set; } = false;

    // --- Thuộc tính chung ---
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Giá nhập")]
    [Range(0, double.MaxValue)]
    public decimal GiaNhap { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Giá bán")]
    public decimal GiaBan { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Giá gốc (Giá trước giảm)")]
    public decimal? GiaGoc { get; set; }

    [Display(Name = "Số lượng tồn kho")]
    [ConcurrencyCheck]
    public int SoLuong { get; set; }

    [MaxLength(50)]
    [Display(Name = "Đơn Vị Tính")]
    public string DonViTinh { get; set; } = "Viên";

    [MaxLength(3000)]
    [Display(Name = "Mô tả chung")]
    public string? MoTa { get; set; }

    [MaxLength(500)]
    [Display(Name = "Hình ảnh (URL)")]
    public string? HinhAnhUrl { get; set; }

    [MaxLength(500)]
    [Display(Name = "Hình ảnh (file)")]
    public string? HinhAnhFile { get; set; }

    [Display(Name = "Thư viện hình ảnh")]
    public string? ThuVienHinhAnh { get; set; }

    // Helper property to get the main image to display
    [NotMapped]
    public string HinhAnhDaiDien
    {
        get
        {
            var hinh = HinhAnhFile ?? HinhAnhUrl;
            if (string.IsNullOrEmpty(hinh) && !string.IsNullOrEmpty(ThuVienHinhAnh))
            {
                var thuVien = ThuVienHinhAnh.Split(';', StringSplitOptions.RemoveEmptyEntries);
                if (thuVien.Length > 0)
                {
                    hinh = thuVien[0];
                }
            }
            return string.IsNullOrEmpty(hinh) ? "/images/no-image.svg" : hinh;
        }
    }

    [Display(Name = "Đang bán (trên kệ)")]
    public bool TrenKe { get; set; }

    [Display(Name = "Nổi bật")]
    public bool NoiBat { get; set; }

    [Display(Name = "Ngày thêm")]
    public DateTime NgayThem { get; set; } = DateTime.Now;

    // --- Thuộc tính Y tế (Chỉ dùng nếu IsThuoc == true) ---
    [Display(Name = "Ngày Sản Xuất")]
    [DataType(DataType.Date)]
    public DateTime? NgaySX { get; set; }

    [Display(Name = "Hạn Sử Dụng")]
    [DataType(DataType.Date)]
    public DateTime? HanSuDung { get; set; }

    [StringLength(200)]
    [Display(Name = "Nhà Sản Xuất")]
    public string? NhaSanXuat { get; set; }

    [StringLength(1000)]
    [Display(Name = "Thành Phần")]
    public string? ThanhPhan { get; set; }

    [StringLength(1000)]
    [Display(Name = "Công Dụng / Chỉ định")]
    public string? CongDung { get; set; }

    [StringLength(1000)]
    [Display(Name = "Tác Dụng Phụ")]
    public string? TacDungPhu { get; set; }

    // Computed: % giảm giá để hiển thị trên UI
    public int? PhanTramGiam => GiaGoc.HasValue && GiaGoc > 0 && GiaGoc > GiaBan
        ? (int)Math.Round((1 - GiaBan / GiaGoc.Value) * 100)
        : null;

    // --- Navigation Properties ---
    public ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();
    public ICollection<LoThuoc> DanhSachLo { get; set; } = new List<LoThuoc>();
}