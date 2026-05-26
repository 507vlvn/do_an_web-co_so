using do_an.Data;
using do_an.Models;
using Microsoft.EntityFrameworkCore;

namespace do_an.Services;

/// <summary>
/// Triển khai service xử lý nghiệp vụ voucher.
/// Trích xuất logic từ GioHangController.ApplyVoucherAjax và DatHang.
/// </summary>
public class VoucherService : IVoucherService
{
    private readonly AppDbContext _context;

    public VoucherService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Kiểm tra tính hợp lệ của voucher và tính tiền giảm.
    /// Logic tương đương GioHangController.ApplyVoucherAjax().
    /// </summary>
    public async Task<VoucherResult> ApDungVoucherAsync(string maVoucher, decimal tongTienHang)
    {
        if (string.IsNullOrWhiteSpace(maVoucher))
            return new VoucherResult { Success = false, Message = "Vui lòng nhập mã giảm giá." };

        var now = DateTime.Now;

        // So sánh không phân biệt hoa thường
        var voucher = await _context.Vouchers.FirstOrDefaultAsync(v =>
            v.MaVoucher.ToLower() == maVoucher.ToLower() && v.KichHoat);

        if (voucher == null)
            return new VoucherResult { Success = false, Message = "Mã giảm giá không tồn tại." };

        if (voucher.DaSuDung >= voucher.SoLanSuDung)
            return new VoucherResult { Success = false, Message = "Mã giảm giá đã hết lượt sử dụng." };

        if (now < voucher.NgayBatDau || now > voucher.NgayHetHan)
            return new VoucherResult { Success = false, Message = "Mã giảm giá đã hết hạn hoặc chưa đến ngày áp dụng." };

        if (tongTienHang < voucher.DonHangToiThieu)
            return new VoucherResult
            {
                Success = false,
                Message = $"Đơn hàng tối thiểu {voucher.DonHangToiThieu:N0}đ để áp dụng mã này."
            };

        // Tính toán giảm giá
        decimal tienGiam = tongTienHang * voucher.PhanTramGiam / 100;
        if (voucher.GiamToiDa.HasValue)
            tienGiam = Math.Min(tienGiam, voucher.GiamToiDa.Value);

        return new VoucherResult
        {
            Success = true,
            Message = $"Áp dụng thành công mã {voucher.MaVoucher}.",
            TienGiam = tienGiam,
            Voucher = voucher
        };
    }

    /// <summary>
    /// Kiểm tra voucher có hợp lệ hay không (không tính tiền giảm).
    /// </summary>
    public async Task<bool> KiemTraVoucherHopLeAsync(string maVoucher)
    {
        if (string.IsNullOrWhiteSpace(maVoucher)) return false;

        var now = DateTime.Now;
        var voucher = await _context.Vouchers.FirstOrDefaultAsync(v =>
            v.MaVoucher.ToLower() == maVoucher.ToLower()
            && v.KichHoat
            && v.DaSuDung < v.SoLanSuDung
            && now >= v.NgayBatDau
            && now <= v.NgayHetHan);

        return voucher != null;
    }
}
