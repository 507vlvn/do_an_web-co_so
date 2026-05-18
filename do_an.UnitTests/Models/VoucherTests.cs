using do_an.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace do_an.UnitTests.Models;

[TestClass]
public class VoucherTests
{
    [TestMethod]
    public void ConHieuLuc_AllConditionsMet_ReturnsTrue()
    {
        // Arrange
        var voucher = new Voucher
        {
            KichHoat = true,
            DaSuDung = 0,
            SoLanSuDung = 10,
            NgayBatDau = DateTime.Now.AddDays(-1),
            NgayHetHan = DateTime.Now.AddDays(1)
        };

        // Act
        var result = voucher.ConHieuLuc;

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void ConHieuLuc_KichHoatFalse_ReturnsFalse()
    {
        // Arrange
        var voucher = new Voucher
        {
            KichHoat = false,
            DaSuDung = 0,
            SoLanSuDung = 10,
            NgayBatDau = DateTime.Now.AddDays(-1),
            NgayHetHan = DateTime.Now.AddDays(1)
        };

        // Act
        var result = voucher.ConHieuLuc;

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void ConHieuLuc_DaSuDungEqualsSoLanSuDung_ReturnsFalse()
    {
        // Arrange
        var voucher = new Voucher
        {
            KichHoat = true,
            DaSuDung = 10,
            SoLanSuDung = 10,
            NgayBatDau = DateTime.Now.AddDays(-1),
            NgayHetHan = DateTime.Now.AddDays(1)
        };

        // Act
        var result = voucher.ConHieuLuc;

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void ConHieuLuc_DaSuDungGreaterThanSoLanSuDung_ReturnsFalse()
    {
        // Arrange
        var voucher = new Voucher
        {
            KichHoat = true,
            DaSuDung = 15,
            SoLanSuDung = 10,
            NgayBatDau = DateTime.Now.AddDays(-1),
            NgayHetHan = DateTime.Now.AddDays(1)
        };

        // Act
        var result = voucher.ConHieuLuc;

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void ConHieuLuc_BeforeNgayBatDau_ReturnsFalse()
    {
        // Arrange
        var voucher = new Voucher
        {
            KichHoat = true,
            DaSuDung = 0,
            SoLanSuDung = 10,
            NgayBatDau = DateTime.Now.AddDays(1),
            NgayHetHan = DateTime.Now.AddDays(10)
        };

        // Act
        var result = voucher.ConHieuLuc;

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void ConHieuLuc_AfterNgayHetHan_ReturnsFalse()
    {
        // Arrange
        var voucher = new Voucher
        {
            KichHoat = true,
            DaSuDung = 0,
            SoLanSuDung = 10,
            NgayBatDau = DateTime.Now.AddDays(-10),
            NgayHetHan = DateTime.Now.AddDays(-1)
        };

        // Act
        var result = voucher.ConHieuLuc;

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void ConHieuLuc_OnNgayBatDau_ReturnsTrue()
    {
        // Arrange
        var voucher = new Voucher
        {
            KichHoat = true,
            DaSuDung = 0,
            SoLanSuDung = 10,
            NgayBatDau = DateTime.Now.AddSeconds(-1),
            NgayHetHan = DateTime.Now.AddDays(1)
        };

        // Act
        var result = voucher.ConHieuLuc;

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void ConHieuLuc_OnNgayHetHan_ReturnsTrue()
    {
        // Arrange
        var voucher = new Voucher
        {
            KichHoat = true,
            DaSuDung = 0,
            SoLanSuDung = 10,
            NgayBatDau = DateTime.Now.AddDays(-1),
            NgayHetHan = DateTime.Now.AddSeconds(1)
        };

        // Act
        var result = voucher.ConHieuLuc;

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void ConHieuLuc_DaSuDungOneLessThanSoLanSuDung_ReturnsTrue()
    {
        // Arrange
        var voucher = new Voucher
        {
            KichHoat = true,
            DaSuDung = 9,
            SoLanSuDung = 10,
            NgayBatDau = DateTime.Now.AddDays(-1),
            NgayHetHan = DateTime.Now.AddDays(1)
        };

        // Act
        var result = voucher.ConHieuLuc;

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void ConHieuLuc_MultipleConditionsFalse_ReturnsFalse()
    {
        // Arrange
        var voucher = new Voucher
        {
            KichHoat = false,
            DaSuDung = 10,
            SoLanSuDung = 10,
            NgayBatDau = DateTime.Now.AddDays(1),
            NgayHetHan = DateTime.Now.AddDays(-1)
        };

        // Act
        var result = voucher.ConHieuLuc;

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void ConHieuLuc_DaSuDungZeroAndSoLanSuDungOne_ReturnsTrue()
    {
        // Arrange
        var voucher = new Voucher
        {
            KichHoat = true,
            DaSuDung = 0,
            SoLanSuDung = 1,
            NgayBatDau = DateTime.Now.AddDays(-1),
            NgayHetHan = DateTime.Now.AddDays(1)
        };

        // Act
        var result = voucher.ConHieuLuc;

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void ConHieuLuc_DaSuDungOneAndSoLanSuDungOne_ReturnsFalse()
    {
        // Arrange
        var voucher = new Voucher
        {
            KichHoat = true,
            DaSuDung = 1,
            SoLanSuDung = 1,
            NgayBatDau = DateTime.Now.AddDays(-1),
            NgayHetHan = DateTime.Now.AddDays(1)
        };

        // Act
        var result = voucher.ConHieuLuc;

        // Assert
        Assert.IsFalse(result);
    }
}
