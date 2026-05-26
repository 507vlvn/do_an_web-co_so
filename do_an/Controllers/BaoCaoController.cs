using do_an.Data;
using do_an.Models;
using do_an.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace do_an.Controllers;

[Authorize(Roles = "Admin")]
public class BaoCaoController : Controller
{
    private readonly AppDbContext _context;

    public BaoCaoController(AppDbContext context) => _context = context;

    // GET: /BaoCao
    public async Task<IActionResult> Index()
    {
        var today = DateTime.Today;
        var startOfMonth = new DateTime(today.Year, today.Month, 1);
        var startOf7Days = today.AddDays(-6);

        // ── Doanh thu 7 ngay gan nhat ──────────────────────────
        var donHangs7Ngay = await _context.DonHangs
            .Where(d => d.NgayDat.Date >= startOf7Days
                     && d.TrangThai != TrangThaiDonHang.DaHuy
                     && (d.TrangThai == TrangThaiDonHang.DaThanhToan
                         || d.TrangThai == TrangThaiDonHang.DaGiao))
            .ToListAsync();

        var doanhThu7Ngay = Enumerable.Range(0, 7)
            .Select(i => today.AddDays(-i))
            .Select(ngay => new DoanhThuTheoNgay
            {
                Ngay = ngay,
                DoanhThu = donHangs7Ngay
                    .Where(d => d.NgayDat.Date == ngay)
                    .Sum(d => d.TongTienHang - d.TienGiam),
                SoDonHang = donHangs7Ngay.Count(d => d.NgayDat.Date == ngay)
            })
            .Reverse()
            .ToList();

        // ── Doanh thu 30 ngay ──────────────────────────────────
        var startOf30Days = today.AddDays(-29);
        var donHangs30Ngay = await _context.DonHangs
            .Where(d => d.NgayDat.Date >= startOf30Days
                     && d.TrangThai != TrangThaiDonHang.DaHuy
                     && (d.TrangThai == TrangThaiDonHang.DaThanhToan
                         || d.TrangThai == TrangThaiDonHang.DaGiao))
            .ToListAsync();

        var doanhThu30Ngay = Enumerable.Range(0, 30)
            .Select(i => today.AddDays(-i))
            .Select(ngay => new DoanhThuTheoNgay
            {
                Ngay = ngay,
                DoanhThu = donHangs30Ngay
                    .Where(d => d.NgayDat.Date == ngay)
                    .Sum(d => d.TongTienHang - d.TienGiam),
                SoDonHang = donHangs30Ngay.Count(d => d.NgayDat.Date == ngay)
            })
            .Reverse()
            .ToList();

        // ── Top san pham ban chay ──────────────────────────────
        var topSanPham = await _context.ChiTietDonHangs
            .Include(ct => ct.DonHang)
            .Where(ct => ct.DonHang.NgayDat >= startOfMonth
                      && ct.DonHang.TrangThai != TrangThaiDonHang.DaHuy
                      && ct.SanPhamId != null)
            .GroupBy(ct => new { ct.SanPhamId, ct.TenSanPhamSnapshot })
            .Select(g => new SanPhamBanChay
            {
                SanPhamId = g.Key.SanPhamId!.Value,
                TenSanPham = g.Key.TenSanPhamSnapshot ?? "",
                SoLuongBan = g.Sum(ct => ct.SoLuong),
                DoanhThu = g.Sum(ct => ct.GiaBan * ct.SoLuong)
            })
            .OrderByDescending(x => x.SoLuongBan)
            .Take(10)
            .ToListAsync();

        // ── Doanh thu theo danh muc ────────────────────────────
        var doanhThuDanhMuc = await _context.ChiTietDonHangs
            .Include(ct => ct.DonHang)
            .Include(ct => ct.SanPham)
                .ThenInclude(s => s!.DanhMuc)
            .Where(ct => ct.DonHang.NgayDat >= startOfMonth
                      && ct.DonHang.TrangThai != TrangThaiDonHang.DaHuy
                      && ct.SanPham != null)
            .GroupBy(ct => ct.SanPham!.DanhMuc.TenDanhMuc)
            .Select(g => new DoanhThuTheoDanhMuc
            {
                TenDanhMuc = g.Key,
                DoanhThu = g.Sum(ct => ct.GiaBan * ct.SoLuong),
                SoLuongBan = g.Sum(ct => ct.SoLuong)
            })
            .OrderByDescending(x => x.DoanhThu)
            .ToListAsync();

        // ── Loi nhuan ──────────────────────────────────────────
        var chiTietThang = await _context.ChiTietDonHangs
            .Include(ct => ct.DonHang)
            .Include(ct => ct.SanPham)
            .Where(ct => ct.DonHang.NgayDat >= startOfMonth
                      && ct.DonHang.TrangThai != TrangThaiDonHang.DaHuy
                      && ct.SanPham != null)
            .ToListAsync();

        var loiNhuanThang = chiTietThang.Sum(ct => (ct.GiaBan - (ct.SanPham!.GiaNhap)) * ct.SoLuong);

        var chiTietHomNay = chiTietThang.Where(ct => ct.DonHang.NgayDat.Date == today).ToList();
        var loiNhuanHomNay = chiTietHomNay.Sum(ct => (ct.GiaBan - (ct.SanPham!.GiaNhap)) * ct.SoLuong);

        // ── Tong quan ──────────────────────────────────────────
        var donHangThang = await _context.DonHangs
            .Where(d => d.NgayDat >= startOfMonth && d.TrangThai != TrangThaiDonHang.DaHuy)
            .ToListAsync();

        var vm = new BaoCaoViewModel
        {
            DoanhThu7Ngay = doanhThu7Ngay,
            DoanhThu30Ngay = doanhThu30Ngay,
            TopSanPhamBanChay = topSanPham,
            DoanhThuTheoDanhMuc = doanhThuDanhMuc,
            LoiNhuanThang = loiNhuanThang,
            LoiNhuanHomNay = loiNhuanHomNay,
            TongDonHangThang = donHangThang.Count,
            DoanhThuThang = donHangThang.Sum(d => d.TongTienHang - d.TienGiam),
            DoanhThuHomNay = donHangThang
                .Where(d => d.NgayDat.Date == today)
                .Sum(d => d.TongTienHang - d.TienGiam)
        };

        return View(vm);
    }
}
