using do_an.Data;
using do_an.HealthChecks;
using do_an.Helpers;
using do_an.Middleware;
using do_an.Models;
using do_an.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

// ── Serilog ──────────────────────────────────────────────────
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/app-.log", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

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

builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<ICartSyncService, CartSyncService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IDonHangService, DonHangService>();
builder.Services.AddScoped<IVoucherService, VoucherService>();
builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("database");

builder.Services.AddHostedService<DonHangAutoHuyService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
// Middleware bao mat
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<RateLimitingMiddleware>();
// Chạy // với database , load nhanh
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
// Health check endpoint
app.MapHealthChecks("/health");
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
    // Thử Migrate trước (tạo bảng mới từ migration), nếu fail thì EnsureCreated
    try
    {
        db.Database.Migrate();
    }
    catch
    {
        db.Database.EnsureCreated();
    }



    if (!db.TaiKhoans.Any(t => t.VaiTro == "Admin"))
    {
        // Doc mat khau tu environment variable, neu khong co thi tao random
        var adminPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD");
        if (string.IsNullOrWhiteSpace(adminPassword))
        {
            adminPassword = Guid.NewGuid().ToString("N")[..12] + "Aa1!";
            Log.Warning("Mat khau admin tu tao: {Password} - HAY DOI NGAY SAU LAN DANG NHAP DAU!", adminPassword);
        }

        db.TaiKhoans.Add(new TaiKhoan
        {
            TenDangNhap = "admin",
            HoTen       = "Quản trị viên",
            MatKhauHash = PasswordHelper.Hash(adminPassword),
            VaiTro      = "Admin"
        });
        db.SaveChanges();
        Log.Information("Da tao tai khoan admin. Mat khau: {Password}", adminPassword);
    }

    // ── Tự động đồng bộ HSD theo lô FEFO mỗi khi app khởi động ──
    await KhoHelper.DongBoTatCaHanSuDungAsync(db);
}

app.Run();
