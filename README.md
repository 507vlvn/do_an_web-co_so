# Nhà Thuốc Plus - Hệ Thống Quản Lý Nhà Thuốc Trực Tuyến

Hệ thống quản lý nhà thuốc toàn diện, bao gồm bán hàng trực tuyến, POS tại quầy, quản lý kho theo lô FEFO, kê đơn thuốc và tư vấn thuốc bằng trợ lý AI.

## Công nghệ sử dụng

| Layer | Công nghệ | Chi tiết |
|-------|-----------|----------|
| **Backend** | ASP.NET Core 10 (C#) | Framework cốt lõi |
| **Database** | SQL Server | Lưu trữ dữ liệu hệ thống |
| **ORM** | Entity Framework Core 9.0.6 | Truy vấn và quản lý migrations |
| **Frontend** | ASP.NET Core MVC (Razor Views) + Bootstrap 5 | Giao diện quản trị và Storefront |
| **Xác thực** | Cookie Authentication | Phân quyền theo vai trò (Role-based) |
| **Mật khẩu** | BCrypt.Net-Next (4.1.0) | Mã hóa mật khẩu bảo mật cao |
| **AI Copilot** | Google Gemini API | Tư vấn thông tin thuốc thông minh |
| **Logging** | Serilog (AspNetCore, Console, File) | Ghi vết hoạt động và giám sát hệ thống |
| **Container** | Docker + Docker Compose | Hỗ trợ đóng gói triển khai |
| **CI/CD** | GitHub Actions | Tự động kiểm thử và tích hợp |

## Phân quyền & Các chức năng chính

Hệ thống phân quyền chi tiết với 4 vai trò chính:

### 1. Khách hàng (Online Storefront)
- Duyệt sản phẩm theo danh mục và tìm kiếm thông minh.
- Giỏ hàng trực tuyến (đồng bộ giữa Session và Database).
- Đặt hàng trực tuyến áp dụng mã giảm giá (Voucher).
- Theo dõi lịch sử và trạng thái giao hàng.
- **Tư vấn AI Chatbot**: Nhận tư vấn sử dụng thuốc và sức khỏe từ trợ lý AI tích hợp Gemini.

### 2. Nhân viên (Dược sĩ bán hàng & quản lý kho)
- **Bán hàng tại quầy (POS)**: Lập hóa đơn nhanh, tự động trừ kho thông minh.
- **Xử lý đơn hàng**: Tiếp nhận, chuẩn bị hàng, giao hàng hoặc hủy đơn online của khách.
- **Quản lý kho**: Xem danh sách sản phẩm, quản lý thông tin lô thuốc trên kệ.
- **Quản lý khách hàng**: Xem thông tin và hỗ trợ cấp lại mật khẩu cho khách hàng.
- **Đơn thuốc**: Xem thông tin đơn thuốc mẫu để cắt liều bán hàng (Không có quyền tạo/duyệt/hủy đơn thuốc).
- **Dashboard**: Xem thống kê hoạt động bán hàng trong ngày.

### 3. Bác sĩ (Kê đơn & Nghiệp vụ hỗ trợ)
- **Quản lý đơn thuốc kê đơn (Quyền đặc biệt)**: 
  - Tạo mới đơn thuốc kê đơn mẫu.
  - Phê duyệt đơn thuốc, hủy đơn thuốc.
  - Đăng đơn thuốc lên Store trực tuyến (để khách hàng có thể đặt mua theo đơn) hoặc gỡ đơn thuốc.
- **Nghiệp vụ bán hàng**: Thực hiện bán hàng POS tại quầy và xử lý đơn hàng online.
- **Quản lý kho & Khách hàng**: Xem sản phẩm, quản lý lô thuốc hiện tại và hỗ trợ khách hàng.
- *Lưu ý: Bác sĩ không có quyền quản lý nhập hàng từ Nhà cung cấp hay quản trị hệ thống.*

### 4. Quản trị viên (Admin)
- Đầy đủ tất cả các quyền của Bác sĩ và Nhân viên.
- **Quản lý hệ thống**: Tạo mới, chỉnh sửa, xóa và quản lý tài khoản Nhân viên, Bác sĩ.
- **Quản lý sản phẩm**: Thêm mới, cập nhật thông tin và danh mục sản phẩm/thuốc.
- **Nghiệp vụ nhập hàng**: Lập phiếu nhập hàng mới vào kho, quản lý danh sách Nhà cung cấp.
- **Tiêu hủy & Điều chỉnh kho**: Quản lý tiêu hủy các lô thuốc hết hạn, đồng bộ hạn sử dụng hệ thống.
- **Quản lý khuyến mãi**: Thiết lập các chương trình Voucher giảm giá.
- **Báo cáo tài chính**: Xem biểu đồ thống kê doanh số bán hàng, lợi nhuận chi tiết.

## Hướng dẫn cài đặt

### Yêu cầu hệ thống
- .NET 10.0 SDK trở lên.
- SQL Server (LocalDB hoặc SQL Server Express).
- (Tùy chọn) Docker + Docker Compose để đóng gói container.

### Chạy ứng dụng Local (PowerShell / Command Prompt)

```bash
# 1. Clone repository về máy
git clone <repo-url>
cd do_an

# 2. Phục hồi các gói thư viện NuGet
dotnet restore

# 3. Thiết lập API Key Gemini (cho AI Chatbot)
cd do_an
dotnet user-secrets init
dotnet user-secrets set "Gemini:ApiKey" "your-api-key-here"

# 4. Chạy ứng dụng
dotnet run
```

Ứng dụng sẽ tự động khởi tạo cơ sở dữ liệu (Database), tự động tạo bảng (EF Migrations) và thiết lập tài khoản Admin mặc định khi chạy lần đầu:
- **Tên đăng nhập**: `admin`
- **Mật khẩu**: Được sinh ngẫu nhiên và hiển thị tại Log Console trong lần chạy đầu tiên.

### Chạy bằng Docker

```bash
# 1. Thiết lập các biến môi trường
export GEMINI_API_KEY="your-api-key"
export ADMIN_PASSWORD="your-admin-password"

# 2. Build và khởi chạy các dịch vụ container
docker-compose up --build
```
Truy cập ứng dụng tại: `http://localhost:5000`

## Cấu trúc thư mục dự án

```
do_an/
  Controllers/       # Các MVC Controller xử lý luồng nghiệp vụ
  Models/            # Các lớp thực thể (Entity Models) ánh xạ xuống DB
  ViewModels/        # Các lớp trung gian định dạng dữ liệu cho View
  Views/             # Giao diện Razor Views (.cshtml) của trang quản trị
  Data/              # Lớp AppDbContext cấu hình DbContext & Seed Data
  Services/          # Các dịch vụ xử lý logic nghiệp vụ
  Helpers/           # Lớp tiện ích (Giỏ hàng, Mã hóa mật khẩu, Trừ kho FEFO)
  Middleware/        # Bộ lọc yêu cầu (Rate limiting, Exception handling)
  HealthChecks/      # Điểm cuối kiểm tra trạng thái hoạt động của hệ thống
  Migrations/        # Lịch sử các phiên bản cấu trúc database
  wwwroot/           # Các file tĩnh (CSS, JS, images, hình ảnh sản phẩm)
```

## Các biến môi trường cấu hình

| Biến môi trường | Mô tả | Mặc định |
|-----------------|-------|----------|
| `ConnectionStrings__DefaultConnection` | Chuỗi kết nối đến cơ sở dữ liệu SQL Server | LocalDB mặc định |
| `GEMINI_API_KEY` | Mã API Key của Google Gemini cho Chatbot AI | (Bắt buộc thiết lập) |
| `ADMIN_PASSWORD` | Mật khẩu thiết lập cho tài khoản admin mặc định | Ngẫu nhiên (xem log) |

## API Endpoints hữu dụng

| Endpoint | Phương thức | Mô tả | Quyền truy cập |
|----------|-------------|-------|----------------|
| `/health` | GET | Kiểm tra trạng thái hoạt động của hệ thống | Công khai (Public) |
| `/api/AiChat/send` | POST | Gửi tin nhắn giao tiếp với trợ lý tư vấn AI | Người dùng đã đăng nhập |

## Bảo mật & Tối ưu hóa

- **Mã hóa mật khẩu**: Sử dụng thuật toán BCrypt với hệ số Salt (Work factor) là 12.
- **Chống tấn công CSRF**: Tích hợp Anti-Forgery Token trên toàn bộ các form nhập liệu.
- **Giới hạn tần suất (Rate Limiting)**: Giới hạn 10 yêu cầu/phút với trang đăng nhập, và tối đa 30 yêu cầu/phút với các trang chung để chống Brute Force / DDoS.
- **Chống lỗi IDOR**: Ràng buộc bảo mật dữ liệu đơn hàng theo định danh tài khoản đăng nhập.
- **Nhật ký hoạt động (Audit Trail)**: Sử dụng Serilog ghi chép lại các hành động nhạy cảm của người dùng (Đăng nhập, thanh toán, sửa sản phẩm, thay đổi lô thuốc).
- **Global Exception Handling**: Middleware bắt lỗi toàn cục, hiển thị trang thông báo thân thiện và ghi log chi tiết phục vụ gỡ lỗi.

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
