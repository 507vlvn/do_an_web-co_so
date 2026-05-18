using do_an.Data;
using do_an.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.Reflection;

namespace do_an.UnitTests;

[TestClass]
public class DonHangAutoHuyServiceTests
{
    [TestMethod]
    public void Constructor_WithValidParameters_SetsFields()
    {
        // Arrange
        var mockScopeFactory = new Mock<IServiceScopeFactory>();
        var mockLogger = new Mock<ILogger<DonHangAutoHuyService>>();

        // Act
        var service = new DonHangAutoHuyService(mockScopeFactory.Object, mockLogger.Object);

        // Assert
        Assert.IsNotNull(service);
    }

    [TestMethod]
    public async Task ExecuteAsync_WithImmediateCancellation_ExitsLoop()
    {
        // Arrange
        var mockScopeFactory = new Mock<IServiceScopeFactory>();
        var mockLogger = new Mock<ILogger<DonHangAutoHuyService>>();
        var service = new DonHangAutoHuyService(mockScopeFactory.Object, mockLogger.Object);
        var cts = new CancellationTokenSource();
        cts.Cancel(); // Cancel immediately

        // Act
        var executeAsyncMethod = typeof(DonHangAutoHuyService).GetMethod("ExecuteAsync", BindingFlags.NonPublic | BindingFlags.Instance);
        var task = (Task?)executeAsyncMethod?.Invoke(service, new object[] { cts.Token });
        await task!;

        // Assert
        Assert.IsTrue(task.IsCompleted);
    }

    [TestMethod]
    public async Task ExecuteAsync_WithCancellationDuringDelay_ExitsLoop()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var mockScope = new Mock<IServiceScope>();
        var mockServiceProvider = new Mock<IServiceProvider>();
        var mockScopeFactory = new Mock<IServiceScopeFactory>();
        var mockLogger = new Mock<ILogger<DonHangAutoHuyService>>();

        using var context = new AppDbContext(options);
        mockServiceProvider.Setup(sp => sp.GetService(typeof(AppDbContext))).Returns(context);
        mockScope.Setup(s => s.ServiceProvider).Returns(mockServiceProvider.Object);
        mockScopeFactory.Setup(f => f.CreateScope()).Returns(mockScope.Object);

        var service = new DonHangAutoHuyService(mockScopeFactory.Object, mockLogger.Object);
        var cts = new CancellationTokenSource();

        // Act
        var executeAsyncMethod = typeof(DonHangAutoHuyService).GetMethod("ExecuteAsync", BindingFlags.NonPublic | BindingFlags.Instance);
        var task = (Task?)executeAsyncMethod?.Invoke(service, new object[] { cts.Token });

        // Cancel after a short delay to let it start
        await Task.Delay(100);
        cts.Cancel();

        await task!;

        // Assert
        Assert.IsTrue(task.IsCompleted);
    }

    [TestMethod]
    public async Task ExecuteAsync_WithExceptionInPrivateMethod_LogsErrorAndContinues()
    {
        // Arrange
        var mockScope = new Mock<IServiceScope>();
        var mockServiceProvider = new Mock<IServiceProvider>();
        var mockScopeFactory = new Mock<IServiceScopeFactory>();
        var mockLogger = new Mock<ILogger<DonHangAutoHuyService>>();

        // Setup to throw exception when GetService is called
        mockServiceProvider.Setup(sp => sp.GetService(typeof(AppDbContext))).Throws(new InvalidOperationException("Test exception"));
        mockScope.Setup(s => s.ServiceProvider).Returns(mockServiceProvider.Object);
        mockScopeFactory.Setup(f => f.CreateScope()).Returns(mockScope.Object);

        var service = new DonHangAutoHuyService(mockScopeFactory.Object, mockLogger.Object);
        var cts = new CancellationTokenSource();

        // Act
        var executeAsyncMethod = typeof(DonHangAutoHuyService).GetMethod("ExecuteAsync", BindingFlags.NonPublic | BindingFlags.Instance);
        var task = (Task?)executeAsyncMethod?.Invoke(service, new object[] { cts.Token });

        // Let it run once and hit the exception
        await Task.Delay(100);
        cts.Cancel();

        await task!;

        // Assert
        Assert.IsTrue(task.IsCompleted);
        mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Lỗi khi chạy background service.")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }

    [TestMethod]
    public async Task ExecuteAsync_WithOperationCanceledExceptionAndRequestedCancellation_BreaksLoop()
    {
        // Arrange
        var mockScope = new Mock<IServiceScope>();
        var mockServiceProvider = new Mock<IServiceProvider>();
        var mockScopeFactory = new Mock<IServiceScopeFactory>();
        var mockLogger = new Mock<ILogger<DonHangAutoHuyService>>();

        // Setup scope to throw OperationCanceledException
        var cts = new CancellationTokenSource();
        mockServiceProvider.Setup(sp => sp.GetService(typeof(AppDbContext)))
            .Throws(new OperationCanceledException(cts.Token));
        mockScope.Setup(s => s.ServiceProvider).Returns(mockServiceProvider.Object);
        mockScopeFactory.Setup(f => f.CreateScope()).Returns(mockScope.Object);

        var service = new DonHangAutoHuyService(mockScopeFactory.Object, mockLogger.Object);

        // Act
        var executeAsyncMethod = typeof(DonHangAutoHuyService).GetMethod("ExecuteAsync", BindingFlags.NonPublic | BindingFlags.Instance);
        var task = (Task?)executeAsyncMethod?.Invoke(service, new object[] { cts.Token });

        // Cancel immediately so the when clause is true
        cts.Cancel();
        await Task.Delay(50); // Give it time to process

        await task!;

        // Assert
        Assert.IsTrue(task.IsCompleted);
    }
}
