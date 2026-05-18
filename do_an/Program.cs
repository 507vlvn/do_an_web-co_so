using do_an.Data;
using do_an.Helpers;
using do_an.Models;
using do_an.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthentication(
    Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {   // Chưa đăng nhập trả về địa chỉ "/Account/Login"
        options.LoginPath = "/Account/Login";
        //  Người dùng không đủ quyền => Cook
        options.AccessDeniedPath = "/Account/AccessDenied";
        // Time lưu Cookie = 8h
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

builder.Services.AddAuthorization(options =>
{   // Policy có thể kếp hợp Role , email, tuổi (# role thông thường k đủ chi tiết )
    // Cho phép lòng điều kiện tại lúc khởi tạo không cần lập lại điều kiện mỗi lần 
    options.AddPolicy("ChiAdmin",        p => p.RequireRole("Admin"));
    options.AddPolicy("AdminVaNhanVien", p => p.RequireRole("Admin", "NhanVien"));
    options.AddPolicy("ChiKhachHang",    p => p.RequireRole("KhachHang"));
});

builder.Services.AddHostedService<DonHangAutoHuyService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
// Chạy // với database , load nhanh 
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
// thư viện áh xạ cho phép load ảnh tỉnh nhanh hơn (chỉ net 9 >)
app.MapStaticAssets();

// trang đầu 
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Store}/{action=Index}/{id?}")
    .WithStaticAssets();

// ── Khởi tạo DB ──────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    // EnsureCreated: tạo DB nếu chưa tồn tại, KHÔNG xóa dữ liệu cũ
    db.Database.EnsureCreated();

    try
    {
        db.Database.ExecuteSqlRaw("ALTER TABLE SanPham ADD ThuVienHinhAnh nvarchar(max) NULL;");
    }
    catch { /* Ignore if column already exists */ }

    if (!db.TaiKhoans.Any(t => t.VaiTro == "Admin"))
    {
        db.TaiKhoans.Add(new TaiKhoan
        {
            TenDangNhap = "admin",
            HoTen       = "Quản trị viên",
            MatKhauHash = PasswordHelper.Hash("Admin@123"),
            VaiTro      = "Admin"
        });
        db.SaveChanges();
    }

    // ── Tự động đồng bộ HSD theo lô FEFO mỗi khi app khởi động ──
    await KhoHelper.DongBoTatCaHanSuDungAsync(db);
}

app.Run();
