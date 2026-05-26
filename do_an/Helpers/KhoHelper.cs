using do_an.Data;
using Microsoft.EntityFrameworkCore;

namespace do_an.Helpers;

public class TruKhoFefoResult
{
    public bool Success { get; set; }
    public List<LoDaTru> CacLoDaTru { get; set; } = new();
}

public class LoDaTru
{
    public string? MaLo { get; set; }
    public int SoLuong { get; set; }
    public decimal GiaBan { get; set; }
}

public static class KhoHelper
{
    /// Thuật toán trừ kho theo nguyên tắc FEFO (First Expire First Out)
    /// Trừ Lô thuốc hết hạn sớm nhất trước. Cập nhật tổng thể cả bảng SanPham.
    /// Dùng cho cả POS (Bán hàng) và Web Store.
    /// Trả về TruKhoFefoResult chứa trạng thái thành công và danh sách các lô đã trừ kèm giá bán.
    public static async Task<TruKhoFefoResult> TruKhoFEFOAsync(AppDbContext context, int sanPhamId, int soLuongCanTru)
    {
        var sp = await context.SanPhams.FindAsync(sanPhamId);
        if (sp == null || sp.SoLuong < soLuongCanTru) 
            return new TruKhoFefoResult { Success = false };

        var cacLoDaTru = new List<LoDaTru>();

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
                    MaLo = "KHOIPHUC-" + DateTime.Now.ToString("yyyyMMdd"),
                    SanPhamId = sanPhamId,
                    SoLuongBanDau = sp.SoLuong,
                    SoLuongKhaDung = sp.SoLuong,
                    NgaySX = DateTime.Now.AddYears(-1),
                    HanSuDung = DateTime.Now.AddYears(1),
                    GiaNhap = sp.GiaNhap,
                    GiaBan = sp.GiaBan,
                    TrangThai = true
                };
                context.LoThuocs.Add(loLegacy);
                await context.SaveChangesAsync();

                cacLo.Add(loLegacy);
                tongTon += loLegacy.SoLuongKhaDung;
            }

            if (tongTon < soLuongCanTru) 
                return new TruKhoFefoResult { Success = false };

            var cacLoSapXepMoiNhat = cacLo.OrderByDescending(l => l.HanSuDung).ToList();
            var loApDung = cacLoSapXepMoiNhat.Count >= 2 ? cacLoSapXepMoiNhat[1] : cacLoSapXepMoiNhat.FirstOrDefault();
            decimal giaApDung = (loApDung != null && loApDung.GiaBan > 0) ? loApDung.GiaBan : sp.GiaBan;
            string? maLoApDung = loApDung?.MaLo;

            int soLuongConLaiCanTru = soLuongCanTru;

            foreach (var lo in cacLo)
            {
                if (soLuongConLaiCanTru <= 0) break;

                int truTuLoNay = Math.Min(lo.SoLuongKhaDung, soLuongConLaiCanTru);
                if (truTuLoNay > 0)
                {
                    lo.SoLuongKhaDung -= truTuLoNay;
                    soLuongConLaiCanTru -= truTuLoNay;
                }
            }

            cacLoDaTru.Add(new LoDaTru
            {
                MaLo = maLoApDung,
                SoLuong = soLuongCanTru,
                GiaBan = giaApDung
            });

            // Lưu thay đổi số lượng khả dụng của lô vào database trước khi truy vấn đồng bộ giá/HSD
            await context.SaveChangesAsync();

            // Đồng bộ HanSuDung/NgaySX của SanPham theo lô FEFO hiện tại
            await CapNhatHanSuDungTheoLoAsync(context, sp);
        }
        else
        {
            // Đối với sản phẩm thường, không có lô
            cacLoDaTru.Add(new LoDaTru
            {
                MaLo = null,
                SoLuong = soLuongCanTru,
                GiaBan = sp.GiaBan
            });
        }

        sp.SoLuong -= soLuongCanTru;
        return new TruKhoFefoResult
        {
            Success = true,
            CacLoDaTru = cacLoDaTru
        };
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

                // Lưu thay đổi vào DB trước khi đồng bộ
                await context.SaveChangesAsync();

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
        // Lấy danh sách tất cả lô còn hàng, còn hạn, đang hoạt động
        var cacLo = await context.LoThuocs
            .Where(l => l.SanPhamId == sp.Id && l.SoLuongKhaDung > 0 && l.TrangThai
                        && l.HanSuDung > DateTime.Today)
            .ToListAsync();

        var loHienTai = cacLo.OrderBy(l => l.HanSuDung).FirstOrDefault();

        if (loHienTai != null)
        {
            // Cập nhật HSD và NgaySX theo lô đang bán (FEFO - hết hạn sớm nhất)
            sp.HanSuDung = loHienTai.HanSuDung;
            sp.NgaySX = loHienTai.NgaySX;

            // Đồng bộ Giá nhập và Giá bán từ lô kề mới nhất (second newest lot) để hiển thị ở POS / Store
            var cacLoSapXepMoiNhat = cacLo.OrderByDescending(l => l.HanSuDung).ToList();
            var loGia = cacLoSapXepMoiNhat.Count >= 2 ? cacLoSapXepMoiNhat[1] : cacLoSapXepMoiNhat.FirstOrDefault();

            if (loGia != null)
            {
                sp.GiaNhap = loGia.GiaNhap;
                sp.GiaBan = loGia.GiaBan;
            }
            else
            {
                sp.GiaNhap = loHienTai.GiaNhap;
                sp.GiaBan = loHienTai.GiaBan;
            }
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

            // Đồng bộ giá trị mặc định cho các lô cũ chưa có giá nhập/bán
            var los = await context.LoThuocs.Where(l => l.SanPhamId == sp.Id).ToListAsync();
            bool needSave = false;

            // Tự động sửa chữa: Nếu tổng số lượng trong các lô nhỏ hơn tổng tồn kho thực tế của thuốc,
            // tạo một lô LEGACY để hấp thụ lượng chênh lệch này giúp hiển thị đầy đủ và không bị lỗi kho.
            int sumLo = los.Sum(l => l.SoLuongKhaDung);
            if (sumLo < sp.SoLuong)
            {
                int delta = sp.SoLuong - sumLo;
                var newLo = new do_an.Models.LoThuoc
                {
                    MaLo = "KHOIPHUC-" + DateTime.Now.ToString("yyyyMMdd"),
                    SanPhamId = sp.Id,
                    SoLuongBanDau = delta,
                    SoLuongKhaDung = delta,
                    NgaySX = DateTime.Today.AddMonths(-6),
                    HanSuDung = DateTime.Today.AddYears(1),
                    GiaNhap = sp.GiaNhap,
                    GiaBan = sp.GiaBan,
                    TrangThai = true
                };
                context.LoThuocs.Add(newLo);
                los.Add(newLo);
                needSave = true;
            }

            foreach (var lo in los)
            {
                if (lo.GiaNhap == 0)
                {
                    lo.GiaNhap = sp.GiaNhap;
                    needSave = true;
                }
                if (lo.GiaBan == 0)
                {
                    lo.GiaBan = sp.GiaBan;
                    needSave = true;
                }
            }
            if (needSave)
            {
                await context.SaveChangesAsync();
            }

            await CapNhatHanSuDungTheoLoAsync(context, sp);

            if (sp.HanSuDung != oldHSD || sp.NgaySX != oldNSX)
                count++;
        }

        await context.SaveChangesAsync();
        return count;
    }

    /// <summary>
    /// Tính giá bán của sản phẩm dựa trên số lượng mua.
    /// Nếu là thuốc, và số lượng mua vượt quá số lượng của lô đầu tiên (FEFO),
    /// ta sẽ lấy giá bán của lô kề mới nhất (second newest lot).
    /// Ngược lại, lấy giá của lô đầu tiên (FEFO).
    /// </summary>
    public static async Task<decimal> LayGiaBanTheoSoLuongAsync(AppDbContext context, int sanPhamId, int soLuong)
    {
        var sp = await context.SanPhams.FindAsync(sanPhamId);
        if (sp == null) return 0;

        if (sp.IsThuoc)
        {
            var cacLo = await context.LoThuocs
                .Where(l => l.SanPhamId == sanPhamId && l.SoLuongKhaDung > 0 && l.TrangThai
                            && l.HanSuDung > DateTime.Today)
                .OrderBy(l => l.HanSuDung)
                .ToListAsync();

            if (cacLo.Any())
            {
                var loDauTien = cacLo.First();
                if (soLuong > loDauTien.SoLuongKhaDung)
                {
                    // Lấy giá của lô kề mới nhất (second newest)
                    var cacLoSapXepMoiNhat = cacLo.OrderByDescending(l => l.HanSuDung).ToList();
                    var loGia = cacLoSapXepMoiNhat.Count >= 2 ? cacLoSapXepMoiNhat[1] : cacLoSapXepMoiNhat.FirstOrDefault();
                    return (loGia != null && loGia.GiaBan > 0) ? loGia.GiaBan : sp.GiaBan;
                }
                else
                {
                    return loDauTien.GiaBan;
                }
            }
        }

        return sp.GiaBan;
    }
}

