BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260329111349_MegaArchitectureUpgrade_FEFO_Cart'
)
BEGIN
    ALTER TABLE [SanPham] ADD [MaThuoc] nvarchar(20) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260329111349_MegaArchitectureUpgrade_FEFO_Cart'
)
BEGIN
    CREATE TABLE [GioHang] (
        [Id] int NOT NULL IDENTITY,
        [TenDangNhap] nvarchar(50) NOT NULL,
        [NgayTao] datetime2 NOT NULL,
        [CapNhatCuoi] datetime2 NOT NULL,
        CONSTRAINT [PK_GioHang] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_GioHang_TaiKhoan_TenDangNhap] FOREIGN KEY ([TenDangNhap]) REFERENCES [TaiKhoan] ([TenDangNhap]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260329111349_MegaArchitectureUpgrade_FEFO_Cart'
)
BEGIN
    CREATE TABLE [LoThuoc] (
        [Id] int NOT NULL IDENTITY,
        [MaLo] nvarchar(50) NOT NULL,
        [MaThuoc] nvarchar(20) NOT NULL,
        [SoLuongBanDau] int NOT NULL,
        [SoLuongKhaDung] int NOT NULL,
        [NgaySX] datetime2 NOT NULL,
        [HanSuDung] datetime2 NOT NULL,
        [TrangThai] bit NOT NULL,
        CONSTRAINT [PK_LoThuoc] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_LoThuoc_Thuoc_MaThuoc] FOREIGN KEY ([MaThuoc]) REFERENCES [Thuoc] ([MaThuoc]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260329111349_MegaArchitectureUpgrade_FEFO_Cart'
)
BEGIN
    CREATE TABLE [ChiTietGioHang] (
        [Id] int NOT NULL IDENTITY,
        [GioHangId] int NOT NULL,
        [SanPhamId] int NOT NULL,
        [SoLuong] int NOT NULL,
        CONSTRAINT [PK_ChiTietGioHang] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ChiTietGioHang_GioHang_GioHangId] FOREIGN KEY ([GioHangId]) REFERENCES [GioHang] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ChiTietGioHang_SanPham_SanPhamId] FOREIGN KEY ([SanPhamId]) REFERENCES [SanPham] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260329111349_MegaArchitectureUpgrade_FEFO_Cart'
)
BEGIN
    CREATE INDEX [IX_SanPham_MaThuoc] ON [SanPham] ([MaThuoc]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260329111349_MegaArchitectureUpgrade_FEFO_Cart'
)
BEGIN
    CREATE INDEX [IX_ChiTietGioHang_GioHangId] ON [ChiTietGioHang] ([GioHangId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260329111349_MegaArchitectureUpgrade_FEFO_Cart'
)
BEGIN
    CREATE INDEX [IX_ChiTietGioHang_SanPhamId] ON [ChiTietGioHang] ([SanPhamId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260329111349_MegaArchitectureUpgrade_FEFO_Cart'
)
BEGIN
    CREATE UNIQUE INDEX [IX_GioHang_TenDangNhap] ON [GioHang] ([TenDangNhap]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260329111349_MegaArchitectureUpgrade_FEFO_Cart'
)
BEGIN
    CREATE INDEX [IX_LoThuoc_MaThuoc] ON [LoThuoc] ([MaThuoc]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260329111349_MegaArchitectureUpgrade_FEFO_Cart'
)
BEGIN
    ALTER TABLE [SanPham] ADD CONSTRAINT [FK_SanPham_Thuoc_MaThuoc] FOREIGN KEY ([MaThuoc]) REFERENCES [Thuoc] ([MaThuoc]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260329111349_MegaArchitectureUpgrade_FEFO_Cart'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260329111349_MegaArchitectureUpgrade_FEFO_Cart', N'9.0.6');
END;

COMMIT;
GO

