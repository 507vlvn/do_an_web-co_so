using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace do_an.Models;

public enum TrangThaiDonHang
{
    ChoThanhToan = 0,
    DaThanhToan = 1,
    DangChuanBi = 2,
    DangGiao = 3,
    DaGiao = 4,
    DaHuy = 5
}

public enum PhuongThucThanhToan
{
    ThanhToanTaiQuay = 0,
    ThanhToanOnline = 1
}

public enum LoaiDonHang
{
    Online = 0,
    POS_TaiQuay = 1
}

public class DonHang
{
    public int Id { get; set; }

    [MaxLength(20)]
    [Display(Name = "Mã đơn hàng / Hóa đơn")]
    public string MaDonHang { get; set; } = "";

    [Display(Name = "Loại đơn hàng")]
    public LoaiDonHang LoaiDon { get; set; } = LoaiDonHang.Online;

    // Ngừoi dùng đặt hàng online
    [MaxLength(50)]
    public string? TenDangNhap { get; set; }

    // Nhân viên thực hiện giao dịch (Nếu bán tại quầy POS)
    [MaxLength(50)]
    public string? NhanVienPhuTrachId { get; set; }

    [MaxLength(100)]
    [Display(Name = "Họ tên người nhận (Tùy chọn cho POS)")]
    public string? HoTenNguoiNhan { get; set; }

    [MaxLength(15)]
    [Display(Name = "Số điện thoại")]
    public string? SoDienThoai { get; set; }

    [MaxLength(250)]
    [Display(Name = "Địa chỉ giao hàng")]
    public string? DiaChi { get; set; }

    [MaxLength(100)]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [MaxLength(500)]
    [Display(Name = "Ghi chú")]
    public string? GhiChu { get; set; }

    [Display(Name = "Tổng tiền hàng")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TongTienHang { get; set; }

    [Display(Name = "Tiền giảm")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TienGiam { get; set; }

    [Display(Name = "Tổng thanh toán")]
    public decimal TongThanhToan => TongTienHang - TienGiam;

    [MaxLength(20)]
    [Display(Name = "Mã voucher")]
    public string? MaVoucher { get; set; }

    [Display(Name = "Trạng thái")]
    public TrangThaiDonHang TrangThai { get; set; } = TrangThaiDonHang.ChoThanhToan;

    [Display(Name = "Phương thức thanh toán")]
    public PhuongThucThanhToan PhuongThucThanhToan { get; set; }

    [Display(Name = "Ngày lập / Đặt")]
    public DateTime NgayDat { get; set; } = DateTime.Now;

    [Display(Name = "Ngày thanh toán")]
    public DateTime? NgayThanhToan { get; set; }

    [Display(Name = "Ngày chuẩn bị")]
    public DateTime? NgayChuanBi { get; set; }

    [Display(Name = "Ngày bắt đầu giao")]
    public DateTime? NgayBatDauGiao { get; set; }

    [Display(Name = "Ngày giao")]
    public DateTime? NgayGiao { get; set; }

    [Display(Name = "Ngày hủy")]
    public DateTime? NgayHuy { get; set; }

    // Navigation
    // Gắn với khách hàng
    [ForeignKey(nameof(TenDangNhap))]
    public TaiKhoan? KhachHang { get; set; }

    // Gắn với nhân viên bán
    [ForeignKey(nameof(NhanVienPhuTrachId))]
    public TaiKhoan? NhanVienBan { get; set; }

    public ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();
}