using do_an.Models;

namespace do_an.Services;

/// <summary>
/// Kết quả áp dụng voucher.
/// </summary>
public class VoucherResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";
    public decimal TienGiam { get; set; }
    public Voucher? Voucher { get; set; }
}

/// <summary>
/// Service xử lý nghiệp vụ voucher.
/// Trích xuất logic từ GioHangController.ApplyVoucherAjax và DatHang.
/// </summary>
public interface IVoucherService
{
    /// <summary>
    /// Kiểm tra tính hợp lệ của voucher và tính tiền giảm.
    /// </summary>
    Task<VoucherResult> ApDungVoucherAsync(string maVoucher, decimal tongTienHang);

    /// <summary>
    /// Kiểm tra voucher có hợp lệ hay không (không tính tiền giảm).
    /// </summary>
    Task<bool> KiemTraVoucherHopLeAsync(string maVoucher);
}
