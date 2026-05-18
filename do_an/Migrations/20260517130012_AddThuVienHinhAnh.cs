using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace do_an.Migrations
{
    /// <inheritdoc />
    public partial class AddThuVienHinhAnh : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietDonHang_SanPham_SanPhamId",
                table: "ChiTietDonHang");

            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietDonThuoc_SanPham_SanPhamId",
                table: "ChiTietDonThuoc");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "SanPham");

            migrationBuilder.AddColumn<string>(
                name: "ThuVienHinhAnh",
                table: "SanPham",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SanPhamId",
                table: "ChiTietDonThuoc",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "TenSanPhamSnapshot",
                table: "ChiTietDonThuoc",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SanPhamId",
                table: "ChiTietDonHang",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "TenSanPhamSnapshot",
                table: "ChiTietDonHang",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietDonHang_SanPham_SanPhamId",
                table: "ChiTietDonHang",
                column: "SanPhamId",
                principalTable: "SanPham",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietDonThuoc_SanPham_SanPhamId",
                table: "ChiTietDonThuoc",
                column: "SanPhamId",
                principalTable: "SanPham",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietDonHang_SanPham_SanPhamId",
                table: "ChiTietDonHang");

            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietDonThuoc_SanPham_SanPhamId",
                table: "ChiTietDonThuoc");

            migrationBuilder.DropColumn(
                name: "ThuVienHinhAnh",
                table: "SanPham");

            migrationBuilder.DropColumn(
                name: "TenSanPhamSnapshot",
                table: "ChiTietDonThuoc");

            migrationBuilder.DropColumn(
                name: "TenSanPhamSnapshot",
                table: "ChiTietDonHang");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "SanPham",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "SanPhamId",
                table: "ChiTietDonThuoc",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SanPhamId",
                table: "ChiTietDonHang",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietDonHang_SanPham_SanPhamId",
                table: "ChiTietDonHang",
                column: "SanPhamId",
                principalTable: "SanPham",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietDonThuoc_SanPham_SanPhamId",
                table: "ChiTietDonThuoc",
                column: "SanPhamId",
                principalTable: "SanPham",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
