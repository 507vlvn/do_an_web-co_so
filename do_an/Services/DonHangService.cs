using do_an.Data;
using do_an.Helpers;
using do_an.Models;
using do_an.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace do_an.Services;

/// <summary>
/// Triển khai service xử lý nghiệp vụ đơn hàng.
/// Trích xuất logic từ GioHangController.DatHang và các action liên quan.
/// </summary>
public class DonHangService : IDonHangService
{
    private readonly AppDbContext _context;
    private readonly IInventoryService _inventoryService;
    private readonly IVoucherService _voucherService;
    private readonly ILogger<DonHangService> _logger;

    public DonHangService(
        AppDbContext context,
        IInventoryService inventoryService,
        IVoucherService voucherService,
        ILogger<DonHangService> logger)
    {
        _context = context;
        _inventoryService = inventoryService;
        _voucherService = voucherService;
        _logger = logger;
    }

    /// <summary>
    /// Tạo đơn hàng mới từ giỏ hàng.
    /// Logic tương đương GioHangController.DatHang().
    /// </summary>
    public async Task<DatHangResult> TaoDonHangAsync(CheckoutViewModel vm, List<CartItem> cart, string? username)
    {
        if (cart.Count == 0)
            return new DatHangResult { Success = false, ErrorMessage = "Giỏ hàng trống." };

        // Kiểm tra tồn kho trước khi tạo đơn
        foreach (var item in cart)
        {
            bool coHang = await _inventoryService.KiemTraTonKhoAsync(item.SanPhamId, item.SoLuong);
            if (!coHang)
                return new DatHangResult
                {
                    Success = false,
                    ErrorMessage = $"\"{item.TenSanPham}\" không đủ hàng."
                };
        }

        // Xử lý voucher
        decimal tienGiam = 0;
        Voucher? voucherApplied = null;
        if (!string.IsNullOrWhiteSpace(vm.MaVoucher))
        {
            var voucherResult = await _voucherService.ApDungVoucherAsync(vm.MaVoucher, vm.TongTienHang);
            if (voucherResult.Success)
            {
                tienGiam = voucherResult.TienGiam;
                voucherApplied = voucherResult.Voucher;
            }
            else
            {
                return new DatHangResult
                {
                    Success = false,
                    ErrorMessage = voucherResult.Message
                };
            }
        }

        // Tạo đơn hàng
        var donHang = new DonHang
        {
            MaDonHang = $"DH{DateTime.Now:yyyyMMddHHmmss}{Random.Shared.Next(100, 999)}",
            TenDangNhap = username,
            HoTenNguoiNhan = vm.HoTenNguoiNhan,
            SoDienThoai = vm.SoDienThoai,
            DiaChi = vm.DiaChi,
            Email = vm.Email,
            GhiChu = vm.GhiChu,
            MaVoucher = vm.MaVoucher,
            TongTienHang = vm.TongTienHang,
            TienGiam = tienGiam,
            PhuongThucThanhToan = vm.PhuongThucThanhToan,
            TrangThai = TrangThaiDonHang.ChoThanhToan,
            NgayDat = DateTime.Now
        };

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Trừ tồn kho và tạo chi tiết
            foreach (var item in cart)
            {
                var sp = await _context.SanPhams.FindAsync(item.SanPhamId);

                var truResult = await _inventoryService.TruKhoAsync(item.SanPhamId, item.SoLuong);
                if (!truResult.Success)
                    throw new DbUpdateException($"\"{item.TenSanPham}\" không đủ kho thực tế.");

                foreach (var split in truResult.CacLoDaTru)
                {
                    donHang.ChiTietDonHangs.Add(new ChiTietDonHang
                    {
                        SanPhamId = item.SanPhamId,
                        TenSanPhamSnapshot = string.IsNullOrEmpty(split.MaLo) 
                            ? sp!.TenSanPham 
                            : $"{sp!.TenSanPham} (Lô: {split.MaLo})",
                        SoLuong = split.SoLuong,
                        GiaBan = split.GiaBan
                    });
                }
            }

            // Cập nhật tổng tiền theo giá thực tế từ DB
            donHang.TongTienHang = donHang.ChiTietDonHangs.Sum(ct => ct.GiaBan * ct.SoLuong);

            // Lưu thông tin KH cho lần sau
            if (!string.IsNullOrEmpty(username))
            {
                var tk = await _context.TaiKhoans.FindAsync(username);
                if (tk is not null)
                {
                    tk.HoTen = vm.HoTenNguoiNhan;
                    tk.SoDienThoai = vm.SoDienThoai;
                    tk.DiaChi = vm.DiaChi;
                    if (!string.IsNullOrEmpty(vm.Email)) tk.Email = vm.Email;
                }
            }

            // Tăng lượt dùng voucher trong transaction
            if (voucherApplied is not null)
                voucherApplied.DaSuDung++;

            _context.DonHangs.Add(donHang);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("Đã tạo đơn hàng {MaDonHang} cho {Username}.", donHang.MaDonHang, username ?? "khách vãng lai");

            return new DatHangResult { Success = true, DonHang = donHang };
        }
        catch (DbUpdateConcurrencyException)
        {
            await transaction.RollbackAsync();
            return new DatHangResult
            {
                Success = false,
                ErrorMessage = "Rất tiếc! Sản phẩm bạn chọn vừa được mua bởi khách hàng khác. Vui lòng thử lại."
            };
        }
        catch (DbUpdateException ex)
        {
            await transaction.RollbackAsync();
            string errorMsg = ex.Message.StartsWith("\"")
                ? ex.Message
                : "Lỗi cơ sở dữ liệu: " + (ex.InnerException?.Message ?? ex.Message);
            return new DatHangResult { Success = false, ErrorMessage = errorMsg };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Lỗi nghiêm trọng khi tạo đơn hàng.");
            return new DatHangResult
            {
                Success = false,
                ErrorMessage = "Đã xảy ra lỗi hệ thống nghiêm trọng. Xin lỗi vì sự bất tiện."
            };
        }
    }

    /// <summary>
    /// Hủy đơn hàng và hoàn kho.
    /// Logic tương đương GioHangController.HuyDonHang().
    /// </summary>
    public async Task<bool> HuyDonHangAsync(int donHangId, string? username)
    {
        var dh = await _context.DonHangs
            .Include(d => d.ChiTietDonHangs)
            .FirstOrDefaultAsync(d => d.Id == donHangId);

        if (dh is null) return false;

        // Chống IDOR: chỉ chủ đơn mới được hủy
        if (dh.TenDangNhap != null && dh.TenDangNhap != username)
            return false;

        if (dh.TrangThai != TrangThaiDonHang.ChoThanhToan &&
            dh.TrangThai != TrangThaiDonHang.DaThanhToan)
            return false;

        dh.TrangThai = TrangThaiDonHang.DaHuy;
        dh.NgayHuy = DateTime.Now;

        // Hoàn lại tồn kho
        foreach (var ct in dh.ChiTietDonHangs)
        {
            await _inventoryService.HoanKhoAsync(ct.SanPhamId ?? 0, ct.SoLuong);
        }

        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Xác nhận thanh toán cho đơn hàng.
    /// Logic tương đương GioHangController.XacNhanThanhToan().
    /// </summary>
    public async Task<bool> XacNhanThanhToanAsync(int donHangId, string? username)
    {
        var dh = await _context.DonHangs.FindAsync(donHangId);
        if (dh is null) return false;

        // Chống IDOR: chỉ chủ đơn mới được xác nhận
        if (dh.TenDangNhap != null && dh.TenDangNhap != username)
            return false;

        if (dh.TrangThai == TrangThaiDonHang.ChoThanhToan)
        {
            dh.TrangThai = TrangThaiDonHang.DaThanhToan;
            dh.NgayThanhToan = DateTime.Now;
            await _context.SaveChangesAsync();
        }

        return true;
    }
}
