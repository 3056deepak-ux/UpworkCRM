using Xunit;
using Moq;
using UpworkERP.Application.Services.Implementation;
using UpworkERP.Core.Interfaces;
using UpworkERP.Core.Entities.HR;

namespace UpworkERP.Tests.Services;

/// <summary>
/// Unit tests for the generic Service class
/// </summary>
public class ServiceTests
{
    private readonly Mock<IRepository<Employee>> _mockRepository;
    private readonly Service<Employee> _service;

    public ServiceTests()
    {
        _mockRepository = new Mock<IRepository<Employee>>();
        _service = new Service<Employee>(_mockRepository.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsEmployee_WhenEmployeeExists()
    {
        // Arrange
        var employeeId = 1;
        var expectedEmployee = new Employee
        {
            Id = employeeId,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Department = "IT",
            Position = "Developer"
        };
        _mockRepository.Setup(r => r.GetByIdAsync(employeeId, default))
                      .ReturnsAsync(expectedEmployee);

        // Act
        var result = await _service.GetByIdAsync(employeeId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(employeeId, result.Id);
        Assert.Equal("John", result.FirstName);
        _mockRepository.Verify(r => r.GetByIdAsync(employeeId, default), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_CallsRepositoryAddAndSaveChanges()
    {
        // Arrange
        var employee = new Employee
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane.smith@example.com",
            Department = "HR",
            Position = "Manager"
        };
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Employee>(), default))
                      .ReturnsAsync(employee);
        _mockRepository.Setup(r => r.SaveChangesAsync(default))
                      .ReturnsAsync(1);

        // Act
        var result = await _service.CreateAsync(employee);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Jane", result.FirstName);
        _mockRepository.Verify(r => r.AddAsync(employee, default), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_CallsRepositoryUpdateAndSaveChanges()
    {
        // Arrange
        var employee = new Employee
        {
            Id = 1,
            FirstName = "Updated",
            LastName = "Name",
            Email = "updated@example.com"
        };
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Employee>(), default))
                      .Returns(Task.CompletedTask);
        _mockRepository.Setup(r => r.SaveChangesAsync(default))
                      .ReturnsAsync(1);

        // Act
        var result = await _service.UpdateAsync(employee);

        // Assert
        Assert.NotNull(result);
        _mockRepository.Verify(r => r.UpdateAsync(employee, default), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_CallsRepositoryDeleteAndSaveChanges_WhenEntityExists()
    {
        // Arrange
        var employeeId = 1;
        var employee = new Employee { Id = employeeId };
        _mockRepository.Setup(r => r.GetByIdAsync(employeeId, default))
                      .ReturnsAsync(employee);
        _mockRepository.Setup(r => r.DeleteAsync(It.IsAny<Employee>(), default))
                      .Returns(Task.CompletedTask);
        _mockRepository.Setup(r => r.SaveChangesAsync(default))
                      .ReturnsAsync(1);

        // Act
        await _service.DeleteAsync(employeeId);

        // Assert
        _mockRepository.Verify(r => r.GetByIdAsync(employeeId, default), Times.Once);
        _mockRepository.Verify(r => r.DeleteAsync(employee, default), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllEmployees()
    {
        // Arrange
        var employees = new List<Employee>
        {
            new Employee { Id = 1, FirstName = "John", LastName = "Doe" },
            new Employee { Id = 2, FirstName = "Jane", LastName = "Smith" }
        };
        _mockRepository.Setup(r => r.GetAllAsync(default))
                      .ReturnsAsync(employees);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockRepository.Verify(r => r.GetAllAsync(default), Times.Once);
    }
}
