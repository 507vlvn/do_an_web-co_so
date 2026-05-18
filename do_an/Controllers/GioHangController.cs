using do_an.Data;
using do_an.Helpers;
using do_an.Models;
using do_an.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace do_an.Controllers;

public class GioHangController : Controller
{
    private readonly AppDbContext _context;
    private readonly ILogger<GioHangController> _logger;

    public GioHangController(AppDbContext context, ILogger<GioHangController> logger)
    {
        _context = context;
        _logger = logger;
    }

// 1. Ghi đè phương thức Filter để can thiệp vào luồng thực thi của Action
    public override async Task OnActionExecutionAsync(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context, Microsoft.AspNetCore.Mvc.Filters.ActionExecutionDelegate next)
    {   // 2. Chỉ xử lý khi người dùng ĐÃ xác thực (Login) nhưng Session giỏ hàng ĐANG trống (Tránh load lại nhiều lần)
        if (User.Identity?.IsAuthenticated == true && !HttpContext.Session.Keys.Contains("GioHang"))
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var dbCart = await _context.GioHangs
                .Include(g => g.ChiTietGioHangs)
                .FirstOrDefaultAsync(g => g.TenDangNhap == username);
            // 3. Nếu DB có dữ liệu => Load vào Session (Ưu tiên DB)
            if (dbCart != null && dbCart.ChiTietGioHangs.Count > 0)
            {
                var loadedItems = new List<CartItem>();
                foreach (var ct in dbCart.ChiTietGioHangs)
                {
                    var sp = await _context.SanPhams.FindAsync(ct.SanPhamId);
                    // 4. Kiểm tra sản phẩm còn hợp lệ (Còn hàng, còn bán)
                    if (sp != null && sp.TrenKe && sp.SoLuong > 0)
                    {
                        loadedItems.Add(new CartItem
                        {
                            SanPhamId  = ct.SanPhamId,
                            TenSanPham = sp.TenSanPham,
                            GiaBan     = sp.GiaBan,
                            SoLuong    = Math.Min(ct.SoLuong, sp.SoLuong),
                            HinhAnh    = sp.HinhAnhDaiDien
                        });
                    }
                }
                CartHelper.SaveCart(HttpContext.Session, loadedItems);
            }
            // 5. Nếu DB trống hoặc lỗi => Reset Session về rỗng
            else
            {
                CartHelper.SaveCart(HttpContext.Session, new List<CartItem>());
            }
        }
        // 6. Tiếp tục thực thi Action (View Product, Cart, Checkout...)
        await base.OnActionExecutionAsync(context, next);
    }

    // GET: /GioHang
    [AllowAnonymous]
    public IActionResult Index()
    {
        var cart = CartHelper.GetCart(HttpContext.Session);
        return View(cart);
    }

    // POST: /GioHang/ThemVao
    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ThemVao(int sanPhamId, int soLuong = 1)
    {
        var sp = await _context.SanPhams.FindAsync(sanPhamId);
        if (sp is null || !sp.TrenKe || sp.SoLuong <= 0)
        {
            TempData["Error"] = "Sản phẩm không tồn tại hoặc hết hàng.";
            return RedirectToAction("Index", "Store");
        }

        var cart = CartHelper.GetCart(HttpContext.Session);
        var existingItem = cart.FirstOrDefault(c => c.SanPhamId == sanPhamId);
        var soLuongHienTai = existingItem?.SoLuong ?? 0;
        if (soLuongHienTai + soLuong > sp.SoLuong)
        {
            TempData["Error"] = $"Số lượng vượt quá tồn kho. Chỉ còn {sp.SoLuong - soLuongHienTai} sản phẩm có thể thêm.";
            return RedirectToAction("Detail", "Store", new { id = sanPhamId });
        }

        CartHelper.AddOrUpdate(HttpContext.Session, new CartItem
        {
            SanPhamId  = sp.Id,
            TenSanPham = sp.TenSanPham,
            GiaBan     = sp.GiaBan,
            SoLuong    = soLuong,
            HinhAnh    = sp.HinhAnhDaiDien
        });

        TempData["Success"] = $"Đã thêm \"{sp.TenSanPham}\" vào giỏ hàng.";
        await SyncCartToDbAsync();
        return RedirectToAction(nameof(Index));
    }

    // POST: /GioHang/CapNhat
    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CapNhat(int sanPhamId, int soLuong)
    {
        var sp = await _context.SanPhams.FindAsync(sanPhamId);
        if (sp is not null)
        {
            if (soLuong > sp.SoLuong)
            {
                TempData["Error"] = $"Chỉ còn {sp.SoLuong} sản phẩm trong kho.";
                soLuong = sp.SoLuong; // Giới hạn số lượng
            }
            CartHelper.UpdateQuantity(HttpContext.Session, sanPhamId, soLuong);
        }
        await SyncCartToDbAsync();
        return RedirectToAction(nameof(Index));
    }

    // POST: /GioHang/ThemVaoAjax
    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ThemVaoAjax(int sanPhamId, int soLuong = 1)
    {
        var sp = await _context.SanPhams.FindAsync(sanPhamId);
        if (sp is null || !sp.TrenKe)
        {
            return Json(new { success = false, message = "Sản phẩm không tồn tại hoặc đã ngừng bán." });
        }

        if (sp.SoLuong <= 0)
        {
            return Json(new { success = false, message = "Sản phẩm đã hết hàng." });
        }

        // Kiểm tra xem giỏ hàng hiện tại đã có bao nhiêu sản phẩm này
        var cart = CartHelper.GetCart(HttpContext.Session);
        var existingItem = cart.FirstOrDefault(c => c.SanPhamId == sanPhamId);
        var soLuongHienTai = existingItem?.SoLuong ?? 0;

        if (soLuongHienTai + soLuong > sp.SoLuong)
        {
            return Json(new { success = false, message = $"Số lượng vượt quá tồn kho. Chỉ còn trống {sp.SoLuong - soLuongHienTai} sản phẩm có thể mua thêm." });
        }

        CartHelper.AddOrUpdate(HttpContext.Session, new CartItem
        {
            SanPhamId  = sp.Id,
            TenSanPham = sp.TenSanPham,
            GiaBan     = sp.GiaBan,
            SoLuong    = soLuong,
            HinhAnh    = sp.HinhAnhDaiDien
        });

        // Tính tổng số lượng item để cập nhật badge
        var updatedCart = CartHelper.GetCart(HttpContext.Session);
        int totalItems = updatedCart.Sum(c => c.SoLuong);

        await SyncCartToDbAsync();
        return Json(new { success = true, message = $"Đã thêm \"{sp.TenSanPham}\" vào giỏ hàng.", cartCount = totalItems });
    }

    // POST: /GioHang/Xoa
    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Xoa(int sanPhamId)
    {
        CartHelper.Remove(HttpContext.Session, sanPhamId);
        await SyncCartToDbAsync();
        return RedirectToAction(nameof(Index));
    }

    // POST: /GioHang/XoaDonThuoc
    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> XoaDonThuoc(int donThuocId)
    {
        CartHelper.RemoveDonThuoc(HttpContext.Session, donThuocId);
        await SyncCartToDbAsync();
        return RedirectToAction(nameof(Index));
    }

    // POST: /GioHang/MuaTheoDon — Thêm tất cả thuốc trong đơn vào giỏ hàng
    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MuaTheoDon(int donThuocId)
    {
        var dt = await _context.DonThuocs
            .Include(d => d.ChiTietDonThuocs)
                .ThenInclude(c => c.SanPham)
            .FirstOrDefaultAsync(d => d.Id == donThuocId
                && d.TrangThai == TrangThaiDonThuoc.DaDuyet);

        if (dt is null)
        {
            TempData["Error"] = "Đơn thuốc không tồn tại hoặc đã hết hiệu lực.";
            return RedirectToAction("Index", "Store");
        }

        int added = 0;
        var thieu = new List<string>();

        foreach (var ct in dt.ChiTietDonThuocs)
        {
            var sp = ct.SanPham;

            if (sp is null || !sp.TrenKe || sp.SoLuong <= 0)
            {
                thieu.Add(sp?.TenSanPham ?? $"Sản phẩm ID {ct.SanPhamId}");
                continue;
            }

            int soLuongCan = ct.SoVienMoiNgay * ct.SoNgayUong;
            int soLuongThuc = Math.Min(soLuongCan, sp.SoLuong);

            CartHelper.AddOrUpdate(HttpContext.Session, new CartItem
            {
                SanPhamId  = sp.Id,
                TenSanPham = sp.TenSanPham,
                GiaBan     = sp.GiaBan,
                SoLuong    = soLuongThuc,
                HinhAnh    = sp.HinhAnhDaiDien,
                DonThuocId = dt.Id,
                TenDonThuoc = dt.TenDonThuoc
            });
            added++;
        }

        await SyncCartToDbAsync();

        if (added > 0 && thieu.Count == 0)
            TempData["Success"] = $"Đã thêm đơn \"{dt.TenDonThuoc}\" (gồm {added} thuốc) vào giỏ hàng.";
        else if (added > 0 && thieu.Count > 0)
            TempData["Success"] = $"Đã thêm {added} thuốc. Thiếu: {string.Join(", ", thieu)} (chưa có trên Store).";
        else
            TempData["Error"] = "Không có thuốc nào trong đơn có sẵn trên Store.";

        return RedirectToAction(nameof(Index));
    }

    // GET: /GioHang/Checkout
    [AllowAnonymous]
    public async Task<IActionResult> Checkout()
    {
        var cart = CartHelper.GetCart(HttpContext.Session);
        if (cart.Count == 0) return RedirectToAction(nameof(Index));

        var vm = new CheckoutViewModel { Items = cart };

        // Auto-fill nếu đã đăng nhập
        if (User.Identity?.IsAuthenticated == true)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var tk = await _context.TaiKhoans.FindAsync(username);
            if (tk is not null)
            {
                vm.HoTenNguoiNhan = tk.HoTen ?? "";
                vm.SoDienThoai    = tk.SoDienThoai ?? "";
                vm.DiaChi         = tk.DiaChi ?? "";
                vm.Email          = tk.Email ?? "";
            }
        }

        return View("Checkout_v2", vm);
    }

    // POST: /GioHang/ApplyVoucherAjax
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> ApplyVoucherAjax(string maVoucher)
    {
        if (string.IsNullOrWhiteSpace(maVoucher))
            return Json(new { success = false, message = "Vui lòng nhập mã giảm giá." });

        var cart = CartHelper.GetCart(HttpContext.Session);
        if (cart.Count == 0)
            return Json(new { success = false, message = "Giỏ hàng trống." });

        decimal tongTienHang = cart.Sum(i => i.ThanhTien);
        var now = DateTime.Now;

        // So sánh không phân biệt hoa thường (OrdinalIgnoreCase)
        var voucher = await _context.Vouchers.FirstOrDefaultAsync(v => 
            v.MaVoucher.ToLower() == maVoucher.ToLower() && v.KichHoat);

        if (voucher == null)
            return Json(new { success = false, message = "Mã giảm giá không tồn tại." });

        if (voucher.DaSuDung >= voucher.SoLanSuDung)
            return Json(new { success = false, message = "Mã giảm giá đã hết lượt sử dụng." });

        if (now < voucher.NgayBatDau || now > voucher.NgayHetHan)
            return Json(new { success = false, message = "Mã giảm giá đã hết hạn hoặc chưa đến ngày áp dụng." });

        if (tongTienHang < voucher.DonHangToiThieu)
            return Json(new { success = false, message = $"Đơn hàng tối thiểu {voucher.DonHangToiThieu:N0}đ để áp dụng mã này." });

        // Tính toán giảm giá
        decimal tienGiam = tongTienHang * voucher.PhanTramGiam / 100;
        if (voucher.GiamToiDa.HasValue)
            tienGiam = Math.Min(tienGiam, voucher.GiamToiDa.Value);

        return Json(new { 
            success = true, 
            message = $"Áp dụng thành công mã {voucher.MaVoucher}.", 
            discountAmount = tienGiam,
            tongTienHang = tongTienHang,
            finalTotal = tongTienHang - tienGiam
        });
    }

    // POST: /GioHang/DatHang
    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DatHang(CheckoutViewModel vm)
    {
        var cart = CartHelper.GetCart(HttpContext.Session);
        if (cart.Count == 0) return RedirectToAction(nameof(Index));

        vm.Items = cart;
        if (!ModelState.IsValid) return View("Checkout_v2", vm);

        // Kiểm tra tồn kho
        foreach (var item in cart)
        {
            var sp = await _context.SanPhams.FindAsync(item.SanPhamId);
            if (sp is null || sp.SoLuong < item.SoLuong)
            {
                ModelState.AddModelError("", $"\"{item.TenSanPham}\" không đủ hàng.");
                return View("Checkout_v2", vm);
            }
        }

        // Tính giảm giá voucher (DaSuDung sẽ tăng trong transaction)
        decimal tienGiam = 0;
        Voucher? voucherApplied = null;
        if (!string.IsNullOrWhiteSpace(vm.MaVoucher))
        {
            var now = DateTime.Now;
            voucherApplied = await _context.Vouchers
                .FirstOrDefaultAsync(v => v.MaVoucher.ToLower() == vm.MaVoucher.ToLower()
                                     && v.KichHoat 
                                     && v.DaSuDung < v.SoLanSuDung
                                     && now >= v.NgayBatDau 
                                     && now <= v.NgayHetHan);
            
            if (voucherApplied is not null && vm.TongTienHang >= voucherApplied.DonHangToiThieu)
            {
                tienGiam = vm.TongTienHang * voucherApplied.PhanTramGiam / 100;
                if (voucherApplied.GiamToiDa.HasValue)
                    tienGiam = Math.Min(tienGiam, voucherApplied.GiamToiDa.Value);
            }
            else
            {
                voucherApplied = null;
                // Optionally add a warning that voucher was invalid/expired
                TempData["Error"] = "Mã giảm giá không hợp lệ, đã hết hạn hoặc hết lượt dùng.";
            }
        }

        // Tạo đơn hàng
        var donHang = new DonHang
        {
            MaDonHang         = $"DH{DateTime.Now:yyyyMMddHHmmss}{Random.Shared.Next(100, 999)}",
            TenDangNhap       = User.Identity?.IsAuthenticated == true
                                  ? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                  : null,
            HoTenNguoiNhan    = vm.HoTenNguoiNhan,
            SoDienThoai       = vm.SoDienThoai,
            DiaChi            = vm.DiaChi,
            Email             = vm.Email,
            GhiChu            = vm.GhiChu,
            MaVoucher         = vm.MaVoucher,
            TongTienHang      = vm.TongTienHang,
            TienGiam          = tienGiam,
            PhuongThucThanhToan = vm.PhuongThucThanhToan,
            TrangThai         = TrangThaiDonHang.ChoThanhToan,
            NgayDat           = DateTime.Now
        };

        // 2. Bảo mật: Chỉ Admin/NhanVien được dùng thanh toán tại quầy
        if (donHang.PhuongThucThanhToan == PhuongThucThanhToan.ThanhToanTaiQuay
            && !(User.IsInRole("Admin") || User.IsInRole("NhanVien")))
        {
            ModelState.AddModelError("", "Phương thức thanh toán tại quầy chỉ dành cho giao dịch trực tiếp. Vui lòng thanh toán online.");
            vm.Items = cart;
            return View("Checkout_v2", vm);
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Trừ tồn kho và tạo chi tiết bằng thuật toán FEFO mới có khóa Lock
            foreach (var item in cart)
            {
                var sp = await _context.SanPhams.FindAsync(item.SanPhamId);
                
                // Sử dụng FEFO nếu sản phẩm là Thuốc
                if (sp!.IsThuoc)
                {
                    bool truThuocThanhCong = await KhoHelper.TruKhoFEFOAsync(_context, sp.Id, item.SoLuong);
                    if (!truThuocThanhCong)
                        throw new DbUpdateException($"\"{item.TenSanPham}\" không đủ kho thực tế.");
                }
                else
                {
                    sp!.SoLuong -= item.SoLuong;
                    if (sp.SoLuong < 0)
                        throw new DbUpdateException($"\"{item.TenSanPham}\" không đủ hàng.");
                }

                donHang.ChiTietDonHangs.Add(new ChiTietDonHang
                {
                    SanPhamId = item.SanPhamId,
                    TenSanPhamSnapshot = sp!.TenSanPham,
                    SoLuong   = item.SoLuong,
                    GiaBan    = sp!.GiaBan
                });
            }

            // Cập nhật tổng tiền theo giá thực tế từ DB
            donHang.TongTienHang = donHang.ChiTietDonHangs.Sum(ct => ct.GiaBan * ct.SoLuong);

            // Lưu thông tin KH cho lần sau
            if (User.Identity?.IsAuthenticated == true)
            {
                var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var tk = await _context.TaiKhoans.FindAsync(username);
                if (tk is not null)
                {
                    tk.HoTen      = vm.HoTenNguoiNhan;
                    tk.SoDienThoai = vm.SoDienThoai;
                    tk.DiaChi     = vm.DiaChi;
                    if (!string.IsNullOrEmpty(vm.Email)) tk.Email = vm.Email;
                }
            }

            // Tăng lượt dùng voucher trong transaction (rollback nếu lỗi)
            if (voucherApplied is not null)
                voucherApplied.DaSuDung++;

            _context.DonHangs.Add(donHang);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync(); // Xác nhận Transaction thành công
            
            CartHelper.Clear(HttpContext.Session);
            // Xóa giỏ hàng database nếu đã Đăng Nhập
            if (User.Identity?.IsAuthenticated == true)
            {
                var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var dbCart = await _context.GioHangs.FirstOrDefaultAsync(g => g.TenDangNhap == username);
                if(dbCart != null)
                {
                    var items = _context.ChiTietGioHangs.Where(c => c.GioHangId == dbCart.Id);
                    _context.ChiTietGioHangs.RemoveRange(items);
                    await _context.SaveChangesAsync();
                }
            }
        }
        catch (DbUpdateConcurrencyException)
        {
            await transaction.RollbackAsync();
            ModelState.AddModelError("", "Rất tiếc! Sản phẩm bạn chọn vừa được mua bởi khách hàng khác. Vui lòng thử lại.");
            return View("Checkout_v2", vm);
        }
        catch (DbUpdateException ex)
        {
            await transaction.RollbackAsync();
            // Các lỗi ta đã throw thủ công như "không đủ hàng"
            if (ex.Message.StartsWith("\"")) {
                ModelState.AddModelError("", ex.Message);
            } else {
                string errorMsg = "Lỗi cơ sở dữ liệu: " + (ex.InnerException?.Message ?? ex.Message);
                ModelState.AddModelError("", errorMsg);
            }
            return View("Checkout_v2", vm);
        }
        catch (Exception) // Catch-all for truly unexpected issues
        {
            await transaction.RollbackAsync();
            ModelState.AddModelError("", "Đã xảy ra lỗi hệ thống nghiêm trọng. Xin lỗi vì sự bất tiện.");
            return View("Checkout", vm);
        }

        // Redirect theo phương thức thanh toán
        return vm.PhuongThucThanhToan == PhuongThucThanhToan.ThanhToanOnline
            ? RedirectToAction(nameof(ThanhToanGiaLap), new { donHangId = donHang.Id })
            : RedirectToAction(nameof(DatHangThanhCong), new { donHangId = donHang.Id });
    }

    // GET: /GioHang/ThanhToanGiaLap/{id}
    [AllowAnonymous]
    public async Task<IActionResult> ThanhToanGiaLap(int donHangId)
    {
        var dh = await _context.DonHangs.FindAsync(donHangId);
        if (dh is null) return NotFound();

        // Chống IDOR: chỉ chủ đơn hoặc khách vãng lai mới được truy cập
        var username = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (dh.TenDangNhap != null && dh.TenDangNhap != username)
            return Forbid();

        return View(dh);
    }

    // POST: /GioHang/XacNhanThanhToan
    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> XacNhanThanhToan(int donHangId)
    {
        var dh = await _context.DonHangs.FindAsync(donHangId);
        if (dh is null) return NotFound();

        // Chống IDOR: chỉ chủ đơn mới được xác nhận thanh toán
        var username = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (dh.TenDangNhap != null && dh.TenDangNhap != username)
            return Forbid();

        if (dh.TrangThai == TrangThaiDonHang.ChoThanhToan)
        {
            dh.TrangThai       = TrangThaiDonHang.DaThanhToan;
            dh.NgayThanhToan   = DateTime.Now;
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(DatHangThanhCong), new { donHangId });
    }

    // GET: /GioHang/DatHangThanhCong/{id}
    [AllowAnonymous]
    public async Task<IActionResult> DatHangThanhCong(int donHangId)
    {
        var dh = await _context.DonHangs
            .Include(d => d.ChiTietDonHangs)
            .ThenInclude(c => c.SanPham)
            .FirstOrDefaultAsync(d => d.Id == donHangId);

        if (dh is null) return NotFound();

        // Chống IDOR: chỉ chủ đơn mới xem được
        var username = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (dh.TenDangNhap != null && dh.TenDangNhap != username)
            return Forbid();

        return View(dh);
    }

    // GET: /GioHang/DonHangCuaToi
    [Authorize]
    public async Task<IActionResult> DonHangCuaToi(TrangThaiDonHang? trangThai)
    {
        var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var query = _context.DonHangs
            .Where(d => d.TenDangNhap == username)
            .AsQueryable();

        if (trangThai.HasValue)
            query = query.Where(d => d.TrangThai == trangThai);

        var orders = await query
            .Include(d => d.ChiTietDonHangs)
            .OrderByDescending(d => d.NgayDat)
            .ToListAsync();

        ViewBag.TrangThai = trangThai;
        return View(orders);
    }

    // POST: /GioHang/HuyDonHang
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> HuyDonHang(int donHangId)
    {
        var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var dh = await _context.DonHangs
            .Include(d => d.ChiTietDonHangs)
            .FirstOrDefaultAsync(d => d.Id == donHangId && d.TenDangNhap == username);

        if (dh is null)
        {
            TempData["Error"] = "Không tìm thấy đơn hàng.";
            return RedirectToAction(nameof(DonHangCuaToi));
        }

        if (dh.TrangThai != TrangThaiDonHang.ChoThanhToan &&
            dh.TrangThai != TrangThaiDonHang.DaThanhToan)
        {
            TempData["Error"] = "Không thể hủy đơn hàng đang xử lý hoặc đã giao.";
            return RedirectToAction(nameof(DonHangCuaToi));
        }

        dh.TrangThai = TrangThaiDonHang.DaHuy;
        dh.NgayHuy = DateTime.Now;

        // Hoàn lại tồn kho
        foreach (var ct in dh.ChiTietDonHangs)
        {
            var sp = await _context.SanPhams.FindAsync(ct.SanPhamId);
            if (sp is not null)
            {
                if (sp.IsThuoc) 
                {
                     await KhoHelper.HoanKhoFEFOAsync(_context, sp.Id, ct.SoLuong);
                }
                else 
                {
                    sp.SoLuong += ct.SoLuong;
                }
            }
        }

        await _context.SaveChangesAsync();
        TempData["Success"] = $"Đã hủy đơn hàng {dh.MaDonHang}.";
        return RedirectToAction(nameof(DonHangCuaToi));
    }
    private async Task SyncCartToDbAsync()
    {
    if (User.Identity?.IsAuthenticated == true)
    {
        var username = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(username)) return;
        var cart = CartHelper.GetCart(HttpContext.Session);
        
        var dbCart = await _context.GioHangs
            .Include(g => g.ChiTietGioHangs)
            .FirstOrDefaultAsync(g => g.TenDangNhap == username);
        if (dbCart == null)
        {
            dbCart = new GioHang { TenDangNhap = username };
            _context.GioHangs.Add(dbCart);
            await _context.SaveChangesAsync();
        }
        // Xoá danh sách cũ và cập nhật lại toàn bộ dựa trên Session
        _context.ChiTietGioHangs.RemoveRange(dbCart.ChiTietGioHangs);
        
        foreach (var item in cart)
        {
            dbCart.ChiTietGioHangs.Add(new ChiTietGioHang
            {
                SanPhamId = item.SanPhamId,
                SoLuong = item.SoLuong,
                GioHangId = dbCart.Id
            });
        }
        dbCart.CapNhatCuoi = DateTime.Now;
        await _context.SaveChangesAsync();
    }
}
}
