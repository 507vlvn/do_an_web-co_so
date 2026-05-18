using do_an.Controllers;
using do_an.Data;
using do_an.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;

namespace do_an.UnitTests;

[TestClass]
public class SanPhamControllerTests
{
    private DbContextOptions<AppDbContext> CreateInMemoryOptions()
    {
        return new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    private SanPhamController CreateControllerWithTempData(AppDbContext context, IWebHostEnvironment env)
    {
        var controller = new SanPhamController(context, env);
        var httpContext = new DefaultHttpContext();
        var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        controller.TempData = tempData;
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
        return controller;
    }

    [TestMethod]
    public void Constructor_WithValidParameters_SetsFields()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new AppDbContext(options);
        var env = Mock.Of<IWebHostEnvironment>();

        // Act
        var controller = new SanPhamController(context, env);

        // Assert
        Assert.IsNotNull(controller);
    }
}
