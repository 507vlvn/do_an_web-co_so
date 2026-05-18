using do_an.Data;
using Microsoft.EntityFrameworkCore;

namespace do_an.Helpers;

public static class KhoHelper
{
    /// Thuật toán trừ kho theo nguyên tắc FEFO (First Expire First Out)
    /// Trừ Lô thuốc hết hạn sớm nhất trước. Cập nhật tổng thể cả bảng SanPham.
    /// Dùng cho cả POS (Bán hàng) và Web Store.
    /// Trả về true nếu đủ hàng và đã trừ, false nếu không đủ hàng.
    public static async Task<bool> TruKhoFEFOAsync(AppDbContext context, int sanPhamId, int soLuongCanTru)
    {
        var sp = await context.SanPhams.FindAsync(sanPhamId);
        if (sp == null || sp.SoLuong < soLuongCanTru) return false;

        // Nếu sản phẩm được quản lý theo lô (là Thuốc)
        if (sp.IsThuoc)
        {
            var cacLo = await context.LoThuocs
                .Where(l => l.SanPhamId == sanPhamId && l.SoLuongKhaDung > 0 && l.TrangThai
                            && l.HanSuDung > DateTime.Today) // Không bán lô đã hết hạn
                .OrderBy(l => l.HanSuDung)
                .ToListAsync();

            int tongTon = cacLo.Sum(l => l.SoLuongKhaDung);

            // Bổ sung lô ảo nếu database bị hụt so với master (chỉ gặp khi đang chuyển đổi database bị lỗi seed)
            if (tongTon < soLuongCanTru)
            {
                var loLegacy = new do_an.Models.LoThuoc
                {
                    MaLo = "LEGACY-" + DateTime.Now.ToString("yyyyMMdd"),
                    SanPhamId = sanPhamId,
                    SoLuongBanDau = sp.SoLuong,
                    SoLuongKhaDung = sp.SoLuong,
                    NgaySX = DateTime.Now.AddYears(-1),
                    HanSuDung = DateTime.Now.AddYears(1),
                    TrangThai = true
                };
                context.LoThuocs.Add(loLegacy);
                await context.SaveChangesAsync();

                cacLo.Add(loLegacy);
                tongTon += loLegacy.SoLuongKhaDung;
            }

            if (tongTon < soLuongCanTru) return false;

            int soLuongConLaiCanTru = soLuongCanTru;

            foreach (var lo in cacLo)
            {
                if (soLuongConLaiCanTru <= 0) break;

                int truTuLoNay = Math.Min(lo.SoLuongKhaDung, soLuongConLaiCanTru);
                
                lo.SoLuongKhaDung -= truTuLoNay;
                soLuongConLaiCanTru -= truTuLoNay;
            }

            // Đồng bộ HanSuDung/NgaySX của SanPham theo lô FEFO hiện tại
            await CapNhatHanSuDungTheoLoAsync(context, sp);
        }

        sp.SoLuong -= soLuongCanTru;
        return true;
    }

    /// Hoàn lại kho vào Lô có hạn sử dụng xa nhất
    public static async Task HoanKhoFEFOAsync(AppDbContext context, int sanPhamId, int soLuongHoan)
    {
        var sp = await context.SanPhams.FindAsync(sanPhamId);
        if (sp != null)
        {
            if (sp.IsThuoc)
            {
                var loPhuHop = await context.LoThuocs
                    .Where(l => l.SanPhamId == sanPhamId && l.SoLuongKhaDung < l.SoLuongBanDau)
                    .OrderByDescending(l => l.HanSuDung)
                    .FirstOrDefaultAsync();

                if (loPhuHop != null)
                {
                    loPhuHop.SoLuongKhaDung += soLuongHoan;
                }
                else
                {
                    var loCuoi = await context.LoThuocs
                        .Where(l => l.SanPhamId == sanPhamId)
                        .OrderByDescending(l => l.HanSuDung)
                        .FirstOrDefaultAsync();
                    if (loCuoi != null) loCuoi.SoLuongKhaDung += soLuongHoan;
                }

                // Đồng bộ HanSuDung/NgaySX sau khi hoàn kho
                await CapNhatHanSuDungTheoLoAsync(context, sp);
            }

            sp.SoLuong += soLuongHoan;
        }
    }

    /// <summary>
    /// Đồng bộ SanPham.HanSuDung và NgaySX theo lô FEFO đang bán hiện tại.
    /// Lô đang bán = lô có SoLuongKhaDung > 0, sắp xếp theo HanSuDung tăng dần (hết hạn sớm nhất = bán trước).
    /// Khi lô cũ bán hết → tự động chuyển sang hiển thị HSD của lô kế tiếp.
    /// </summary>
    public static async Task CapNhatHanSuDungTheoLoAsync(AppDbContext context, do_an.Models.SanPham sp)
    {
        // Tìm lô đang bán hiện tại (FEFO: lô hết hạn sớm nhất còn hàng)
        var loHienTai = await context.LoThuocs
            .Where(l => l.SanPhamId == sp.Id && l.SoLuongKhaDung > 0 && l.TrangThai
                        && l.HanSuDung > DateTime.Today) // Chỉ lấy lô còn hạn
            .OrderBy(l => l.HanSuDung)
            .FirstOrDefaultAsync();

        if (loHienTai != null)
        {
            // Cập nhật HSD và NgaySX theo lô đang bán
            sp.HanSuDung = loHienTai.HanSuDung;
            sp.NgaySX = loHienTai.NgaySX;
        }
        else
        {
            // Không còn lô nào có hàng → xóa HSD (hết hàng hoàn toàn)
            sp.HanSuDung = null;
            sp.NgaySX = null;
        }
    }

    /// <summary>
    /// Đồng bộ HanSuDung/NgaySX cho TẤT CẢ sản phẩm thuốc có lô.
    /// Dùng 1 lần để fix dữ liệu cũ, hoặc gọi khi cần re-sync toàn bộ.
    /// </summary>
    public static async Task<int> DongBoTatCaHanSuDungAsync(AppDbContext context)
    {
        var danhSachThuoc = await context.SanPhams
            .Where(s => s.IsThuoc)
            .ToListAsync();

        int count = 0;
        foreach (var sp in danhSachThuoc)
        {
            var oldHSD = sp.HanSuDung;
            var oldNSX = sp.NgaySX;

            await CapNhatHanSuDungTheoLoAsync(context, sp);

            if (sp.HanSuDung != oldHSD || sp.NgaySX != oldNSX)
                count++;
        }

        await context.SaveChangesAsync();
        return count;
    }
}

