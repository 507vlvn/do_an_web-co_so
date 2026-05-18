namespace do_an.Helpers;

public static class PasswordHelper
{
    /// Hash mật khẩu bằng BCrypt (tự động tạo Salt ngẫu nhiên, chống Rainbow Table).

    public static string Hash(string password)
        => BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);


    /// So khớp mật khẩu người dùng nhập với chuỗi Hash đã lưu trong DB.
    /// Tương thích cả Hash BCrypt mới lẫn Hash SHA256 cũ (để không lock-out user cũ).

    public static bool Verify(string password, string storedHash)
    {
        // Kiểm tra xem hash cũ có phải SHA256 hay không (SHA256 hex luôn 64 ký tự)
        // $2 Chỉ xuất hiện khi dùng BCrypt
        if (storedHash.Length == 64 && !storedHash.StartsWith("$2"))
        {
            // Đây là hash SHA256 cũ -> so sánh bằng thuật toán cũ
            return LegacySHA256Verify(password, storedHash);
        }

        // Hash BCrypt mới
        return BCrypt.Net.BCrypt.Verify(password, storedHash);
    }

    /// [Legacy] So khớp bằng SHA256 cho các tài khoản chưa đổi mật khẩu.
    private static bool LegacySHA256Verify(string password, string hash)
    {
        var bytes = System.Security.Cryptography.SHA256.HashData(
            System.Text.Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes).Equals(hash, StringComparison.OrdinalIgnoreCase);
    }
}
