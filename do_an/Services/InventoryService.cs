using do_an.Data;
using do_an.Helpers;

namespace do_an.Services;

/// <summary>
/// Triển khai service quản lý tồn kho.
/// Wrapper cho KhoHelper, cung cấp DI-friendly interface.
/// </summary>
public class InventoryService : IInventoryService
{
    private readonly AppDbContext _context;

    public InventoryService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Trừ tồn kho theo FEFO (thuốc) hoặc giảm trực tiếp (sản phẩm thường).
    /// </summary>
    public async Task<Helpers.TruKhoFefoResult> TruKhoAsync(int sanPhamId, int soLuong)
    {
        return await KhoHelper.TruKhoFEFOAsync(_context, sanPhamId, soLuong);
    }

    /// <summary>
    /// Hoàn lại tồn kho theo FEFO (thuốc) hoặc tăng trực tiếp (sản phẩm thường).
    /// </summary>
    public async Task HoanKhoAsync(int sanPhamId, int soLuong)
    {
        await KhoHelper.HoanKhoFEFOAsync(_context, sanPhamId, soLuong);
    }

    /// <summary>
    /// Kiểm tra tồn kho có đủ hay không.
    /// </summary>
    public async Task<bool> KiemTraTonKhoAsync(int sanPhamId, int soLuongCan)
    {
        var sp = await _context.SanPhams.FindAsync(sanPhamId);
        return sp != null && sp.SoLuong >= soLuongCan;
    }

    /// <summary>
    /// Đồng bộ hạn sử dụng cho tất cả sản phẩm thuốc.
    /// </summary>
    public async Task DongBoHanSuDungAsync()
    {
        await KhoHelper.DongBoTatCaHanSuDungAsync(_context);
    }
}
