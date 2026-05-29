# Nha Thuoc Plus - He thong Quan Ly Nha Thuoc Truc Tuyen

He thong quan ly nha thuoc toan dien, bao gom ban hang truc tuyen, POS tai quay, quan ly kho theo lo FEFO, va tu van thuoc bang AI.

## Cong nghe su dung

| Layer | Cong nghe |
|-------|-----------|
| Backend | ASP.NET Core 10 (C#) |
| Database | SQL Server + Entity Framework Core 9.0.6 |
| Frontend | Razor Pages + Bootstrap 5 |
| Auth | Cookie Authentication + BCrypt |
| AI | Google Gemini API |
| Logging | Serilog |
| Container | Docker + Docker Compose |
| CI/CD | GitHub Actions |

## Cac chuc nang chinh

### Khach hang
- Duyet san pham theo danh muc, tim kiem
- Gio hang (Session + DB sync)
- Dat hang truc tuyen voi voucher
- Theo doi don hang
- Tu van thuoc bang AI chatbot

### Nhan vien
- Ban hang tai quay (POS)
- Quan ly don hang truc tuyen
- Quan ly don thuoc

### Admin
- Dashboard thong ke doanh thu
- Quan ly san pham, danh muc
- Quan ly kho theo lo (FEFO)
- Quan ly nhan vien, tai khoan
- Quan ly voucher
- Bao cao doanh thu

## Hu dan cai dat

### Yeu cau truoc
- .NET 10.0 SDK
- SQL Server (LocalDB hoac SQL Server Express)
- (Tuy chon) Docker + Docker Compose

### Chay local

```bash
# 1. Clone repo
git clone <repo-url>
cd do_an

# 2. Restore packages
dotnet restore

# 3. Set User Secrets (cho API key Gemini)
cd do_an
dotnet user-secrets init
dotnet user-secrets set "Gemini:ApiKey" "your-api-key-here"

# 4. Chay ung dung
dotnet run
```

Ung dung se tu dong tao database va tai khoan admin mac dinh:
- Username: `admin`
- Password: xem trong log console (lan dau chay)

### Chay voi Docker

```bash
# 1. Set environment variables
export GEMINI_API_KEY="your-api-key"
export ADMIN_PASSWORD="your-admin-password"

# 2. Build va chay
docker-compose up --build
```

Truy cap: http://localhost:5000

## Cau truc du an

```
do_an/
  Controllers/       # 12 Controllers (MVC + API)
  Models/            # Entity models
  ViewModels/        # View models cho cac trang
  Views/             # Razor views (.cshtml)
  Data/              # AppDbContext + EF Core config
  Services/          # Business logic services
  Helpers/           # Utility classes (Cart, Password, Kho)
  Middleware/        # Rate limiting, Exception handling
  HealthChecks/      # Health check endpoints
  Migrations/        # EF Core migrations
  wwwroot/           # Static files (CSS, JS, images)
```

## Bien moi truong

| Bien | Mo ta | Mac dinh |
|------|-------|----------|
| `ConnectionStrings__DefaultConnection` | Chuoi ket noi SQL Server | LocalDB |
| `GEMINI_API_KEY` | API key cho AI chatbot | (bat buoc) |
| `ADMIN_PASSWORD` | Mat khau admin | Random (xem log) |

## API Endpoints

| Endpoint | Phuong thuc | Mo ta |
|----------|-------------|-------|
| `/health` | GET | Kiem tra trang thai he thong |
| `/api/AiChat/send` | POST | Gui tin nhan tu van AI |

## Bao mat

- BCrypt hash mat khau (work factor 12)
- CSRF protection (Anti-Forgery Token)
- Rate limiting (10 req/phut cho login, 30 req/phut chung)
- Anti-IDOR tren don hang
- Audit trail cho cac hanh dong nhay cam
- Global exception handling

## Bản quyền & Điều khoản sử dụng (License & Copyright)

Dự án này được phát triển phục vụ cho mục đích học tập và làm đồ án tốt nghiệp. Khi sử dụng, sao chép hoặc phát triển tiếp dự án này, vui lòng tuân thủ các điều khoản sau:

### 1. Quyền sử dụng (Terms of Use)
- **Mục đích phi thương mại**: Dự án được cấp quyền sử dụng miễn phí cho các mục đích học tập, nghiên cứu cá nhân hoặc tham khảo phi thương mại.
- **Nghiêm cấm thương mại hóa**: Không được phép sử dụng bất kỳ phần nào của mã nguồn dự án này để kinh doanh, thương mại hóa hoặc tích hợp vào các sản phẩm thương mại khi chưa có sự đồng ý bằng văn bản từ tác giả.

### 2. Trích dẫn nguồn (Attribution & Citation)
Mọi hình thức tham khảo hoặc tái sử dụng một phần hay toàn bộ mã nguồn đều cần phải ghi công và trích dẫn nguồn đầy đủ:
- **Tên tác giả**: Nguyễn Thái Bình (507vlvn)
- **Liên kết dự án gốc**: [https://github.com/507vlvn/do_an_web-co_so](https://github.com/507vlvn/do_an_web-co_so)
- **Mẫu trích dẫn đề xuất**:
  > *"Dự án này có tham khảo/sử dụng mã nguồn từ hệ thống Nhà Thuốc Plus, phát triển bởi Nguyễn Thái Bình ([Link repository](https://github.com/507vlvn/do_an_web-co_so))."*
