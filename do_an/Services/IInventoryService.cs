namespace do_an.Services;

/// <summary>
/// Service quản lý tồn kho.
/// Trích xuất logic liên quan đến kho từ GioHangController và KhoHelper.
/// </summary>
public interface IInventoryService
{
    /// <summary>
    /// Trừ tồn kho cho sản phẩm. Đối với thuốc, sử dụng thuật toán FEFO.
    /// Trả về kết quả trừ kho chứa thông tin các lô đã trừ.
    /// </summary>
    Task<Helpers.TruKhoFefoResult> TruKhoAsync(int sanPhamId, int soLuong);

    /// <summary>
    /// Hoàn lại tồn kho cho sản phẩm (khi hủy đơn hàng).
    /// Đối với thuốc, hoàn vào lô có HSD xa nhất.
    /// </summary>
    Task HoanKhoAsync(int sanPhamId, int soLuong);

    /// <summary>
    /// Kiểm tra sản phẩm có đủ tồn kho hay không.
    /// </summary>
    Task<bool> KiemTraTonKhoAsync(int sanPhamId, int soLuongCan);

    /// <summary>
    /// Đồng bộ hạn sử dụng cho tất cả sản phẩm thuốc theo lô FEFO.
    /// Gọi khi khởi động app hoặc khi cần re-sync.
    /// </summary>
    Task DongBoHanSuDungAsync();
}
