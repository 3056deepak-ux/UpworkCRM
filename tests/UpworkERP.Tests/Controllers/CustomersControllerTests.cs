using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System.Net;
using UpworkERP.Web.Controllers;
using UpworkERP.Application.Services.Interfaces;
using UpworkERP.Core.Entities.CRM;
using UpworkERP.Core.Enums;

namespace UpworkERP.Tests.Controllers;

/// <summary>
/// Unit tests for CustomersController
/// </summary>
public class CustomersControllerTests
{
    private readonly Mock<IService<Customer>> _mockCustomerService;
    private readonly Mock<IActivityLogService> _mockActivityLogService;
    private readonly CustomersController _controller;

    public CustomersControllerTests()
    {
        _mockCustomerService = new Mock<IService<Customer>>();
        _mockActivityLogService = new Mock<IActivityLogService>();
        _controller = new CustomersController(_mockCustomerService.Object, _mockActivityLogService.Object);
        
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
    public async Task Index_ReturnsViewWithCustomers()
    {
        // Arrange
        var customers = new List<Customer>
        {
            new Customer { Id = 1, Name = "Customer 1", Email = "customer1@test.com" },
            new Customer { Id = 2, Name = "Customer 2", Email = "customer2@test.com" }
        };
        _mockCustomerService.Setup(s => s.GetAllAsync())
                           .ReturnsAsync(customers);

        // Act
        var result = await _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<Customer>>(viewResult.Model);
        Assert.Equal(2, model.Count());
        _mockCustomerService.Verify(s => s.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task Details_ReturnsViewWithCustomer_WhenCustomerExists()
    {
        // Arrange
        var customer = new Customer { Id = 1, Name = "Test Customer", Email = "test@test.com" };
        _mockCustomerService.Setup(s => s.GetByIdAsync(1))
                           .ReturnsAsync(customer);

        // Act
        var result = await _controller.Details(1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Customer>(viewResult.Model);
        Assert.Equal(1, model.Id);
        Assert.Equal("Test Customer", model.Name);
    }

    [Fact]
    public async Task Details_ReturnsNotFound_WhenCustomerDoesNotExist()
    {
        // Arrange
        _mockCustomerService.Setup(s => s.GetByIdAsync(999))
                           .ReturnsAsync((Customer?)null);

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
        var customer = new Customer
        {
            Name = "New Customer",
            Email = "new@test.com",
            PhoneNumber = "123456789",
            Company = "Test Company",
            Status = CustomerStatus.Active
        };

        _mockCustomerService.Setup(s => s.CreateAsync(It.IsAny<Customer>()))
                           .ReturnsAsync(customer);
        _mockActivityLogService.Setup(s => s.LogActivityAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 
            It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>(), 
            It.IsAny<string>()))
                              .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Create(customer);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        _mockCustomerService.Verify(s => s.CreateAsync(customer), Times.Once);
    }

    [Fact]
    public async Task Create_Post_ReturnsView_WhenModelStateIsInvalid()
    {
        // Arrange
        var customer = new Customer();
        _controller.ModelState.AddModelError("Name", "Required");

        // Act
        var result = await _controller.Create(customer);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(customer, viewResult.Model);
        _mockCustomerService.Verify(s => s.CreateAsync(It.IsAny<Customer>()), Times.Never);
    }

    [Fact]
    public async Task DeleteConfirmed_RedirectsToIndex()
    {
        // Arrange
        _mockCustomerService.Setup(s => s.DeleteAsync(1))
                           .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteConfirmed(1);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        _mockCustomerService.Verify(s => s.DeleteAsync(1), Times.Once);
    }
}
