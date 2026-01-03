# Database & Entity Framework Core Standards

**ORM:** Entity Framework Core 8
**Primary Database:** PostgreSQL
**Development Database:** SQLite
**Last Updated:** 2026-01-03

---

## Table of Contents

1. [Entity Design](#entity-design)
2. [DbContext Configuration](#dbcontext-configuration)
3. [Relationships](#relationships)
4. [Value Conversions](#value-conversions)
5. [Migrations](#migrations)
6. [Querying](#querying)
7. [Performance](#performance)
8. [Data Seeding](#data-seeding)
9. [Audit & Timestamps](#audit--timestamps)
10. [Repository Pattern](#repository-pattern)

---

## Entity Design

### Entity naming & structure
Name entities in singular form, matching database tables:
```csharp
// ✓ Good
public class Process
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<Measurement> Measurements { get; set; } = new List<Measurement>();
}

public class Measurement
{
    public int Id { get; set; }
    public int ProcessId { get; set; }
    public required double Value { get; set; }
    public DateTime MeasuredAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public required Process Process { get; set; }
}
```

### Primary keys
Always use `Id` for the primary key:
```csharp
public class Process
{
    // ✓ Good - EF automatically recognizes as PK
    public int Id { get; set; }

    // ✗ Avoid - non-standard naming
    public int ProcessId { get; set; }
    public int process_id { get; set; }
}
```

### Property naming
```csharp
// ✓ Good - clear intent with timestamps
public class Entity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}

// ✗ Avoid - ambiguous names
public class Entity
{
    public int id { get; set; }
    public string name { get; set; }
    public DateTime created { get; set; }
    public DateTime modified { get; set; }
}
```

### Required properties
Use `required` keyword for mandatory fields:
```csharp
public class Process
{
    public int Id { get; set; }
    public required string Name { get; set; } // must always have value
    public string? Description { get; set; } // optional
    public required double TargetValue { get; set; }
}
```

### Computed properties
Use `[NotMapped]` for computed properties that shouldn't be persisted:
```csharp
public class Measurement
{
    public int Id { get; set; }
    public int ProcessId { get; set; }
    public double Value { get; set; }

    [NotMapped]
    public double AbsoluteValue => Math.Abs(Value); // computed, not persisted

    [NotMapped]
    public DateTime CreatedDate => CreatedAt.Date; // computed from timestamp
}
```

---

## DbContext Configuration

### DbContext setup
```csharp
namespace BenchLibrary.Data;

public class BenchLibraryDbContext : DbContext
{
    public BenchLibraryDbContext(DbContextOptions<BenchLibraryDbContext> options)
        : base(options)
    {
    }

    // DbSets in logical order
    public DbSet<Process> Processes { get; set; } = null!;
    public DbSet<Measurement> Measurements { get; set; } = null!;
    public DbSet<ProcessSpecification> ProcessSpecifications { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Entity configurations
        ConfigureProcess(modelBuilder);
        ConfigureMeasurement(modelBuilder);
        ConfigureProcessSpecification(modelBuilder);

        // Global query filters
        ApplyGlobalFilters(modelBuilder);
    }

    private void ConfigureProcess(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Process>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now() at time zone 'UTC'");

            entity.HasMany(e => e.Measurements)
                .WithOne(m => m.Process)
                .HasForeignKey(m => m.ProcessId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.Name)
                .IsUnique();

            entity.HasIndex(e => e.CreatedAt);
        });
    }

    private void ConfigureMeasurement(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Measurement>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Value)
                .HasPrecision(18, 4);

            entity.Property(e => e.MeasuredAt)
                .HasDefaultValueSql("now() at time zone 'UTC'");

            entity.HasIndex(e => new { e.ProcessId, e.MeasuredAt });
        });
    }

    private void ConfigureProcessSpecification(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProcessSpecification>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.LowerSpecificationLimit)
                .HasPrecision(18, 4);

            entity.Property(e => e.UpperSpecificationLimit)
                .HasPrecision(18, 4);

            entity.Property(e => e.TargetValue)
                .HasPrecision(18, 4);
        });
    }

    private void ApplyGlobalFilters(ModelBuilder modelBuilder)
    {
        // Add soft-delete filters if needed
        // modelBuilder.Entity<Process>()
        //     .HasQueryFilter(p => !p.IsDeleted);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries<IAuditableEntity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}
```

### DbContext dependency injection
Register in `Program.cs`:
```csharp
// ✓ Good
services.AddDbContext<BenchLibraryDbContext>(options =>
{
    if (app.Environment.IsDevelopment())
    {
        options.UseSqlite("Data Source=BenchLibrary.db");
    }
    else
    {
        options.UseNpgsql(
            connectionString,
            npgsql => npgsql.EnableRetryOnFailure());
    }
});
```

---

## Relationships

### One-to-Many relationships
```csharp
// Parent entity
public class Process
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Navigation property - collection
    public ICollection<Measurement> Measurements { get; set; } = new List<Measurement>();
}

// Child entity
public class Measurement
{
    public int Id { get; set; }
    public int ProcessId { get; set; }
    public double Value { get; set; }

    // Navigation property - reference
    public required Process Process { get; set; }
}

// Configuration
modelBuilder.Entity<Process>()
    .HasMany(p => p.Measurements)
    .WithOne(m => m.Process)
    .HasForeignKey(m => m.ProcessId)
    .OnDelete(DeleteBehavior.Cascade); // delete measurements if process deleted
```

### Many-to-Many relationships
```csharp
// With payload (intermediate entity)
public class ProcessOperator
{
    public int ProcessId { get; set; }
    public int OperatorId { get; set; }
    public DateTime AssignedAt { get; set; }

    public required Process Process { get; set; }
    public required Operator Operator { get; set; }
}

// Configuration
modelBuilder.Entity<ProcessOperator>(entity =>
{
    entity.HasKey(po => new { po.ProcessId, po.OperatorId });

    entity.HasOne(po => po.Process)
        .WithMany()
        .HasForeignKey(po => po.ProcessId)
        .OnDelete(DeleteBehavior.Cascade);

    entity.HasOne(po => po.Operator)
        .WithMany()
        .HasForeignKey(po => po.OperatorId)
        .OnDelete(DeleteBehavior.Cascade);
});

// Entities
public class Process
{
    public int Id { get; set; }
    public ICollection<ProcessOperator> ProcessOperators { get; set; } = new List<ProcessOperator>();
}

public class Operator
{
    public int Id { get; set; }
    public ICollection<ProcessOperator> ProcessOperators { get; set; } = new List<ProcessOperator>();
}
```

### Self-referencing relationships
```csharp
public class ProcessHierarchy
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? ParentId { get; set; }

    // Self-reference
    [ForeignKey(nameof(ParentId))]
    public ProcessHierarchy? Parent { get; set; }

    public ICollection<ProcessHierarchy> Children { get; set; } = new List<ProcessHierarchy>();
}

// Configuration
modelBuilder.Entity<ProcessHierarchy>(entity =>
{
    entity.HasOne(p => p.Parent)
        .WithMany(p => p.Children)
        .HasForeignKey(p => p.ParentId)
        .IsRequired(false)
        .OnDelete(DeleteBehavior.NoAction);
});
```

---

## Value Conversions

### Enum conversion
```csharp
// Entity with enum
public class Process
{
    public int Id { get; set; }
    public ProcessType Type { get; set; }
    public ProcessStatus Status { get; set; }
}

public enum ProcessType
{
    Continuous,
    Discrete
}

public enum ProcessStatus
{
    Draft,
    Active,
    Archived
}

// Configuration - store as string
modelBuilder.Entity<Process>(entity =>
{
    entity.Property(e => e.Type)
        .HasConversion<string>(); // stores "Continuous", not 0

    entity.Property(e => e.Status)
        .HasDefaultValue(ProcessStatus.Draft);
});
```

### Value object conversion
```csharp
public class Measurement
{
    public int Id { get; set; }
    public required MeasurementValue Value { get; set; }
}

// Value object
public record MeasurementValue(double Value)
{
    public MeasurementValue() : this(0) { }
}

// Configuration
modelBuilder.Entity<Measurement>(entity =>
{
    entity.Property(e => e.Value)
        .HasConversion(
            v => v.Value,
            v => new MeasurementValue(v));
});
```

### DateTime conversion (UTC)
```csharp
modelBuilder.Entity<Process>(entity =>
{
    entity.Property(e => e.CreatedAt)
        .HasColumnType("timestamp with time zone")
        .HasConversion(
            v => v.ToUniversalTime(),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
});
```

---

## Migrations

### Migration creation
Always create explicit, descriptive migrations:
```bash
# ✓ Good - clear intent
dotnet ef migrations add AddProcessMeasurementsTable

# ✓ Good - describes change
dotnet ef migrations add AddCreatedAtTimestampToProcess

# ✗ Avoid - vague
dotnet ef migrations add UpdateDatabase
dotnet ef migrations add Fix
```

### Migration structure
```csharp
public partial class AddProcessMeasurementsTable : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Create table with proper indexes
        migrationBuilder.CreateTable(
            name: "Measurement",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                ProcessId = table.Column<int>(type: "integer", nullable: false),
                Value = table.Column<double>(type: "numeric(18,4)", nullable: false),
                MeasuredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Measurement", x => x.Id);
                table.ForeignKey(
                    name: "FK_Measurement_Process_ProcessId",
                    column: x => x.ProcessId,
                    principalTable: "Process",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        // Create indexes
        migrationBuilder.CreateIndex(
            name: "IX_Measurement_ProcessId",
            table: "Measurement",
            column: "ProcessId");

        migrationBuilder.CreateIndex(
            name: "IX_Measurement_ProcessId_MeasuredAt",
            table: "Measurement",
            columns: new[] { "ProcessId", "MeasuredAt" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Measurement");
    }
}
```

### Data migrations
For data transformations, use SQL in migrations:
```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    // Add new column
    migrationBuilder.AddColumn<string>(
        name: "Category",
        table: "Process",
        type: "text",
        nullable: true);

    // Migrate data
    migrationBuilder.Sql(@"
        UPDATE ""Process""
        SET ""Category"" = CASE
            WHEN ""Type"" = 'Continuous' THEN 'Manufacturing'
            WHEN ""Type"" = 'Discrete' THEN 'Assembly'
            ELSE 'Unknown'
        END");

    // Make column required
    migrationBuilder.AlterColumn<string>(
        name: "Category",
        table: "Process",
        type: "text",
        nullable: false);
}
```

---

## Querying

### Basic queries
```csharp
// ✓ Good - specific property selection
public async Task<ProcessSummary?> GetProcessByIdAsync(int id)
{
    return await _context.Processes
        .Where(p => p.Id == id)
        .Select(p => new ProcessSummary(p.Id, p.Name, p.CreatedAt))
        .FirstOrDefaultAsync();
}

// ✗ Avoid - loading entire entity
public async Task<Process?> GetProcessByIdAsync(int id)
{
    return await _context.Processes
        .FirstOrDefaultAsync(p => p.Id == id);
}
```

### Include relationships
```csharp
// ✓ Good - explicit loading
public async Task<Process?> GetProcessWithMeasurementsAsync(int id)
{
    return await _context.Processes
        .Include(p => p.Measurements)
        .FirstOrDefaultAsync(p => p.Id == id);
}

// ✓ Good - select needed data only
public async Task<ProcessWithMeasurementCountDto> GetProcessStatsAsync(int id)
{
    return await _context.Processes
        .Where(p => p.Id == id)
        .Select(p => new ProcessWithMeasurementCountDto(
            p.Id,
            p.Name,
            p.Measurements.Count,
            p.Measurements.Average(m => m.Value)))
        .FirstOrDefaultAsync();
}

// ✗ Avoid - N+1 query problem
var process = await _context.Processes.FirstOrDefaultAsync(p => p.Id == id);
var measurementCount = await _context.Measurements
    .CountAsync(m => m.ProcessId == process.Id); // separate query
```

### Filtering
```csharp
// ✓ Good - build predicate incrementally
public async Task<List<Process>> FindProcessesAsync(ProcessFilter filter)
{
    var query = _context.Processes.AsQueryable();

    if (!string.IsNullOrEmpty(filter.Name))
        query = query.Where(p => p.Name.Contains(filter.Name));

    if (filter.CreatedAfter.HasValue)
        query = query.Where(p => p.CreatedAt >= filter.CreatedAfter);

    if (!filter.IncludeArchived)
        query = query.Where(p => p.Status != ProcessStatus.Archived);

    return await query.ToListAsync();
}
```

### Pagination
```csharp
public async Task<PagedResult<ProcessDto>> GetProcessesPagedAsync(int pageNumber, int pageSize)
{
    var total = await _context.Processes.CountAsync();
    var items = await _context.Processes
        .OrderByDescending(p => p.CreatedAt)
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .Select(p => new ProcessDto(p.Id, p.Name))
        .ToListAsync();

    return new PagedResult<ProcessDto>(items, total, pageNumber, pageSize);
}
```

---

## Performance

### Batch operations
```csharp
// ✓ Good - batch update
public async Task UpdateProcessStatusAsync(IEnumerable<int> processIds, ProcessStatus status)
{
    await _context.Processes
        .Where(p => processIds.Contains(p.Id))
        .ExecuteUpdateAsync(s => s
            .SetProperty(p => p.Status, status)
            .SetProperty(p => p.UpdatedAt, DateTime.UtcNow));
}

// ✗ Avoid - loads into memory
public async Task UpdateProcessStatusAsync(IEnumerable<int> processIds, ProcessStatus status)
{
    var processes = await _context.Processes
        .Where(p => processIds.Contains(p.Id))
        .ToListAsync();

    foreach (var process in processes)
    {
        process.Status = status;
    }

    await _context.SaveChangesAsync(); // separate update per entity
}
```

### Indexes
Always add indexes for frequently filtered/joined columns:
```csharp
modelBuilder.Entity<Measurement>(entity =>
{
    // Composite index for common filter
    entity.HasIndex(m => new { m.ProcessId, m.MeasuredAt });

    // Ascending/descending for sorting
    entity.HasIndex(m => m.CreatedAt).IsDescending();

    // Unique index where needed
    entity.HasIndex(m => m.ExternalId).IsUnique();
});
```

### Lazy loading vs. explicit
Always use explicit `.Include()`. Don't enable lazy loading:
```csharp
// ✓ Good - explicit
public async Task<Process> GetProcessAsync(int id)
{
    return await _context.Processes
        .Include(p => p.Measurements)
        .FirstAsync(p => p.Id == id);
}

// ✗ Avoid - lazy loading (performance issue)
// Even if configured, still creates N+1 problem
var process = await _context.Processes.FirstAsync(p => p.Id == id);
var measurements = process.Measurements; // triggers query per access
```

### No-tracking queries for read-only
```csharp
public async Task<List<ProcessDto>> GetAllProcessesReadOnlyAsync()
{
    return await _context.Processes
        .AsNoTracking() // don't track for change detection
        .Select(p => new ProcessDto(p.Id, p.Name))
        .ToListAsync();
}
```

---

## Data Seeding

### Seed in configuration
```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // Seed reference data
    modelBuilder.Entity<ProcessType>().HasData(
        new ProcessType { Id = 1, Name = "Continuous" },
        new ProcessType { Id = 2, Name = "Discrete" }
    );

    modelBuilder.Entity<ProcessStatus>().HasData(
        new ProcessStatus { Id = 1, Name = "Draft" },
        new ProcessStatus { Id = 2, Name = "Active" },
        new ProcessStatus { Id = 3, Name = "Archived" }
    );
}
```

### Seed in migration
```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.InsertData(
        table: "ProcessType",
        columns: new[] { "Id", "Name" },
        values: new object[,]
        {
            { 1, "Continuous" },
            { 2, "Discrete" }
        });
}
```

### Seed on startup (development)
```csharp
public static async Task SeedDevelopmentDataAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<BenchLibraryDbContext>();

    if (await context.Processes.AnyAsync())
        return; // Already seeded

    var processes = new[]
    {
        new Process { Id = 1, Name = "Assembly Line A", CreatedAt = DateTime.UtcNow },
        new Process { Id = 2, Name = "Assembly Line B", CreatedAt = DateTime.UtcNow }
    };

    context.Processes.AddRange(processes);
    await context.SaveChangesAsync();
}

// In Program.cs
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<BenchLibraryDbContext>();
        context.Database.Migrate();
        await SeedDevelopmentDataAsync(app.Services);
    }
}
```

---

## Audit & Timestamps

### Audit interface
```csharp
public interface IAuditableEntity
{
    DateTime CreatedAt { get; set; }
    DateTime UpdatedAt { get; set; }
    string? CreatedBy { get; set; }
    string? UpdatedBy { get; set; }
}

public class Process : IAuditableEntity
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}
```

### Auto-update timestamps
```csharp
public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
{
    UpdateTimestamps();
    return await base.SaveChangesAsync(cancellationToken);
}

private void UpdateTimestamps()
{
    var now = DateTime.UtcNow;
    var entries = ChangeTracker.Entries<IAuditableEntity>();

    foreach (var entry in entries)
    {
        if (entry.State == EntityState.Added)
        {
            entry.Entity.CreatedAt = now;
            entry.Entity.UpdatedAt = now;
        }
        else if (entry.State == EntityState.Modified)
        {
            entry.Entity.UpdatedAt = now;
        }
    }
}
```

---

## Repository Pattern

### Generic repository
```csharp
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<List<T>> GetAllAsync();
    Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task DeleteAsync(int id);
    Task<int> SaveChangesAsync();
}

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly BenchLibraryDbContext Context;

    public Repository(BenchLibraryDbContext context)
    {
        Context = context;
    }

    public async Task<T?> GetByIdAsync(int id) =>
        await Context.Set<T>().FindAsync(id);

    public async Task<List<T>> GetAllAsync() =>
        await Context.Set<T>().ToListAsync();

    public async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate) =>
        await Context.Set<T>().Where(predicate).ToListAsync();

    public async Task<T> AddAsync(T entity)
    {
        Context.Set<T>().Add(entity);
        return entity;
    }

    public async Task<T> UpdateAsync(T entity)
    {
        Context.Set<T>().Update(entity);
        return entity;
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
            Context.Set<T>().Remove(entity);
    }

    public async Task<int> SaveChangesAsync() =>
        await Context.SaveChangesAsync();
}
```

### Specialized repository
```csharp
public interface IProcessRepository : IRepository<Process>
{
    Task<Process?> GetByNameAsync(string name);
    Task<List<Process>> GetActiveProcessesAsync();
    Task<int> GetMeasurementCountAsync(int processId);
}

public class ProcessRepository : Repository<Process>, IProcessRepository
{
    public ProcessRepository(BenchLibraryDbContext context) : base(context) { }

    public async Task<Process?> GetByNameAsync(string name) =>
        await Context.Processes
            .FirstOrDefaultAsync(p => p.Name == name);

    public async Task<List<Process>> GetActiveProcessesAsync() =>
        await Context.Processes
            .Where(p => p.Status == ProcessStatus.Active)
            .ToListAsync();

    public async Task<int> GetMeasurementCountAsync(int processId) =>
        await Context.Measurements
            .CountAsync(m => m.ProcessId == processId);
}
```

---

## Key Rules Summary

✓ **DO:**
- Use `Id` for primary keys
- Use `required` keyword for mandatory fields
- Name entities in singular form
- Use `.Include()` for eager loading
- Add indexes for frequently filtered columns
- Use async methods (`Async` suffix)
- Implement timestamps (`CreatedAt`, `UpdatedAt`)
- Use value objects for complex types
- Create specific repositories for complex queries
- Use `AsNoTracking()` for read-only queries

✗ **DON'T:**
- Use non-standard primary key names
- Leave important queries unindexed
- Use lazy loading
- Block on async operations
- Load entire entities when only fields are needed
- Create overly complex entities (>20 properties)
- Ignore null safety in queries
- Use `ExecuteUpdate` without proper filtering
- Skip migration comments
- Hardcode connection strings

