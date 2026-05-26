using Xunit;
using do_an.Helpers;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace do_an.UnitTests.Helpers;

public class CartHelperTests
{
    private ISession CreateSession()
    {
        return new TestSession();
    }

    [Fact]
    public void GetCart_KhiSessionRong_ReturnsEmptyList()
    {
        var session = CreateSession();
        var cart = CartHelper.GetCart(session);
        Assert.NotNull(cart);
        Assert.Empty(cart);
    }

    [Fact]
    public void AddOrUpdate_ThemSanPhamMoi_AddsToCart()
    {
        var session = CreateSession();
        var item = new CartItem { SanPhamId = 1, TenSanPham = "Test", GiaBan = 10000, SoLuong = 2 };

        CartHelper.AddOrUpdate(session, item);

        var cart = CartHelper.GetCart(session);
        Assert.Single(cart);
        Assert.Equal(1, cart[0].SanPhamId);
        Assert.Equal(2, cart[0].SoLuong);
    }

    [Fact]
    public void AddOrUpdate_SanPhamDaCo_TangSoLuong()
    {
        var session = CreateSession();
        var item1 = new CartItem { SanPhamId = 1, TenSanPham = "Test", GiaBan = 10000, SoLuong = 2 };
        var item2 = new CartItem { SanPhamId = 1, TenSanPham = "Test", GiaBan = 10000, SoLuong = 3 };

        CartHelper.AddOrUpdate(session, item1);
        CartHelper.AddOrUpdate(session, item2);

        var cart = CartHelper.GetCart(session);
        Assert.Single(cart);
        Assert.Equal(5, cart[0].SoLuong);
    }

    [Fact]
    public void UpdateQuantity_CapNhatSoLuong()
    {
        var session = CreateSession();
        CartHelper.AddOrUpdate(session, new CartItem { SanPhamId = 1, TenSanPham = "Test", GiaBan = 10000, SoLuong = 2 });

        CartHelper.UpdateQuantity(session, 1, 5);

        var cart = CartHelper.GetCart(session);
        Assert.Single(cart);
        Assert.Equal(5, cart[0].SoLuong);
    }

    [Fact]
    public void UpdateQuantity_SoLuong0_XoaKhoiGio()
    {
        var session = CreateSession();
        CartHelper.AddOrUpdate(session, new CartItem { SanPhamId = 1, TenSanPham = "Test", GiaBan = 10000, SoLuong = 2 });

        CartHelper.UpdateQuantity(session, 1, 0);

        var cart = CartHelper.GetCart(session);
        Assert.Empty(cart);
    }

    [Fact]
    public void Remove_XoaSanPham()
    {
        var session = CreateSession();
        CartHelper.AddOrUpdate(session, new CartItem { SanPhamId = 1, TenSanPham = "A", GiaBan = 10000, SoLuong = 1 });
        CartHelper.AddOrUpdate(session, new CartItem { SanPhamId = 2, TenSanPham = "B", GiaBan = 20000, SoLuong = 1 });

        CartHelper.Remove(session, 1);

        var cart = CartHelper.GetCart(session);
        Assert.Single(cart);
        Assert.Equal(2, cart[0].SanPhamId);
    }

    [Fact]
    public void Clear_XoaTatCa()
    {
        var session = CreateSession();
        CartHelper.AddOrUpdate(session, new CartItem { SanPhamId = 1, TenSanPham = "A", GiaBan = 10000, SoLuong = 1 });
        CartHelper.AddOrUpdate(session, new CartItem { SanPhamId = 2, TenSanPham = "B", GiaBan = 20000, SoLuong = 1 });

        CartHelper.Clear(session);

        var cart = CartHelper.GetCart(session);
        Assert.Empty(cart);
    }

    [Fact]
    public void GetCount_TinhTongSoLuong()
    {
        var session = CreateSession();
        CartHelper.AddOrUpdate(session, new CartItem { SanPhamId = 1, TenSanPham = "A", GiaBan = 10000, SoLuong = 3 });
        CartHelper.AddOrUpdate(session, new CartItem { SanPhamId = 2, TenSanPham = "B", GiaBan = 20000, SoLuong = 2 });

        Assert.Equal(5, CartHelper.GetCount(session));
    }

    [Fact]
    public void GetCount_KhiRong_ReturnsZero()
    {
        var session = CreateSession();
        Assert.Equal(0, CartHelper.GetCount(session));
    }

    [Fact]
    public void RemoveDonThuoc_XoaTatCaSanPhamCuaDonThuoc()
    {
        var session = CreateSession();
        CartHelper.AddOrUpdate(session, new CartItem { SanPhamId = 1, TenSanPham = "A", GiaBan = 10000, SoLuong = 1, DonThuocId = 1 });
        CartHelper.AddOrUpdate(session, new CartItem { SanPhamId = 2, TenSanPham = "B", GiaBan = 20000, SoLuong = 1, DonThuocId = 1 });
        CartHelper.AddOrUpdate(session, new CartItem { SanPhamId = 3, TenSanPham = "C", GiaBan = 30000, SoLuong = 1, DonThuocId = null });

        CartHelper.RemoveDonThuoc(session, 1);

        var cart = CartHelper.GetCart(session);
        Assert.Single(cart);
        Assert.Equal(3, cart[0].SanPhamId);
    }

    // TestSession - ISession implementation for testing
    private class TestSession : ISession
    {
        private readonly Dictionary<string, byte[]> _store = new();

        public bool IsAvailable => true;
        public string Id => Guid.NewGuid().ToString();
        public IEnumerable<string> Keys => _store.Keys;

        public void Clear() => _store.Clear();
        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public void Remove(string key) => _store.Remove(key);
        public void Set(string key, byte[] value) => _store[key] = value;

        public bool TryGetValue(string key, out byte[]? value) => _store.TryGetValue(key, out value);
    }
}
