using do_an.Models;
using do_an.ViewModels;

namespace do_an.Services;

/// <summary>
/// Kết quả trả về từ quá trình tạo đơn hàng.
/// </summary>
public class DatHangResult
{
    public bool Success { get; set; }
    public DonHang? DonHang { get; set; }
    public string? ErrorMessage { get; set; }
    public List<string> Errors { get; set; } = new();
}

/// <summary>
/// Service xử lý nghiệp vụ đơn hàng.
/// Trích xuất logic từ GioHangController.DatHang.
/// </summary>
public interface IDonHangService
{
    /// <summary>
    /// Tạo đơn hàng mới từ giỏ hàng (online checkout).
    /// Bao gồm: kiểm tra tồn kho, trừ kho, áp dụng voucher, lưu đơn.
    /// </summary>
    Task<DatHangResult> TaoDonHangAsync(CheckoutViewModel vm, List<Helpers.CartItem> cart, string? username);

    /// <summary>
    /// Hủy đơn hàng và hoàn lại tồn kho.
    /// </summary>
    Task<bool> HuyDonHangAsync(int donHangId, string? username);

    /// <summary>
    /// Xác nhận thanh toán cho đơn hàng.
    /// </summary>
    Task<bool> XacNhanThanhToanAsync(int donHangId, string? username);
}
