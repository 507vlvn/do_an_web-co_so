using System.Text.Json;

namespace do_an.Helpers;

public class CartItem
{
    public int    SanPhamId  { get; set; }
    public string TenSanPham { get; set; } = "";
    public decimal GiaBan    { get; set; }
    public int    SoLuong    { get; set; }
    public string? HinhAnh   { get; set; }
    public int? DonThuocId { get; set; }
    public string? TenDonThuoc { get; set; }
    public decimal ThanhTien => GiaBan * SoLuong;
}

public static class CartHelper
{
    private const string Key = "GioHang";

    public static List<CartItem> GetCart(ISession session)
    {
        var json = session.GetString(Key);
        return json is null
            ? new List<CartItem>()
            : JsonSerializer.Deserialize<List<CartItem>>(json) ?? new List<CartItem>();
    }

    public static void SaveCart(ISession session, List<CartItem> cart)
        => session.SetString(Key, JsonSerializer.Serialize(cart));

    public static void AddOrUpdate(ISession session, CartItem item)
    {
        var cart = GetCart(session);
        var existing = cart.FirstOrDefault(c => c.SanPhamId == item.SanPhamId && c.DonThuocId == item.DonThuocId);
        if (existing is not null)
            existing.SoLuong += item.SoLuong;
        else
            cart.Add(item);
        SaveCart(session, cart);
    }

    public static void UpdateQuantity(ISession session, int sanPhamId, int soLuong, int? donThuocId = null)
    {
        var cart = GetCart(session);
        var item = cart.FirstOrDefault(c => c.SanPhamId == sanPhamId && c.DonThuocId == donThuocId);
        if (item is null) return;
        if (soLuong <= 0) cart.Remove(item);
        else item.SoLuong = soLuong;
        SaveCart(session, cart);
    }

    public static void Remove(ISession session, int sanPhamId, int? donThuocId = null)
    {
        var cart = GetCart(session);
        cart.RemoveAll(c => c.SanPhamId == sanPhamId && c.DonThuocId == donThuocId);
        SaveCart(session, cart);
    }

    public static void RemoveDonThuoc(ISession session, int donThuocId)
    {
        var cart = GetCart(session);
        cart.RemoveAll(c => c.DonThuocId == donThuocId);
        SaveCart(session, cart);
    }

    public static void Clear(ISession session) => session.Remove(Key);

    public static int GetCount(ISession session)
        => GetCart(session).Sum(c => c.SoLuong);
}