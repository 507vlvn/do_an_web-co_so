using do_an.Migrations;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System.Reflection;

namespace do_an.UnitTests.Migrations;

[TestClass]
public class InitialCreateTests
{
    private InitialCreate _migration = null!;

    [TestInitialize]
    public void Setup()
    {
        _migration = new InitialCreate();
    }

    [TestMethod]
    public void Up_CallsMigrationBuilder_ExecutesSuccessfully()
    {
        // Arrange
        var migration = new InitialCreate();
        var migrationBuilder = new MigrationBuilder("SqlServer");

        // Act
        InvokeProtectedMethod(migration, "Up", migrationBuilder);

        // Assert
        Assert.IsNotNull(migrationBuilder.Operations);
        Assert.IsNotEmpty(migrationBuilder.Operations);
    }

    [TestMethod]
    public void Up_CreatesTables_CreatesExpectedNumberOfTables()
    {
        // Arrange
        var migrationBuilder = new MigrationBuilder("SqlServer");

        // Act
        InvokeProtectedMethod(_migration, "Up", migrationBuilder);

        // Assert
        var createTableOps = migrationBuilder.Operations.OfType<CreateTableOperation>().ToList();
        Assert.HasCount(11, createTableOps);
    }

    [TestMethod]
    public void Up_CreatesTables_CreatesDanhMucSanPhamTable()
    {
        // Arrange
        var migrationBuilder = new MigrationBuilder("SqlServer");

        // Act
        InvokeProtectedMethod(_migration, "Up", migrationBuilder);

        // Assert
        var createTableOps = migrationBuilder.Operations.OfType<CreateTableOperation>().ToList();
        var danhMucTable = createTableOps.FirstOrDefault(t => t.Name == "DanhMucSanPham");
        Assert.IsNotNull(danhMucTable);
        Assert.HasCount(5, danhMucTable.Columns);
    }

    [TestMethod]
    public void Up_CreatesTables_CreatesDonThuocTable()
    {
        // Arrange
        var migrationBuilder = new MigrationBuilder("SqlServer");

        // Act
        InvokeProtectedMethod(_migration, "Up", migrationBuilder);

        // Assert
        var createTableOps = migrationBuilder.Operations.OfType<CreateTableOperation>().ToList();
        var donThuocTable = createTableOps.FirstOrDefault(t => t.Name == "DonThuoc");
        Assert.IsNotNull(donThuocTable);
        Assert.HasCount(7, donThuocTable.Columns);
    }

    [TestMethod]
    public void Up_CreatesTables_CreatesTaiKhoanTable()
    {
        // Arrange
        var migrationBuilder = new MigrationBuilder("SqlServer");

        // Act
        InvokeProtectedMethod(_migration, "Up", migrationBuilder);

        // Assert
        var createTableOps = migrationBuilder.Operations.OfType<CreateTableOperation>().ToList();
        var taiKhoanTable = createTableOps.FirstOrDefault(t => t.Name == "TaiKhoan");
        Assert.IsNotNull(taiKhoanTable);
        Assert.HasCount(10, taiKhoanTable.Columns);
    }

    [TestMethod]
    public void Up_CreatesTables_CreatesVoucherTable()
    {
        // Arrange
        var migrationBuilder = new MigrationBuilder("SqlServer");

        // Act
        InvokeProtectedMethod(_migration, "Up", migrationBuilder);

        // Assert
        var createTableOps = migrationBuilder.Operations.OfType<CreateTableOperation>().ToList();
        var voucherTable = createTableOps.FirstOrDefault(t => t.Name == "Voucher");
        Assert.IsNotNull(voucherTable);
        Assert.HasCount(11, voucherTable.Columns);
    }

    [TestMethod]
    public void Up_CreatesTables_CreatesSanPhamTable()
    {
        // Arrange
        var migrationBuilder = new MigrationBuilder("SqlServer");

        // Act
        InvokeProtectedMethod(_migration, "Up", migrationBuilder);

        // Assert
        var createTableOps = migrationBuilder.Operations.OfType<CreateTableOperation>().ToList();
        var sanPhamTable = createTableOps.FirstOrDefault(t => t.Name == "SanPham");
        Assert.IsNotNull(sanPhamTable);
        Assert.HasCount(21, sanPhamTable.Columns);
    }

    [TestMethod]
    public void Up_CreatesTables_CreatesDonHangTable()
    {
        // Arrange
        var migrationBuilder = new MigrationBuilder("SqlServer");

        // Act
        InvokeProtectedMethod(_migration, "Up", migrationBuilder);

        // Assert
        var createTableOps = migrationBuilder.Operations.OfType<CreateTableOperation>().ToList();
        var donHangTable = createTableOps.FirstOrDefault(t => t.Name == "DonHang");
        Assert.IsNotNull(donHangTable);
        Assert.HasCount(21, donHangTable.Columns);
    }

    [TestMethod]
    public void Up_CreatesTables_CreatesGioHangTable()
    {
        // Arrange
        var migrationBuilder = new MigrationBuilder("SqlServer");

        // Act
        InvokeProtectedMethod(_migration, "Up", migrationBuilder);

        // Assert
        var createTableOps = migrationBuilder.Operations.OfType<CreateTableOperation>().ToList();
        var gioHangTable = createTableOps.FirstOrDefault(t => t.Name == "GioHang");
        Assert.IsNotNull(gioHangTable);
        Assert.HasCount(4, gioHangTable.Columns);
    }

    [TestMethod]
    public void Up_CreatesTables_CreatesChiTietDonThuocTable()
    {
        // Arrange
        var migrationBuilder = new MigrationBuilder("SqlServer");

        // Act
        InvokeProtectedMethod(_migration, "Up", migrationBuilder);

        // Assert
        var createTableOps = migrationBuilder.Operations.OfType<CreateTableOperation>().ToList();
        var chiTietDonThuocTable = createTableOps.FirstOrDefault(t => t.Name == "ChiTietDonThuoc");
        Assert.IsNotNull(chiTietDonThuocTable);
        Assert.HasCount(8, chiTietDonThuocTable.Columns);
    }

    [TestMethod]
    public void Up_CreatesTables_CreatesLoThuocTable()
    {
        // Arrange
        var migrationBuilder = new MigrationBuilder("SqlServer");

        // Act
        InvokeProtectedMethod(_migration, "Up", migrationBuilder);

        // Assert
        var createTableOps = migrationBuilder.Operations.OfType<CreateTableOperation>().ToList();
        var loThuocTable = createTableOps.FirstOrDefault(t => t.Name == "LoThuoc");
        Assert.IsNotNull(loThuocTable);
        Assert.HasCount(8, loThuocTable.Columns);
    }

    [TestMethod]
    public void Up_CreatesTables_CreatesChiTietDonHangTable()
    {
        // Arrange
        var migrationBuilder = new MigrationBuilder("SqlServer");

        // Act
        InvokeProtectedMethod(_migration, "Up", migrationBuilder);

        // Assert
        var createTableOps = migrationBuilder.Operations.OfType<CreateTableOperation>().ToList();
        var chiTietDonHangTable = createTableOps.FirstOrDefault(t => t.Name == "ChiTietDonHang");
        Assert.IsNotNull(chiTietDonHangTable);
        Assert.HasCount(10, chiTietDonHangTable.Columns);
    }

    [TestMethod]
    public void Up_CreatesTables_CreatesChiTietGioHangTable()
    {
        // Arrange
        var migrationBuilder = new MigrationBuilder("SqlServer");

        // Act
        InvokeProtectedMethod(_migration, "Up", migrationBuilder);

        // Assert
        var createTableOps = migrationBuilder.Operations.OfType<CreateTableOperation>().ToList();
        var chiTietGioHangTable = createTableOps.FirstOrDefault(t => t.Name == "ChiTietGioHang");
        Assert.IsNotNull(chiTietGioHangTable);
        Assert.HasCount(4, chiTietGioHangTable.Columns);
    }

    [TestMethod]
    public void Up_InsertsSeedData_InsertsDanhMucSanPhamData()
    {
        // Arrange
        var migrationBuilder = new MigrationBuilder("SqlServer");

        // Act
        InvokeProtectedMethod(_migration, "Up", migrationBuilder);

        // Assert
        var insertOps = migrationBuilder.Operations.OfType<InsertDataOperation>().ToList();
        Assert.HasCount(1, insertOps);
        var insertOp = insertOps[0];
        Assert.AreEqual("DanhMucSanPham", insertOp.Table);
        Assert.AreEqual(7, insertOp.Values.GetLength(0));
    }

    [TestMethod]
    public void Up_CreatesIndexes_CreatesExpectedNumberOfIndexes()
    {
        // Arrange
        var migrationBuilder = new MigrationBuilder("SqlServer");

        // Act
        InvokeProtectedMethod(_migration, "Up", migrationBuilder);

        // Assert
        var createIndexOps = migrationBuilder.Operations.OfType<CreateIndexOperation>().ToList();
        Assert.HasCount(11, createIndexOps);
    }

    [TestMethod]
    public void Up_CreatesIndexes_CreatesChiTietDonHangIndexes()
    {
        // Arrange
        var migrationBuilder = new MigrationBuilder("SqlServer");

        // Act
        InvokeProtectedMethod(_migration, "Up", migrationBuilder);

        // Assert
        var createIndexOps = migrationBuilder.Operations.OfType<CreateIndexOperation>().ToList();
        var chiTietDonHangIndexes = createIndexOps.Where(i => i.Table == "ChiTietDonHang").ToList();
        Assert.HasCount(2, chiTietDonHangIndexes);
    }

    [TestMethod]
    public void Up_CreatesIndexes_CreatesChiTietDonThuocIndexes()
    {
        // Arrange
        var migrationBuilder = new MigrationBuilder("SqlServer");

        // Act
        InvokeProtectedMethod(_migration, "Up", migrationBuilder);

        // Assert
        var createIndexOps = migrationBuilder.Operations.OfType<CreateIndexOperation>().ToList();
        var chiTietDonThuocIndexes = createIndexOps.Where(i => i.Table == "ChiTietDonThuoc").ToList();
        Assert.HasCount(2, chiTietDonThuocIndexes);
    }

    [TestMethod]
    public void Up_CreatesIndexes_CreatesChiTietGioHangIndexes()
    {
        // Arrange
        var migrationBuilder = new MigrationBuilder("SqlServer");

        // Act
        InvokeProtectedMethod(_migration, "Up", migrationBuilder);

        // Assert
        var createIndexOps = migrationBuilder.Operations.OfType<CreateIndexOperation>().ToList();
        var chiTietGioHangIndexes = createIndexOps.Where(i => i.Table == "ChiTietGioHang").ToList();
        Assert.HasCount(2, chiTietGioHangIndexes);
    }

    [TestMethod]
    public void Up_CreatesIndexes_CreatesDonHangIndexes()
    {
        // Arrange
        var migrationBuilder = new MigrationBuilder("SqlServer");

        // Act
        InvokeProtectedMethod(_migration, "Up", migrationBuilder);

        // Assert
        var createIndexOps = migrationBuilder.Operations.OfType<CreateIndexOperation>().ToList();
        var donHangIndexes = createIndexOps.Where(i => i.Table == "DonHang").ToList();
        Assert.HasCount(2, donHangIndexes);
    }

    [TestMethod]
    public void Up_CreatesIndexes_CreatesGioHangIndex()
    {
        // Arrange
        var migrationBuilder = new MigrationBuilder("SqlServer");

        // Act
        InvokeProtectedMethod(_migration, "Up", migrationBuilder);

        // Assert
        var createIndexOps = migrationBuilder.Operations.OfType<CreateIndexOperation>().ToList();
        var gioHangIndex = createIndexOps.FirstOrDefault(i => i.Table == "GioHang");
        Assert.IsNotNull(gioHangIndex);
        Assert.IsTrue(gioHangIndex.IsUnique);
    }

    [TestMethod]
    public void Up_CreatesIndexes_CreatesLoThuocIndex()
    {
        // Arrange
        var migrationBuilder = new MigrationBuilder("SqlServer");

        // Act
        InvokeProtectedMethod(_migration, "Up", migrationBuilder);

        // Assert
        var createIndexOps = migrationBuilder.Operations.OfType<CreateIndexOperation>().ToList();
        var loThuocIndex = createIndexOps.FirstOrDefault(i => i.Table == "LoThuoc");
        Assert.IsNotNull(loThuocIndex);
    }

    [TestMethod]
    public void Up_CreatesIndexes_CreatesSanPhamIndex()
    {
        // Arrange
        var migrationBuilder = new MigrationBuilder("SqlServer");

        // Act
        InvokeProtectedMethod(_migration, "Up", migrationBuilder);

        // Assert
        var createIndexOps = migrationBuilder.Operations.OfType<CreateIndexOperation>().ToList();
        var sanPhamIndex = createIndexOps.FirstOrDefault(i => i.Table == "SanPham");
        Assert.IsNotNull(sanPhamIndex);
    }

    [TestMethod]
    public void Down_CallsMigrationBuilder_ExecutesSuccessfully()
    {
        // Arrange
        var migrationBuilder = new MigrationBuilder("SqlServer");

        // Act
        InvokeProtectedMethod(_migration, "Down", migrationBuilder);

        // Assert
        Assert.IsNotNull(migrationBuilder.Operations);
        Assert.IsNotEmpty(migrationBuilder.Operations);
    }

    [TestMethod]
    public void Down_DropsTables_DropsExpectedNumberOfTables()
    {
        // Arrange
        var migrationBuilder = new MigrationBuilder("SqlServer");

        // Act
        InvokeProtectedMethod(_migration, "Down", migrationBuilder);

        // Assert
        var dropTableOps = migrationBuilder.Operations.OfType<DropTableOperation>().ToList();
        Assert.HasCount(11, dropTableOps);
    }

    [TestMethod]
    public void Down_DropsTables_DropsChiTietDonHangTable()
    {
        // Arrange
        var migrationBuilder = new MigrationBuilder("SqlServer");

        // Act
        InvokeProtectedMethod(_migration, "Down", migrationBuilder);

        // Assert
        var dropTableOps = migrationBuilder.Operations.OfType<DropTableOperation>().ToList();
        var droppedTable = dropTableOps.FirstOrDefault(t => t.Name == "ChiTietDonHang");
        Assert.IsNotNull(droppedTable);
    }

    [TestMethod]
    public void Down_DropsTables_DropsChiTietDonThuocTable()
    {
        // Arrange
        var migrationBuilder = new MigrationBuilder("SqlServer");

        // Act
        InvokeProtectedMethod(_migration, "Down", migrationBuilder);

        // Assert
        var dropTableOps = migrationBuilder.Operations.OfType<DropTableOperation>().ToList();
        var droppedTable = dropTableOps.FirstOrDefault(t => t.Name == "ChiTietDonThuoc");
        Assert.IsNotNull(droppedTable);
    }

    [TestMethod]
    public void Down_DropsTables_DropsChiTietGioHangTable()
    {
        // Arrange
        var migrationBuilder = new MigrationBuilder("SqlServer");

        // Act
        InvokeProtectedMethod(_migration, "Down", migrationBuilder);

        // Assert
        var dropTableOps = migrationBuilder.Operations.OfType<DropTableOperation>().ToList();
        var droppedTable = dropTableOps.FirstOrDefault(t => t.Name == "ChiTietGioHang");
        Assert.IsNotNull(droppedTable);
    }

    [TestMethod]
    public void Down_DropsTables_DropsLoThuocTable()
    {
        // Arrange
        var migrationBuilder = new MigrationBuilder("SqlServer");

        // Act
        InvokeProtectedMethod(_migration, "Down", migrationBuilder);

        // Assert
        var dropTableOps = migrationBuilder.Operations.OfType<DropTableOperation>().ToList();
        var droppedTable = dropTableOps.FirstOrDefault(t => t.Name == "LoThuoc");
        Assert.IsNotNull(droppedTable);
    }

    [TestMethod]
    public void Down_DropsTables_DropsVoucherTable()
    {
        // Arrange
        var migrationBuilder = new MigrationBuilder("SqlServer");

        // Act
        InvokeProtectedMethod(_migration, "Down", migrationBuilder);

        // Assert
        var dropTableOps = migrationBuilder.Operations.OfType<DropTableOperation>().ToList();
        var droppedTable = dropTableOps.FirstOrDefault(t => t.Name == "Voucher");
        Assert.IsNotNull(droppedTable);
    }

    [TestMethod]
    public void Down_DropsTables_DropsDonHangTable()
    {
        // Arrange
        var migrationBuilder = new MigrationBuilder("SqlServer");

        // Act
        InvokeProtectedMethod(_migration, "Down", migrationBuilder);

        // Assert
        var dropTableOps = migrationBuilder.Operations.OfType<DropTableOperation>().ToList();
        var droppedTable = dropTableOps.FirstOrDefault(t => t.Name == "DonHang");
        Assert.IsNotNull(droppedTable);
    }

    [TestMethod]
    public void Down_DropsTables_DropsDonThuocTable()
    {
        // Arrange
        var migrationBuilder = new MigrationBuilder("SqlServer");

        // Act
        InvokeProtectedMethod(_migration, "Down", migrationBuilder);

        // Assert
        var dropTableOps = migrationBuilder.Operations.OfType<DropTableOperation>().ToList();
        var droppedTable = dropTableOps.FirstOrDefault(t => t.Name == "DonThuoc");
        Assert.IsNotNull(droppedTable);
    }

    [TestMethod]
    public void Down_DropsTables_DropsGioHangTable()
    {
        // Arrange
        var migrationBuilder = new MigrationBuilder("SqlServer");

        // Act
        InvokeProtectedMethod(_migration, "Down", migrationBuilder);

        // Assert
        var dropTableOps = migrationBuilder.Operations.OfType<DropTableOperation>().ToList();
        var droppedTable = dropTableOps.FirstOrDefault(t => t.Name == "GioHang");
        Assert.IsNotNull(droppedTable);
    }

    [TestMethod]
    public void Down_DropsTables_DropsSanPhamTable()
    {
        // Arrange
        var migrationBuilder = new MigrationBuilder("SqlServer");

        // Act
        InvokeProtectedMethod(_migration, "Down", migrationBuilder);

        // Assert
        var dropTableOps = migrationBuilder.Operations.OfType<DropTableOperation>().ToList();
        var droppedTable = dropTableOps.FirstOrDefault(t => t.Name == "SanPham");
        Assert.IsNotNull(droppedTable);
    }

    [TestMethod]
    public void Down_DropsTables_DropsTaiKhoanTable()
    {
        // Arrange
        var migrationBuilder = new MigrationBuilder("SqlServer");

        // Act
        InvokeProtectedMethod(_migration, "Down", migrationBuilder);

        // Assert
        var dropTableOps = migrationBuilder.Operations.OfType<DropTableOperation>().ToList();
        var droppedTable = dropTableOps.FirstOrDefault(t => t.Name == "TaiKhoan");
        Assert.IsNotNull(droppedTable);
    }

    [TestMethod]
    public void Down_DropsTables_DropsDanhMucSanPhamTable()
    {
        // Arrange
        var migrationBuilder = new MigrationBuilder("SqlServer");

        // Act
        InvokeProtectedMethod(_migration, "Down", migrationBuilder);

        // Assert
        var dropTableOps = migrationBuilder.Operations.OfType<DropTableOperation>().ToList();
        var droppedTable = dropTableOps.FirstOrDefault(t => t.Name == "DanhMucSanPham");
        Assert.IsNotNull(droppedTable);
    }

    [TestMethod]
    public void Up_ChiTietGioHangTable_HasForeignKeyToSanPham()
    {
        // Arrange
        var migrationBuilder = new MigrationBuilder("SqlServer");

        // Act
        InvokeProtectedMethod(_migration, "Up", migrationBuilder);

        // Assert
        var createTableOps = migrationBuilder.Operations.OfType<CreateTableOperation>().ToList();
        var chiTietGioHangTable = createTableOps.FirstOrDefault(t => t.Name == "ChiTietGioHang");
        Assert.IsNotNull(chiTietGioHangTable);
        var foreignKey = chiTietGioHangTable.ForeignKeys.FirstOrDefault(fk => fk.PrincipalTable == "SanPham");
        Assert.IsNotNull(foreignKey);
    }

    private static void InvokeProtectedMethod(object obj, string methodName, params object[] parameters)
    {
        var method = obj.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(method, $"Method {methodName} not found");
        method.Invoke(obj, parameters);
    }
}
