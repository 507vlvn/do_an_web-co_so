#!/usr/bin/env dotnet-script
// Script reset mat khau admin
// Chay: dotnet script reset_admin.csx

#r "nuget: BCrypt.Net-Next, 4.0.3"
#r "nuget: Microsoft.Data.SqlClient, 5.2.1"

using BCrypt.Net;
using Microsoft.Data.SqlClient;

var connectionString = "Server=(localdb)\\mssqllocaldb;Database=QuanLyQuayThuoc;Trusted_Connection=True;TrustServerCertificate=True;";
var newPassword = "Admin@123456";
var hash = BCrypt.Net.BCrypt.HashPassword(newPassword, workFactor: 12);

Console.WriteLine($"[INFO] Dang reset mat khau admin...");
Console.WriteLine($"[INFO] Mat khau moi: {newPassword}");

using var conn = new SqlConnection(connectionString);
conn.Open();

using var cmd = conn.CreateCommand();
cmd.CommandText = "UPDATE TaiKhoan SET MatKhauHash = @hash WHERE TenDangNhap = 'admin'";
cmd.Parameters.AddWithValue("@hash", hash);
var rows = cmd.ExecuteNonQuery();

if (rows > 0)
    Console.WriteLine($"[OK] Da reset thanh cong! Dang nhap: admin / {newPassword}");
else
    Console.WriteLine("[ERR] Khong tim thay tai khoan admin trong DB!");
