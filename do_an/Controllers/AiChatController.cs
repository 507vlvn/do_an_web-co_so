using do_an.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace do_an.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AiChatController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private static readonly HttpClient _httpClient = new HttpClient();

        public AiChatController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public class ChatRequest
        {
            public string Message { get; set; } = string.Empty;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] ChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Message))
                return BadRequest("Tin nhắn trống");

            try
            {
                // 1. Lấy API Key
                var apiKey = _config["Gemini:ApiKey"];
                if (string.IsNullOrEmpty(apiKey))
                {
                    return Ok(new { response = "Hệ thống đang bảo trì AI (Thiếu API Key). Vui lòng thử lại sau hoặc liên hệ Admin." });
                }

                // 2. Tìm kiếm sản phẩm liên quan để cung cấp Context cho AI
                var keywords = request.Message.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var query = _context.SanPhams.Where(s => s.TrenKe && s.SoLuong > 0 && !s.IsThuoc);
                
                // Giới hạn context khoảng 5 sản phẩm khớp nhất
                var relatedProducts = await query.ToListAsync();
                
                // Lọc cơ bản trên memory vì sqlite/sql server có thể không hỗ trợ regex full text tốt
                var matchedProducts = relatedProducts
                    .Where(s => keywords.Any(k => s.TenSanPham.ToLower().Contains(k) || (s.CongDung != null && s.CongDung.ToLower().Contains(k))))
                    .Take(5)
                    .ToList();

                // Nếu không tìm thấy bằng keyword, lấy ngẫu nhiên 5 sản phẩm nổi bật
                if (matchedProducts.Count == 0)
                {
                    matchedProducts = relatedProducts.Where(s => s.NoiBat).Take(5).ToList();
                }

                // 3. Xây dựng System Prompt
                var contextBuilder = new StringBuilder();
                contextBuilder.AppendLine("Bạn là một dược sĩ tư vấn tận tâm và chuyên nghiệp của nhà thuốc online. Nhiệm vụ của bạn là tư vấn sức khỏe cơ bản và gợi ý các sản phẩm phù hợp cho khách hàng dựa vào danh sách sản phẩm sau:");
                
                foreach (var sp in matchedProducts)
                {
                    contextBuilder.AppendLine($"- Tên: {sp.TenSanPham}. Giá: {sp.GiaBan:N0} VNĐ.");
                    if (!string.IsNullOrEmpty(sp.CongDung)) contextBuilder.AppendLine($"  Công dụng: {sp.CongDung}");
                    if (!string.IsNullOrEmpty(sp.ThanhPhan)) contextBuilder.AppendLine($"  Thành phần: {sp.ThanhPhan}");
                }
                
                contextBuilder.AppendLine("Quy tắc trả lời:");
                contextBuilder.AppendLine("1. Luôn lịch sự, ngắn gọn và dễ hiểu.");
                contextBuilder.AppendLine("2. Chỉ khuyên dùng các sản phẩm có trong danh sách trên nếu phù hợp. Nếu không có sản phẩm nào hợp, hãy nói thật là hiện cửa hàng chưa có thuốc đó và khuyên họ đi khám bác sĩ.");
                contextBuilder.AppendLine("3. Trả lời bằng tiếng Việt, định dạng rõ ràng (có thể dùng icon).");
                contextBuilder.AppendLine("4. Luôn thêm câu nhắc: \"Lưu ý: Bạn nên tham khảo ý kiến bác sĩ hoặc chuyên gia y tế trước khi sử dụng bất kỳ loại thuốc nào.\" nếu tư vấn về bệnh.");

                // 4. Tạo payload gửi lên Gemini
                var payload = new
                {
                    systemInstruction = new { parts = new[] { new { text = contextBuilder.ToString() } } },
                    contents = new[]
                    {
                        new { role = "user", parts = new[] { new { text = request.Message } } }
                    },
                    generationConfig = new
                    {
                        temperature = 0.5
                    }
                };

                var jsonPayload = JsonSerializer.Serialize(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                // 5. Gửi request
                var geminiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-flash-latest:generateContent?key={apiKey}";
                var response = await _httpClient.PostAsync(geminiUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorMsg = await response.Content.ReadAsStringAsync();
                    // Log error to console
                    Console.WriteLine("GEMINI ERROR: " + errorMsg);
                    return Ok(new { response = $"Lỗi từ Gemini API: {errorMsg}" });
                }

                var responseString = await response.Content.ReadAsStringAsync();
                
                // 6. Parse kết quả trả về (Cấu trúc JSON của Gemini API)
                using var doc = JsonDocument.Parse(responseString);
                var root = doc.RootElement;
                var candidates = root.GetProperty("candidates");
                if (candidates.GetArrayLength() > 0)
                {
                    var text = candidates[0]
                        .GetProperty("content")
                        .GetProperty("parts")[0]
                        .GetProperty("text")
                        .GetString();

                    return Ok(new { response = text });
                }

                return Ok(new { response = "Tôi chưa hiểu rõ ý bạn, bạn có thể giải thích thêm không?" });
            }
            catch (Exception ex)
            {
                // Ghi log lỗi nếu cần
                return Ok(new { response = "Có lỗi xảy ra trong quá trình xử lý câu hỏi của bạn. Vui lòng thử lại." });
            }
        }
    }
}
