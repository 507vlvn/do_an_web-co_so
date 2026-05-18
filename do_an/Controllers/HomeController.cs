using do_an.Data;
using do_an.Models;
using do_an.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace do_an.Controllers;

[Authorize(Roles = "Admin,NhanVien")]
public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var today = DateTime.Today;
        var twoMonthLater = today.AddDays(60);
        var now = DateTime.Now;

        // Tính doanh thu 7 ngày gần nhất (Online và POS)
        var last7Days = Enumerable.Range(0, 7).Select(i => today.AddDays(-i)).Reverse().ToList();
        var chartLabels = last7Days.Select(d => d.ToString("dd/MM")).ToList();
        var chartData = new List<decimal>();

        foreach (var d in last7Days)
        {
            // Trạng thái thành công cho DonHang có thể là:
            // POS: DaThanhToan
            // Online: DaGiao
            // Để đơn giản, tính tổng các đơn đã thanh toán hoặc đã giao
            var revenue = await _context.DonHangs
                .Where(dh => dh.NgayDat.Date == d && 
                             (dh.TrangThai == TrangThaiDonHang.DaThanhToan || dh.TrangThai == TrangThaiDonHang.DaGiao))
                .SumAsync(dh => (decimal?)(dh.TongTienHang - dh.TienGiam)) ?? 0;
            chartData.Add(revenue);
        }

        ViewBag.ChartLabels = System.Text.Json.JsonSerializer.Serialize(chartLabels);
        ViewBag.ChartData = System.Text.Json.JsonSerializer.Serialize(chartData);

        var vm = new DashboardViewModel
        {
            TotalSanPham = await _context.SanPhams.CountAsync(),
            TotalTaiKhoan = await _context.TaiKhoans.CountAsync(),
            NhanVienCount = await _context.TaiKhoans.CountAsync(t => t.VaiTro == "NhanVien"),

            // ── Quản lý Lô & Tồn Kho ─────────────────────────────────
            //logic : thuốc sắp hết : <20 đơn vị 
            //        thuốc hết hạn : <60 ngày
            LowStockSanPham = await _context.SanPhams
                .Where(s => s.SoLuong < 20)
                .OrderBy(s => s.SoLuong)
                .Take(5)
                .ToListAsync(),
           ExpiringSanPham = new List<SanPham>(), // HSD giờ cảnh báo qua ExpiringLoThuoc (chính xác hơn)


            ExpiringLoThuoc = await _context.LoThuocs
            .Include(l => l.SanPham)
            .Where(l => l.HanSuDung <= twoMonthLater && l.HanSuDung >= today && l.SoLuongKhaDung > 0)
            .OrderBy(l => l.HanSuDung)
            .Take(5)

            .ToListAsync(),
            // ── POS (Bán Hàng Tại Quầy) ──────────────────────────────
            POSInvoicesToday = await _context.DonHangs.CountAsync(d => d.LoaiDon == LoaiDonHang.POS_TaiQuay && d.NgayDat.Date == today),
            POSRevenueToday = await _context.DonHangs
                .Where(d => d.LoaiDon == LoaiDonHang.POS_TaiQuay && d.NgayDat.Date == today && (d.TrangThai == TrangThaiDonHang.DaThanhToan || d.TrangThai == TrangThaiDonHang.DaGiao))
                .SumAsync(d => (decimal?)(d.TongTienHang - d.TienGiam)) ?? 0,
            POSRevenueThisMonth = await _context.DonHangs
                .Where(d => d.LoaiDon == LoaiDonHang.POS_TaiQuay && d.NgayDat.Year == now.Year && d.NgayDat.Month == now.Month && (d.TrangThai == TrangThaiDonHang.DaThanhToan || d.TrangThai == TrangThaiDonHang.DaGiao))
                .SumAsync(d => (decimal?)(d.TongTienHang - d.TienGiam)) ?? 0,
            TotalPOSInvoices = await _context.DonHangs.CountAsync(d => d.LoaiDon == LoaiDonHang.POS_TaiQuay),

            // ── Online Shop ──────────────────────────────────────────
            DonHangChoXuLy = await _context.DonHangs.CountAsync(d =>
                d.LoaiDon == LoaiDonHang.Online &&
                (d.TrangThai == TrangThaiDonHang.ChoThanhToan ||
                d.TrangThai == TrangThaiDonHang.DaThanhToan ||
                d.TrangThai == TrangThaiDonHang.DangChuanBi)),
            DonHangOnlineHomNay = await _context.DonHangs.CountAsync(d =>
                d.LoaiDon == LoaiDonHang.Online && d.NgayDat.Date == today),
            DoanhThuOnlineHomNay = await _context.DonHangs
                .Where(d => d.LoaiDon == LoaiDonHang.Online && d.NgayDat.Date == today &&
                            d.TrangThai == TrangThaiDonHang.DaGiao)
                .SumAsync(d => (decimal?)(d.TongTienHang - d.TienGiam)) ?? 0,
            DoanhThuOnlineThang = await _context.DonHangs
                .Where(d => d.LoaiDon == LoaiDonHang.Online && d.NgayDat.Year == now.Year &&
                            d.NgayDat.Month == now.Month &&
                            d.TrangThai == TrangThaiDonHang.DaGiao)
                .SumAsync(d => (decimal?)(d.TongTienHang - d.TienGiam)) ?? 0,
            DonHangMoiNhat = await _context.DonHangs
                .Where(d => d.LoaiDon == LoaiDonHang.Online)
                .OrderByDescending(d => d.NgayDat)
                .Take(5)
                .ToListAsync(),

            // ── Đơn thuốc kê đơn ─────────────────────────────────────
            TotalDonThuoc = await _context.DonThuocs.CountAsync(),
            DonThuocChoDuyet = await _context.DonThuocs.CountAsync(d =>
                d.TrangThai == TrangThaiDonThuoc.MoiTao),
            DonThuocDaDuyet = await _context.DonThuocs.CountAsync(d =>
                d.TrangThai == TrangThaiDonThuoc.DaDuyet),
            DonThuocTrenStore = await _context.DonThuocs.CountAsync(d =>
                d.DaDangStore),
            DonThuocMoiNhat = await _context.DonThuocs
                .Include(d => d.ChiTietDonThuocs)
                .OrderByDescending(d => d.Id)
                .Take(5)
                .ToListAsync()
        };

        return View(vm);
    }

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
        => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}