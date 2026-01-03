# Testing Standards & Practices

**Testing Framework:** xUnit
**Mocking Library:** Moq
**Assertion Library:** FluentAssertions
**Integration Testing:** Testcontainers (default)
**Coverage Target:** Minimum 80%
**Regression Tests:** Required for all customer issues
**Last Updated:** 2026-01-03

---

## Table of Contents

1. [Test Project Structure](#test-project-structure)
2. [Unit Test Naming](#unit-test-naming)
3. [Test Organization](#test-organization)
4. [AAA Pattern](#aaa-pattern)
5. [Mocking & Dependencies](#mocking--dependencies)
6. [Test Data](#test-data)
7. [Assertions](#assertions)
8. [Async Tests](#async-tests)
9. [Integration Tests with Testcontainers](#integration-tests-with-testcontainers)
10. [Regression Tests for Customer Issues](#regression-tests-for-customer-issues)
11. [Code Coverage](#code-coverage)

---

## Test Project Structure

### Project naming
Test projects mirror source projects with `.Tests` suffix:
```
src/
├── BenchLibrary.Core/
├── BenchLibrary.SixSigma/
├── BenchLibrary.Data/
└── BenchLibrary.Web/

tests/
├── BenchLibrary.Core.Tests/
├── BenchLibrary.SixSigma.Tests/
├── BenchLibrary.Data.Tests/
└── BenchLibrary.Web.Tests/
```

### Test file structure
Mirror the source structure in test projects:
```
BenchLibrary.Core.Tests/
├── Models/
│   ├── ProcessModelTests.cs
│   └── MeasurementTests.cs
├── Services/
│   ├── ProcessServiceTests.cs
│   └── AnalyzerServiceTests.cs
├── Fixtures/
│   ├── ProcessFixture.cs
│   └── MeasurementFixture.cs
└── Helpers/
    ├── TestDataBuilder.cs
    └── MockFactory.cs
```

### Test project (.csproj)
```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <IsTestProject>true</IsTestProject>
    <LangVersion>12</LangVersion>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <!-- Unit Testing -->
    <PackageReference Include="xunit" Version="2.6.4" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.5.4" />
    <PackageReference Include="Moq" Version="4.20.69" />
    <PackageReference Include="FluentAssertions" Version="6.12.0" />
    <PackageReference Include="Microsoft.Net.Test.Sdk" Version="17.8.2" />

    <!-- Integration Testing with Testcontainers -->
    <PackageReference Include="Testcontainers" Version="3.7.0" />
    <PackageReference Include="Testcontainers.PostgreSql" Version="3.7.0" />
    <PackageReference Include="Testcontainers.MsSql" Version="3.7.0" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="../../src/BenchLibrary.Core/BenchLibrary.Core.csproj" />
  </ItemGroup>

</Project>
```

---

## Unit Test Naming

### Test method naming convention
Use `MethodName_Scenario_ExpectedResult` pattern:
```csharp
// ✓ Good - clear, describes what is tested
public void CalculateCapabilityIndex_WithValidMeasurements_ReturnsPositiveValue()
{
    // arrange
    var measurements = new[] { 1.0, 2.0, 3.0 };

    // act
    var result = _analyzer.CalculateCapabilityIndex(measurements);

    // assert
    result.Should().BePositive();
}

public void CalculateCapabilityIndex_WithEmptyMeasurements_ThrowsArgumentException()
{
    // arrange
    var measurements = Array.Empty<double>();

    // act & assert
    _analyzer.Invoking(a => a.CalculateCapabilityIndex(measurements))
        .Should().Throw<ArgumentException>();
}

public void GetProcessById_WithNonExistentId_ReturnsNull()
{
    // arrange
    var nonExistentId = 999;

    // act
    var result = _service.GetProcessById(nonExistentId);

    // assert
    result.Should().BeNull();
}

// ✗ Avoid - vague naming
public void TestCalculateCapabilityIndex()
public void TestGetProcess()
public void ProcessTest()
```

### Naming variations
```csharp
// For properties
public void ProcessName_WhenSet_IsRetrievable()
{
    // arrange
    var process = new Process();

    // act
    process.Name = "Test Process";

    // assert
    process.Name.Should().Be("Test Process");
}

// For collections
public void Measurements_WhenAdded_IncrementsCount()
{
    // arrange
    var process = new Process();

    // act
    process.Measurements.Add(new Measurement { Value = 1.0 });

    // assert
    process.Measurements.Count.Should().Be(1);
}

// For edge cases
public void CalculateIndex_WithNegativeValues_IgnoresNegatives()
{
    // arrange
    var measurements = new[] { 1.0, -2.0, 3.0 };

    // act
    var result = _analyzer.CalculateIndex(measurements);

    // assert
    // expectations...
}
```

---

## Test Organization

### Fixture usage
Use xUnit fixtures for setup/teardown:
```csharp
public class ProcessAnalyzerTests : IDisposable
{
    private readonly ProcessAnalyzer _analyzer;
    private readonly Mock<ILogger<ProcessAnalyzer>> _loggerMock;
    private readonly Mock<IRepository<Measurement>> _repositoryMock;

    public ProcessAnalyzerTests()
    {
        _loggerMock = new Mock<ILogger<ProcessAnalyzer>>();
        _repositoryMock = new Mock<IRepository<Measurement>>();
        _analyzer = new ProcessAnalyzer(_repositoryMock.Object, _loggerMock.Object);
    }

    public void Dispose()
    {
        // Cleanup if needed
        _loggerMock.Reset();
        _repositoryMock.Reset();
    }

    [Fact]
    public void SampleTest()
    {
        // test code
    }
}
```

### Collection fixtures
For shared setup across multiple test classes:
```csharp
public class DatabaseFixture : IAsyncLifetime
{
    private readonly BenchLibraryDbContext _context;

    public DatabaseFixture()
    {
        var options = new DbContextOptionsBuilder<BenchLibraryDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new BenchLibraryDbContext(options);
    }

    public async Task InitializeAsync()
    {
        await _context.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _context.Database.EnsureDeletedAsync();
        await _context.DisposeAsync();
    }

    public BenchLibraryDbContext GetContext() => _context;
}

[CollectionDefinition("Database collection")]
public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
{
    // This has no code, just used to define collection
}

[Collection("Database collection")]
public class ProcessRepositoryTests
{
    private readonly DatabaseFixture _fixture;

    public ProcessRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task AddProcess_WithValidData_IsPersisted()
    {
        // test code
    }
}
```

### Class organization
Organize test classes logically:
```csharp
public class ProcessServiceTests
{
    private readonly ProcessService _service;
    private readonly Mock<IRepository<Process>> _repositoryMock;

    public ProcessServiceTests()
    {
        _repositoryMock = new Mock<IRepository<Process>>();
        _service = new ProcessService(_repositoryMock.Object);
    }

    // Tests for GetProcess methods
    public class GetProcessTests
    {
        // arrange and act typically short
        // focus on assertions
    }

    // Tests for CreateProcess methods
    public class CreateProcessTests
    {
        // test code
    }

    // Tests for UpdateProcess methods
    public class UpdateProcessTests
    {
        // test code
    }

    // Tests for DeleteProcess methods
    public class DeleteProcessTests
    {
        // test code
    }
}
```

---

## AAA Pattern

All tests follow **Arrange-Act-Assert** pattern:

```csharp
[Fact]
public void CalculateCapabilityIndex_WithValidData_ReturnsCorrectValue()
{
    // ===== ARRANGE =====
    // Set up test data and mocks
    var measurements = new double[] { 1.0, 2.0, 3.0, 4.0, 5.0 };
    var specification = new ProcessSpecification
    {
        LowerSpecificationLimit = 0,
        UpperSpecificationLimit = 10
    };
    var analyzer = new ProcessAnalyzer();

    // ===== ACT =====
    // Execute the code under test
    var result = analyzer.CalculateCapabilityIndex(measurements, specification);

    // ===== ASSERT =====
    // Verify the results
    result.Should().BeGreaterThan(1.0);
    result.Should().BeLessThan(2.0);
}
```

### Arrange section
```csharp
// ✓ Good - clear setup
var measurement = new Measurement
{
    Id = 1,
    ProcessId = 1,
    Value = 25.5,
    MeasuredAt = DateTime.UtcNow
};

// ✓ Good - use builders for complex objects
var process = new ProcessBuilder()
    .WithName("Assembly Line A")
    .WithStatus(ProcessStatus.Active)
    .Build();

// ✗ Avoid - unclear magic values
var m = new Measurement { Id = 1, ProcessId = 1, Value = 25.5 };
```

### Act section
```csharp
// ✓ Good - single action, clear line
var result = _service.AnalyzeProcess(process);

// ✗ Avoid - multiple actions (separate into multiple tests)
_service.LoadProcess(process);
_service.ValidateProcess();
_service.AnalyzeProcess();
var result = _service.GetResult();
```

### Assert section
```csharp
// ✓ Good - multiple related assertions
result.Should().NotBeNull();
result.IsValid.Should().BeTrue();
result.Message.Should().Contain("Success");

// ✓ Good - fluent, readable
result.Should()
    .NotBeNull()
    .And.Subject.IsValid.Should().BeTrue();

// ✗ Avoid - too many unrelated assertions
Assert.NotNull(result);
Assert.True(result.IsValid);
Assert.Equal(42, someOtherObject.Value);
Assert.Contains("xyz", result.Description);
```

---

## Mocking & Dependencies

### Mock setup
```csharp
// ✓ Good - clear mock setup
var repositoryMock = new Mock<IRepository<Process>>();
repositoryMock
    .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
    .ReturnsAsync(new Process { Id = 1, Name = "Test" });

var service = new ProcessService(repositoryMock.Object);

// ✓ Good - strict mock (fails if unexpected calls)
var loggerMock = new Mock<ILogger<ProcessService>>(MockBehavior.Strict);

// ✓ Good - loose mock (default, ignores unexpected calls)
var cacheService = new Mock<ICacheService>(MockBehavior.Loose);
```

### Verification
```csharp
[Fact]
public async Task CreateProcess_WithValidData_CallsRepository()
{
    // arrange
    var repositoryMock = new Mock<IRepository<Process>>();
    var service = new ProcessService(repositoryMock.Object);

    // act
    await service.CreateProcessAsync(new Process { Name = "Test" });

    // assert - verify repository was called
    repositoryMock.Verify(
        r => r.AddAsync(It.Is<Process>(p => p.Name == "Test")),
        Times.Once);
}
```

### Argument matching
```csharp
// ✓ Good - specific matching
_repositoryMock.Setup(r => r.GetByIdAsync(
    It.Is<int>(id => id > 0)))
    .ReturnsAsync(new Process());

// ✓ Good - flexible matching
_repositoryMock.Setup(r => r.FindAsync(
    It.IsAny<Expression<Func<Process, bool>>>()))
    .ReturnsAsync(new List<Process>());

// ✓ Good - complex matching
_loggerMock.Setup(l => l.Log(
    It.IsAny<LogLevel>(),
    It.IsAny<EventId>(),
    It.IsAny<It.IsAnyType>(),
    It.IsAny<Exception>(),
    It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
```

### Avoid mocking everything
```csharp
// ✗ Avoid - over-mocking makes tests brittle
[Fact]
public void ProcessData_WithMocks_DoesNotTestRealLogic()
{
    var processorMock = new Mock<IProcessor>();
    var filterMock = new Mock<IFilter>();
    var transformerMock = new Mock<ITransformer>();
    var loggerMock = new Mock<ILogger>();

    processorMock.Setup(p => p.Process(It.IsAny<Data>())).Returns(new Data());
    // ... more mocks

    // This test doesn't verify actual behavior
}

// ✓ Good - mock only external dependencies
[Fact]
public void ProcessData_WithRepositoryMock_ReturnsTransformedData()
{
    var repositoryMock = new Mock<IRepository<Data>>();
    repositoryMock.Setup(r => r.GetAsync()).ReturnsAsync(new Data { Value = 5 });

    var processor = new DataProcessor(repositoryMock.Object); // real processor

    var result = processor.ProcessAndTransform();

    result.Value.Should().Be(10); // tests actual logic
}
```

---

## Test Data

### Data builders
```csharp
public class ProcessBuilder
{
    private int _id = 1;
    private string _name = "Default Process";
    private ProcessStatus _status = ProcessStatus.Active;
    private DateTime _createdAt = DateTime.UtcNow;

    public ProcessBuilder WithId(int id)
    {
        _id = id;
        return this;
    }

    public ProcessBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public ProcessBuilder WithStatus(ProcessStatus status)
    {
        _status = status;
        return this;
    }

    public ProcessBuilder WithCreatedAt(DateTime createdAt)
    {
        _createdAt = createdAt;
        return this;
    }

    public Process Build() => new()
    {
        Id = _id,
        Name = _name,
        Status = _status,
        CreatedAt = _createdAt
    };
}

// Usage
var process = new ProcessBuilder()
    .WithName("Assembly Line B")
    .WithStatus(ProcessStatus.Archived)
    .Build();
```

### Test data constants
```csharp
public static class TestData
{
    public const int ValidProcessId = 1;
    public const int InvalidProcessId = 999;
    public const string ValidProcessName = "Assembly Line A";
    public const double ValidMeasurementValue = 25.5;

    public static readonly DateTime ValidTimestamp = new DateTime(2024, 1, 15, 10, 30, 0, DateTimeKind.Utc);

    public static readonly Process[] SampleProcesses = new[]
    {
        new Process { Id = 1, Name = "Line A" },
        new Process { Id = 2, Name = "Line B" },
        new Process { Id = 3, Name = "Line C" }
    };
}

// Usage
[Fact]
public void GetProcess_WithValidId_ReturnsProcess()
{
    var result = _service.GetProcess(TestData.ValidProcessId);
    result?.Name.Should().Be(TestData.ValidProcessName);
}
```

### In-memory database for data tests
```csharp
public class ProcessRepositoryTests
{
    private readonly BenchLibraryDbContext _context;
    private readonly ProcessRepository _repository;

    public ProcessRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<BenchLibraryDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new BenchLibraryDbContext(options);
        _repository = new ProcessRepository(_context);
    }

    [Fact]
    public async Task AddProcess_WithValidData_IsPersisted()
    {
        // arrange
        var process = new Process { Name = "Test" };

        // act
        await _repository.AddAsync(process);
        await _repository.SaveChangesAsync();

        // assert
        var retrieved = await _repository.GetByIdAsync(process.Id);
        retrieved?.Name.Should().Be("Test");
    }
}
```

---

## Assertions

### FluentAssertions usage
```csharp
// ✓ Good - readable, chainable assertions
result.Should().NotBeNull()
    .And.BeOfType<ProcessResult>()
    .Which.IsValid.Should().BeTrue();

result.Value.Should().Be(25.5);
result.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));

list.Should().HaveCount(5)
    .And.AllSatisfy(m => m.Value.Should().BePositive());

items.Should().ContainSingle(i => i.Id == 1);
items.Should().BeInAscendingOrder(i => i.CreatedAt);

// ✗ Avoid - less readable assertions
Assert.NotNull(result);
Assert.IsType<ProcessResult>(result);
Assert.True(result.IsValid);
Assert.Equal(25.5, result.Value);
```

### Exception assertions
```csharp
[Fact]
public void CalculateIndex_WithNullMeasurements_ThrowsArgumentNullException()
{
    // ✓ Good
    _analyzer.Invoking(a => a.CalculateIndex(null))
        .Should().Throw<ArgumentNullException>()
        .WithParameterName("measurements");

    // ✓ Good
    var ex = Assert.Throws<ArgumentException>(
        () => _analyzer.CalculateIndex(Array.Empty<double>()));
    ex.Message.Should().Contain("empty");
}
```

### Collection assertions
```csharp
[Fact]
public void GetActiveProcesses_ReturnsOnlyActive()
{
    var results = _service.GetActiveProcesses();

    results.Should()
        .NotBeEmpty()
        .And.AllSatisfy(p => p.Status.Should().Be(ProcessStatus.Active))
        .And.BeInDescendingOrder(p => p.CreatedAt);
}
```

---

## Async Tests

### Async test patterns
```csharp
// ✓ Good - async method
[Fact]
public async Task GetProcessAsync_WithValidId_ReturnsProcess()
{
    // arrange
    var processId = 1;

    // act
    var result = await _service.GetProcessAsync(processId);

    // assert
    result.Should().NotBeNull();
}

// ✓ Good - async void (use for Task-returning, not void)
[Fact]
public async Task SaveProcessAsync_WithValidData_Succeeds()
{
    // arrange
    var process = new Process { Name = "Test" };

    // act
    var action = async () => await _service.SaveProcessAsync(process);

    // assert
    await action.Should().NotThrowAsync();
}

// ✗ Avoid - synchronous method (not async)
[Fact]
public void GetProcessAsync_Test()
{
    var result = _service.GetProcessAsync(1).Result; // blocking!
}
```

### Task assertions
```csharp
[Fact]
public async Task LongRunningOperation_CompletesWithin_SpecifiedTimeout()
{
    var operation = _service.ProcessLargeDatasetAsync();

    // ✓ Good - verify completion within timeout
    await operation.Should()
        .CompleteWithinAsync(TimeSpan.FromSeconds(5));
}
```

---

## Integration Tests with Testcontainers

**Default Approach:** All integration tests should use Testcontainers to run actual database instances in Docker containers. This ensures tests run against real database behavior, not in-memory substitutes.

### Testcontainers setup fixture
Create a base fixture for database integration tests:

```csharp
using Testcontainers.PostgreSql;
using Xunit;

namespace BenchLibrary.Data.Tests.Fixtures;

public class PostgreSqlFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase("benchlibrary_test")
        .WithUsername("testuser")
        .WithPassword("TestPassword123!")
        .Build();

    private BenchLibraryDbContext _context = null!;

    public string ConnectionString => _container.GetConnectionString();

    public BenchLibraryDbContext GetContext() => _context;

    public async Task InitializeAsync()
    {
        // Start the PostgreSQL container
        await _container.StartAsync();

        // Create DbContext with container connection string
        var options = new DbContextOptionsBuilder<BenchLibraryDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        _context = new BenchLibraryDbContext(options);

        // Apply migrations
        await _context.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        // Cleanup
        await _context.DisposeAsync();
        await _container.StopAsync();
        await _container.DisposeAsync();
    }
}

// xUnit collection definition
[CollectionDefinition("PostgreSQL Database collection")]
public class PostgreSqlCollection : ICollectionFixture<PostgreSqlFixture>
{
    // This class has no code, just used to define the collection
}
```

### Database integration tests with Testcontainers
```csharp
using Xunit;
using FluentAssertions;

namespace BenchLibrary.Data.Tests.Repositories;

[Collection("PostgreSQL Database collection")]
public class ProcessRepositoryIntegrationTests
{
    private readonly PostgreSqlFixture _fixture;
    private readonly ProcessRepository _repository;
    private readonly BenchLibraryDbContext _context;

    public ProcessRepositoryIntegrationTests(PostgreSqlFixture fixture)
    {
        _fixture = fixture;
        _context = fixture.GetContext();
        _repository = new ProcessRepository(_context);
    }

    [Fact]
    public async Task AddProcess_WithValidData_IsPersisted()
    {
        // arrange
        var process = new Process
        {
            Name = "Assembly Line A",
            Status = ProcessStatus.Active
        };

        // act
        await _repository.AddAsync(process);
        await _repository.SaveChangesAsync();

        // assert - verify it's actually in the database
        var retrieved = await _repository.GetByIdAsync(process.Id);
        retrieved.Should().NotBeNull();
        retrieved!.Name.Should().Be("Assembly Line A");
        retrieved.Status.Should().Be(ProcessStatus.Active);
    }

    [Fact]
    public async Task AddProcessWithMeasurements_PersistsHierarchy()
    {
        // arrange
        var process = new Process { Name = "Test Process" };
        var measurement = new Measurement
        {
            Value = 25.5,
            MeasuredAt = DateTime.UtcNow
        };
        process.Measurements.Add(measurement);

        // act
        await _repository.AddAsync(process);
        await _repository.SaveChangesAsync();

        // assert - verify relationship is persisted
        var retrieved = await _context.Processes
            .Include(p => p.Measurements)
            .FirstAsync(p => p.Id == process.Id);

        retrieved.Measurements.Should().HaveCount(1);
        retrieved.Measurements.First().Value.Should().Be(25.5);
    }

    [Fact]
    public async Task QueryProcess_WithComplexFilter_UsesIndexEfficiently()
    {
        // arrange - create test data
        for (int i = 0; i < 100; i++)
        {
            var process = new Process { Name = $"Process {i:D3}" };
            await _repository.AddAsync(process);
        }
        await _repository.SaveChangesAsync();

        // act - query with filter
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var results = await _repository.FindAsync(p => p.Name.Contains("Process 05"));
        stopwatch.Stop();

        // assert - verify result and performance
        results.Should().NotBeEmpty();
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(100); // index should be fast
    }
}
```

### Web API integration tests with Testcontainers
```csharp
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using FluentAssertions;

namespace BenchLibrary.Web.Tests.Integration;

[Collection("PostgreSQL Database collection")]
public class ProcessEndpointIntegrationTests : IAsyncLifetime
{
    private readonly PostgreSqlFixture _dbFixture;
    private readonly WebApplicationFactory<Program> _webFactory = null!;
    private readonly HttpClient _client = null!;
    private readonly BenchLibraryDbContext _context = null!;

    public ProcessEndpointIntegrationTests(PostgreSqlFixture dbFixture)
    {
        _dbFixture = dbFixture;
    }

    public async Task InitializeAsync()
    {
        // Create web app with testcontainers database
        _webFactory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Replace DbContext with testcontainers connection
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<BenchLibraryDbContext>));

                    if (descriptor != null)
                        services.Remove(descriptor);

                    services.AddDbContext<BenchLibraryDbContext>(options =>
                        options.UseNpgsql(_dbFixture.ConnectionString));
                });
            });

        _client = _webFactory.CreateClient();
        _context = _dbFixture.GetContext();
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        _webFactory.Dispose();
    }

    [Fact]
    public async Task GetProcess_WithValidId_Returns200()
    {
        // arrange
        var process = new Process { Name = "Test Process" };
        _context.Processes.Add(process);
        await _context.SaveChangesAsync();

        // act
        var response = await _client.GetAsync($"/api/processes/{process.Id}");

        // assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain(process.Name);
    }

    [Fact]
    public async Task CreateProcess_WithValidData_ReturnsCr eated()
    {
        // arrange
        var createRequest = new { name = "New Process" };
        var json = JsonSerializer.Serialize(createRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // act
        var response = await _client.PostAsync("/api/processes", content);

        // assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);

        // verify it was actually saved
        var processes = await _context.Processes
            .Where(p => p.Name == "New Process")
            .ToListAsync();
        processes.Should().ContainSingle();
    }
}
```

### Benefits of Testcontainers
- ✅ **Real database behavior** - Tests run against actual PostgreSQL, not in-memory substitute
- ✅ **Isolated tests** - Each test run gets a clean database instance
- ✅ **No side effects** - Containers are destroyed after tests
- ✅ **Catches database issues** - Migrations, constraints, triggers all tested
- ✅ **Production-like** - Database version matches production
- ✅ **Parallel execution** - Multiple container instances run simultaneously

---

## Regression Tests for Customer Issues

**Standard Practice:** When a customer complaint, warranty return, or root cause failure is identified, a regression test must be created that reproduces the issue. These tests ensure the bug never resurfaces and provide documentation of the problem.

### Creating a regression test

**Step 1: Document the issue**
```csharp
// Create test file with issue reference
namespace BenchLibrary.Data.Tests.RegressionTests;

/// <summary>
/// Regression tests for customer issues and warranty returns.
/// These tests document real-world failures and ensure they don't resurface.
/// </summary>
public class CustomerIssueRegressionTests
{
    // Issue: GitHub #234, Warranty Return WR-2024-001
    // Customer: Acme Manufacturing
    // Problem: Capability index calculation returns negative value for valid data
    // Root Cause: Division by zero when standard deviation is zero
    // Status: Fixed in v1.1.0
    [Fact]
    public void CalculateCapabilityIndex_WithZeroStandardDeviation_ReturnsZeroNotNegative()
    {
        // arrange - constant measurements produce zero standard deviation
        var measurements = new[] { 25.0, 25.0, 25.0, 25.0, 25.0 };
        var specification = new ProcessSpecification
        {
            LowerSpecificationLimit = 20.0,
            UpperSpecificationLimit = 30.0,
            TargetValue = 25.0
        };
        var analyzer = new ProcessAnalyzer(_logger);

        // act
        var result = analyzer.CalculateCapabilityIndex(measurements, specification);

        // assert - should not be negative, should handle edge case gracefully
        result.Should().BeGreaterThanOrEqualTo(0);
        result.Should().NotBeNaN();
    }
}
```

### Guidelines for regression tests

**Test naming with issue references:**
```csharp
// ✓ Good - includes issue reference
public void FeatureName_CustomerScenario_IssueNumber()
{
    // test code
}

// ✓ Good - descriptive with context
public void ProcessAnalyzer_NegativeCapabilityIndexBug_GH234()
{
    // GitHub issue #234 context
}

public void DatabaseMigration_SQLiteDateTime_WR202401()
{
    // Warranty return WR-2024-001 context
}
```

### Test case template
```csharp
/// <summary>
/// Regression test for [Issue Reference]
/// Customer: [Customer Name]
/// Original Problem: [Brief description of what customer experienced]
/// Root Cause: [Why the bug occurred]
/// Fix Implemented: [What was changed to fix it]
/// Status: [Fixed in vX.X.X / Pending]
/// </summary>
[Fact]
[Trait("Category", "Regression")]
[Trait("Issue", "GH-234")]
public async Task FeatureName_CustomerScenario_IsReproducedAndFixed()
{
    // arrange - reproduce exact conditions from customer report
    var testData = new TestDataBuilder()
        .WithCustomerConditions() // exact scenario from report
        .Build();

    var service = new CustomerService(_dbFixture.GetContext());

    // act
    var result = await service.PerformOperationAsync(testData);

    // assert - verify bug is fixed
    result.Should().NotBeNull();
    result.IsValid.Should().BeTrue(); // previously failed here
    result.Value.Should().NotBeNegative(); // previously returned -1
}
```

### Organization of regression tests

Create dedicated regression test files:
```
tests/
├── BenchLibrary.Core.Tests/
│   ├── RegressionTests/
│   │   ├── CustomerIssueRegressionTests.cs
│   │   ├── WarrantyReturnRegressionTests.cs
│   │   └── RootCauseAnalysisTests.cs
│
├── BenchLibrary.Data.Tests/
│   ├── RegressionTests/
│   │   ├── MigrationRegressionTests.cs
│   │   └── DatabaseConstraintRegressionTests.cs
│
└── BenchLibrary.Web.Tests/
    └── RegressionTests/
        └── UIBugRegressionTests.cs
```

### Marking regression tests
```csharp
[Trait("Category", "Regression")]
[Trait("Issue", "GH-234")]
[Trait("Severity", "High")] // Critical, High, Medium, Low
[Trait("Customer", "Acme Manufacturing")]
[Trait("FixedIn", "v1.1.0")]
public async Task TestMethod()
{
    // test code
}
```

### Running regression tests
```bash
# Run all regression tests
dotnet test --filter "Category=Regression"

# Run regression tests for specific issue
dotnet test --filter "Issue=GH-234"

# Run critical regression tests
dotnet test --filter "Severity=Critical"

# Run regressions fixed in specific version
dotnet test --filter "FixedIn=v1.1.0"
```

### Process for handling customer issues

1. **Report Received**: Customer reports issue or warranty return
2. **Triage**: Root cause analysis performed
3. **Test First**: Before fixing, create failing regression test
4. **Fix**: Implement fix that makes regression test pass
5. **Verify**: Regression test passes + related tests pass + coverage maintained
6. **Document**: Tag test with issue reference, customer, and severity
7. **Prevent**: Ensure test prevents future regressions

### Real-world regression test examples

**Example 1: Database datetime issue**
```csharp
// Issue: GH-156, Warranty Return WR-2024-002
// Problem: SQLite datetime serialization loses milliseconds, causing measurement timestamps to be off
[Fact]
[Trait("Category", "Regression")]
[Trait("Issue", "GH-156")]
public async Task SaveMeasurement_WithMilliseconds_PreservesAccuracy()
{
    // arrange
    var originalTime = new DateTime(2024, 1, 15, 10, 30, 45, 123, DateTimeKind.Utc);
    var measurement = new Measurement
    {
        Value = 25.5,
        MeasuredAt = originalTime
    };

    var context = _fixture.GetContext();
    context.Measurements.Add(measurement);
    await context.SaveChangesAsync();

    // act
    var retrieved = await context.Measurements.FirstAsync();

    // assert - verify milliseconds preserved
    retrieved.MeasuredAt.Millisecond.Should().Be(123);
    retrieved.MeasuredAt.Should().Be(originalTime);
}
```

**Example 2: Numerical precision issue**
```csharp
// Issue: GH-187, Customer: Precision Electronics
// Problem: Capability index calculation loses precision for very small tolerances
[Fact]
[Trait("Category", "Regression")]
[Trait("Issue", "GH-187")]
[Trait("Severity", "High")]
public void CalculateCapabilityIndex_WithSmallTolerance_MaintainsPrecision()
{
    // arrange
    var measurements = new[] { 0.00001, 0.00002, 0.00003 };
    var specification = new ProcessSpecification
    {
        LowerSpecificationLimit = 0.000005,
        UpperSpecificationLimit = 0.000035,
        TargetValue = 0.000020
    };

    var analyzer = new ProcessAnalyzer(_logger);

    // act
    var result = analyzer.CalculateCapabilityIndex(measurements, specification);

    // assert - 4 decimal places precision
    result.Should().BeApproximatelyEqual(1.33, 0.01m);
    result.Should().NotBeNaN();
    result.Should().NotBe(double.PositiveInfinity);
}
```

### Benefits of regression tests
- ✅ **Prevents re-occurrence** - Bug can't happen twice
- ✅ **Documents issues** - Real customer problems recorded
- ✅ **Quality metric** - Tracks how many bugs have been fixed
- ✅ **Regression coverage** - Identifies patterns in failures
- ✅ **Customer confidence** - Shows issues are taken seriously
- ✅ **Knowledge base** - Future team members learn from past issues

---

## Code Coverage

### Coverage target
**Minimum 80% code coverage** for production code.

### Measuring coverage
```bash
# Run tests with coverage
dotnet test /p:CollectCoverage=true /p:CoverageFormat=lcov

# View coverage report
dotnet test /p:CollectCoverage=true /p:CoverageFormat=opencover
```

### Coverage configuration (.editorconfig)
```ini
# Exclude from coverage
[**/Generated/**/*.cs]
generated_code = true

[**/Models/**/*.cs]
# Auto-generated models don't need coverage
```

### What to test
```csharp
// ✓ ALWAYS test
// - Business logic and calculations
// - Validation and error conditions
// - Public APIs
// - Complex decision branches

[Fact]
public void CalculateCapabilityIndex_WithVariousInputs_ReturnsCorrectResults()
{
    // Multiple test cases for business logic
}

// ✓ OFTEN test
// - State changes
// - Exception scenarios
// - Integration between components

[Fact]
public void UpdateProcess_WithInvalidStatus_ThrowsException()
{
}

// ✗ DON'T necessarily test
// - Trivial getters/setters
// - Third-party library code
// - Dependency injection setup
// - Boilerplate (attributes, implicit properties)
```

---

## Key Rules Summary

✓ **DO:**
- Use `MethodName_Scenario_ExpectedResult` naming
- Follow AAA pattern consistently
- Use FluentAssertions for readable tests
- Mock external dependencies only
- Use builders for complex test data
- Verify async operations with async tests
- Test both happy paths and error cases
- Keep tests focused on single behavior
- **Use Testcontainers for all integration tests** (default approach)
- **Create regression tests for every customer issue/warranty return**
- **Mark regression tests with Traits (Category, Issue, Severity)**
- Aim for 80%+ code coverage

✗ **DON'T:**
- Use vague test names
- Mix Arrange/Act/Assert
- Over-mock (tests become brittle)
- Test implementation details
- Have more than 3 assertions unrelated to core test
- Block on async operations (`.Result`)
- Create shared state between tests
- Skip error case testing
- Test framework code
- Use in-memory databases for integration tests (use Testcontainers)
- Fix customer issues without creating regression tests first
- Forget to tag regression tests with issue references

