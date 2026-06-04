namespace do_an.Helpers;

public static class PasswordHelper
{
    /// <summary>
    /// Hash mật khẩu bằng BCrypt (tự động tạo Salt ngẫu nhiên, chống Rainbow Table).
    /// </summary>
    public static string Hash(string password)
        => BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);

    /// <summary>
    /// So khớp mật khẩu người dùng nhập với chuỗi Hash đã lưu trong DB.
    /// </summary>
    public static bool Verify(string password, string storedHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, storedHash);
    }
}
