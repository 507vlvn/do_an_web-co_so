using do_an.Data;
using do_an.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace do_an.Controllers;

[AllowAnonymous]
public class StoreController : Controller
{
    private readonly AppDbContext _context;

    public StoreController(AppDbContext context) => _context = context;

    public override async Task OnActionExecutionAsync(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context, Microsoft.AspNetCore.Mvc.Filters.ActionExecutionDelegate next)
    {
        if (User.Identity?.IsAuthenticated == true && !HttpContext.Session.Keys.Contains("GioHang"))
        {
            var username = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var dbCart = await _context.GioHangs
                .Include(g => g.ChiTietGioHangs)
                .FirstOrDefaultAsync(g => g.TenDangNhap == username);

            if (dbCart != null && dbCart.ChiTietGioHangs.Count > 0)
            {
                var loadedItems = new List<do_an.Helpers.CartItem>();
                foreach (var ct in dbCart.ChiTietGioHangs)
                {
                    var sp = await _context.SanPhams.FindAsync(ct.SanPhamId);
                    if (sp != null && sp.TrenKe && sp.SoLuong > 0)
                    {
                        loadedItems.Add(new do_an.Helpers.CartItem
                        {
                            SanPhamId = ct.SanPhamId,
                            TenSanPham = sp.TenSanPham,
                            GiaBan = sp.GiaBan,
                            SoLuong = Math.Min(ct.SoLuong, sp.SoLuong),
                            HinhAnh = sp.HinhAnhDaiDien
                        });
                    }
                }
                do_an.Helpers.CartHelper.SaveCart(HttpContext.Session, loadedItems);
            }
            else
            {
                do_an.Helpers.CartHelper.SaveCart(HttpContext.Session, new List<do_an.Helpers.CartItem>());
            }
        }
        await base.OnActionExecutionAsync(context, next);
    }

    // GET: /Store
    public async Task<IActionResult> Index(int? danhMucId, string? search)
    {
        // ✅ ĐÃ XÓA: && !s.IsThuoc
        var query = _context.SanPhams
            .Include(s => s.DanhMuc)
            .Where(s => s.TrenKe && s.SoLuong > 0
                        && !_context.ChiTietDonThuocs.Any(c => c.SanPhamId == s.Id)
                        && (!s.HanSuDung.HasValue || s.HanSuDung > DateTime.Today))
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim().ToLower();
            query = query.Where(s =>
                s.TenSanPham.ToLower().Contains(search) ||
                (s.MoTa != null && s.MoTa.ToLower().Contains(search)) ||
                (s.CongDung != null && s.CongDung.ToLower().Contains(search)) ||
                (s.ThanhPhan != null && s.ThanhPhan.ToLower().Contains(search)));
        }
        if (danhMucId.HasValue)
            query = query.Where(s => s.DanhMucId == danhMucId);

        var danhMucs = await _context.DanhMucSanPhams
            .Where(d => d.KichHoat)
            .OrderBy(d => d.TenDanhMuc)
            .ToListAsync();

        // ── Đếm SP per danh mục (cho category nav) ──
        // ✅ ĐÃ XÓA: && !s.IsThuoc
        var countPerDm = await _context.SanPhams
            .Where(s => s.TrenKe && s.SoLuong > 0
                        && !_context.ChiTietDonThuocs.Any(c => c.SanPhamId == s.Id)
                        && (!s.HanSuDung.HasValue || s.HanSuDung > DateTime.Today))
            .GroupBy(s => s.DanhMucId)
            .Select(g => new { DanhMucId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.DanhMucId, x => x.Count);

        // ✅ ĐÃ XÓA: && !s.IsThuoc
        var noiBat = await _context.SanPhams
            .Include(s => s.DanhMuc)
            .Where(s => s.TrenKe && s.NoiBat && s.SoLuong > 0
                        && !_context.ChiTietDonThuocs.Any(c => c.SanPhamId == s.Id)
                        && (!s.HanSuDung.HasValue || s.HanSuDung > DateTime.Today))
            .Take(8)
            .ToListAsync();

        // Đơn thuốc nổi bật (Đã duyệt & cho phép bán lên Store)
        var donThuocQuery = _context.DonThuocs
            .Include(d => d.ChiTietDonThuocs)
                .ThenInclude(c => c.SanPham)
            .Where(d => d.TrangThai == TrangThaiDonThuoc.DaDuyet);

        if (!string.IsNullOrWhiteSpace(search))
            donThuocQuery = donThuocQuery.Where(d =>
                d.TenDonThuoc.ToLower().Contains(search) ||
                d.MaDonThuoc.ToLower().Contains(search));

        // Nếu người dùng đang lọc theo Danh Mục, thì ẩn mục Đơn Thuốc đi
        if (danhMucId.HasValue)
            donThuocQuery = donThuocQuery.Where(d => false);

        var donThuocs = await donThuocQuery.OrderByDescending(d => d.Id).ToListAsync();

        // 1. Thu thập tất cả SanPhamId từ tất cả đơn thuốc (chỉ lấy các ID duy nhất)
        var allSanPhamIds = donThuocs
            .SelectMany(dt => dt.ChiTietDonThuocs.Where(c => c.SanPhamId.HasValue).Select(c => c.SanPhamId.Value))
            .Distinct()
            .ToList();

        // 2. Truy vấn DB MỘT LẦN để lấy tất cả hình ảnh của các sản phẩm đó
        var hinhAnhRaw = await _context.SanPhams
            .Where(s => allSanPhamIds.Contains(s.Id))
            .Select(s => new { s.Id, s.HinhAnhFile, s.HinhAnhUrl, s.ThuVienHinhAnh })
            .ToListAsync();

        var hinhAnhLookup = hinhAnhRaw
            .Select(s => new {
                s.Id,
                HinhAnh = new SanPham { HinhAnhFile = s.HinhAnhFile, HinhAnhUrl = s.HinhAnhUrl, ThuVienHinhAnh = s.ThuVienHinhAnh }.HinhAnhDaiDien
            })
            .Where(x => x.HinhAnh != "/images/no-image.svg")
            .ToDictionary(x => x.Id, x => x.HinhAnh);

        // 3. Map ngược lại vào Dictionary của đơn thuốc trong bộ nhớ
        var donThuocImages = donThuocs.ToDictionary(
            dt => dt.Id,
            dt => dt.ChiTietDonThuocs
                .Where(c => c.SanPhamId.HasValue)
                .Select(c => hinhAnhLookup.GetValueOrDefault(c.SanPhamId.Value))
                .Where(img => img != null)
                .Take(4)
                .ToList()
        );

        int totalAll = countPerDm.Values.Sum();

        ViewBag.DanhMucs = danhMucs;
        ViewBag.DanhMucId = danhMucId;
        ViewBag.Search = search;
        ViewBag.SanPhamNoiBat = noiBat;
        ViewBag.DonThuocs = donThuocs;
        ViewBag.DonThuocImages = donThuocImages;
        ViewBag.CountPerDm = countPerDm;
        ViewBag.TotalAll = totalAll;

        var products = await query.OrderByDescending(s => s.NgayThem).ToListAsync();
        return View(products);
    }

    // GET: /Store/Detail/5
    public async Task<IActionResult> Detail(int id)
    {
        var sp = await _context.SanPhams
            .Include(s => s.DanhMuc)
            .FirstOrDefaultAsync(s => s.Id == id && s.TrenKe);

        if (sp is null) return NotFound();

        // ✅ ĐÃ XÓA: && !s.IsThuoc
        var related = await _context.SanPhams
            .Include(s => s.DanhMuc)
            .Where(s => s.TrenKe && s.DanhMucId == sp.DanhMucId && s.Id != id
                        && !_context.ChiTietDonThuocs.Any(c => c.SanPhamId == s.Id)
                        && (!s.HanSuDung.HasValue || s.HanSuDung > DateTime.Today))
            .Take(4)
            .ToListAsync();

        ViewBag.RelatedProducts = related;
        return View(sp);
    }

    // GET: /Store/DonThuocDetail/5
    public async Task<IActionResult> DonThuocDetail(int id)
    {
        var dt = await _context.DonThuocs
            .Include(d => d.ChiTietDonThuocs)
                .ThenInclude(c => c.SanPham)
            .FirstOrDefaultAsync(d => d.Id == id && d.TrangThai == TrangThaiDonThuoc.DaDuyet);

        if (dt is null) return NotFound();

        return View(dt);
    }
}