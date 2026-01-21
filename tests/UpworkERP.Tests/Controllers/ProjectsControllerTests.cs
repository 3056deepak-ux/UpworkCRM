using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System.Net;
using UpworkERP.Web.Controllers;
using UpworkERP.Application.Services.Interfaces;
using UpworkERP.Core.Entities.Projects;
using UpworkERP.Core.Enums;

namespace UpworkERP.Tests.Controllers;

/// <summary>
/// Unit tests for ProjectsController
/// </summary>
public class ProjectsControllerTests
{
    private readonly Mock<IService<Project>> _mockProjectService;
    private readonly Mock<IActivityLogService> _mockActivityLogService;
    private readonly ProjectsController _controller;

    public ProjectsControllerTests()
    {
        _mockProjectService = new Mock<IService<Project>>();
        _mockActivityLogService = new Mock<IActivityLogService>();
        _controller = new ProjectsController(_mockProjectService.Object, _mockActivityLogService.Object);
        
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
    public async Task Index_ReturnsViewWithProjects()
    {
        // Arrange
        var projects = new List<Project>
        {
            new Project { Id = 1, Name = "Project 1", ProjectManager = "John Doe", Budget = 100000, Status = ProjectStatus.InProgress },
            new Project { Id = 2, Name = "Project 2", ProjectManager = "Jane Smith", Budget = 200000, Status = ProjectStatus.Planning }
        };
        _mockProjectService.Setup(s => s.GetAllAsync())
                           .ReturnsAsync(projects);

        // Act
        var result = await _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<Project>>(viewResult.Model);
        Assert.Equal(2, model.Count());
        _mockProjectService.Verify(s => s.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task Details_ReturnsViewWithProject_WhenProjectExists()
    {
        // Arrange
        var project = new Project { Id = 1, Name = "Test Project", ProjectManager = "John Doe", Budget = 150000, Status = ProjectStatus.InProgress };
        _mockProjectService.Setup(s => s.GetByIdAsync(1))
                           .ReturnsAsync(project);

        // Act
        var result = await _controller.Details(1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<Project>(viewResult.Model);
        Assert.Equal(1, model.Id);
        Assert.Equal("Test Project", model.Name);
        Assert.Equal("John Doe", model.ProjectManager);
    }

    [Fact]
    public async Task Details_ReturnsNotFound_WhenProjectDoesNotExist()
    {
        // Arrange
        _mockProjectService.Setup(s => s.GetByIdAsync(999))
                           .ReturnsAsync((Project?)null);

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
        var project = new Project
        {
            Name = "New Project",
            Description = "Project Description",
            ProjectManager = "John Doe",
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddMonths(6),
            Budget = 250000,
            Status = ProjectStatus.Planning
        };

        _mockProjectService.Setup(s => s.CreateAsync(It.IsAny<Project>()))
                           .ReturnsAsync(project);
        _mockActivityLogService.Setup(s => s.LogActivityAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 
            It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>(), 
            It.IsAny<string>()))
                              .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Create(project);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        _mockProjectService.Verify(s => s.CreateAsync(project), Times.Once);
    }

    [Fact]
    public async Task Create_Post_ReturnsView_WhenModelStateIsInvalid()
    {
        // Arrange
        var project = new Project();
        _controller.ModelState.AddModelError("Name", "Required");

        // Act
        var result = await _controller.Create(project);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(project, viewResult.Model);
        _mockProjectService.Verify(s => s.CreateAsync(It.IsAny<Project>()), Times.Never);
    }

    [Fact]
    public async Task DeleteConfirmed_RedirectsToIndex()
    {
        // Arrange
        _mockProjectService.Setup(s => s.DeleteAsync(1))
                           .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteConfirmed(1);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        _mockProjectService.Verify(s => s.DeleteAsync(1), Times.Once);
    }
}
