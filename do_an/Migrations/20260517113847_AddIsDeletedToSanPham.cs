using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace do_an.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDeletedToSanPham : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "SanPham",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "SanPham");

            migrationBuilder.CreateTable(
                name: "BienBanTieuHuy",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SanPhamId = table.Column<int>(type: "int", nullable: false),
                    HanSuDung = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LyDo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    MaLo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NgayLap = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NguoiLap = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SoLuong = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BienBanTieuHuy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BienBanTieuHuy_SanPham_SanPhamId",
                        column: x => x.SanPhamId,
                        principalTable: "SanPham",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BienBanTieuHuy_SanPhamId",
                table: "BienBanTieuHuy",
                column: "SanPhamId");
        }
    }
}
