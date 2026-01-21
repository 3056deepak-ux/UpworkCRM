using Microsoft.EntityFrameworkCore;
using UpworkERP.Core.Entities.CRM;
using UpworkERP.Core.Entities.Finance;
using UpworkERP.Core.Entities.HR;
using UpworkERP.Core.Entities.Inventory;
using UpworkERP.Core.Entities.Projects;

namespace UpworkERP.Infrastructure.Data;

/// <summary>
/// Database context for ERP system
/// </summary>
public class ERPDbContext : DbContext
{
    public ERPDbContext(DbContextOptions<ERPDbContext> options) : base(options)
    {
    }

    // CRM
    public DbSet<Customer> Customers { get; set; }

    // Finance
    public DbSet<Account> Accounts { get; set; }

    // HR
    public DbSet<Employee> Employees { get; set; }

    // Inventory
    public DbSet<Product> Products { get; set; }

    // Projects
    public DbSet<Project> Projects { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure entity indexes and constraints
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasIndex(e => e.Email);
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasIndex(e => e.SKU).IsUnique();
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasIndex(e => e.AccountNumber).IsUnique();
            entity.HasQueryFilter(e => !e.IsDeleted);
        });
    }
}
