using do_an.Models;

namespace do_an.ViewModels;

public class DashboardViewModel
{
    // ── Chung ────────────────────────────────────────────────
    public int TotalSanPham       { get; set; }
    public int TotalTaiKhoan      { get; set; }
    public int NhanVienCount      { get; set; }

    // ── Quản lý Lô & Tồn Kho ─────────────────────────────────
    public IEnumerable<SanPham> LowStockSanPham  { get; set; } = new List<SanPham>();
    public IEnumerable<SanPham> ExpiringSanPham  { get; set; } = new List<SanPham>();
    public IEnumerable<LoThuoc> ExpiringLoThuoc  { get; set; } = new List<LoThuoc>();

    // ── POS (Bán Hàng Tại Quầy) ──────────────────────────────
    public int POSInvoicesToday      { get; set; }
    public decimal POSRevenueToday   { get; set; }
    public decimal POSRevenueThisMonth { get; set; }
    public int TotalPOSInvoices      { get; set; }

    // ── Online Shop ──────────────────────────────────────────
    public int DonHangChoXuLy           { get; set; } // ChoThanhToan + DaThanhToan + DangChuanBi
    public int DonHangOnlineHomNay      { get; set; }
    public decimal DoanhThuOnlineHomNay { get; set; }
    public decimal DoanhThuOnlineThang  { get; set; }
    public IEnumerable<DonHang> DonHangMoiNhat { get; set; } = new List<DonHang>();

    // ── Đơn thuốc kê đơn ─────────────────────────────────────
    public int TotalDonThuoc { get; set; }
    public int DonThuocChoDuyet { get; set; }
    public int DonThuocDaDuyet { get; set; }
    public int DonThuocTrenStore { get; set; }
    public IEnumerable<DonThuoc> DonThuocMoiNhat { get; set; } = new List<DonThuoc>();
}
