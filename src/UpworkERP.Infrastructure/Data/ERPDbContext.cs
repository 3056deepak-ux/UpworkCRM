using Microsoft.EntityFrameworkCore;
using UpworkERP.Core.Entities.Common;
using UpworkERP.Core.Entities.HR;
using UpworkERP.Core.Entities.Finance;
using UpworkERP.Core.Entities.CRM;
using UpworkERP.Core.Entities.Projects;
using UpworkERP.Core.Entities.Inventory;

namespace UpworkERP.Infrastructure.Data;

/// <summary>
/// Main database context for the ERP system
/// </summary>
public class ERPDbContext : DbContext
{
    public ERPDbContext(DbContextOptions<ERPDbContext> options) : base(options)
    {
    }

    // Common entities
    public DbSet<User> Users { get; set; }
    public DbSet<ActivityLog> ActivityLogs { get; set; }
    public DbSet<AuditTrail> AuditTrails { get; set; }

    // HR entities
    public DbSet<Employee> Employees { get; set; }
    public DbSet<LeaveRequest> LeaveRequests { get; set; }
    public DbSet<PayrollRecord> PayrollRecords { get; set; }

    // Finance entities
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Budget> Budgets { get; set; }

    // CRM entities
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Lead> Leads { get; set; }

    // Project entities
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectTask> ProjectTasks { get; set; }
    public DbSet<TimeEntry> TimeEntries { get; set; }

    // Inventory entities
    public DbSet<Product> Products { get; set; }
    public DbSet<StockMovement> StockMovements { get; set; }
    public DbSet<Warehouse> Warehouses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure entity relationships and constraints
        ConfigureCommonEntities(modelBuilder);
        ConfigureHREntities(modelBuilder);
        ConfigureFinanceEntities(modelBuilder);
        ConfigureCRMEntities(modelBuilder);
        ConfigureProjectEntities(modelBuilder);
        ConfigureInventoryEntities(modelBuilder);
    }

    private void ConfigureCommonEntities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.UserName)
            .IsUnique();
    }

    private void ConfigureHREntities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>()
            .HasIndex(e => e.Email)
            .IsUnique();

        modelBuilder.Entity<LeaveRequest>()
            .HasOne(lr => lr.Employee)
            .WithMany(e => e.LeaveRequests)
            .HasForeignKey(lr => lr.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PayrollRecord>()
            .HasOne(pr => pr.Employee)
            .WithMany(e => e.PayrollRecords)
            .HasForeignKey(pr => pr.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private void ConfigureFinanceEntities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>()
            .HasIndex(a => a.AccountNumber)
            .IsUnique();

        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.Account)
            .WithMany(a => a.Transactions)
            .HasForeignKey(t => t.AccountId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private void ConfigureCRMEntities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Lead>()
            .HasOne(l => l.Customer)
            .WithMany(c => c.Leads)
            .HasForeignKey(l => l.CustomerId)
            .OnDelete(DeleteBehavior.SetNull);
    }

    private void ConfigureProjectEntities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProjectTask>()
            .HasOne(pt => pt.Project)
            .WithMany(p => p.Tasks)
            .HasForeignKey(pt => pt.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TimeEntry>()
            .HasOne(te => te.Project)
            .WithMany(p => p.TimeEntries)
            .HasForeignKey(te => te.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TimeEntry>()
            .HasOne(te => te.Task)
            .WithMany()
            .HasForeignKey(te => te.TaskId)
            .OnDelete(DeleteBehavior.SetNull);
    }

    private void ConfigureInventoryEntities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>()
            .HasIndex(p => p.SKU)
            .IsUnique();

        modelBuilder.Entity<StockMovement>()
            .HasOne(sm => sm.Product)
            .WithMany(p => p.StockMovements)
            .HasForeignKey(sm => sm.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<StockMovement>()
            .HasOne(sm => sm.Warehouse)
            .WithMany(w => w.StockMovements)
            .HasForeignKey(sm => sm.WarehouseId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
