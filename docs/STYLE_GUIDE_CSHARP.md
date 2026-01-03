# C# Code Style Guide

**Language Version:** C# 12
**Target Framework:** .NET 8 LTS
**Last Updated:** 2026-01-03

---

## Table of Contents

1. [Naming Conventions](#naming-conventions)
2. [Code Formatting](#code-formatting)
3. [Language Features & Patterns](#language-features--patterns)
4. [Nullability](#nullability)
5. [Async/Await](#asyncawait)
6. [LINQ](#linq)
7. [Comments & Documentation](#comments--documentation)
8. [Error Handling](#error-handling)
9. [Performance](#performance)
10. [Code Organization](#code-organization)

---

## Naming Conventions

All naming follows **Microsoft C# Naming Conventions**:

### Type Names (PascalCase)
```csharp
public class BenchmarkResult { }
public interface IStatisticalAnalysis { }
public enum ProcessCapability { }
public record MeasurementData { }
```

### Method Names (PascalCase)
```csharp
public void CalculateCapabilityIndex() { }
public async Task ProcessMeasurementsAsync() { }
public int GetSampleCount() { }
```

### Property Names (PascalCase)
```csharp
public string ProcessName { get; set; }
public int SampleSize { get; private set; }
public DateTime CreatedAt { get; init; }
```

### Local Variables & Parameters (camelCase)
```csharp
var processData = new List<Measurement>();
int sampleCount = measurements.Count;
void ProcessItems(string itemName, int itemId) { }
```

### Private Fields (camelCase with leading underscore)
```csharp
private string _instrumentId;
private readonly ILogger<ProcessAnalyzer> _logger;
private int _sampleBufferSize = 100;
```

### Constant Names (PascalCase)
```csharp
public const int DefaultBufferSize = 1024;
private const string DefaultDatabaseProvider = "PostgreSQL";
```

### Acronyms
Keep acronyms as **PascalCase words**, not ALL_CAPS:
```csharp
// ✓ Good
public class SpcCalculator { }
public interface IScpiInstrument { }

// ✗ Avoid
public class SPCCalculator { }
public interface ISCPIInstrument { }
```

### Async Method Suffix
Always use `Async` suffix for asynchronous methods:
```csharp
public async Task<ProcessResult> AnalyzeProcessAsync() { }
public async Task SaveMeasurementsAsync(IEnumerable<Measurement> measurements) { }
```

---

## Code Formatting

### Line Length
Target **100-120 characters** per line. Break up longer lines for readability:
```csharp
// ✓ Good
var capabilityIndex = analyzer.CalculateCapabilityIndex(
    measurements,
    specification);

// ✗ Avoid
var capabilityIndex = analyzer.CalculateCapabilityIndex(measurements, specification);
```

### Indentation
Use **4 spaces** (enforced by `.editorconfig`). No tabs.

### Braces
Use **Allman style** (opening brace on new line):
```csharp
public void AnalyzeProcess()
{
    if (measurements.Count == 0)
    {
        return;
    }

    foreach (var measurement in measurements)
    {
        ProcessMeasurement(measurement);
    }
}
```

### Spacing
```csharp
// Around operators
int result = x + y;
string message = $"Result: {result}";

// Inside generics (no spaces)
var data = new Dictionary<string, int>();
List<Measurement> measurements = new();

// Inside method calls (no spaces)
CalculateIndex(x, y, z);

// Control flow statements
if (condition)
{
    // code
}

for (int i = 0; i < count; i++)
{
    // code
}
```

### Blank Lines
- One blank line between method definitions
- One blank line between logical sections within a method
- Two blank lines between class definitions in the same file (avoid this; use one file per class)

---

## Language Features & Patterns

### Use Modern C# Features (C# 12)

#### Top-level statements (only for console apps/scripts)
```csharp
// ✓ In Program.cs
var services = new ServiceCollection();
services.AddScoped<IProcessAnalyzer, ProcessAnalyzer>();
var provider = services.BuildServiceProvider();
```

#### File-scoped namespace (prefer over block-scoped)
```csharp
// ✓ Good
namespace BenchLibrary.SixSigma;

public class ProcessAnalyzer
{
    // code
}
```

#### Records for data transfer & immutable types
```csharp
// ✓ Good for DTOs and value types
public record MeasurementData(
    string PartId,
    double Value,
    DateTime Timestamp);

// ✓ Good for immutable data classes
public record ProcessSpecification
{
    public required double LowerSpecificationLimit { get; init; }
    public required double UpperSpecificationLimit { get; init; }
    public required double TargetValue { get; init; }
}
```

#### Init-only properties
```csharp
public class BenchmarkConfiguration
{
    public required string ProcessName { get; init; }
    public int SampleSize { get; init; } = 30;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
```

#### Required keyword
```csharp
public class Instrument
{
    public required string Address { get; set; }
    public required InstrumentType Type { get; set; }
    public int? Timeout { get; set; } // optional
}
```

#### Target-typed new
```csharp
// ✓ Good
ProcessAnalyzer analyzer = new();
List<Measurement> measurements = new();

// ✗ Avoid (redundant type information)
ProcessAnalyzer analyzer = new ProcessAnalyzer();
```

#### Range operator for collections
```csharp
// ✓ Good
var firstTen = measurements[..10];
var lastFive = measurements[^5..];
```

#### Collection expressions (C# 12)
```csharp
// ✓ Good
int[] numbers = [1, 2, 3, 4, 5];
var list = [..existingList, ..newItems];
var dict = new Dictionary<string, int> { ["key"] = 42 };
```

#### Expression-bodied members (use appropriately)
```csharp
// ✓ Good for simple, single-line logic
public int GetSampleCount() => measurements.Count;
public bool IsValid => Value >= 0 && Value <= 100;
public string GetDisplayName() => $"{ProcessName} - {DateTime.UtcNow:yyyy-MM-dd}";

// ✗ Avoid for complex logic
public ProcessResult Analyze() => // too complex for expression body
{
    ValidateMeasurements();
    var stats = CalculateStatistics();
    return TransformResult(stats);
};
```

### Dependency Injection
Use constructor injection exclusively:
```csharp
public class ProcessAnalyzer
{
    private readonly ILogger<ProcessAnalyzer> _logger;
    private readonly IRepository<Measurement> _repository;

    public ProcessAnalyzer(
        ILogger<ProcessAnalyzer> logger,
        IRepository<Measurement> repository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }
}
```

### String Interpolation
Always use string interpolation over concatenation:
```csharp
// ✓ Good
var message = $"Processing {processName} with {sampleCount} samples";

// ✗ Avoid
var message = "Processing " + processName + " with " + sampleCount + " samples";

// ✓ Use expression interpolation
var timestamp = $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] {message}";
```

---

## Nullability

**Enable `<Nullable>enable</Nullable>` globally** (configured in `Directory.Build.props`).

### Nullable Reference Types
```csharp
// ✓ Good - explicit about nullability
public string? GetProcessName(int id)
{
    return _repository.Find(id)?.Name;
}

public required string ProcessName { get; set; } // never null
public string? OptionalDescription { get; set; } // can be null

// ✗ Avoid using non-nullable without null-checks
public string ProcessName { get; set; } // warning if assigned null
```

### Null-coalescing & null-conditional operators
```csharp
// ✓ Good
var name = measurement?.ProcessName ?? "Unknown";
var count = items?.Count ?? 0;
measurements?.ForEach(m => ProcessMeasurement(m));
```

### Null argument checks
Use `ArgumentNullException.ThrowIfNull()` (C# 11+):
```csharp
public void ProcessMeasurements(IEnumerable<Measurement> measurements)
{
    ArgumentNullException.ThrowIfNull(measurements);
    // rest of method
}
```

---

## Async/Await

### Always use async when available
```csharp
// ✓ Good
public async Task<ProcessResult> AnalyzeAsync()
{
    var data = await _repository.GetMeasurementsAsync();
    return ProcessData(data);
}

// ✗ Avoid blocking calls
public ProcessResult Analyze()
{
    var data = _repository.GetMeasurementsAsync().Result; // blocks!
}
```

### ConfigureAwait for libraries
Use `ConfigureAwait(false)` in libraries to avoid capturing UI context:
```csharp
// ✓ Good in libraries
public async Task<ProcessResult> AnalyzeAsync()
{
    var data = await _repository.GetMeasurementsAsync().ConfigureAwait(false);
    return ProcessData(data);
}

// ✗ Not needed in web/UI applications
```

### Avoid async void
Only use async void for event handlers:
```csharp
// ✓ Good - event handler
private async void OnButtonClickAsync(object sender, EventArgs e)
{
    await ProcessAsync();
}

// ✗ Avoid - use Task return
public async void ProcessAsync() { } // bad pattern

// ✓ Good - use Task return
public async Task ProcessAsync() { }
```

---

## LINQ

### Prefer method syntax over query syntax
```csharp
// ✓ Good
var results = measurements
    .Where(m => m.Value > 0)
    .OrderBy(m => m.Timestamp)
    .Select(m => new { m.ProcessName, m.Value })
    .ToList();

// ✗ Avoid query syntax (harder to follow)
var results = (from m in measurements
               where m.Value > 0
               orderby m.Timestamp
               select new { m.ProcessName, m.Value }).ToList();
```

### Defer materialization
```csharp
// ✓ Good - lazy evaluation
IEnumerable<Measurement> activeMeasurements = measurements.Where(m => m.IsActive);

// Only materialize when needed
var count = activeMeasurements.Count(); // evaluates now
var list = activeMeasurements.ToList(); // explicit materialization
```

### Performance-conscious LINQ
```csharp
// ✓ Good - efficient
var highValueItems = measurements
    .Where(m => m.Value > threshold) // filter early
    .OrderBy(m => m.Timestamp)
    .Take(100) // limit results
    .ToList();

// ✗ Avoid - processes all items
var highValueItems = measurements
    .OrderBy(m => m.Timestamp)
    .ToList()
    .Where(m => m.Value > threshold)
    .Take(100)
    .ToList();
```

---

## Comments & Documentation

### XML Documentation
Add XML documentation to all **public** members:
```csharp
/// <summary>
/// Calculates the process capability index (Cpk) based on measurements.
/// </summary>
/// <param name="measurements">Collection of process measurements.</param>
/// <param name="specification">Upper and lower specification limits.</param>
/// <returns>The calculated capability index value.</returns>
/// <exception cref="ArgumentNullException">Thrown when measurements or specification is null.</exception>
/// <exception cref="InvalidOperationException">Thrown when measurements collection is empty.</exception>
public double CalculateCapabilityIndex(
    IEnumerable<Measurement> measurements,
    ProcessSpecification specification)
{
    ArgumentNullException.ThrowIfNull(measurements);
    ArgumentNullException.ThrowIfNull(specification);
    // implementation
}
```

### Code Comments (use sparingly)
Only comment **why**, not **what**:
```csharp
// ✓ Good - explains intent
// Validate measurements before statistical analysis to avoid
// skewed results from outliers or instrument errors
if (HasOutliers(measurements))
{
    RemoveOutliers(measurements);
}

// ✗ Bad - states the obvious
// Remove outliers
RemoveOutliers(measurements);

// ✓ Good - explains non-obvious logic
// Use exponential moving average for real-time trending
// instead of simple average to weight recent data more heavily
double trend = CalculateEMA(recentMeasurements);
```

### TODO comments
Use structured TODO comments with context:
```csharp
// TODO: [Feature] Implement instrument timeout retry logic (Issue #42)
// TODO: [Performance] Cache statistical calculations for repeated queries (Issue #89)
// TODO: [Bug] Fix datetime serialization for SQLite (Issue #15)
```

---

## Error Handling

### Use specific exceptions
```csharp
// ✓ Good
public void UpdateMeasurement(int id, double value)
{
    if (id <= 0)
        throw new ArgumentOutOfRangeException(nameof(id), "ID must be positive");

    if (value < 0)
        throw new ArgumentException("Measurement value cannot be negative", nameof(value));

    var measurement = _repository.Find(id)
        ?? throw new InvalidOperationException($"Measurement {id} not found");
}

// ✗ Avoid generic exceptions
public void UpdateMeasurement(int id, double value)
{
    if (id <= 0 || value < 0)
        throw new Exception("Invalid input"); // too generic
}
```

### Async exception handling
```csharp
// ✓ Good - use try-catch with async
public async Task<ProcessResult> AnalyzeAsync()
{
    try
    {
        var data = await _repository.GetMeasurementsAsync();
        return ProcessData(data);
    }
    catch (RepositoryException ex)
    {
        _logger.LogError(ex, "Failed to retrieve measurements");
        throw;
    }
}

// ✓ Good - use guard clauses
public async Task<ProcessResult> AnalyzeAsync()
{
    var data = await _repository.GetMeasurementsAsync();

    if (data is null || data.Count == 0)
        return ProcessResult.Empty;

    return ProcessData(data);
}
```

---

## Performance

### Avoid allocations in hot paths
```csharp
// ✓ Good - reuse collections
public class MeasurementProcessor
{
    private readonly List<double> _buffer = new(capacity: 1000);

    public void ProcessBatch(IEnumerable<Measurement> measurements)
    {
        _buffer.Clear();
        _buffer.AddRange(measurements.Select(m => m.Value));
        AnalyzeBuffer(_buffer);
    }
}

// ✗ Avoid - allocates new list each time
public void ProcessBatch(IEnumerable<Measurement> measurements)
{
    var buffer = new List<double>(measurements.Select(m => m.Value).ToList());
    AnalyzeBuffer(buffer);
}
```

### Use appropriate collection types
```csharp
// ✓ Good - use right collection for use case
var orderedItems = new SortedSet<Measurement>(); // sorted access
var uniqueItems = new HashSet<string>(); // fast lookup
var sequentialItems = new Queue<Measurement>(); // FIFO
var measurements = new List<Measurement>(); // general list
```

### Cache expensive calculations
```csharp
// ✓ Good - cache with invalidation
public class ProcessAnalyzer
{
    private double? _cachedCapability;
    private IEnumerable<Measurement>? _cachedMeasurements;

    public double GetCapabilityIndex(IEnumerable<Measurement> measurements)
    {
        if (_cachedMeasurements == measurements && _cachedCapability.HasValue)
            return _cachedCapability.Value;

        _cachedCapability = CalculateCapabilityIndex(measurements);
        _cachedMeasurements = measurements;
        return _cachedCapability.Value;
    }
}
```

---

## Code Organization

### Class organization
Organize class members in this order:
1. Constants and static fields
2. Private fields
3. Properties (public, then private/internal)
4. Constructors
5. Public methods
6. Internal/private methods
7. Nested classes/enums

```csharp
public class ProcessAnalyzer
{
    // Constants
    private const int MinimumSampleSize = 5;

    // Static fields
    private static readonly Dictionary<string, ProcessAnalyzer> _cache = new();

    // Instance fields
    private readonly IRepository<Measurement> _repository;
    private readonly ILogger<ProcessAnalyzer> _logger;

    // Properties
    public string ProcessName { get; set; }
    public int SampleSize { get; private set; }

    // Constructor
    public ProcessAnalyzer(
        IRepository<Measurement> repository,
        ILogger<ProcessAnalyzer> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    // Public methods
    public async Task<ProcessResult> AnalyzeAsync()
    {
        ValidateMeasurements();
        return await CalculateResultAsync();
    }

    // Private methods
    private void ValidateMeasurements()
    {
        // implementation
    }

    // Nested types
    private enum ProcessState
    {
        Idle,
        Running,
        Complete
    }
}
```

### File structure
- **One public type per file** (class, interface, enum, record)
- **Exception**: nested private/internal types are OK
- **Filename matches type name**: `ProcessAnalyzer.cs` contains `class ProcessAnalyzer`

### Namespace organization
Follow project structure:
```
BenchLibrary.SixSigma/
├── Analyzers/
│   ├── ProcessAnalyzer.cs
│   ├── CapabilityAnalyzer.cs
├── Models/
│   ├── ProcessResult.cs
│   ├── Measurement.cs
├── Exceptions/
│   ├── InvalidProcessException.cs
```

Namespace structure:
```csharp
namespace BenchLibrary.SixSigma.Analyzers;
namespace BenchLibrary.SixSigma.Models;
namespace BenchLibrary.SixSigma.Exceptions;
```

---

## Key Rules Summary

✓ **DO:**
- Use PascalCase for types, methods, properties
- Use camelCase for local variables and parameters
- Use `_camelCase` for private fields
- Implement nullable reference types
- Use async/await patterns
- Write XML documentation for public APIs
- Use string interpolation
- Prefer method LINQ syntax
- Use dependency injection via constructor
- Comment the "why", not the "what"

✗ **DON'T:**
- Use ALL_CAPS for constants (use PascalCase)
- Mix case styles in the same type
- Use async void except for event handlers
- Block on async operations with `.Result`
- Use generic `Exception` type
- Create types larger than 300-400 lines
- Include multiple public types in one file
- Omit XML documentation on public members
- Use `goto` statements
- Create overly complex expressions

