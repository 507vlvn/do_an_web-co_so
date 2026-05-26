using do_an.Data;
using do_an.Helpers;
using do_an.Models;
using Microsoft.EntityFrameworkCore;

namespace do_an.Services;

public class DonHangAutoHuyService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DonHangAutoHuyService> _logger;

    public DonHangAutoHuyService(IServiceScopeFactory scopeFactory,
        ILogger<DonHangAutoHuyService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await HuyDonHangQuaHanAsync();
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // App đang tắt → thoát vòng lặp bình thường
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tự động hủy đơn hàng quá hạn.");
            }
        }
    }

    private async Task HuyDonHangQuaHanAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var deadline = DateTime.Now.AddMinutes(-30);

        var donHangsQuaHan = await db.DonHangs
            .Include(d => d.ChiTietDonHangs)
            .Where(d => d.TrangThai == TrangThaiDonHang.ChoThanhToan
                     && d.LoaiDon == LoaiDonHang.Online
                     && d.PhuongThucThanhToan == PhuongThucThanhToan.ThanhToanOnline
                     && d.NgayDat < deadline)
            .ToListAsync();

        if (donHangsQuaHan.Count == 0) return;

        foreach (var donHang in donHangsQuaHan)
        {
            donHang.TrangThai = TrangThaiDonHang.DaHuy;
            donHang.NgayHuy = DateTime.Now;

            foreach (var ct in donHang.ChiTietDonHangs)
            {
                var sp = await db.SanPhams.FindAsync(ct.SanPhamId);
                if (sp is not null)
                {
                    if (sp.IsThuoc)
                    {
                        await KhoHelper.HoanKhoFEFOAsync(db, sp.Id, ct.SoLuong);
                    }
                    else
                    {
                        sp.SoLuong += ct.SoLuong;
                    }
                }
            }

            _logger.LogInformation("Tự động hủy đơn {Ma} (quá 30 phút chưa thanh toán).",
                donHang.MaDonHang);
        }

        await db.SaveChangesAsync();
    }
}