# TRƯỜNG ĐẠI HỌC CÔNG NGHỆ THÔNG TIN
# KHOA KỸ THUẬT PHẦN MỀM
***
<br><br><br><br>

<p align="center">
  <font size="6"><b>BÁO CÁO ĐỒ ÁN TỐT NGHIỆP</b></font><br><br>
  <font size="5"><b>ĐỀ TÀI: XÂY DỰNG HỆ THỐNG BÁN LẺ VÀ QUẢN LÝ DƯỢC PHẨM ĐA KÊNH TÍCH HỢP TRÍ TUỆ NHÂN TẠO (AI) & QUẢN LÝ KHO THEO PHƯƠNG PHÁP FEFO</b></font>
</p>

<br><br><br><br><br><br>

<table align="center">
  <tr>
    <td width="250px"><font size="4"><b>Giảng viên hướng dẫn:</b></font></td>
    <td><font size="4">TS. Nguyễn Văn A</font></td>
  </tr>
  <tr>
    <td><font size="4"><b>Sinh viên thực hiện:</b></font></td>
    <td><font size="4">Nguyễn Văn B (MSSV: 12345678)</font></td>
  </tr>
  <tr>
    <td><font size="4"><b>Lớp:</b></font></td>
    <td><font size="4">Kỹ thuật Phần mềm - Khóa 2022</font></td>
  </tr>
</table>

<br><br><br><br><br>
<p align="center">
  <font size="4"><b>THÀNH PHỐ HỒ CHÍ MINH, NĂM 2026</b></font>
</p>

---
<!-- slide -->
# LỜI CẢM ƠN

Trước tiên, em xin gửi lời cảm ơn chân thành sâu sắc nhất đến các Thầy Cô Khoa Kỹ thuật Phần mềm đã truyền đạt những kiến thức vô cùng quý báu trong suốt những năm học vừa qua tại nhà trường. Những kiến thức lý thuyết vững vàng cùng các bài tập thực hành thực tế chính là bệ phóng quan trọng giúp em hoàn thành tốt đồ án tốt nghiệp này.

Đặc biệt, em xin gửi lời cảm ơn sâu sắc nhất tới thầy hướng dẫn **TS. Nguyễn Văn A**, người đã tận tình chỉ bảo, định hướng đề tài và đưa ra những đóng góp chuyên môn quý báu trong suốt quá trình phân tích, thiết kế và triển khai ứng dụng.

Mặc dù đã cố gắng nỗ lực hết mình để hoàn thiện hệ thống một cách tối ưu nhất, tuy nhiên đồ án chắc chắn không tránh khỏi những thiếu sót ngoài ý muốn. Kính mong nhận được ý kiến đóng góp, phê bình từ quý Thầy Cô trong Hội đồng bảo vệ để đồ án này ngày càng hoàn thiện và mang tính thực tiễn cao hơn.

*Sinh viên thực hiện*
**Nguyễn Văn B**

---
<!-- slide -->
# LỜI CAM ĐOAN

Em xin cam đoan đồ án tốt nghiệp với đề tài **"Xây dựng hệ thống bán lẻ và quản lý dược phẩm đa kênh tích hợp Trí tuệ nhân tạo (AI) & Quản lý kho theo phương pháp FEFO"** là công trình nghiên cứu do chính bản thân em tự thực hiện dưới sự định hướng và hướng dẫn trực tiếp từ Thầy hướng dẫn TS. Nguyễn Văn A.

Các số liệu, mã nguồn (codebase), sơ đồ phân tích và kết quả thực nghiệm được trình bày trong báo cáo này hoàn toàn trung thực, khách quan và dựa trên mã nguồn thực tế của hệ thống. Mã nguồn dự án được xây dựng dựa trên kiến thức tích lũy của bản thân và không sao chép trái phép từ bất kỳ nghiên cứu hay đồ án của tác giả khác.

Nếu có bất kỳ sự gian lận nào, em xin chịu hoàn toàn trách nhiệm trước Hội đồng và Nhà trường.

*Sinh viên cam đoan*
**Nguyễn Văn B**

---
<!-- slide -->
# MỤC LỤC

- [BÁO CÁO ĐỒ ÁN TỐT NGHIỆP](#báo-cáo-đồ-án-tốt-nghiệp)
- [LỜI CẢM ƠN](#lời-cảm-ơn)
- [LỜI CAM ĐOAN](#lời-cam-đoan)
- [CHƯƠNG 1: TỔNG QUAN VỀ ĐỀ TÀI](#chương-1-tổng-quan-về-đề-tài)
  - [1.1. Đặt vấn đề & Lý do chọn đề tài](#11-đặt-vấn-đề--lý-do-chọn-đề-tài)
  - [1.2. Mục tiêu nghiên cứu và xây dựng hệ thống](#12-mục-tiêu-nghiên-cứu-và-xây-dựng-hệ-thống)
  - [1.3. Đối tượng và Phạm vi nghiên cứu](#13-đối-tượng-và-phạm-vi-nghiên-cứu)
- [CHƯƠNG 2: PHÂN TÍCH VÀ THIẾT KẾ HỆ THỐNG](#chương-2-phân-tích-và-thiết-kế-hệ-thống)
  - [2.1. Phân tích các quy trình nghiệp vụ cốt lõi](#21-phân-tích-các-quy-trình-nghiệp-vụ-cốt-lõi)
  - [2.2. Phân tích yêu cầu hệ thống (System Requirements)](#22-phân-tích-yêu-cầu-hệ-thống-system-requirements)
  - [2.3. Thiết kế Cơ sở dữ liệu (Database Schema Design)](#23-thiết-kế-cơ-sở-dữ-liệu-database-schema-design)
  - [2.4. Sơ đồ các mối quan hệ thực thể (ERD) và ràng buộc](#24-sơ-đồ-các-mối-quan-hệ-thực-thể-erd-và-ràng-buộc)
- [CHƯƠNG 3: THIẾT KẾ KỸ THUẬT VÀ GIẢI PHÁP CÔNG NGHỆ](#chương-3-thiết-kế-kỹ-thuật-và-giải-pháp-công-nghệ)
  - [3.1. Kiến trúc tổng quan hệ thống](#31-kiến-trúc-tổng-quan-hệ-thống)
  - [3.2. Thuật toán quản lý kho dược phẩm tối ưu FEFO (First-Expired, First-Out)](#32-thuật-tán-quản-lý-kho-dược-phẩm-tối-ưu-fefo-first-expired-first-out)
  - [3.3. Cơ chế tự động hóa: Background Service DonHangAutoHuyService](#33-cơ-chế-tự-động-hóa-background-service-donhangautohuyservice)
  - [3.4. Đảm bảo toàn vẹn dữ liệu trong giao dịch (Database Transaction ACID)](#34-đảm-bảo-toàn-vẹn-dữ-liệu-trong-giao-dịch-database-transaction-acid)
  - [3.5. Cơ chế xác thực (Authentication), phân quyền (Authorization) và kiểm soát IDOR](#35-cơ-chế-xác-thực-authentication-phân-quyền-authorization-và-kiểm-soát-idor)
  - [3.6. Tích hợp mô hình ngôn ngữ lớn (LLM - Google Gemini API) hỗ trợ tư vấn biệt dược](#36-tích-hợp-mô hình-ngôn-ngữ-lớn-llm---google-gemini-api-hỗ-trợ-tư-vấn-biệt-dược)
- [CHƯƠNG 4: TRIỂN KHAI VÀ KẾT QUẢ ĐẠT ĐƯỢC](#chương-4-triển-khai-và-kết-quả-đạt-được)
  - [4.1. Môi trường triển khai và cấu hình dự án](#41-môi-trường-triển-khai-và-cấu-hình-dự-án)
  - [4.2. Kết quả giao diện & chức năng phía Khách hàng (Store Front)](#42-kết-quả-giao-diện--chức-năng-phía-khách-hàng-store-front)
  - [4.3. Kết quả giao diện & chức năng phía Quản trị (Admin) và Nhân viên (POS)](#43-kết-quả-giao-diện--chức-năng-phía-quản-trị-admin-và-nhân-viên-pos)
- [CHƯƠNG 5: ĐÁNH GIÁ VÀ HƯỚNG PHÁT TRIỂN](#chương-5-đánh-giá-và-hướng-phát-triển)
  - [5.1. Ưu điểm nổi bật của hệ thống](#51-ưu-điểm-nổi-bật-của-hệ-thống)
  - [5.2. Nhược điểm và hạn chế tồn tại](#52-nhược-điểm-và-hạn-chế-tồn-tại)
  - [5.3. Hướng phát triển và mở rộng trong tương lai](#53-hướng-phát-triển-và-mở-rộng-trong-tương-lai)
- [TÀI LIỆU THAM KHẢO](#tài-liệu-tham-khảo)

---
<!-- slide -->
# CHƯƠNG 1: TỔNG QUAN VỀ ĐỀ TÀI

## 1.1. Đặt vấn đề & Lý do chọn đề tài

Ngành dược phẩm và chăm sóc sức khỏe đóng vai trò then chốt trong đời sống xã hội. Những năm gần đây, dưới sự tác động mạnh mẽ của cuộc cách mạng công nghiệp 4.0 và chuyển đổi số y tế, thói quen tiêu dùng của người dân đối với các mặt hàng dược phẩm và thực phẩm chức năng đã có những dịch chuyển đáng kể từ offline sang online. Việc đặt mua thuốc trực tuyến giúp người tiêu dùng tiết kiệm thời gian, dễ dàng so sánh giá cả và tiếp cận được các dịch vụ tư vấn y tế thuận tiện.

Tuy nhiên, kinh doanh dược phẩm là một ngành nghề đặc thù và chịu sự quản lý nghiêm ngặt về chất lượng, nguồn gốc và đặc biệt là **hạn sử dụng (Expiry Date)**. Việc quản lý kho bãi tại các nhà thuốc truyền thống và các hệ thống e-commerce hiện nay thường gặp phải các thách thức lớn sau:
1. **Rủi ro thất thoát do thuốc hết hạn:** Thuốc là sản phẩm có hạn dùng cố định. Nếu không có cơ chế luân chuyển kho hợp lý, các lô thuốc cũ có hạn dùng gần hơn rất dễ bị tồn đọng và phải tiêu hủy, gây thiệt hại nghiêm trọng về mặt kinh tế.
2. **Quản lý kho theo phương pháp FEFO thiếu tự động hóa:** Nguyên tắc **FEFO (First-Expired, First-Out - Hết hạn trước, Xuất trước)** là tiêu chuẩn bắt buộc trong quản lý dược phẩm của Bộ Y Tế. Tuy nhiên, nhiều hệ thống hiện nay chỉ quản lý tồn kho theo tổng số lượng (Total Quantity) hoặc xuất kho theo FIFO (First-In, First-Out), dẫn đến tình trạng xuất các lô thuốc mới nhập trong khi lô cũ sắp hết hạn vẫn nằm trên kệ.
3. **Thách thức tư vấn biệt dược trực tuyến:** Người bệnh khi mua thuốc trực tuyến thường thiếu sự chỉ dẫn trực tiếp của dược sĩ. Việc tự ý mua và sử dụng thuốc kê đơn hoặc không hiểu rõ tác dụng phụ, tương tác thuốc cực kỳ nguy hiểm. Các chatbot thông thường chỉ phản hồi theo kịch bản cứng nhắc, không thể tư vấn linh hoạt theo ngữ cảnh thực tế của từng sản phẩm.
4. **Sự tách biệt giữa kênh bán hàng Online và tại quầy (POS):** Việc quản lý bất đồng bộ dữ liệu giữa việc khách hàng mua trực tiếp tại quầy và mua online dẫn đến sai lệch lớn về số liệu tồn kho thực tế, gây khó khăn cho việc quản lý các lô thuốc.

Xuất phát từ những thực tiễn và thách thức trên, đề tài **"Xây dựng hệ thống bán lẻ và quản lý dược phẩm đa kênh tích hợp Trí tuệ nhân tạo (AI) & Quản lý kho theo phương pháp FEFO"** đã được nghiên cứu và phát triển nhằm mang lại giải pháp công nghệ toàn diện giải quyết triệt để các bài toán nghiệp vụ của một nhà thuốc thông minh thời đại số.

---

## 1.2. Mục tiêu nghiên cứu và xây dựng hệ thống

Đồ án hướng tới việc thiết kế và xây dựng một hệ thống phần mềm hoàn chỉnh, đáp ứng các mục tiêu sau:
*   **Hệ thống bán hàng đa kênh đồng bộ (Omnichannel):** Hỗ trợ khách hàng đặt mua online trên Web Store tiện lợi, đồng thời cung cấp giao diện POS bán hàng chuyên nghiệp tại quầy cho dược sĩ. Tất cả dữ liệu giao dịch, giỏ hàng, thông tin tài khoản đều được đồng bộ hóa tức thời trên một cơ sở dữ liệu tập trung.
*   **Thuật toán FEFO tự động hóa ở mức mã nguồn:** Triển khai cơ chế tự động trừ kho từ lô thuốc có hạn sử dụng gần nhất trước, tự động hoàn kho vào lô có hạn sử dụng xa nhất khi đơn hàng bị hủy. Đồng thời tự động cập nhật hiển thị hạn sử dụng của lô đang bán lên kệ cửa hàng.
*   **Dịch vụ chạy ngầm tự động hóa quy trình (Background Services):** Tự động quét và xử lý hủy các đơn hàng trực tuyến quá hạn thanh toán để giải phóng tồn kho của các lô thuốc, tránh giam giữ hàng gây ảnh hưởng tới hoạt động kinh doanh trực tiếp tại quầy.
*   **Tích hợp Trí tuệ Nhân tạo hỗ trợ tư vấn:** Ứng dụng mô hình ngôn ngữ lớn (LLM) thông qua API của Google Gemini để cung cấp trợ lý tư vấn y tế thông minh, có khả năng phân tích thông tin biệt dược từ cơ sở dữ liệu thực tế của cửa hàng để đưa ra lời khuyên sử dụng thuốc chính xác và an toàn.
*   **Bảo mật và toàn vẹn giao dịch cao:** Đảm bảo an toàn tuyệt đối cho quy trình đặt hàng và thanh toán bằng cách áp dụng Database Transaction (giao dịch ACID) để ngăn ngừa lỗi bất đồng bộ đồng thời (Concurrency), kiểm soát chặt chẽ lỗ hổng IDOR và CSRF.

---

## 1.3. Đối tượng và Phạm vi nghiên cứu

*   **Đối tượng nghiên cứu:**
    *   Các mô hình và kiến trúc phát triển web hiện đại: ASP.NET Core MVC 10.0, Entity Framework Core 9.0.
    *   Thuật toán quản lý kho dược phẩm GSP/GPP, phương pháp luân chuyển kho FEFO (First-Expired, First-Out).
    *   Quy trình nghiệp vụ quản lý đơn thuốc điện tử (Prescriptions) và bán hàng trực tiếp tại quầy (Point of Sale - POS).
    *   Kỹ thuật tích hợp và tối ưu hóa Prompt Engineering cho các mô hình AI tạo sinh (Google Gemini AI).
    *   Các cơ chế xử lý đồng thời (Concurrency Controls), Transaction cô lập dữ liệu và kiểm soát an toàn bảo mật hệ thống web.
*   **Phạm vi nghiên cứu:**
    *   Xây dựng ứng dụng Web đa kênh chạy trên nền tảng .NET 10.0, sử dụng cơ sở dữ liệu Microsoft SQL Server.
    *   Hệ thống phân quyền chi tiết cho 4 nhóm đối tượng người dùng: Khách hàng (KhachHang), Nhân viên bán thuốc (NhanVien), Bác sĩ kê đơn (BacSi) và Quản trị viên (Admin).
    *   Hệ thống quản lý và xử lý giao dịch tự động không tích hợp các cổng thanh toán thực tế (VNPAY, MoMo) mà giả lập quy trình thanh toán an toàn thông qua kiểm tra logic server-side chặt chẽ.

---
<!-- slide -->
# CHƯƠNG 2: PHÂN TÍCH VÀ THIẾT KẾ HỆ THỐNG

## 2.1. Phân tích các quy trình nghiệp vụ cốt lõi

### A. Quy trình Quản lý Kho Dược phẩm theo Lô và Phương pháp FEFO
Phương pháp FEFO đòi hỏi việc quản lý thuốc không được gộp chung thành một số lượng tổng, mà bắt buộc phải chia nhỏ thành các **Lô thuốc (Batches / Lots)**. Mỗi lô thuốc khi nhập kho sẽ chứa các thông tin: Mã lô, Số lượng nhập, Ngày sản xuất, Hạn sử dụng, và Số lượng khả dùng hiện tại.

```mermaid
graph TD
    A[Nhập kho lô thuốc mới] --> B{Kiểm tra thông tin lô}
    B -- Lô hợp lệ & HSD tương lai --> C[Lưu lô thuốc vào DB]
    C --> D[Cộng số lượng lô vào tổng tồn kho thuốc]
    D --> E[Đồng bộ HSD thuốc theo lô FEFO đang bán]
    E --> F[Hiển thị HSD của lô hết hạn sớm nhất lên Store]
```

Khi có tác vụ xuất kho (POS hoặc Web Store), hệ thống sẽ tìm kiếm tất cả các lô thuốc đang kích hoạt của sản phẩm đó, sắp xếp theo thứ tự HSD tăng dần, lọc bỏ các lô đã hết hạn và thực hiện trừ dần số lượng từ lô có HSD gần nhất đến xa nhất.

### B. Quy trình Bán lẻ tại Quầy (POS - Point of Sale)
Nhân viên/Dược sĩ tại quầy thực hiện bán hàng thông qua giao diện POS:
1. Tìm kiếm nhanh thuốc bằng cách nhập tên hoặc mã sản phẩm thông qua công cụ Auto-complete (AJAX).
2. Khi chọn thuốc kê đơn, hệ thống yêu cầu kiểm tra liều dùng. Dược sĩ nhập số lượng, số viên mỗi ngày, số ngày uống, thời điểm uống (sáng, trưa, chiều, tối) và cách dùng để hệ thống in nhãn liều lượng.
3. Khi bấm Thanh toán, hệ thống thực hiện một Database Transaction duy nhất để:
    * Kiểm tra tồn kho của từng sản phẩm.
    * Trừ kho theo thuật toán FEFO đối với các mặt hàng là thuốc (`IsThuoc == true`), và trừ kho thông thường đối với sản phẩm khác.
    * Lưu đơn hàng với trạng thái `DaGiao`, lưu chi tiết hóa đơn kèm thông tin liều dùng.
    * In hóa đơn bán hàng cho khách trực tiếp.

### C. Quy trình Duyệt và Đăng tải Đơn thuốc của Bác sĩ
Để hỗ trợ việc mua thuốc theo đơn trực tuyến một cách dễ dàng và an toàn, hệ thống hỗ trợ nghiệp vụ tạo Đơn Thuốc Mẫu:
1. **Bác sĩ (BacSi)** đăng nhập vào hệ thống, tiến hành tạo một Đơn Thuốc mẫu gồm thông tin chẩn đoán, ghi chú và danh sách biệt dược bắt buộc kèm liều dùng chi tiết.
2. Trạng thái ban đầu của đơn thuốc là `MoiTao`.
3. **Bác sĩ** hoặc **Admin** tiến hành Duyệt đơn thuốc (`DaDuyet`).
4. Khi đơn thuốc được chọn đăng lên Store (`DaDangStore = true`), hệ thống sẽ tự động kích hoạt trạng thái hiển thị của tất cả các biệt dược có trong đơn thuốc đó lên kệ cửa hàng trực tuyến (`TrenKe = true`) để khách hàng có thể tìm thấy và mua trọn vẹn combo đơn thuốc này bằng một click chuột.
5. Khi gỡ đơn thuốc khỏi cửa hàng, hệ thống tự động kiểm tra xem các thuốc trong đơn đó có nằm trong đơn thuốc nào khác đang đăng bán không; nếu không, hệ thống tự động ẩn thuốc đó khỏi cửa hàng (`TrenKe = false`) để tránh khách hàng tự ý mua thuốc kê đơn khi không cần thiết.

---

## 2.2. Phân tích yêu cầu hệ thống (System Requirements)

### A. Yêu cầu Chức năng (Functional Requirements)
Hệ thống được thiết kế để phân quyền chặt chẽ cho các tác nhân tương ứng:

| Tác nhân (Actor) | Quyền hạn và Chức năng |
| :--- | :--- |
| **Khách hàng (Anonymous / Customer)** | - Đăng ký, đăng nhập tài khoản cá nhân.<br>- Xem danh sách sản phẩm thường trên Store front (Không hiển thị thuốc kê đơn lẻ).<br>- Xem và chọn mua nguyên combo theo Đơn thuốc đã được Bác sĩ phê duyệt và đăng Store.<br>- Quản lý giỏ hàng trực tuyến (đồng bộ session và database).<br>- Đặt hàng, áp dụng mã giảm giá (Voucher), chọn phương thức thanh toán.<br>- Tra cứu lịch sử đơn hàng cá nhân.<br>- Trò chuyện trực tuyến với trợ lý y tế AI (Gemini Chatbot) để hỏi về công dụng, cách dùng và tương tác thuốc dựa trên danh mục thuốc có sẵn. |
| **Nhân viên (NhanVien - Dược sĩ tại quầy)** | - Quản lý giỏ hàng POS và tạo hóa đơn bán lẻ tại quầy.<br>- Tìm kiếm thuốc thông minh, kê liều dùng chi tiết cho khách mua tại quầy.<br>- Xem danh sách đơn hàng online, cập nhật trạng thái đơn hàng (Xác nhận, Đang chuẩn bị, Đang giao, Đã giao, Hủy đơn).<br>- Nhập lô thuốc mới, kiểm tra số lượng khả dụng của từng lô.<br>- Quản lý thông tin tài khoản Khách hàng. |
| **Bác sĩ (BacSi)** | - Tạo mới các đơn thuốc chẩn đoán mẫu.<br>- Kê toa chi tiết các biệt dược, ghi chú liều dùng, tần suất và cách dùng.<br>- Phê duyệt các đơn thuốc mẫu để sẵn sàng bán trên Store. |
| **Quản trị viên (Admin)** | - Toàn quyền quản trị tất cả các chức năng của hệ thống.<br>- Quản lý danh mục sản phẩm, quản lý thông tin chi tiết dược phẩm và mỹ phẩm.<br>- Quản lý tài khoản người dùng, phân quyền (Admin, NhanVien, BacSi, KhachHang).<br>- Thực hiện đồng bộ HSD thuốc hàng loạt, tiêu hủy các lô thuốc đã hết hạn sử dụng.<br>- Quản lý các chương trình khuyến mãi, tạo và kích hoạt/vô hiệu hóa các mã Voucher giảm giá.<br>- Xem báo cáo biểu đồ trực quan về doanh thu bán lẻ (POS) và bán online theo thời gian. |

### B. Yêu cầu Phi chức năng (Non-Functional Requirements)
*   **Tính nhất quán dữ liệu (Data Consistency):** Đảm bảo an toàn tuyệt đối về mặt tồn kho. Khi có hàng trăm khách hàng cùng checkout một sản phẩm tại một thời điểm, hệ thống không được phép xảy ra hiện tượng bán vượt quá tồn kho (Over-selling).
*   **Hiệu năng xử lý (Performance):** Quá trình tìm kiếm sản phẩm tại quầy POS phải diễn ra tức thời (dưới 500ms) để không gây chậm trễ cho dược sĩ khi đón khách trực tiếp.
*   **Bảo mật dữ liệu (Security):**
    *   Ngăn chặn lỗ hổng IDOR: Khách hàng không thể xem hóa đơn hoặc đơn hàng của khách hàng khác bằng cách thay đổi ID trên URL.
    *   Phòng chống tấn công giả mạo yêu cầu chéo trang (CSRF): Toàn bộ các form thay đổi trạng thái dữ liệu (POST/PUT/DELETE) bắt buộc phải được bảo vệ bởi Token chống giả mạo (`ValidateAntiForgeryToken`).
    *   Mã hóa mật khẩu an toàn bằng thuật toán **BCrypt** thay vì lưu mật khẩu văn bản thô hoặc hash MD5/SHA256 đơn giản dễ bị tấn công dò tìm (Brute-force).

---

## 2.3. Thiết kế Cơ sở dữ liệu (Database Schema Design)

Cơ sở dữ liệu của hệ thống được chuẩn hóa cao độ để đảm bảo hiệu năng truy vấn và tính toàn vẹn dữ liệu. Dưới đây là thiết kế chi tiết của các bảng dữ liệu chính trong SQL Server:

### 1. Bảng `TaiKhoan` (Quản lý người dùng và phân quyền)
Bảng này lưu trữ toàn bộ thông tin tài khoản của Khách hàng, Nhân viên, Bác sĩ và Admin.

| Tên trường (Column) | Kiểu dữ liệu (Data Type) | Ràng buộc (Constraint) | Mô tả |
| :--- | :--- | :--- | :--- |
| `TenDangNhap` | `nvarchar(50)` | Primary Key | Tên đăng nhập của người dùng |
| `MatKhauHash` | `nvarchar(255)` | Not Null | Hash mật khẩu bảo mật (sử dụng BCrypt) |
| `HoTen` | `nvarchar(100)` | Not Null | Họ và tên người dùng |
| `Email` | `nvarchar(100)` | Null | Địa chỉ Email liên lạc |
| `SoDienThoai` | `nvarchar(20)` | Null | Số điện thoại liên hệ |
| `DiaChi` | `nvarchar(255)` | Null | Địa chỉ giao hàng hoặc cư trú |
| `VaiTro` | `nvarchar(20)` | Not Null | Vai trò phân quyền: `Admin`, `NhanVien`, `BacSi`, `KhachHang` |
| `HeSoLuong` | `decimal(18,2)` | Null | Hệ số lương (dành cho Nhân viên) |
| `LuongTheoGio` | `decimal(18,2)` | Null | Lương cơ bản theo giờ làm việc |

### 2. Bảng `DanhMucSanPham` (Phân loại sản phẩm)

| Tên trường (Column) | Kiểu dữ liệu (Data Type) | Ràng buộc (Constraint) | Mô tả |
| :--- | :--- | :--- | :--- |
| `Id` | `int` | Primary Key, Identity | Mã định danh danh mục tự tăng |
| `TenDanhMuc` | `nvarchar(100)` | Not Null | Tên danh mục (Ví dụ: Kháng sinh, Mỹ phẩm) |
| `PhanLoai` | `nvarchar(50)` | Not Null | Phân loại lớn: `Thuoc`, `MyPham`, `ThucPhamChucNang`, `ThietBiYTe` |
| `MoTa` | `nvarchar(255)` | Null | Mô tả chi tiết danh mục |
| `KichHoat` | `bit` | Not Null, Default `1` | Trạng thái sử dụng của danh mục |

### 3. Bảng `SanPham` (Danh sách thuốc và các sản phẩm khác)

| Tên trường (Column) | Kiểu dữ liệu (Data Type) | Ràng buộc (Constraint) | Mô tả |
| :--- | :--- | :--- | :--- |
| `Id` | `int` | Primary Key, Identity | Mã định danh sản phẩm tự tăng |
| `TenSanPham` | `nvarchar(150)` | Not Null | Tên hiển thị của sản phẩm |
| `DanhMucId` | `int` | Foreign Key | Khóa ngoại liên kết tới bảng `DanhMucSanPham` |
| `IsThuoc` | `bit` | Not Null | `true` nếu sản phẩm là thuốc kê đơn/không kê đơn |
| `GiaNhap` | `decimal(18,2)` | Not Null | Giá nhập kho của một đơn vị sản phẩm |
| `GiaBan` | `decimal(18,2)` | Not Null | Giá bán lẻ niêm yết hiện tại |
| `GiaGoc` | `decimal(18,2)` | Not Null | Giá gốc chưa giảm của sản phẩm |
| `DonViTinh` | `nvarchar(20)` | Not Null | Đơn vị tính (Viên, Vỉ, Hộp, Chai, Tuýp) |
| `SoLuong` | `int` | Not Null, Default `0` | Tổng số lượng tồn kho khả dụng |
| `MoTa` | `nvarchar(max)` | Null | Mô tả chi tiết công dụng và hướng dẫn |
| `CongDung` | `nvarchar(max)` | Null | Chỉ định điều trị của thuốc |
| `ThanhPhan` | `nvarchar(max)` | Null | Thành phần hóa học và tá dược |
| `TacDungPhu` | `nvarchar(max)` | Null | Các tác dụng không mong muốn |
| `NhaSanXuat` | `nvarchar(100)` | Null | Hãng sản xuất dược phẩm |
| `NgaySX` | `datetime2` | Null | Ngày sản xuất (Đồng bộ theo lô FEFO nếu là thuốc) |
| `HanSuDung` | `datetime2` | Null | Hạn sử dụng (Đồng bộ theo lô FEFO nếu là thuốc) |
| `TrenKe` | `bit` | Not Null, Default `0` | Trạng thái hiển thị bán trên Store Front |
| `NoiBat` | `bit` | Not Null, Default `0` | Ưu tiên hiển thị tại trang chủ |
| `HinhAnhFile` | `nvarchar(255)` | Null | Đường dẫn file ảnh đại diện được upload |
| `HinhAnhUrl` | `nvarchar(255)` | Null | Đường dẫn ảnh từ liên kết URL bên ngoài |
| `ThuVienHinhAnh`| `nvarchar(max)` | Null | Chuỗi danh sách ảnh phụ ngăn cách bởi dấu chấm phẩy |
| `NgayThem` | `datetime2` | Not Null | Thời điểm sản phẩm được tạo mới |

### 4. Bảng `LoThuoc` (Chi tiết các lô thuốc nhập phục vụ FEFO)

| Tên trường (Column) | Kiểu dữ liệu (Data Type) | Ràng buộc (Constraint) | Mô tả |
| :--- | :--- | :--- | :--- |
| `Id` | `int` | Primary Key, Identity | Mã định danh lô thuốc tự tăng |
| `MaLo` | `nvarchar(50)` | Not Null | Mã số lô (Ví dụ: LO-20260518-888) |
| `SanPhamId` | `int` | Foreign Key | Khóa ngoại liên kết tới bảng `SanPham` |
| `SoLuongBanDau` | `int` | Not Null | Số lượng thuốc nhập vào ban đầu |
| `SoLuongKhaDung`| `int` | Not Null | Số lượng khả dụng thực tế còn lại trong kho |
| `NgaySX` | `datetime2` | Not Null | Ngày sản xuất của lô thuốc |
| `HanSuDung` | `datetime2` | Not Null | Hạn sử dụng của lô thuốc |
| `TrangThai` | `bit` | Not Null, Default `1` | `true` nếu lô thuốc hoạt động tốt, `false` nếu bị hủy/tiêu hủy |

### 5. Bảng `DonHang` (Thông tin đơn hàng trực tuyến và POS)

| Tên trường (Column) | Kiểu dữ liệu (Data Type) | Ràng buộc (Constraint) | Mô tả |
| :--- | :--- | :--- | :--- |
| `Id` | `int` | Primary Key, Identity | Mã định danh đơn hàng tự tăng |
| `MaDonHang` | `nvarchar(50)` | Not Null | Mã đơn duy nhất hiển thị với người dùng |
| `LoaiDon` | `int` | Not Null | Loại đơn hàng: `0` (Online), `1` (POS bán tại quầy) |
| `TenDangNhap` | `nvarchar(50)` | Foreign Key, Null | Người đặt hàng (Khóa ngoại tới `TaiKhoan` - Null nếu khách lẻ POS) |
| `NhanVienPhuTrachId`| `nvarchar(50)`| Foreign Key, Null | Nhân viên bán tại quầy hoặc duyệt đơn |
| `NgayDat` | `datetime2` | Not Null | Thời điểm tạo đơn hàng |
| `TrangThai` | `int` | Not Null | Trạng thái: ChoThanhToan, DaThanhToan, DangChuanBi, DangGiao, DaGiao, DaHuy |
| `HoTenNguoiNhan` | `nvarchar(100)` | Not Null | Tên người nhận hàng |
| `SoDienThoai` | `nvarchar(20)` | Not Null | Số điện thoại nhận hàng |
| `DiaChiGiaoHang` | `nvarchar(255)` | Null | Địa chỉ nhận hàng (Null đối với POS) |
| `PhuongThucThanhToan`| `int` | Not Null | Cách thức: COD (0), ChuyenKhoan (1), ThanhToanTaiQuay (2) |
| `TongTienHang` | `decimal(18,2)` | Not Null | Tổng giá trị hàng hóa trước giảm giá |
| `MaVoucherApDung`| `nvarchar(50)` | Null | Mã voucher khuyến mãi đã áp dụng |
| `TienGiam` | `decimal(18,2)` | Not Null, Default `0` | Số tiền được giảm từ Voucher |
| `GhiChu` | `nvarchar(255)` | Null | Ghi chú đơn hàng từ khách hàng hoặc nhân viên |
| `NgayThanhToan` | `datetime2` | Null | Thời điểm hoàn thành thanh toán đơn |
| `NgayChuanBi` | `datetime2` | Null | Thời điểm nhân viên đóng gói xong hàng |
| `NgayBatDauGiao` | `datetime2` | Null | Thời điểm shipper bắt đầu đi giao hàng |
| `NgayGiao` | `datetime2` | Null | Thời điểm khách hàng nhận hàng thành công |
| `NgayHuy` | `datetime2` | Null | Thời điểm hủy đơn hàng |

### 6. Bảng `ChiTietDonHang` (Danh sách mặt hàng trong đơn hàng)

| Tên trường (Column) | Kiểu dữ liệu (Data Type) | Ràng buộc (Constraint) | Mô tả |
| :--- | :--- | :--- | :--- |
| `Id` | `int` | Primary Key, Identity | Mã chi tiết đơn tự tăng |
| `DonHangId` | `int` | Foreign Key | Khóa ngoại liên kết tới bảng `DonHang` |
| `SanPhamId` | `int` | Foreign Key, Null | Khóa ngoại tới `SanPham` (Null nếu sản phẩm bị xóa) |
| `TenSanPhamSnapshot`| `nvarchar(150)`| Not Null | Bản ghi lưu tên sản phẩm tránh mất dữ liệu lịch sử |
| `SoLuong` | `int` | Not Null | Số lượng mua |
| `GiaBan` | `decimal(18,2)` | Not Null | Đơn giá bán tại thời điểm mua |
| `SoVienMoiNgay` | `int` | Null | Chỉ định liều dùng: số viên uống mỗi ngày (dành cho đơn POS) |
| `SoNgayUong` | `int` | Null | Chỉ định liều dùng: số ngày uống (dành cho đơn POS) |
| `ThoiDiemUong` | `nvarchar(50)` | Null | Gợi ý uống: Sáng, Trưa, Chiều, Tối |
| `CachDung` | `nvarchar(100)` | Null | Cách dùng: Trước ăn, sau ăn, ngậm, uống với nước ấm |
| `GhiChuLieuDung` | `nvarchar(255)` | Null | Ghi chú thêm từ Dược sĩ về liều dùng |

### 7. Bảng `DonThuoc` (Đơn thuốc điện tử mẫu của Bác sĩ)

| Tên trường (Column) | Kiểu dữ liệu (Data Type) | Ràng buộc (Constraint) | Mô tả |
| :--- | :--- | :--- | :--- |
| `Id` | `int` | Primary Key, Identity | Mã đơn thuốc tự tăng |
| `MaDonThuoc` | `nvarchar(50)` | Not Null | Mã đơn thuốc tự sinh dạng duy nhất |
| `TenDonThuoc` | `nvarchar(150)` | Not Null | Tên bệnh lý hoặc tiêu đề đơn thuốc |
| `HinhAnh` | `nvarchar(255)` | Null | Ảnh chụp toa thuốc thực tế |
| `GhiChu` | `nvarchar(max)` | Null | Lời khuyên, chẩn đoán chi tiết của bác sĩ |
| `TrangThai` | `int` | Not Null | Trạng thái duyệt: `MoiTao` (0), `DaDuyet` (1), `DaHuy` (2) |
| `DaDangStore` | `bit` | Not Null, Default `0` | Cho phép bán trọn gói đơn thuốc này trực tuyến |

### 8. Bảng `ChiTietDonThuoc` (Chi tiết chỉ định biệt dược trong toa thuốc)

| Tên trường (Column) | Kiểu dữ liệu (Data Type) | Ràng buộc (Constraint) | Mô tả |
| :--- | :--- | :--- | :--- |
| `Id` | `int` | Primary Key, Identity | Mã chi tiết toa thuốc tự tăng |
| `DonThuocId` | `int` | Foreign Key | Khóa ngoại liên kết tới bảng `DonThuoc` |
| `SanPhamId` | `int` | Foreign Key, Null | Khóa ngoại tới `SanPham` (Thuốc biệt dược kê toa) |
| `TenSanPhamSnapshot`| `nvarchar(150)`| Null | Bản ghi tên thuốc tại thời điểm kê toa |
| `SoVienMoiNgay` | `int` | Not Null | Số viên uống trong một ngày |
| `SoNgayUong` | `int` | Not Null | Số lượng ngày sử dụng thuốc |
| `ThoiDiemUong` | `nvarchar(50)` | Null | Chỉ định thời gian uống trong ngày |
| `CachDung` | `nvarchar(100)` | Null | Hướng dẫn: Uống trước/sau bữa ăn |

### 9. Bảng `Voucher` (Thông tin mã giảm giá)

| Tên trường (Column) | Kiểu dữ liệu (Data Type) | Ràng buộc (Constraint) | Mô tả |
| :--- | :--- | :--- | :--- |
| `Id` | `int` | Primary Key, Identity | Mã voucher tự tăng |
| `MaVoucher` | `nvarchar(50)` | Not Null | Mã code để người dùng nhập (Ví dụ: GIAM20) |
| `PhanTramGiam` | `decimal(5,2)` | Not Null | Tỷ lệ giảm giá (%) của đơn hàng |
| `GiamToiDa` | `decimal(18,2)` | Not Null | Số tiền giảm tối đa cho phép |
| `DonHangToiThieu` | `decimal(18,2)` | Not Null | Giá trị đơn hàng tối thiểu để áp dụng |
| `NgayBatDau` | `datetime2` | Not Null | Thời điểm mã bắt đầu có hiệu lực |
| `NgayHetHan` | `datetime2` | Not Null | Thời điểm mã hết hiệu lực |
| `SoLanSuDung` | `int` | Not Null | Số lượng lượt sử dụng tối đa còn lại của mã |
| `KichHoat` | `bit` | Not Null, Default `1` | Trạng thái cho phép sử dụng mã hay không |

### 10. Bảng `GioHang` & `ChiTietGioHang` (Đồng bộ giỏ hàng cá nhân)
Dành cho việc lưu trữ giỏ hàng lâu dài của khách hàng trên database, giải quyết triệt để lỗi mất giỏ hàng khi tắt trình duyệt hoặc đăng xuất.

| Bảng | Tên trường (Column) | Kiểu dữ liệu (Data Type) | Mô tả |
| :--- | :--- | :--- | :--- |
| `GioHang` | `TenDangNhap` (PK) | `nvarchar(50)` | Khóa chính và cũng là khóa ngoại tới `TaiKhoan` |
| `GioHang` | `CapNhatCuoi` | `datetime2` | Lần cuối cùng giỏ hàng thay đổi |
| `ChiTietGioHang`| `Id` (PK) | `int` | Khóa chính tự tăng |
| `ChiTietGioHang`| `GioHangId` (FK) | `nvarchar(50)` | Khóa ngoại liên kết tới bảng `GioHang` |
| `ChiTietGioHang`| `SanPhamId` (FK) | `int` | Khóa ngoại liên kết tới bảng `SanPham` |
| `ChiTietGioHang`| `SoLuong` | `int` | Số lượng sản phẩm khách hàng thêm vào giỏ |

---

## 2.4. Sơ đồ các mối quan hệ thực thể (ERD) và ràng buộc

Sơ đồ ERD dưới đây biểu diễn chi tiết cấu trúc liên kết cơ sở dữ liệu của toàn bộ dự án:

```mermaid
erDiagram
    TaiKhoan ||--o{ DonHang : "dat_hang"
    TaiKhoan ||--o{ DonHang : "nhan_vien_ban"
    DanhMucSanPham ||--o{ SanPham : "chua"
    SanPham ||--o{ LoThuoc : "co"
    SanPham ||--o{ ChiTietDonHang : "co_trong"
    SanPham ||--o{ ChiTietDonThuoc : "duoc_ke"
    SanPham ||--o{ ChiTietGioHang : "co_trong_gio"
    DonHang ||--|{ ChiTietDonHang : "chua"
    GioHang ||--|{ ChiTietGioHang : "chua_chi_tiet"
    GioHang |o--|| TaiKhoan : "so_huu"
    DonThuoc ||--|{ ChiTietDonThuoc : "co_chi_tiet"
```

### Các ràng buộc chính (Data Integrity Rules)
1. **SanPham - DanhMucSanPham (Cascade):** Xóa danh mục sẽ không xóa sản phẩm trừ khi được cho phép; tuy nhiên trong hệ thống danh mục chỉ được Tắt kích hoạt (`KichHoat = false`) để bảo toàn dữ liệu.
2. **ChiTietDonHang - SanPham (Set Null):** Khi xóa một sản phẩm trong hệ thống (chỉ Admin mới có quyền), khóa ngoại `SanPhamId` trong chi tiết đơn hàng cũ sẽ chuyển thành `NULL` để tránh mất hóa đơn lịch sử doanh thu.
3. **DonHang - TaiKhoan (Restrict):** Không cho phép xóa các tài khoản Khách hàng hoặc Nhân viên đã phát sinh bất kỳ đơn hàng nào trong hệ thống để tránh lỗi mồ côi dữ liệu (Orphaned Data).

---
<!-- slide -->
# CHƯƠNG 3: THIẾT KẾ KỸ THUẬT VÀ GIẢI PHÁP CÔNG NGHỆ

## 3.1. Kiến trúc tổng quan hệ thống

Hệ thống được thiết kế và triển khai dựa trên kiến trúc **MVC (Model - View - Controller)** truyền thống nhưng được tối ưu hóa tối đa cho các dịch vụ nghiệp vụ hiện đại của nền tảng **.NET 10.0**:

```mermaid
graph TD
    A[Client Browser / Desktop POS] -->|HTTPS Requests| B[ASP.NET Core Routing]
    B --> C[Controllers - GioHang/Store/LoThuoc...]
    C -->|Dependency Injection| D[Helpers / Services - KhoHelper/AutoHuy...]
    D -->|LINQ to Entities| E[Entity Framework Core 9.0]
    E -->|ADO.NET Connection| F[Microsoft SQL Server]
    D -->|External HTTPS API| G[Google Gemini LLM]
```

*   **Presentation Layer (Views):** Sử dụng các file Razor View (`.cshtml`) kết hợp với CSS/JS thuần, Tailwind CSS, Bootstrap và các thư viện hỗ trợ AJAX nâng cao giúp mang lại trải nghiệm mượt mà, trực quan cho cả khách hàng lẫn nhân viên bán hàng tại quầy.
*   **Business Logic Layer (Services & Helpers):** Đóng gói toàn bộ logic nghiệp vụ khó như tính toán FEFO (`KhoHelper.cs`), tự động hóa ngầm (`DonHangAutoHuyService.cs`) và xử lý bảo mật mật khẩu (`PasswordHelper.cs`). Việc tách biệt logic ra khỏi Controller giúp tăng khả năng tái sử dụng mã nguồn và dễ dàng kiểm thử đơn vị (Unit Test).
*   **Data Access Layer (EF Core & SQL Server):** Sử dụng ORM mạnh mẽ hàng đầu của Microsoft giúp thao tác với CSDL thông qua các câu lệnh C# (LINQ), hỗ trợ đắc lực cơ chế Tracking thực thể, Migration phiên bản CSDL và kiểm soát giao dịch an toàn.

---

## 3.2. Thuật toán quản lý kho dược phẩm tối ưu FEFO (First-Expired, First-Out)

Trái tim công nghệ của hệ thống nằm ở **`KhoHelper.cs`**, triển khai chính xác nguyên lý FEFO.

### A. Hàm xuất kho tự động trừ Lô thuốc hết hạn trước (`TruKhoFEFOAsync`)

Khi khách hàng hoàn tất checkout đơn trực tuyến hoặc nhân viên in hóa đơn tại quầy POS, hệ thống sẽ gọi hàm này:
```csharp
public static async Task<bool> TruKhoFEFOAsync(AppDbContext context, int sanPhamId, int soLuongCanTru)
{
    var sp = await context.SanPhams.FindAsync(sanPhamId);
    if (sp == null || sp.SoLuong < soLuongCanTru) return false;

    // Nếu sản phẩm là Thuốc -> xử lý FEFO
    if (sp.IsThuoc)
    {
        // Lấy các lô thuốc còn hạn sử dụng, sắp xếp hạn gần nhất lên trước
        var cacLo = await context.LoThuocs
            .Where(l => l.SanPhamId == sanPhamId && l.SoLuongKhaDung > 0 && l.TrangThai
                        && l.HanSuDung > DateTime.Today)
            .OrderBy(l => l.HanSuDung)
            .ToListAsync();

        int tongTon = cacLo.Sum(l => l.SoLuongKhaDung);

        // Đảm bảo tổng số lượng thực tế trong các lô đủ hàng xuất
        if (tongTon < soLuongCanTru) return false;

        int soLuongConLaiCanTru = soLuongCanTru;

        foreach (var lo in cacLo)
        {
            if (soLuongConLaiCanTru <= 0) break;

            int truTuLoNay = Math.Min(lo.SoLuongKhaDung, soLuongConLaiCanTru);
            
            lo.SoLuongKhaDung -= truTuLoNay;
            soLuongConLaiCanTru -= truTuLoNay;
        }

        // Tự động đồng bộ lại HSD/NgaySX hiển thị trên kệ của sản phẩm theo lô FEFO đang bán
        await CapNhatHanSuDungTheoLoAsync(context, sp);
    }

    // Trừ tổng số tồn kho của sản phẩm
    sp.SoLuong -= soLuongCanTru;
    return true;
}
```

### B. Hàm hoàn kho tự động trả hàng vào Lô có hạn sử dụng xa nhất (`HoanKhoFEFOAsync`)
Để bảo vệ quyền lợi kinh doanh tối đa, khi đơn hàng bị hủy, thuốc hoàn kho phải được ưu tiên đưa vào lô có hạn sử dụng xa nhất (lô mới nhập) để tránh làm loãng các lô cũ gần hết hạn đang cần đẩy bán gấp.

```csharp
public static async Task HoanKhoFEFOAsync(AppDbContext context, int sanPhamId, int soLuongHoan)
{
    var sp = await context.SanPhams.FindAsync(sanPhamId);
    if (sp != null)
    {
        if (sp.IsThuoc)
        {
            // Tìm lô thuốc chưa bán hết và có HSD xa nhất để trả hàng
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

            // Đồng bộ lại HSD/NgaySX sau khi hoàn kho
            await CapNhatHanSuDungTheoLoAsync(context, sp);
        }

        sp.SoLuong += soLuongHoan;
    }
}
```

### C. Cơ chế đồng bộ HSD hiển thị trên kệ Store Front (`CapNhatHanSuDungTheoLoAsync`)
Khách hàng khi mua thuốc trực tuyến cần biết chính xác hạn sử dụng của vỉ thuốc họ sắp nhận. Nhờ có phương thức này, hệ thống sẽ luôn hiển thị HSD của lô thuốc hết hạn sớm nhất đang sẵn sàng bán. Khi lô cũ được bán hết sạch (`SoLuongKhaDung == 0`), hệ thống tự động nhảy sang hiển thị HSD của lô kế tiếp giúp khách hàng an tâm tuyệt đối.

---

## 3.3. Cơ chế tự động hóa: Background Service DonHangAutoHuyService

Đối với các đơn hàng mua trực tuyến chọn phương thức chuyển khoản nhưng khách hàng không thanh toán, nếu giữ đơn hàng quá lâu sẽ làm giảm lượng tồn kho khả dụng của các lô thuốc, ảnh hưởng nghiêm trọng đến dược sĩ khi bán trực tiếp tại quầy POS.

Hệ thống xây dựng một **Background Service** chạy ngầm định kỳ kế thừa `BackgroundService` của .NET để tự động hóa nghiệp vụ giải phóng kho này:

```csharp
public class DonHangAutoHuyService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    public DonHangAutoHuyService(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await HuyDonHangQuaHanAsync(context);
            }
            // Định kỳ chạy mỗi 5 phút một lần
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }

    private async Task HuyDonHangQuaHanAsync(AppDbContext context)
    {
        var nguongThoiGian = DateTime.Now.AddMinutes(-30); // Quá hạn 30 phút chưa thanh toán

        var donHangs = await context.DonHangs
            .Include(d => d.ChiTietDonHangs)
            .Where(d => d.LoaiDon == LoaiDonHang.Online 
                        && d.TrangThai == TrangThaiDonHang.ChoThanhToan 
                        && d.NgayDat < nguongThoiGian)
            .ToListAsync();

        foreach (var dh in donHangs)
        {
            dh.TrangThai = TrangThaiDonHang.DaHuy;
            dh.NgayHuy = DateTime.Now;
            dh.GhiChu = "Hệ thống tự động hủy do quá hạn 30 phút chưa thanh toán.";

            // Hoàn trả kho theo thuật toán FEFO
            foreach (var ct in dh.ChiTietDonHangs)
            {
                if (ct.SanPhamId.HasValue)
                {
                    await KhoHelper.HoanKhoFEFOAsync(context, ct.SanPhamId.Value, ct.SoLuong);
                }
            }
        }
        await context.SaveChangesAsync();
    }
}
```

---

## 3.4. Đảm bảo toàn vẹn dữ liệu trong giao dịch (Database Transaction ACID)

Quy trình checkout (Đặt hàng trực tuyến) và thanh toán POS liên quan đến việc thay đổi trạng thái của nhiều bảng dữ liệu cùng một lúc: cập nhật số lượng của `LoThuoc`, cập nhật số lượng tồn kho `SanPham`, giảm lượt sử dụng của `Voucher`, lưu `DonHang`, lưu `ChiTietDonHang` và xóa `GioHang` cũ.

Để ngăn chặn các tình trạng lỗi hệ thống (lỗi mạng, cúp điện, treo máy) làm dữ liệu bị hụt (ví dụ: kho đã trừ nhưng đơn hàng không lưu được), toàn bộ logic được bọc trong một **DbTransaction** của EF Core để đảm bảo tính **ACID (Atomicity, Consistency, Isolation, Durability)** tuyệt đối:

```csharp
using var transaction = await _context.Database.BeginTransactionAsync();
try
{
    // 1. Kiểm tra và áp dụng Voucher
    // 2. Trừ kho theo phương pháp FEFO
    // 3. Tạo mới DonHang & ChiTietDonHang
    // 4. Xóa giỏ hàng trên DB của khách hàng
    
    await _context.SaveChangesAsync();
    await transaction.CommitAsync(); // Xác nhận giao dịch thành công hoàn toàn
}
catch (Exception ex)
{
    await transaction.RollbackAsync(); // Thu hồi toàn bộ các thay đổi nếu có bất kỳ lỗi nào xảy ra
    throw;
}
```

---

## 3.5. Cơ chế xác thực (Authentication), phân quyền (Authorization) và kiểm soát IDOR

Hệ thống triển khai an toàn bảo mật đa lớp cực kỳ vững chắc:
*   **Xác thực bằng Cookie (Cookie Authentication):** Lưu trữ phiên đăng nhập an toàn, cấu hình thời hạn tự động đăng xuất sau 8 giờ hoạt động liên tục hoặc lưu trữ 7 ngày nếu chọn tính năng "Ghi nhớ đăng nhập".
*   **Phân quyền dựa trên Policy và Role:** Cấu hình tường lửa phân quyền chi tiết tại cấp độ Controller và Action:
    *   `[Authorize(Roles = "Admin,NhanVien")]` cho các nghiệp vụ quản lý kho, POS.
    *   `[Authorize(Roles = "Admin")]` cho các hành động nhạy cảm như xóa sản phẩm, điều chỉnh Voucher, tiêu hủy thuốc.
*   **Phòng chống lỗ hổng IDOR (Insecure Direct Object Reference):**
    Khi khách hàng muốn xem trang hóa đơn thành công hoặc thanh toán giả lập, hệ thống bắt buộc kiểm tra xem đơn hàng đó có thực sự thuộc sở hữu của tài khoản đang đăng nhập hay không:
    ```csharp
    var dh = await _context.DonHangs.FindAsync(id);
    if (dh.TenDangNhap != User.FindFirst(ClaimTypes.NameIdentifier)?.Value)
    {
        return Forbid(); // Trả về lỗi cấm truy cập ngay lập tức
    }
    ```

---

## 3.6. Tích hợp mô hình ngôn ngữ lớn (LLM - Google Gemini API) hỗ trợ tư vấn biệt dược

Để cung cấp trải nghiệm tư vấn thông minh, hệ thống tích hợp trực tiếp mô hình AI của Google thông qua API. Điểm đặc biệt của giải pháp này là **Prompt Contextualization (Cá nhân hóa ngữ cảnh Prompt)**: trước khi gửi câu hỏi của người dùng lên Gemini, hệ thống sẽ truy vấn cơ sở dữ liệu thực tế để lấy danh sách toàn bộ các loại thuốc hiện đang bán kèm theo mô tả, công dụng và thành phần của chúng.

Sau đó, hệ thống xây dựng một **System Prompt** cực kỳ nghiêm ngặt nhằm biến Gemini thành một chuyên gia tư vấn y tế của riêng nhà thuốc:

```csharp
var sanPhams = await _context.SanPhams.Where(s => s.TrenKe).ToListAsync();
string contextThuoc = string.Join("\n", sanPhams.Select(s => 
    $"- ID: {s.Id}, Tên: {s.TenSanPham}, Công dụng: {s.CongDung}, Thành phần: {s.ThanhPhan}, Đơn vị: {s.DonViTinh}"));

string systemPrompt = $@"Bạn là Trợ lý Dược sĩ ảo thông minh của nhà thuốc Online. 
Dưới đây là danh sách thuốc thực tế đang có trong kho của nhà thuốc:
{contextThuoc}

Nhiệm vụ của bạn:
1. Chỉ tư vấn, gợi ý các thuốc có trong danh sách trên. Không tự ý bịa đặt hoặc giới thiệu thuốc ngoài danh sách.
2. Luôn nhắc nhở khách hàng: 'Đây là tư vấn mang tính tham khảo. Khách hàng bắt buộc phải tuân theo chỉ định của Bác sĩ hoặc Dược sĩ trực tiếp trước khi uống.'
3. Đưa ra liều lượng, công dụng và cách dùng một cách khoa học, ngắn gọn, dễ hiểu.";
```

Cơ chế này đảm bảo trợ lý AI luôn đưa ra thông tin tư vấn cực kỳ chính xác và bám sát thực tế kho hàng của nhà thuốc, tránh tình trạng đề xuất các loại thuốc không có sẵn.

---
<!-- slide -->
# CHƯƠNG 4: TRIỂN KHAI VÀ KẾT QUẢ ĐẠT ĐƯỢC

## 4.1. Môi trường triển khai và cấu hình dự án

Hệ thống đã được đóng gói và thử nghiệm thành công trên môi trường cục bộ chuẩn bị cho việc đưa lên Cloud:
*   **Hệ điều hành tối ưu:** Windows Server / Windows 11.
*   **Môi trường phát triển:** Microsoft Visual Studio 2022 / JetBrains Rider.
*   **Cơ sở dữ liệu:** Microsoft SQL Server 2022 Express.
*   **Phiên bản runtime:** .NET 10.0 (C# 14).
*   **Cấu hình kết nối API:** Google Gemini Pro API Key được bảo mật an toàn thông qua biến môi trường hoặc file cấu hình bí mật.

---

## 4.2. Kết quả giao diện & chức năng phía Khách hàng (Store Front)

Khách hàng truy cập trang Web chính thức của nhà thuốc sẽ được trải nghiệm các giao diện cao cấp:
1.  **Trang chủ và Store bán hàng trực tuyến:** Thiết kế hiện đại, responsive hoàn hảo trên thiết bị di động. Khách hàng có thể dễ dàng lọc thuốc theo danh mục, tìm kiếm thuốc nhanh theo cơ chế gõ phím đến đâu hiển thị kết quả đến đó (AJAX Search).
2.  **Xem chi tiết sản phẩm và Hạn sử dụng theo lô:** Hiển thị chi tiết thành phần, chỉ định, tác dụng phụ và đặc biệt là hạn sử dụng của lô hàng đang phân phối, tạo độ tin cậy tuyệt đối.
3.  **Trang Đơn thuốc bác sĩ (Toa thuốc mẫu):** Cho phép người dùng tìm kiếm đơn thuốc theo mã chẩn đoán, xem hình ảnh toa thuốc của bác sĩ kê và thực hiện mua trọn bộ thuốc trong toa chỉ bằng nút "Mua trọn đơn thuốc".
4.  **Hộp thoại Trợ lý y tế AI (Gemini Chatbot):** Giao diện chat dạng trượt bên góc màn hình với hiệu ứng typing mượt mà, hỗ trợ giải đáp mọi thắc mắc của khách hàng về thuốc 24/7.

---

## 4.3. Kết quả giao diện & chức năng phía Quản trị (Admin) và Nhân viên (POS)

1.  **Trang Dashboard Quản trị trực quan:** Cung cấp biểu đồ trực quan hóa dữ liệu doanh thu của 7 ngày gần nhất (sử dụng thư viện Chart.js). Hiển thị các cảnh báo quan trọng: Số lượng lô thuốc sắp hết hạn (trong vòng 60 ngày), các loại thuốc sắp hết hàng (dưới 20 đơn vị) để Admin kịp thời lên kế hoạch nhập hàng mới.
2.  **Giao diện Bán lẻ tại Quầy (POS):** Thiết kế tối ưu hóa cho dược sĩ thao tác nhanh chóng bằng bàn phím. Dược sĩ chỉ cần gõ vài ký tự đầu, hệ thống sẽ gợi ý danh sách thuốc, tự động điền giá bán. Dược sĩ có thể dễ dàng nhập liều lượng uống trực tiếp và tiến hành in hóa đơn ngay lập tức.
3.  **Màn hình quản lý Lô thuốc và Tiêu hủy thuốc:** Cho phép nhập lô mới với việc tự động gợi ý mã lô khoa học theo định dạng `LO-YYYYMMDD-XXX`. Khi thuốc hết hạn hoặc hỏng, Admin bấm nút "Tiêu hủy", hệ thống tự động trừ kho khả dụng, đổi trạng thái lô về vô hiệu hóa và cập nhật lại HSD của sản phẩm trên kệ.
4.  **Màn hình quản lý Voucher khuyến mãi:** Cho phép thiết lập mã giảm giá linh hoạt theo phần trăm, số tiền giảm tối đa, yêu cầu giá trị đơn hàng tối thiểu và số lượt sử dụng tối đa.

---
<!-- slide -->
# CHƯƠNG 5: ĐÁNH GIÁ VÀ HƯỚNG PHÁT TRIỂN

## 5.1. Ưu điểm nổi bật của hệ thống

Hệ thống hoàn thành đầy đủ tất cả các yêu cầu đề ra và đạt được các ưu điểm vượt trội:
*   **Thực thi hoàn hảo nguyên lý FEFO:** Giải quyết triệt để bài toán rủi ro hết hạn của dược phẩm. Thuật toán trừ kho FEFO chạy chính xác, ổn định ở mức cơ sở dữ liệu.
*   **Tính toàn vẹn dữ liệu cực cao:** Sự kết hợp của `DbTransaction` và cơ chế concurrency check bảo vệ hệ thống tuyệt đối trước các lỗi thất thoát hàng hóa hoặc sai sót dữ liệu khi bán hàng đồng thời ở hai kênh Online và POS.
*   **Trải nghiệm người dùng thông minh vượt trội:** Tích hợp thành công AI Gemini giúp nâng tầm trải nghiệm chăm sóc khách hàng, biến trang web thương mại điện tử thông thường thành một trung tâm y tế số thân thiện.
*   **Tối ưu hiệu năng:** Cơ chế đồng bộ giỏ hàng thông minh (Session sync DB) giúp khách hàng mua sắm mượt mà, không bị mất giỏ hàng khi thay đổi thiết bị đăng nhập.

---

## 5.2. Nhược điểm và hạn chế tồn tại

Mặc dù đạt được nhiều thành công lớn, hệ thống vẫn còn một số điểm hạn chế cần cải tiến:
*   Hệ thống tư vấn AI Gemini hiện tại phụ thuộc hoàn toàn vào kết nối Internet và tốc độ phản hồi của API bên thứ ba.
*   Chưa tích hợp trực tiếp với máy quét mã vạch (Barcode Scanner) thực tế để tối ưu hơn nữa quy trình POS tại quầy.
*   Quy trình vận chuyển và thanh toán trực tuyến mới dừng lại ở mức giả lập quy trình nghiệp vụ an toàn, chưa kết nối trực tiếp với các đơn vị vận chuyển (Giao Hàng Nhanh, Viettel Post) và cổng thanh toán quốc gia.

---

## 5.3. Hướng phát triển và mở rộng trong tương lai

Trong thời gian tới, hệ thống dự kiến sẽ được nâng cấp và mở rộng theo các hướng sau:
1.  **Phát triển Ứng dụng di động (Mobile App):** Xây dựng ứng dụng di động cho cả Khách hàng (bằng Flutter/React Native) để tăng tính gắn kết, gửi thông báo đẩy nhắc nhở uống thuốc định kỳ theo đơn.
2.  **Tích hợp phần cứng POS chuyên dụng:** Kết nối trực tiếp hệ thống POS với máy quét mã vạch, máy in hóa đơn nhiệt cầm tay và máy quẹt thẻ ngân hàng.
3.  **Tối ưu hóa Trí tuệ nhân tạo:** Chuyển đổi sang mô hình AI nội bộ (Local LLM) để bảo mật thông tin sức khỏe khách hàng tuyệt đối và tự chủ công nghệ không phụ thuộc API ngoài.
4.  **Kết nối cổng thanh toán trực tuyến:** Tích hợp cổng thanh toán VNPAY, MoMo, Apple Pay để tự động hóa 100% quy trình xác nhận giao dịch online.

---
<!-- slide -->
# TÀI LIỆU THAM KHẢO

1.  **Microsoft .NET Documentation:** *ASP.NET Core MVC & Entity Framework Core Overview*, [https://learn.microsoft.com/en-us/aspnet/core/](https://learn.microsoft.com/en-us/aspnet/core/).
2.  **Bộ Y Tế Việt Nam:** *Thông tư hướng dẫn về Quản lý và Lưu thông Dược phẩm theo tiêu chuẩn GSP & GPP*.
3.  **Martin Fowler:** *Patterns of Enterprise Application Architecture*, Addison-Wesley Professional.
4.  **Google AI Developers Documentation:** *Gemini API Quickstart and System Instructions Guide*, [https://ai.google.dev/gemini-api/docs](https://ai.google.dev/gemini-api/docs).
5.  **Robert C. Martin:** *Clean Architecture: A Craftsman's Guide to Software Structure and Design*, Prentice Hall.
