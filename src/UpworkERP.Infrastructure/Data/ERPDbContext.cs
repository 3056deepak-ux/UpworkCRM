using Microsoft.EntityFrameworkCore;
using UpworkERP.Core.Entities.Common;
using UpworkERP.Core.Entities.HR;
using UpworkERP.Core.Entities.Finance;
using UpworkERP.Core.Entities.CRM;
using UpworkERP.Core.Entities.Projects;
using UpworkERP.Core.Entities.Inventory;
using UpworkERP.Core.Entities.Auth;

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

    // Auth entities for RBAC
    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }

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
        ConfigureAuthEntities(modelBuilder);
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

    private void ConfigureAuthEntities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>()
            .HasIndex(r => r.Name)
            .IsUnique();

        modelBuilder.Entity<Permission>()
            .HasIndex(p => new { p.Module, p.Action })
            .IsUnique();

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Role)
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(rp => rp.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Permission)
            .WithMany(p => p.RolePermissions)
            .HasForeignKey(rp => rp.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private void ConfigureHREntities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>()
            .HasIndex(e => e.Email)
            .IsUnique();

        modelBuilder.Entity<Employee>()
            .Property(e => e.Salary)
            .HasPrecision(18, 2);

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

        modelBuilder.Entity<PayrollRecord>()
            .Property(pr => pr.GrossPay)
            .HasPrecision(18, 2);

        modelBuilder.Entity<PayrollRecord>()
            .Property(pr => pr.Deductions)
            .HasPrecision(18, 2);

        modelBuilder.Entity<PayrollRecord>()
            .Property(pr => pr.NetPay)
            .HasPrecision(18, 2);
    }

    private void ConfigureFinanceEntities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>()
            .HasIndex(a => a.AccountNumber)
            .IsUnique();

        modelBuilder.Entity<Account>()
            .Property(a => a.Balance)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.Account)
            .WithMany(a => a.Transactions)
            .HasForeignKey(t => t.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Transaction>()
            .Property(t => t.Amount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Budget>()
            .Property(b => b.AllocatedAmount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Budget>()
            .Property(b => b.SpentAmount)
            .HasPrecision(18, 2);
    }

    private void ConfigureCRMEntities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Lead>()
            .HasOne(l => l.Customer)
            .WithMany(c => c.Leads)
            .HasForeignKey(l => l.CustomerId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Lead>()
            .Property(l => l.EstimatedValue)
            .HasPrecision(18, 2);
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

        modelBuilder.Entity<Project>()
            .Property(p => p.Budget)
            .HasPrecision(18, 2);

        modelBuilder.Entity<TimeEntry>()
            .Property(te => te.Hours)
            .HasPrecision(18, 2);
    }

    private void ConfigureInventoryEntities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>()
            .HasIndex(p => p.SKU)
            .IsUnique();

        modelBuilder.Entity<Product>()
            .Property(p => p.UnitPrice)
            .HasPrecision(18, 2);

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
