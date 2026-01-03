using BenchLibrary.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace BenchLibrary.Data;

/// <summary>
/// Entity Framework Core database context for BenchLibrary.
/// </summary>
public class BenchLibraryDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BenchLibraryDbContext"/> class.
    /// </summary>
    /// <param name="options">The database context options.</param>
    public BenchLibraryDbContext(DbContextOptions<BenchLibraryDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the devices under test.
    /// </summary>
    public DbSet<DeviceUnderTest> Devices => Set<DeviceUnderTest>();

    /// <summary>
    /// Gets or sets the test results.
    /// </summary>
    public DbSet<TestResult> TestResults => Set<TestResult>();

    /// <summary>
    /// Gets or sets the measurements.
    /// </summary>
    public DbSet<Measurement> Measurements => Set<Measurement>();

    /// <summary>
    /// Configures the model using the Fluent API.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure DeviceUnderTest
        modelBuilder.Entity<DeviceUnderTest>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SerialNumber).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ModelNumber).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Revision).HasMaxLength(50);
            entity.Property(e => e.LotNumber).HasMaxLength(50);
            entity.Property(e => e.CurrentLocation).HasMaxLength(100);
            entity.Property(e => e.Notes).HasMaxLength(1000);

            entity.HasIndex(e => e.SerialNumber).IsUnique();
            entity.HasIndex(e => e.ModelNumber);
            entity.HasIndex(e => e.LotNumber);

            entity.HasMany(e => e.TestResults)
                .WithOne()
                .HasForeignKey("DeviceId")
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure TestResult
        modelBuilder.Entity<TestResult>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SerialNumber).IsRequired().HasMaxLength(100);
            entity.Property(e => e.TestName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Operator).HasMaxLength(100);
            entity.Property(e => e.Workstation).HasMaxLength(100);
            entity.Property(e => e.ErrorMessage).HasMaxLength(2000);

            entity.HasIndex(e => e.SerialNumber);
            entity.HasIndex(e => e.TestName);
            entity.HasIndex(e => e.StartTime);
            entity.HasIndex(e => e.Status);

            entity.HasMany(e => e.Measurements)
                .WithOne()
                .HasForeignKey(m => m.TestResultId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Measurement
        modelBuilder.Entity<Measurement>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);

            entity.HasIndex(e => e.TestResultId);
            entity.HasIndex(e => e.Name);
        });
    }

    /// <summary>
    /// Saves changes and updates timestamp fields.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The number of entities saved.</returns>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Saves changes and updates timestamp fields.
    /// </summary>
    /// <returns>The number of entities saved.</returns>
    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    private void UpdateTimestamps()
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<DeviceUnderTest>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
            }

            if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
            }
        }

        foreach (var entry in ChangeTracker.Entries<TestResult>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
            }

            if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
            }
        }
    }
}
