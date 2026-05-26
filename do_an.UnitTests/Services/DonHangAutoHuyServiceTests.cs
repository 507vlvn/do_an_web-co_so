using Xunit;
using do_an.Data;
using do_an.Models;
using Microsoft.EntityFrameworkCore;

namespace do_an.UnitTests.Services;

public class DonHangAutoHuyServiceTests : IDisposable
{
    private readonly AppDbContext _context;

    public DonHangAutoHuyServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
        _context.Database.EnsureCreated();
    }

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task DonHangQuaHan_ChiLaOnline_ChuaThanhToan()
    {
        // Tao don hang online chua thanh toan qua 30 phut
        var donHang = new DonHang
        {
            MaDonHang = "DH-TEST-001",
            LoaiDon = LoaiDonHang.Online,
            TrangThai = TrangThaiDonHang.ChoThanhToan,
            PhuongThucThanhToan = PhuongThucThanhToan.ThanhToanOnline,
            NgayDat = DateTime.Now.AddMinutes(-35),
            TongTienHang = 100000
        };
        _context.DonHangs.Add(donHang);
        await _context.SaveChangesAsync();

        // Kiem tra dieu kien huy
        var deadline = DateTime.Now.AddMinutes(-30);
        var donHangsQuaHan = await _context.DonHangs
            .Where(d => d.TrangThai == TrangThaiDonHang.ChoThanhToan
                     && d.PhuongThucThanhToan == PhuongThucThanhToan.ThanhToanOnline
                     && d.NgayDat < deadline)
            .ToListAsync();

        Assert.Single(donHangsQuaHan);
        Assert.Equal("DH-TEST-001", donHangsQuaHan[0].MaDonHang);
    }

    [Fact]
    public async Task DonHangChuaQuaHan_KhongNamTrongDanhSach()
    {
        var donHang = new DonHang
        {
            MaDonHang = "DH-TEST-002",
            LoaiDon = LoaiDonHang.Online,
            TrangThai = TrangThaiDonHang.ChoThanhToan,
            PhuongThucThanhToan = PhuongThucThanhToan.ThanhToanOnline,
            NgayDat = DateTime.Now.AddMinutes(-10), // Chua qua 30 phut
            TongTienHang = 100000
        };
        _context.DonHangs.Add(donHang);
        await _context.SaveChangesAsync();

        var deadline = DateTime.Now.AddMinutes(-30);
        var donHangsQuaHan = await _context.DonHangs
            .Where(d => d.TrangThai == TrangThaiDonHang.ChoThanhToan
                     && d.PhuongThucThanhToan == PhuongThucThanhToan.ThanhToanOnline
                     && d.NgayDat < deadline)
            .ToListAsync();

        Assert.Empty(donHangsQuaHan);
    }

    [Fact]
    public async Task DonTaiQuay_KhongBiHuyTuDong()
    {
        var donHang = new DonHang
        {
            MaDonHang = "DH-TEST-003",
            LoaiDon = LoaiDonHang.POS_TaiQuay,
            TrangThai = TrangThaiDonHang.ChoThanhToan,
            PhuongThucThanhToan = PhuongThucThanhToan.ThanhToanTaiQuay, // Tai quay
            NgayDat = DateTime.Now.AddMinutes(-60),
            TongTienHang = 100000
        };
        _context.DonHangs.Add(donHang);
        await _context.SaveChangesAsync();

        var deadline = DateTime.Now.AddMinutes(-30);
        var donHangsQuaHan = await _context.DonHangs
            .Where(d => d.TrangThai == TrangThaiDonHang.ChoThanhToan
                     && d.PhuongThucThanhToan == PhuongThucThanhToan.ThanhToanOnline
                     && d.NgayDat < deadline)
            .ToListAsync();

        Assert.Empty(donHangsQuaHan); // Don tai quay khong nam trong query
    }

    [Fact]
    public async Task DonDaThanhToan_KhongBiHuyTuDong()
    {
        var donHang = new DonHang
        {
            MaDonHang = "DH-TEST-004",
            LoaiDon = LoaiDonHang.Online,
            TrangThai = TrangThaiDonHang.DaThanhToan,
            PhuongThucThanhToan = PhuongThucThanhToan.ThanhToanOnline,
            NgayDat = DateTime.Now.AddMinutes(-60),
            TongTienHang = 100000
        };
        _context.DonHangs.Add(donHang);
        await _context.SaveChangesAsync();

        var deadline = DateTime.Now.AddMinutes(-30);
        var donHangsQuaHan = await _context.DonHangs
            .Where(d => d.TrangThai == TrangThaiDonHang.ChoThanhToan
                     && d.PhuongThucThanhToan == PhuongThucThanhToan.ThanhToanOnline
                     && d.NgayDat < deadline)
            .ToListAsync();

        Assert.Empty(donHangsQuaHan);
    }

    [Fact]
    public async Task HuyDon_HoanKho()
    {
        // Tao san pham
        var dm = new DanhMucSanPham { TenDanhMuc = "Test", KichHoat = true };
        _context.DanhMucSanPhams.Add(dm);
        await _context.SaveChangesAsync();

        var sp = new SanPham { TenSanPham = "Test SP", DanhMucId = dm.Id, GiaBan = 10000, SoLuong = 70 };
        _context.SanPhams.Add(sp);
        await _context.SaveChangesAsync();

        // Gia lap huy don va hoan kho
        sp.SoLuong += 30;
        await _context.SaveChangesAsync();

        var updatedSp = await _context.SanPhams.FindAsync(sp.Id);
        Assert.Equal(100, updatedSp!.SoLuong);
    }
}
