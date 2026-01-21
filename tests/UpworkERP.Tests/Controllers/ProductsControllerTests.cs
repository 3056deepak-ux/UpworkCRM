using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System.Net;
using UpworkERP.Web.Controllers;
using UpworkERP.Application.Services.Interfaces;
using UpworkERP.Core.Entities.Inventory;
using UpworkERP.Core.Enums;

namespace UpworkERP.Tests.Controllers;

/// <summary>
/// Unit tests for ProductsController
/// </summary>
public class ProductsControllerTests
{
    private readonly Mock<IService<Product>> _mockProductService;
    private readonly Mock<IActivityLogService> _mockActivityLogService;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        _mockProductService = new Mock<IService<Product>>();
        _mockActivityLogService = new Mock<IActivityLogService>();
        _controller = new ProductsController(_mockProductService.Object, _mockActivityLogService.Object);
        
        // Setup HttpContext with Connection for controller
        var httpContext = new DefaultHttpContext();
        var connection = new Mock<ConnectionInfo>();
        connection.Setup(c => c.RemoteIpAddress).Returns(IPAddress.Parse("127.0.0.1"));
        httpContext.Features.Set(connection.Object);
        
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
    }

    [Fact]
    public async Task Index_ReturnsViewWithProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Product 1", SKU = "SKU001", Category = "Electronics", UnitPrice = 99.99m },
            new Product { Id = 2, Name = "Product 2", SKU = "SKU002", Category = "Books", UnitPrice = 19.99m }
        };
        _mockProductService.Setup(s => s.GetAllAsync())
                           .ReturnsAsync(products);

        // Act
        var result = await _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<Product>>(viewResult.Model);
        Assert.Equal(2, model.Count());
        _mockProductService.Verify(s => s.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task Details_ReturnsViewWithProduct_WhenProductExists()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "Test Product", SKU = "SKU001", Category = "Electronics", UnitPrice = 99.99m };
        _mockProductService.Setup(s => s.GetByIdAsync(1))
                           .ReturnsAsync(product);

        // Act
        var result = await _controller.Details(1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Product>(viewResult.Model);
        Assert.Equal(1, model.Id);
        Assert.Equal("Test Product", model.Name);
        Assert.Equal("SKU001", model.SKU);
    }

    [Fact]
    public async Task Details_ReturnsNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        _mockProductService.Setup(s => s.GetByIdAsync(999))
                           .ReturnsAsync((Product?)null);

        // Act
        var result = await _controller.Details(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void Create_Get_ReturnsView()
    {
        // Act
        var result = _controller.Create();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Create_Post_RedirectsToIndex_WhenModelStateIsValid()
    {
        // Arrange
        var product = new Product
        {
            Name = "New Product",
            Description = "Test Description",
            Category = "Electronics",
            SKU = "SKU003",
            UnitPrice = 149.99m,
            QuantityInStock = 100,
            ReorderLevel = 10,
            Status = ProductStatus.Active
        };

        _mockProductService.Setup(s => s.CreateAsync(It.IsAny<Product>()))
                           .ReturnsAsync(product);
        _mockActivityLogService.Setup(s => s.LogActivityAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 
            It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>(), 
            It.IsAny<string>()))
                              .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Create(product);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        _mockProductService.Verify(s => s.CreateAsync(product), Times.Once);
    }

    [Fact]
    public async Task Create_Post_ReturnsView_WhenModelStateIsInvalid()
    {
        // Arrange
        var product = new Product();
        _controller.ModelState.AddModelError("Name", "Required");

        // Act
        var result = await _controller.Create(product);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(product, viewResult.Model);
        _mockProductService.Verify(s => s.CreateAsync(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task DeleteConfirmed_RedirectsToIndex()
    {
        // Arrange
        _mockProductService.Setup(s => s.DeleteAsync(1))
                           .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteConfirmed(1);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        _mockProductService.Verify(s => s.DeleteAsync(1), Times.Once);
    }
}
