using do_an.Data;
using do_an.Helpers;
using do_an.Models;
using Microsoft.EntityFrameworkCore;

namespace do_an.Services;

/// <summary>
/// Triển khai đồng bộ giỏ hàng giữa Session và Database.
/// Trích xuất từ logic trùng lặp trong GioHangController.OnActionExecutionAsync
/// và StoreController.OnActionExecutionAsync.
/// </summary>
public class CartSyncService : ICartSyncService
{
    private readonly AppDbContext _context;

    public CartSyncService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Load giỏ hàng từ DB vào Session.
    /// Logic tương đương OnActionExecutionAsync trong GioHangController/StoreController.
    /// </summary>
    public async Task SyncSessionFromDbAsync(HttpContext httpContext, string username)
    {
        var dbCart = await _context.GioHangs
            .Include(g => g.ChiTietGioHangs)
            .FirstOrDefaultAsync(g => g.TenDangNhap == username);

        if (dbCart != null && dbCart.ChiTietGioHangs.Count > 0)
        {
            var loadedItems = new List<CartItem>();
            foreach (var ct in dbCart.ChiTietGioHangs)
            {
                var sp = await _context.SanPhams.FindAsync(ct.SanPhamId);
                // Kiểm tra sản phẩm còn hợp lệ (Còn hàng, còn bán)
                if (sp != null && sp.TrenKe && sp.SoLuong > 0)
                {
                    loadedItems.Add(new CartItem
                    {
                        SanPhamId = ct.SanPhamId,
                        TenSanPham = sp.TenSanPham,
                        GiaBan = sp.GiaBan,
                        SoLuong = Math.Min(ct.SoLuong, sp.SoLuong),
                        HinhAnh = sp.HinhAnhDaiDien
                    });
                }
            }
            CartHelper.SaveCart(httpContext.Session, loadedItems);
        }
        else
        {
            // DB trống hoặc lỗi => Reset Session về rỗng
            CartHelper.SaveCart(httpContext.Session, new List<CartItem>());
        }
    }

    /// <summary>
    /// Đồng bộ giỏ hàng từ Session lên DB.
    /// Logic tương đương GioHangController.SyncCartToDbAsync().
    /// </summary>
    public async Task SyncCartToDbAsync(HttpContext httpContext, string username)
    {
        if (string.IsNullOrEmpty(username)) return;

        var cart = CartHelper.GetCart(httpContext.Session);

        var dbCart = await _context.GioHangs
            .Include(g => g.ChiTietGioHangs)
            .FirstOrDefaultAsync(g => g.TenDangNhap == username);

        if (dbCart == null)
        {
            dbCart = new GioHang { TenDangNhap = username };
            _context.GioHangs.Add(dbCart);
            await _context.SaveChangesAsync();
        }

        // Xoá danh sách cũ và cập nhật lại toàn bộ dựa trên Session
        _context.ChiTietGioHangs.RemoveRange(dbCart.ChiTietGioHangs);

        foreach (var item in cart)
        {
            dbCart.ChiTietGioHangs.Add(new ChiTietGioHang
            {
                SanPhamId = item.SanPhamId,
                SoLuong = item.SoLuong,
                GioHangId = dbCart.Id
            });
        }

        dbCart.CapNhatCuoi = DateTime.Now;
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Gộp giỏ hàng từ Session vào DB khi đăng nhập.
    /// Giữ lại sản phẩm từ cả 2 nguồn, ưu tiên số lượng lớn hơn nếu trùng.
    /// </summary>
    public async Task MergeSessionToDbAsync(HttpContext httpContext, string username)
    {
        var sessionCart = CartHelper.GetCart(httpContext.Session);

        var dbCart = await _context.GioHangs
            .Include(g => g.ChiTietGioHangs)
            .FirstOrDefaultAsync(g => g.TenDangNhap == username);

        if (dbCart == null)
        {
            dbCart = new GioHang { TenDangNhap = username };
            _context.GioHangs.Add(dbCart);
            await _context.SaveChangesAsync();
        }

        // Chuyển đổi DB items sang dictionary để dễ merge
        var dbItems = dbCart.ChiTietGioHangs.ToDictionary(c => c.SanPhamId);

        // Merge: nếu sản phẩm đã có trong DB, cập nhật số lượng (lấy max)
        foreach (var sessionItem in sessionCart)
        {
            if (dbItems.TryGetValue(sessionItem.SanPhamId, out var existingDbItem))
            {
                // Lấy số lượng lớn hơn giữa Session và DB
                if (sessionItem.SoLuong > existingDbItem.SoLuong)
                {
                    existingDbItem.SoLuong = sessionItem.SoLuong;
                }
            }
            else
            {
                // Sản phẩm mới từ Session, thêm vào DB
                dbCart.ChiTietGioHangs.Add(new ChiTietGioHang
                {
                    SanPhamId = sessionItem.SanPhamId,
                    SoLuong = sessionItem.SoLuong,
                    GioHangId = dbCart.Id
                });
            }
        }

        dbCart.CapNhatCuoi = DateTime.Now;
        await _context.SaveChangesAsync();

        // Sau khi merge, load lại từ DB vào Session để đồng bộ
        await SyncSessionFromDbAsync(httpContext, username);
    }
}
