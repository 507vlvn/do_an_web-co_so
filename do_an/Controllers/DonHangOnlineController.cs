using do_an.Data;
using do_an.Helpers;
using do_an.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace do_an.Controllers;

[Authorize(Roles = "Admin,NhanVien")]
public class DonHangOnlineController : Controller
{
    private readonly AppDbContext _context;

    public DonHangOnlineController(AppDbContext context) => _context = context;

    // GET: /DonHang
    public async Task<IActionResult> Index(TrangThaiDonHang? trangThai, string? search)
    {
        var query = _context.DonHangs
            .Where(d => d.LoaiDon == LoaiDonHang.Online)
            .AsQueryable();

        if (trangThai.HasValue)
            query = query.Where(d => d.TrangThai == trangThai);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(d => d.MaDonHang.Contains(search) ||
                                     (d.HoTenNguoiNhan != null && d.HoTenNguoiNhan.Contains(search)) ||
                                     (d.SoDienThoai != null && d.SoDienThoai.Contains(search)));

        ViewBag.TrangThai = trangThai;
        ViewBag.Search    = search;
        ViewBag.Stats     = new
        {
            ChoThanhToan = await _context.DonHangs.CountAsync(d => d.TrangThai == TrangThaiDonHang.ChoThanhToan && d.LoaiDon == LoaiDonHang.Online),
            DaThanhToan  = await _context.DonHangs.CountAsync(d => d.TrangThai == TrangThaiDonHang.DaThanhToan && d.LoaiDon == LoaiDonHang.Online),
            DangGiao     = await _context.DonHangs.CountAsync(d => d.TrangThai == TrangThaiDonHang.DangGiao && d.LoaiDon == LoaiDonHang.Online),
            DaGiao       = await _context.DonHangs.CountAsync(d => d.TrangThai == TrangThaiDonHang.DaGiao && d.LoaiDon == LoaiDonHang.Online),
            DaHuy        = await _context.DonHangs.CountAsync(d => d.TrangThai == TrangThaiDonHang.DaHuy && d.LoaiDon == LoaiDonHang.Online)
        };

        return View(await query.OrderByDescending(d => d.NgayDat).ToListAsync());
    }

    // GET: /DonHang/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var dh = await _context.DonHangs
            .Include(d => d.ChiTietDonHangs)
            .ThenInclude(c => c.SanPham)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (dh is null) return NotFound();
        return View(dh);
    }

    // POST: /DonHang/CapNhatTrangThai
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CapNhatTrangThai(int id, TrangThaiDonHang trangThai)
    {
        var dh = await _context.DonHangs.FindAsync(id);
        if (dh is null) return NotFound();

        var chuyenHopLe = (dh.TrangThai, trangThai) switch
        {
            (TrangThaiDonHang.ChoThanhToan, TrangThaiDonHang.DaThanhToan) => true,
            (TrangThaiDonHang.ChoThanhToan, TrangThaiDonHang.DaHuy)       => true,
            (TrangThaiDonHang.DaThanhToan,  TrangThaiDonHang.DangChuanBi) => true,
            (TrangThaiDonHang.DaThanhToan,  TrangThaiDonHang.DaHuy)       => true,
            (TrangThaiDonHang.DangChuanBi,  TrangThaiDonHang.DangGiao)    => true,
            (TrangThaiDonHang.DangChuanBi,  TrangThaiDonHang.DaHuy)       => true,
            (TrangThaiDonHang.DangGiao,     TrangThaiDonHang.DaGiao)      => true,
            _ => false
        };

        if (!chuyenHopLe)
        {
            TempData["Error"] = $"Không thể chuyển từ \"{TrangThaiLabel(dh.TrangThai)}\" sang \"{TrangThaiLabel(trangThai)}\".";
            return RedirectToAction(nameof(Details), new { id });
        }

        if (trangThai == TrangThaiDonHang.DaHuy)
        {
            var chiTiet = await _context.ChiTietDonHangs.Where(c => c.DonHangId == id).ToListAsync();
            foreach (var ct in chiTiet)
            {
                if (ct.SanPhamId.HasValue)
                {
                    await KhoHelper.HoanKhoFEFOAsync(_context, ct.SanPhamId.Value, ct.SoLuong);
                }
            }
            dh.NgayHuy = DateTime.Now;
        }

        switch (trangThai)
        {
            case TrangThaiDonHang.DaThanhToan: dh.NgayThanhToan = DateTime.Now; break;
            case TrangThaiDonHang.DangChuanBi: dh.NgayChuanBi = DateTime.Now; break;
            case TrangThaiDonHang.DangGiao: dh.NgayBatDauGiao = DateTime.Now; break;
            case TrangThaiDonHang.DaGiao: dh.NgayGiao = DateTime.Now; break;
        }

        dh.TrangThai = trangThai;
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Đơn {dh.MaDonHang}: đã chuyển sang \"{TrangThaiLabel(trangThai)}\".";
        return RedirectToAction(nameof(Details), new { id });
    }

    // GET: /DonHang/Delete/{id}
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var donHang = await _context.DonHangs
            .Include(d => d.ChiTietDonHangs)
            .FirstOrDefaultAsync(m => m.Id == id && m.LoaiDon == LoaiDonHang.Online);
        
        if (donHang == null) return NotFound();

        return View(donHang);
    }

    // POST: /DonHang/Delete/{id}
    [Authorize(Roles = "Admin")]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, bool hoanKho = true)
    {
        var donHang = await _context.DonHangs
            .Include(d => d.ChiTietDonHangs)
            .FirstOrDefaultAsync(d => d.Id == id && d.LoaiDon == LoaiDonHang.Online);

        if (donHang == null) return NotFound();

        if (hoanKho)
        {
            // Hoàn kho
            foreach (var ct in donHang.ChiTietDonHangs)
            {
                if (ct.SanPhamId.HasValue)
                {
                    await KhoHelper.HoanKhoFEFOAsync(_context, ct.SanPhamId.Value, ct.SoLuong);
                }
            }
        }
        
        // Xóa vĩnh viễn khỏi CSDL
        _context.DonHangs.Remove(donHang);
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Đã xóa đơn hàng #{donHang.MaDonHang} thành công {(hoanKho ? "(Có hoàn kho)" : "(Không hoàn kho)")}.";
        return RedirectToAction(nameof(Index));
    }

    private static string TrangThaiLabel(TrangThaiDonHang t) => t switch
    {
        TrangThaiDonHang.ChoThanhToan => "Chờ thanh toán",
        TrangThaiDonHang.DaThanhToan  => "Đã thanh toán",
        TrangThaiDonHang.DangChuanBi  => "Đang chuẩn bị",
        TrangThaiDonHang.DangGiao     => "Đang giao",
        TrangThaiDonHang.DaGiao       => "Đã giao",
        TrangThaiDonHang.DaHuy        => "Đã hủy",
        _                             => t.ToString()
    };  
}
