using Xunit;
using do_an.Helpers;

namespace do_an.UnitTests.Helpers;

public class PasswordHelperTests
{
    [Fact]
    public void Hash_ReturnsNonEmptyString()
    {
        var hash = PasswordHelper.Hash("TestPassword123");
        Assert.False(string.IsNullOrEmpty(hash));
    }

    [Fact]
    public void Hash_StartsWithBCryptPrefix()
    {
        var hash = PasswordHelper.Hash("TestPassword123");
        Assert.StartsWith("$2", hash);
    }

    [Fact]
    public void Hash_ProducesDifferentHashesForSameInput()
    {
        var hash1 = PasswordHelper.Hash("TestPassword123");
        var hash2 = PasswordHelper.Hash("TestPassword123");
        Assert.NotEqual(hash1, hash2); // Salt ngau nhien moi lan
    }

    [Fact]
    public void Verify_CorrectPassword_ReturnsTrue()
    {
        var password = "MySecurePass!";
        var hash = PasswordHelper.Hash(password);
        Assert.True(PasswordHelper.Verify(password, hash));
    }

    [Fact]
    public void Verify_WrongPassword_ReturnsFalse()
    {
        var hash = PasswordHelper.Hash("CorrectPassword");
        Assert.False(PasswordHelper.Verify("WrongPassword", hash));
    }

    [Fact]
    public void Verify_EmptyPassword_ReturnsFalse()
    {
        var hash = PasswordHelper.Hash("SomePassword");
        Assert.False(PasswordHelper.Verify("", hash));
    }

    [Fact]
    public void Verify_LegacySHA256_ReturnsTrueForCorrectPassword()
    {
        // SHA256 hash cua "test123"
        var sha256Hash = Convert.ToHexString(
            System.Security.Cryptography.SHA256.HashData(
                System.Text.Encoding.UTF8.GetBytes("test123")));

        Assert.True(PasswordHelper.Verify("test123", sha256Hash));
    }

    [Fact]
    public void Verify_LegacySHA256_ReturnsFalseForWrongPassword()
    {
        var sha256Hash = Convert.ToHexString(
            System.Security.Cryptography.SHA256.HashData(
                System.Text.Encoding.UTF8.GetBytes("test123")));

        Assert.False(PasswordHelper.Verify("wrongpass", sha256Hash));
    }

    [Fact]
    public void Verify_DistinguishesBCryptFromSHA256()
    {
        var bcryptHash = PasswordHelper.Hash("test");
        // BCrypt hash dai hon 64 ky tu va bat dau bang $2
        Assert.True(PasswordHelper.Verify("test", bcryptHash));
    }

    [Theory]
    [InlineData("short")]
    [InlineData("a very long password with special chars !@#$%^&*()")]
    [InlineData("matkhauTiengViet")]
    [InlineData("12345678")]
    public void HashAndVerify_VariousPasswords_WorksCorrectly(string password)
    {
        var hash = PasswordHelper.Hash(password);
        Assert.True(PasswordHelper.Verify(password, hash));
    }
}
