using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System.Net;
using UpworkERP.Web.Controllers;
using UpworkERP.Application.Services.Interfaces;
using UpworkERP.Core.Entities.Finance;
using UpworkERP.Core.Enums;

namespace UpworkERP.Tests.Controllers;

/// <summary>
/// Unit tests for AccountsController
/// </summary>
public class AccountsControllerTests
{
    private readonly Mock<IService<Account>> _mockAccountService;
    private readonly Mock<IActivityLogService> _mockActivityLogService;
    private readonly AccountsController _controller;

    public AccountsControllerTests()
    {
        _mockAccountService = new Mock<IService<Account>>();
        _mockActivityLogService = new Mock<IActivityLogService>();
        _controller = new AccountsController(_mockAccountService.Object, _mockActivityLogService.Object);
        
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
    public async Task Index_ReturnsViewWithAccounts()
    {
        // Arrange
        var accounts = new List<Account>
        {
            new Account { Id = 1, AccountName = "Account 1", AccountNumber = "ACC001", Currency = "USD", Balance = 10000, Type = AccountType.Asset },
            new Account { Id = 2, AccountName = "Account 2", AccountNumber = "ACC002", Currency = "EUR", Balance = 5000, Type = AccountType.Liability }
        };
        _mockAccountService.Setup(s => s.GetAllAsync())
                           .ReturnsAsync(accounts);

        // Act
        var result = await _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<Account>>(viewResult.Model);
        Assert.Equal(2, model.Count());
        _mockAccountService.Verify(s => s.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task Details_ReturnsViewWithAccount_WhenAccountExists()
    {
        // Arrange
        var account = new Account { Id = 1, AccountName = "Test Account", AccountNumber = "ACC001", Currency = "USD", Balance = 15000, Type = AccountType.Asset };
        _mockAccountService.Setup(s => s.GetByIdAsync(1))
                           .ReturnsAsync(account);

        // Act
        var result = await _controller.Details(1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Account>(viewResult.Model);
        Assert.Equal(1, model.Id);
        Assert.Equal("Test Account", model.AccountName);
        Assert.Equal("ACC001", model.AccountNumber);
    }

    [Fact]
    public async Task Details_ReturnsNotFound_WhenAccountDoesNotExist()
    {
        // Arrange
        _mockAccountService.Setup(s => s.GetByIdAsync(999))
                           .ReturnsAsync((Account?)null);

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
        var account = new Account
        {
            AccountName = "New Account",
            AccountNumber = "ACC003",
            Currency = "USD",
            Balance = 20000,
            Type = AccountType.Asset
        };

        _mockAccountService.Setup(s => s.CreateAsync(It.IsAny<Account>()))
                           .ReturnsAsync(account);
        _mockActivityLogService.Setup(s => s.LogActivityAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 
            It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>(), 
            It.IsAny<string>()))
                              .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Create(account);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        _mockAccountService.Verify(s => s.CreateAsync(account), Times.Once);
    }

    [Fact]
    public async Task Create_Post_ReturnsView_WhenModelStateIsInvalid()
    {
        // Arrange
        var account = new Account();
        _controller.ModelState.AddModelError("AccountName", "Required");

        // Act
        var result = await _controller.Create(account);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(account, viewResult.Model);
        _mockAccountService.Verify(s => s.CreateAsync(It.IsAny<Account>()), Times.Never);
    }

    [Fact]
    public async Task DeleteConfirmed_RedirectsToIndex()
    {
        // Arrange
        _mockAccountService.Setup(s => s.DeleteAsync(1))
                           .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteConfirmed(1);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        _mockAccountService.Verify(s => s.DeleteAsync(1), Times.Once);
    }
}
