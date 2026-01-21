using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System.Net;
using UpworkERP.Web.Controllers;
using UpworkERP.Application.Services.Interfaces;
using UpworkERP.Core.Entities.HR;
using UpworkERP.Core.Enums;

namespace UpworkERP.Tests.Controllers;

/// <summary>
/// Unit tests for EmployeesController
/// </summary>
public class EmployeesControllerTests
{
    private readonly Mock<IService<Employee>> _mockEmployeeService;
    private readonly Mock<IActivityLogService> _mockActivityLogService;
    private readonly EmployeesController _controller;

    public EmployeesControllerTests()
    {
        _mockEmployeeService = new Mock<IService<Employee>>();
        _mockActivityLogService = new Mock<IActivityLogService>();
        _controller = new EmployeesController(_mockEmployeeService.Object, _mockActivityLogService.Object);
        
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
    public async Task Index_ReturnsViewWithEmployees()
    {
        // Arrange
        var employees = new List<Employee>
        {
            new Employee { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@test.com", Department = "IT", Position = "Developer" },
            new Employee { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "jane.smith@test.com", Department = "HR", Position = "Manager" }
        };
        _mockEmployeeService.Setup(s => s.GetAllAsync())
                           .ReturnsAsync(employees);

        // Act
        var result = await _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<Employee>>(viewResult.Model);
        Assert.Equal(2, model.Count());
        _mockEmployeeService.Verify(s => s.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task Details_ReturnsViewWithEmployee_WhenEmployeeExists()
    {
        // Arrange
        var employee = new Employee { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@test.com", Department = "IT", Position = "Developer" };
        _mockEmployeeService.Setup(s => s.GetByIdAsync(1))
                           .ReturnsAsync(employee);

        // Act
        var result = await _controller.Details(1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Employee>(viewResult.Model);
        Assert.Equal(1, model.Id);
        Assert.Equal("John", model.FirstName);
        Assert.Equal("Doe", model.LastName);
    }

    [Fact]
    public async Task Details_ReturnsNotFound_WhenEmployeeDoesNotExist()
    {
        // Arrange
        _mockEmployeeService.Setup(s => s.GetByIdAsync(999))
                           .ReturnsAsync((Employee?)null);

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
        var employee = new Employee
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@test.com",
            PhoneNumber = "123456789",
            Department = "IT",
            Position = "Developer",
            Salary = 50000,
            HireDate = DateTime.Now,
            Status = EmployeeStatus.Active
        };

        _mockEmployeeService.Setup(s => s.CreateAsync(It.IsAny<Employee>()))
                           .ReturnsAsync(employee);
        _mockActivityLogService.Setup(s => s.LogActivityAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 
            It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>(), 
            It.IsAny<string>()))
                              .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Create(employee);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        _mockEmployeeService.Verify(s => s.CreateAsync(employee), Times.Once);
    }

    [Fact]
    public async Task Create_Post_ReturnsView_WhenModelStateIsInvalid()
    {
        // Arrange
        var employee = new Employee();
        _controller.ModelState.AddModelError("FirstName", "Required");

        // Act
        var result = await _controller.Create(employee);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(employee, viewResult.Model);
        _mockEmployeeService.Verify(s => s.CreateAsync(It.IsAny<Employee>()), Times.Never);
    }

    [Fact]
    public async Task DeleteConfirmed_RedirectsToIndex()
    {
        // Arrange
        _mockEmployeeService.Setup(s => s.DeleteAsync(1))
                           .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteConfirmed(1);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        _mockEmployeeService.Verify(s => s.DeleteAsync(1), Times.Once);
    }
}
