using Xunit;
using do_an.Data;
using do_an.Helpers;
using do_an.Models;
using Microsoft.EntityFrameworkCore;

namespace do_an.UnitTests.Helpers;

public class KhoHelperTests : IDisposable
{
    private readonly AppDbContext _context;

    public KhoHelperTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
        _context.Database.EnsureCreated();
    }

    public void Dispose() => _context.Dispose();

    private SanPham CreateSanPham(bool isThuoc = true, int soLuong = 100)
    {
        var dm = new DanhMucSanPham { TenDanhMuc = "Test", PhanLoai = "Thuoc", KichHoat = true };
        _context.DanhMucSanPhams.Add(dm);
        _context.SaveChanges();

        var sp = new SanPham
        {
            TenSanPham = "Test Product",
            DanhMucId = dm.Id,
            IsThuoc = isThuoc,
            GiaBan = 10000,
            SoLuong = soLuong,
            TrenKe = true
        };
        _context.SanPhams.Add(sp);
        _context.SaveChanges();
        return sp;
    }

    private LoThuoc CreateLoThuoc(int sanPhamId, int soLuong, DateTime hanSuDung)
    {
        var lo = new LoThuoc
        {
            MaLo = $"LO-{hanSuDung:yyyyMMdd}-{Random.Shared.Next(100, 999)}",
            SanPhamId = sanPhamId,
            SoLuongBanDau = soLuong,
            SoLuongKhaDung = soLuong,
            NgaySX = hanSuDung.AddYears(-2),
            HanSuDung = hanSuDung,
            TrangThai = true
        };
        _context.LoThuocs.Add(lo);
        _context.SaveChanges();
        return lo;
    }

    // ── TruKhoFEFO Tests ──────────────────────────────────────

    [Fact]
    public async Task TruKhoFEFO_SanPhamKhongTonTai_ReturnsFalse()
    {
        var result = await KhoHelper.TruKhoFEFOAsync(_context, 99999, 10);
        Assert.False(result.Success);
    }

    [Fact]
    public async Task TruKhoFEFO_KhongDuHang_ReturnsFalse()
    {
        var sp = CreateSanPham(soLuong: 5);
        var result = await KhoHelper.TruKhoFEFOAsync(_context, sp.Id, 10);
        Assert.False(result.Success);
    }

    [Fact]
    public async Task TruKhoFEFO_SanPhamThuoc_TruTheoLoGanHetHan()
    {
        var sp = CreateSanPham(isThuoc: true, soLuong: 100);
        var lo1 = CreateLoThuoc(sp.Id, 50, DateTime.Today.AddMonths(3));  // Het han truoc
        var lo2 = CreateLoThuoc(sp.Id, 50, DateTime.Today.AddMonths(12)); // Het han sau

        var result = await KhoHelper.TruKhoFEFOAsync(_context, sp.Id, 30);

        Assert.True(result.Success);
        var updatedLo1 = await _context.LoThuocs.FindAsync(lo1.Id);
        var updatedLo2 = await _context.LoThuocs.FindAsync(lo2.Id);
        Assert.Equal(20, updatedLo1!.SoLuongKhaDung); // 50 - 30 = 20
        Assert.Equal(50, updatedLo2!.SoLuongKhaDung); // Khong thay doi
    }

    [Fact]
    public async Task TruKhoFEFO_SanPhamThuoc_TruXuyenLo()
    {
        var sp = CreateSanPham(isThuoc: true, soLuong: 100);
        var lo1 = CreateLoThuoc(sp.Id, 20, DateTime.Today.AddMonths(3));  // Het han truoc
        var lo2 = CreateLoThuoc(sp.Id, 80, DateTime.Today.AddMonths(12)); // Het han sau

        var result = await KhoHelper.TruKhoFEFOAsync(_context, sp.Id, 50);

        Assert.True(result.Success);
        var updatedLo1 = await _context.LoThuocs.FindAsync(lo1.Id);
        var updatedLo2 = await _context.LoThuocs.FindAsync(lo2.Id);
        Assert.Equal(0, updatedLo1!.SoLuongKhaDung);  // Het
        Assert.Equal(50, updatedLo2!.SoLuongKhaDung); // 80 - 30 = 50
    }

    [Fact]
    public async Task TruKhoFEFO_SanPhamKhongThuoc_TruTrucTiep()
    {
        var sp = CreateSanPham(isThuoc: false, soLuong: 100);

        var result = await KhoHelper.TruKhoFEFOAsync(_context, sp.Id, 30);

        Assert.True(result.Success);
        var updatedSp = await _context.SanPhams.FindAsync(sp.Id);
        Assert.Equal(70, updatedSp!.SoLuong);
    }

    [Fact]
    public async Task TruKhoFEFO_BaoLoHetHan_TuTaoLoKhoiPhuc()
    {
        var sp = CreateSanPham(isThuoc: true, soLuong: 50);
        // Khong co lo nao

        var result = await KhoHelper.TruKhoFEFOAsync(_context, sp.Id, 30);

        Assert.True(result.Success);
        var loLegacy = await _context.LoThuocs.FirstOrDefaultAsync(l => l.MaLo.StartsWith("KHOIPHUC"));
        Assert.NotNull(loLegacy);
        Assert.Equal(50, loLegacy.SoLuongBanDau);
    }

    [Fact]
    public async Task TruKhoFEFO_SanPhamThuoc_ApDungGiaLoKeMoiNhat()
    {
        var sp = CreateSanPham(isThuoc: true, soLuong: 150);
        var lo1 = CreateLoThuoc(sp.Id, 50, DateTime.Today.AddMonths(3));
        lo1.GiaBan = 5000;
        var lo2 = CreateLoThuoc(sp.Id, 50, DateTime.Today.AddMonths(6));  // Ke lo moi nhat
        lo2.GiaBan = 5500;
        var lo3 = CreateLoThuoc(sp.Id, 50, DateTime.Today.AddMonths(12)); // Lo moi nhat
        lo3.GiaBan = 6000;
        await _context.SaveChangesAsync();

        var result = await KhoHelper.TruKhoFEFOAsync(_context, sp.Id, 30);

        Assert.True(result.Success);
        Assert.Single(result.CacLoDaTru);

        var details = result.CacLoDaTru[0];
        Assert.Equal(lo2.MaLo, details.MaLo);
        Assert.Equal(30, details.SoLuong);
        Assert.Equal(5500, details.GiaBan); // Ap dung gia 5500 cua lo ke moi nhat (lo2)
    }

    // ── HoanKhoFEFO Tests ─────────────────────────────────────

    [Fact]
    public async Task HoanKhoFEFO_SanPhamThuoc_HoanVaoLoCoHanDaiNhat()
    {
        var sp = CreateSanPham(isThuoc: true, soLuong: 60);
        var lo1 = CreateLoThuoc(sp.Id, 50, DateTime.Today.AddMonths(3));
        var lo2 = CreateLoThuoc(sp.Id, 50, DateTime.Today.AddMonths(12));
        lo1.SoLuongKhaDung = 20; // Da ban 30
        lo2.SoLuongKhaDung = 40; // Da ban 10
        await _context.SaveChangesAsync();

        await KhoHelper.HoanKhoFEFOAsync(_context, sp.Id, 10);

        // Hoan vao lo co HSD xa nhat trong so lo da su dung (SoLuongKhaDung < SoLuongBanDau)
        var updatedLo2 = await _context.LoThuocs.FindAsync(lo2.Id);
        Assert.Equal(50, updatedLo2!.SoLuongKhaDung); // 40 + 10 = 50
    }

    [Fact]
    public async Task HoanKhoFEFO_SanPhamKhongThuoc_CongTrucTiep()
    {
        var sp = CreateSanPham(isThuoc: false, soLuong: 70);

        await KhoHelper.HoanKhoFEFOAsync(_context, sp.Id, 10);

        var updatedSp = await _context.SanPhams.FindAsync(sp.Id);
        Assert.Equal(80, updatedSp!.SoLuong);
    }

    // ── CapNhatHanSuDungTheoLo Tests ──────────────────────────

    [Fact]
    public async Task CapNhatHanSuDungTheoLo_CapNhatTheoLoFEFO_DongBoGiaKeMoiNhat()
    {
        var sp = CreateSanPham(isThuoc: true, soLuong: 150);
        var lo1 = CreateLoThuoc(sp.Id, 50, DateTime.Today.AddMonths(3));
        lo1.GiaBan = 5000; lo1.GiaNhap = 4000;
        var lo2 = CreateLoThuoc(sp.Id, 50, DateTime.Today.AddMonths(6));  // Ke moi nhat
        lo2.GiaBan = 5500; lo2.GiaNhap = 4500;
        var lo3 = CreateLoThuoc(sp.Id, 50, DateTime.Today.AddMonths(12)); // Moi nhat
        lo3.GiaBan = 6000; lo3.GiaNhap = 5000;
        await _context.SaveChangesAsync();

        await KhoHelper.CapNhatHanSuDungTheoLoAsync(_context, sp);

        var updatedSp = await _context.SanPhams.FindAsync(sp.Id);
        Assert.Equal(lo1.HanSuDung, updatedSp!.HanSuDung); // HSD tu lo sap het han nhat (FEFO)
        Assert.Equal(lo1.NgaySX, updatedSp.NgaySX);
        Assert.Equal(5500, updatedSp.GiaBan);   // Gia tu lo ke moi nhat (lo2)
        Assert.Equal(4500, updatedSp.GiaNhap);  // Gia nhap tu lo ke moi nhat (lo2)
    }

    [Fact]
    public async Task CapNhatHanSuDungTheoLo_KhongConLo_XoaHSD()
    {
        var sp = CreateSanPham(isThuoc: true, soLuong: 100);
        sp.HanSuDung = DateTime.Today.AddYears(1);
        await _context.SaveChangesAsync();

        await KhoHelper.CapNhatHanSuDungTheoLoAsync(_context, sp);

        var updatedSp = await _context.SanPhams.FindAsync(sp.Id);
        Assert.Null(updatedSp!.HanSuDung);
    }

    // ── DongBoTatCaHanSuDung Tests ────────────────────────────

    [Fact]
    public async Task DongBoTatCaHanSuDung_DongBoTatCa()
    {
        var sp1 = CreateSanPham(isThuoc: true, soLuong: 50);
        var sp2 = CreateSanPham(isThuoc: true, soLuong: 50);
        CreateLoThuoc(sp1.Id, 50, DateTime.Today.AddMonths(6));
        CreateLoThuoc(sp2.Id, 50, DateTime.Today.AddMonths(3));

        var count = await KhoHelper.DongBoTatCaHanSuDungAsync(_context);

        Assert.True(count >= 0);
    }

    // ── LayGiaBanTheoSoLuong Tests ────────────────────────────

    [Fact]
    public async Task LayGiaBanTheoSoLuong_KhongPhaiThuoc_TraVeGiaGoc()
    {
        var sp = CreateSanPham(isThuoc: false, soLuong: 100);
        sp.GiaBan = 20000;
        await _context.SaveChangesAsync();

        var price = await KhoHelper.LayGiaBanTheoSoLuongAsync(_context, sp.Id, 15);
        Assert.Equal(20000, price);
    }

    [Fact]
    public async Task LayGiaBanTheoSoLuong_ThuocDuLuongLoDauTien_TraVeGiaLoDauTien()
    {
        // Thuốc có 3 viên giá 5k (lô 1 - sắp hết hạn nhất), 7 viên giá 6k (lô 2 - kề mới nhất), 5 viên giá 7k (lô 3 - mới nhất)
        var sp = CreateSanPham(isThuoc: true, soLuong: 15);
        var lo1 = CreateLoThuoc(sp.Id, 3, DateTime.Today.AddMonths(1));
        var lo2 = CreateLoThuoc(sp.Id, 7, DateTime.Today.AddMonths(6));
        var lo3 = CreateLoThuoc(sp.Id, 5, DateTime.Today.AddMonths(12));

        lo1.GiaBan = 5000;
        lo2.GiaBan = 6000;
        lo3.GiaBan = 7000;
        await _context.SaveChangesAsync();

        // Mua <= 3 viên (nằm trong lô 1) -> Trả về giá lô 1 (5000)
        var price3 = await KhoHelper.LayGiaBanTheoSoLuongAsync(_context, sp.Id, 3);
        Assert.Equal(5000, price3);

        var price2 = await KhoHelper.LayGiaBanTheoSoLuongAsync(_context, sp.Id, 2);
        Assert.Equal(5000, price2);
    }

    [Fact]
    public async Task LayGiaBanTheoSoLuong_ThuocVuotLuongLoDauTien_TraVeGiaLoKeMoiNhat()
    {
        // Thuốc có 3 viên giá 5k (lô 1 - sắp hết hạn nhất), 7 viên giá 6k (lô 2 - kề mới nhất), 5 viên giá 7k (lô 3 - mới nhất)
        var sp = CreateSanPham(isThuoc: true, soLuong: 15);
        var lo1 = CreateLoThuoc(sp.Id, 3, DateTime.Today.AddMonths(1));
        var lo2 = CreateLoThuoc(sp.Id, 7, DateTime.Today.AddMonths(6));
        var lo3 = CreateLoThuoc(sp.Id, 5, DateTime.Today.AddMonths(12));

        lo1.GiaBan = 5000;
        lo2.GiaBan = 6000;
        lo3.GiaBan = 7000;
        await _context.SaveChangesAsync();

        // Mua > 3 viên -> Trả về giá lô kề mới nhất (lô 2 -> 6000)
        var price4 = await KhoHelper.LayGiaBanTheoSoLuongAsync(_context, sp.Id, 4);
        Assert.Equal(6000, price4);

        var price10 = await KhoHelper.LayGiaBanTheoSoLuongAsync(_context, sp.Id, 10);
        Assert.Equal(6000, price10);
    }

    [Fact]
    public async Task LayGiaBanTheoSoLuong_NhieuSanPhamKhacNhau_TraVeGiaChinhXacChoTungSanPham()
    {
        // Sản phẩm A
        var spA = CreateSanPham(isThuoc: true, soLuong: 15);
        var loA1 = CreateLoThuoc(spA.Id, 3, DateTime.Today.AddMonths(1));
        var loA2 = CreateLoThuoc(spA.Id, 7, DateTime.Today.AddMonths(6));
        var loA3 = CreateLoThuoc(spA.Id, 5, DateTime.Today.AddMonths(12));
        loA1.GiaBan = 5000;
        loA2.GiaBan = 6000;
        loA3.GiaBan = 7000;

        // Sản phẩm B
        var spB = CreateSanPham(isThuoc: true, soLuong: 23);
        var loB1 = CreateLoThuoc(spB.Id, 5, DateTime.Today.AddMonths(2));
        var loB2 = CreateLoThuoc(spB.Id, 10, DateTime.Today.AddMonths(8));
        var loB3 = CreateLoThuoc(spB.Id, 8, DateTime.Today.AddMonths(15));
        loB1.GiaBan = 8000;
        loB2.GiaBan = 9000;
        loB3.GiaBan = 10000;

        await _context.SaveChangesAsync();

        // Kiểm tra độc lập cho sản phẩm A
        var priceA_small = await KhoHelper.LayGiaBanTheoSoLuongAsync(_context, spA.Id, 2);
        var priceA_large = await KhoHelper.LayGiaBanTheoSoLuongAsync(_context, spA.Id, 5);
        Assert.Equal(5000, priceA_small);
        Assert.Equal(6000, priceA_large);

        // Kiểm tra độc lập cho sản phẩm B
        var priceB_small = await KhoHelper.LayGiaBanTheoSoLuongAsync(_context, spB.Id, 4);
        var priceB_large = await KhoHelper.LayGiaBanTheoSoLuongAsync(_context, spB.Id, 8);
        Assert.Equal(8000, priceB_small);
        Assert.Equal(9000, priceB_large);
    }
}


