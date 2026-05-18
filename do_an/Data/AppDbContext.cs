using do_an.Models;
using Microsoft.EntityFrameworkCore;

namespace do_an.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<TaiKhoan> TaiKhoans { get; set; }
    public DbSet<DanhMucSanPham> DanhMucSanPhams { get; set; }
    public DbSet<SanPham> SanPhams { get; set; }
    public DbSet<DonHang> DonHangs { get; set; }
    public DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }
    public DbSet<Voucher> Vouchers { get; set; }
    public DbSet<LoThuoc> LoThuocs { get; set; }
    public DbSet<GioHang> GioHangs { get; set; }
    public DbSet<ChiTietGioHang> ChiTietGioHangs { get; set; }
    public DbSet<DonThuoc> DonThuocs { get; set; }
    public DbSet<ChiTietDonThuoc> ChiTietDonThuocs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ── Tên bảng ──────────────────────────────────────────
        modelBuilder.Entity<TaiKhoan>().ToTable("TaiKhoan");
        modelBuilder.Entity<DanhMucSanPham>().ToTable("DanhMucSanPham");
        modelBuilder.Entity<SanPham>().ToTable("SanPham");
        modelBuilder.Entity<DonHang>().ToTable("DonHang");
        modelBuilder.Entity<ChiTietDonHang>().ToTable("ChiTietDonHang");
        modelBuilder.Entity<Voucher>().ToTable("Voucher");
        modelBuilder.Entity<LoThuoc>().ToTable("LoThuoc");
        modelBuilder.Entity<GioHang>().ToTable("GioHang");
        modelBuilder.Entity<ChiTietGioHang>().ToTable("ChiTietGioHang");
        modelBuilder.Entity<DonThuoc>().ToTable("DonThuoc");
        modelBuilder.Entity<ChiTietDonThuoc>().ToTable("ChiTietDonThuoc");

        // ── Quan hệ ───────────────────────────────────
        modelBuilder.Entity<SanPham>()
            .HasOne(s => s.DanhMuc)
            .WithMany(d => d.SanPhams)
            .HasForeignKey(s => s.DanhMucId);

        modelBuilder.Entity<ChiTietDonHang>()
            .HasOne(c => c.DonHang)
            .WithMany(d => d.ChiTietDonHangs)
            .HasForeignKey(c => c.DonHangId);

        modelBuilder.Entity<ChiTietDonHang>()
            .HasOne(c => c.SanPham)
            .WithMany(s => s.ChiTietDonHangs)
            .HasForeignKey(c => c.SanPhamId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<DonHang>()
            .HasOne(d => d.KhachHang)
            .WithMany(t => t.DonHangsDat)
            .HasForeignKey(d => d.TenDangNhap)
            .IsRequired(false);

        modelBuilder.Entity<DonHang>()
            .HasOne(d => d.NhanVienBan)
            .WithMany(t => t.DonHangsXuLy)
            .HasForeignKey(d => d.NhanVienPhuTrachId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        // ── Quan hệ Đơn thuốc (mới) ──────────────────────────
        modelBuilder.Entity<ChiTietDonThuoc>()
            .HasOne(c => c.DonThuoc)
            .WithMany(d => d.ChiTietDonThuocs)
            .HasForeignKey(c => c.DonThuocId);

        modelBuilder.Entity<ChiTietDonThuoc>()
            .HasOne(c => c.SanPham)
            .WithMany()
            .HasForeignKey(c => c.SanPhamId)
            .OnDelete(DeleteBehavior.SetNull);

        // Ignore computed properties
        modelBuilder.Entity<ChiTietDonThuoc>().Ignore(c => c.TongSoVien);
        modelBuilder.Entity<DonHang>().Ignore(d => d.TongThanhToan);
        modelBuilder.Entity<ChiTietDonHang>().Ignore(c => c.ThanhTien);
        modelBuilder.Entity<Voucher>().Ignore(v => v.ConHieuLuc);
        modelBuilder.Entity<SanPham>().Ignore(s => s.PhanTramGiam);

        // ── Kiểu decimal ──────────────────────────────────────
        modelBuilder.Entity<TaiKhoan>(e =>
        {
            e.Property(t => t.HeSoLuong).HasColumnType("decimal(18,2)");
            e.Property(t => t.LuongTheoGio).HasColumnType("decimal(18,2)");
        });

        modelBuilder.Entity<SanPham>(e =>
        {
            e.Property(s => s.GiaBan).HasColumnType("decimal(18,2)");
            e.Property(s => s.GiaGoc).HasColumnType("decimal(18,2)");
            e.Property(s => s.GiaNhap).HasColumnType("decimal(18,2)");
        });

        modelBuilder.Entity<DonHang>(e =>
        {
            e.Property(d => d.TongTienHang).HasColumnType("decimal(18,2)");
            e.Property(d => d.TienGiam).HasColumnType("decimal(18,2)");
        });

        modelBuilder.Entity<ChiTietDonHang>(e =>
        {
            e.Property(c => c.GiaBan).HasColumnType("decimal(18,2)");
        });

        modelBuilder.Entity<Voucher>(e =>
        {
            e.Property(v => v.PhanTramGiam).HasColumnType("decimal(5,2)");
            e.Property(v => v.GiamToiDa).HasColumnType("decimal(18,2)");
            e.Property(v => v.DonHangToiThieu).HasColumnType("decimal(18,2)");
        });

        // ── Seed data ─────────────────────────────────
        modelBuilder.Entity<DanhMucSanPham>().HasData(
            new DanhMucSanPham { Id = 1, TenDanhMuc = "Kháng sinh", PhanLoai = "Thuoc", MoTa = "Nhóm thuốc kháng sinh" },
            new DanhMucSanPham { Id = 2, TenDanhMuc = "Giảm đau - Hạ sốt", PhanLoai = "Thuoc", MoTa = "Nhóm thuốc giảm đau" },
            new DanhMucSanPham { Id = 3, TenDanhMuc = "Tim mạch", PhanLoai = "Thuoc", MoTa = "Nhóm thuốc điều trị tim mạch" },
            new DanhMucSanPham { Id = 4, TenDanhMuc = "Tiêu hóa", PhanLoai = "Thuoc", MoTa = "Nhóm thuốc tiêu hóa" },
            new DanhMucSanPham { Id = 5, TenDanhMuc = "Mỹ phẩm", PhanLoai = "MyPham", MoTa = "Sản phẩm chăm sóc và làm đẹp da" },
            new DanhMucSanPham { Id = 6, TenDanhMuc = "Thực phẩm chức năng", PhanLoai = "ThucPhamChucNang", MoTa = "Bổ sung dinh dưỡng" },
            new DanhMucSanPham { Id = 7, TenDanhMuc = "Thiết bị y tế", PhanLoai = "ThietBiYTe", MoTa = "Dụng cụ gia đình" }
        );
    }
}