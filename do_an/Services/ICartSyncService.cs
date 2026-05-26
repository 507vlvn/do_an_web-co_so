namespace do_an.Services;

/// <summary>
/// Service đồng bộ giỏ hàng giữa Session và Database.
/// Tránh trùng lặp logic giữa GioHangController và StoreController.
/// </summary>
public interface ICartSyncService
{
    /// <summary>
    /// Load giỏ hàng từ DB vào Session (khi user đăng nhập và Session trống).
    /// </summary>
    Task SyncSessionFromDbAsync(HttpContext httpContext, string username);

    /// <summary>
    /// Đồng bộ giỏ hàng từ Session lên DB (sau khi thêm/sửa/xóa sản phẩm).
    /// </summary>
    Task SyncCartToDbAsync(HttpContext httpContext, string username);

    /// <summary>
    /// Gộp giỏ hàng Session vào DB (dùng khi login, giữ lại cả 2 nguồn).
    /// </summary>
    Task MergeSessionToDbAsync(HttpContext httpContext, string username);
}
