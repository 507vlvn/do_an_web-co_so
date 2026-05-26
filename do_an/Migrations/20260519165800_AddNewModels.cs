using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace do_an.Migrations
{
    /// <inheritdoc />
    public partial class AddNewModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLog",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDangNhap = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    HanhDong = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ChiTiet = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ThoiGian = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EntityName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EntityId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DanhGia",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SanPhamId = table.Column<int>(type: "int", nullable: false),
                    TenDangNhap = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SoSao = table.Column<int>(type: "int", nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    NgayDanhGia = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DaDuyet = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhGia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DanhGia_SanPham_SanPhamId",
                        column: x => x.SanPhamId,
                        principalTable: "SanPham",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DanhGia_TaiKhoan_TenDangNhap",
                        column: x => x.TenDangNhap,
                        principalTable: "TaiKhoan",
                        principalColumn: "TenDangNhap",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NhaCungCap",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenNCC = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    SoDienThoai = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NguoiLienHe = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    KichHoat = table.Column<bool>(type: "bit", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhaCungCap", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PhieuNhapHang",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaPhieu = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    NhaCungCapId = table.Column<int>(type: "int", nullable: false),
                    NhanVienNhapId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NgayNhap = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TongTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TrangThai = table.Column<int>(type: "int", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuNhapHang", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhieuNhapHang_NhaCungCap_NhaCungCapId",
                        column: x => x.NhaCungCapId,
                        principalTable: "NhaCungCap",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhieuNhapHang_TaiKhoan_NhanVienNhapId",
                        column: x => x.NhanVienNhapId,
                        principalTable: "TaiKhoan",
                        principalColumn: "TenDangNhap",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietPhieuNhap",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhieuNhapHangId = table.Column<int>(type: "int", nullable: false),
                    SanPhamId = table.Column<int>(type: "int", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    GiaNhap = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaLo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NgaySX = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HanSuDung = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietPhieuNhap", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChiTietPhieuNhap_PhieuNhapHang_PhieuNhapHangId",
                        column: x => x.PhieuNhapHangId,
                        principalTable: "PhieuNhapHang",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChiTietPhieuNhap_SanPham_SanPhamId",
                        column: x => x.SanPhamId,
                        principalTable: "SanPham",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietPhieuNhap_PhieuNhapHangId",
                table: "ChiTietPhieuNhap",
                column: "PhieuNhapHangId");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietPhieuNhap_SanPhamId",
                table: "ChiTietPhieuNhap",
                column: "SanPhamId");

            migrationBuilder.CreateIndex(
                name: "IX_DanhGia_SanPhamId",
                table: "DanhGia",
                column: "SanPhamId");

            migrationBuilder.CreateIndex(
                name: "IX_DanhGia_TenDangNhap",
                table: "DanhGia",
                column: "TenDangNhap");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuNhapHang_NhaCungCapId",
                table: "PhieuNhapHang",
                column: "NhaCungCapId");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuNhapHang_NhanVienNhapId",
                table: "PhieuNhapHang",
                column: "NhanVienNhapId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLog");

            migrationBuilder.DropTable(
                name: "ChiTietPhieuNhap");

            migrationBuilder.DropTable(
                name: "DanhGia");

            migrationBuilder.DropTable(
                name: "PhieuNhapHang");

            migrationBuilder.DropTable(
                name: "NhaCungCap");
        }
    }
}
