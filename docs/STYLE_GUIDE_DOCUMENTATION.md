# Documentation Standards

**Format:** Markdown + XML Comments
**Tools:** GitHub Wiki, Swagger/OpenAPI
**Audience:** Developers, API users, contributors
**Last Updated:** 2026-01-03

---

## Table of Contents

1. [XML Documentation Comments](#xml-documentation-comments)
2. [README Standards](#readme-standards)
3. [Architecture Documentation](#architecture-documentation)
4. [API Documentation](#api-documentation)
5. [Developer Guides](#developer-guides)
6. [Code Examples](#code-examples)
7. [Changelog](#changelog)
8. [Inline Comments](#inline-comments)

---

## XML Documentation Comments

### Required documentation
Document all **public** members:
```csharp
/// <summary>
/// Calculates the process capability index (Cpk).
/// </summary>
public class ProcessAnalyzer
{
    /// <summary>
    /// Initializes a new instance of the ProcessAnalyzer class.
    /// </summary>
    /// <param name="logger">Logger instance for diagnostic output.</param>
    /// <exception cref="ArgumentNullException">Thrown when logger is null.</exception>
    public ProcessAnalyzer(ILogger<ProcessAnalyzer> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
    }

    /// <summary>
    /// Calculates the process capability index based on measurements.
    /// </summary>
    /// <param name="measurements">Collection of process measurements.</param>
    /// <param name="specification">Specification limits for the process.</param>
    /// <returns>
    /// The calculated capability index value. Values above 1.33 indicate
    /// an acceptable process capable of meeting specifications.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when measurements or specification is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when measurements collection is empty.
    /// </exception>
    /// <remarks>
    /// The Cpk calculation assumes a normal distribution. For non-normal
    /// distributions, consider alternative capability metrics.
    /// </remarks>
    public double CalculateCapabilityIndex(
        IEnumerable<double> measurements,
        ProcessSpecification specification)
    {
        // implementation
    }
}
```

### XML tags reference

**`<summary>`** - Brief description (1-2 sentences)
```csharp
/// <summary>
/// Validates process measurements for statistical analysis.
/// </summary>
```

**`<param>`** - Parameter documentation
```csharp
/// <param name="measurements">Raw measurement data from instruments.</param>
/// <param name="threshold">Minimum acceptable measurement value.</param>
```

**`<returns>`** - Return value documentation
```csharp
/// <returns>
/// A ProcessResult object containing calculated statistics and validation status.
/// </returns>
```

**`<exception>`** - Exception documentation (use for all thrown exceptions)
```csharp
/// <exception cref="ArgumentNullException">Thrown when measurements is null.</exception>
/// <exception cref="InvalidOperationException">Thrown when measurements is empty.</exception>
```

**`<remarks>`** - Additional context and important notes
```csharp
/// <remarks>
/// This method performs in-memory calculations and is suitable for
/// datasets up to 1 million measurements. For larger datasets, consider
/// streaming or database-based approaches.
/// </remarks>
```

**`<example>`** - Code examples
```csharp
/// <example>
/// <code>
/// var analyzer = new ProcessAnalyzer(logger);
/// var cpk = analyzer.CalculateCapabilityIndex(measurements, spec);
/// if (cpk > 1.33)
/// {
///     Console.WriteLine("Process is capable");
/// }
/// </code>
/// </example>
```

**`<see cref>`** - Cross-references
```csharp
/// <summary>
/// Validates measurements. See also <see cref="ProcessSpecification"/>.
/// </summary>

/// <remarks>
/// Use <see cref="CalculateCapabilityIndex"/> for advanced metrics.
/// </remarks>
```

**`<seealso cref>`** - Related topics
```csharp
/// <seealso cref="ProcessValidator"/>
/// <seealso cref="StatisticalAnalyzer"/>
```

### Null and optional parameters
```csharp
/// <param name="description">
/// Optional description of the process. Can be null or empty.
/// </param>
public void SetDescription(string? description)
{
    // implementation
}

/// <param name="timeout">
/// Timeout in milliseconds. If null, uses default timeout of 30 seconds.
/// </param>
public async Task ProcessAsync(int? timeout = null)
{
    // implementation
}
```

---

## README Standards

### Root README.md
Should be the project overview and quick start:

```markdown
# BenchLibrary

[![Build](https://github.com/YourOrg/BenchLibrary/workflows/Build/badge.svg)](...)
[![Test Coverage](https://codecov.io/gh/YourOrg/BenchLibrary/branch/main/graph/badge.svg)](...)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

Brief, compelling description of BenchLibrary and its purpose.

## Quick Start

### Installation

```bash
dotnet add package BenchLibrary.Core
```

### Basic Usage

```csharp
using BenchLibrary.SixSigma;

var analyzer = new ProcessAnalyzer(logger);
var cpk = analyzer.CalculateCapabilityIndex(measurements, specification);
```

## Features

- ✨ Process capability analysis (Cpk, Cp calculations)
- 📊 Statistical process control with control charts
- 🔌 SCPI instrument integration (Phase 2)
- 🚀 Dashboard UI with real-time updates
- 📱 Edge device support (Phase 2)

## Documentation

- [Getting Started](docs/GETTING_STARTED.md)
- [API Reference](docs/API.md)
- [Contributing](CONTRIBUTING.md)
- [Style Guides](docs/STYLE_GUIDES.md)

## Requirements

- .NET 8.0 LTS or later
- PostgreSQL 13+ (production)
- SQLite 3.40+ (development)

## License

Licensed under the MIT License. See [LICENSE](LICENSE) file.
```

### Project README.md
Each major project should have a README in its directory:

```markdown
# BenchLibrary.SixSigma

Statistical Process Control (SPC) and Six Sigma analysis library.

## Contents

- **Analyzers** - Core SPC calculation algorithms
- **Models** - Data structures for process data
- **Exceptions** - Domain-specific exceptions
- **Extensions** - Utility methods

## Key Classes

### ProcessAnalyzer
Main class for capability analysis.

```csharp
var analyzer = new ProcessAnalyzer(logger);
var result = analyzer.AnalyzeProcess(measurements, specs);
```

### ControlChartCalculator
Generates control limits for Shewhart charts.

## Dependencies

- None (minimal dependencies for core library)

## Testing

```bash
dotnet test --project tests/BenchLibrary.SixSigma.Tests
```
```

---

## Architecture Documentation

### Design decisions document
Create `docs/DESIGN.md` for architectural decisions:

```markdown
# Architecture & Design Decisions

## Layered Architecture

BenchLibrary uses a layered architecture:

```
┌─────────────────────────────────────┐
│         Web UI (Blazor Server)      │
├─────────────────────────────────────┤
│        Application Services         │
├─────────────────────────────────────┤
│    Domain Logic (SixSigma, Core)    │
├─────────────────────────────────────┤
│      Data Access (EF Core)          │
├─────────────────────────────────────┤
│      PostgreSQL / SQLite            │
└─────────────────────────────────────┘
```

### Benefits
- Clear separation of concerns
- Testable business logic
- Database-agnostic domain layer
- Easy to add new frontends

## Entity Framework Core

### Why EF Core?
- Excellent .NET ecosystem integration
- LINQ support for queries
- Database migrations built-in
- Support for both PostgreSQL and SQLite

### Key patterns
- Repository pattern for data access
- Owned entities for value objects
- Global query filters for soft deletes
- Audit entities with CreatedAt/UpdatedAt

## Dependency Injection

All services use constructor injection via `IServiceCollection`.

### Service registration example
```csharp
services.AddScoped<IProcessService, ProcessService>();
services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
```

### Benefits
- Easy to mock for testing
- Clear dependency visibility
- Container manages lifetimes
```

### Component diagrams
Use ASCII art or tools like PlantUML:

```markdown
## Process Analysis Flow

```
┌──────────────┐
│  Measurements│  Raw data from instruments
└──────┬───────┘
       │
       v
┌──────────────────┐
│   Validation     │  Check for outliers, nulls
└──────┬───────────┘
       │
       v
┌──────────────────┐
│ Statistical      │  Calculate mean, std dev
│ Analysis         │
└──────┬───────────┘
       │
       v
┌──────────────────┐
│ Capability       │  Cpk, Cp, Pp, Ppk
│ Calculation      │
└──────┬───────────┘
       │
       v
┌──────────────────┐
│ Results & Report │  Display to user
└──────────────────┘
```
```

---

## API Documentation

### REST API documentation
Use Swagger/OpenAPI annotations:

```csharp
/// <summary>
/// Gets a process by ID.
/// </summary>
/// <param name="id">The process ID.</param>
/// <returns>Process details if found.</returns>
/// <response code="200">Process found and returned.</response>
/// <response code="404">Process not found.</response>
[HttpGet("{id}")]
[ProducesResponseType(typeof(ProcessDto), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<ActionResult<ProcessDto>> GetProcess(int id)
{
    var process = await _service.GetProcessAsync(id);
    if (process == null)
        return NotFound();

    return Ok(_mapper.Map<ProcessDto>(process));
}
```

### Markdown API documentation
Create `docs/API.md`:

```markdown
# API Reference

## Processes

### Get Process
Get a single process by ID.

**Endpoint:** `GET /api/processes/{id}`

**Parameters:**
- `id` (path, required): Process ID

**Response:**
```json
{
  "id": 1,
  "name": "Assembly Line A",
  "status": "Active",
  "createdAt": "2024-01-15T10:30:00Z"
}
```

**Status Codes:**
- `200 OK` - Process found
- `404 Not Found` - Process does not exist

### List Processes
Get all processes with pagination.

**Endpoint:** `GET /api/processes?page=1&pageSize=10`

**Query Parameters:**
- `page` (optional): Page number, default 1
- `pageSize` (optional): Items per page, default 20
- `status` (optional): Filter by status

**Response:**
```json
{
  "items": [...],
  "totalCount": 42,
  "page": 1,
  "pageSize": 20
}
```
```

---

## Developer Guides

### Getting Started Guide
Create `docs/GETTING_STARTED.md`:

```markdown
# Getting Started with BenchLibrary

## Prerequisites

- .NET 8.0 SDK or later
- Visual Studio 2022 or VS Code
- PostgreSQL 13+ (for production builds)
- Git

## Development Setup

### 1. Clone the repository
```bash
git clone https://github.com/YourOrg/BenchLibrary.git
cd BenchLibrary
```

### 2. Restore dependencies
```bash
dotnet restore
```

### 3. Setup database

For development, SQLite is automatically configured.

### 4. Run tests
```bash
dotnet test
```

### 5. Run the application
```bash
dotnet run --project src/BenchLibrary.Web
```

Navigate to `https://localhost:7001` in your browser.

## Project Structure

- `src/` - Source code organized by domain
- `tests/` - Unit and integration tests
- `docs/` - Documentation
- `samples/` - Example projects

## Common Tasks

### Creating a new feature

1. Create a feature branch: `git checkout -b feature/my-feature`
2. Write tests first (TDD)
3. Implement the feature
4. Ensure all tests pass: `dotnet test`
5. Submit a pull request

### Adding a database migration

```bash
cd src/BenchLibrary.Data
dotnet ef migrations add DescriptiveNameOfChange
dotnet ef database update
```

### Running code analysis

```bash
dotnet build /p:TreatWarningsAsErrors=true
```

## Troubleshooting

### Tests fail with "database error"
Ensure you have run `dotnet ef database update` to apply migrations.

### Port already in use
The app defaults to port 7001. Change in `launchSettings.json` if needed.
```

### Architecture Decision Records (ADR)
Document major decisions in `docs/adr/`:

```markdown
# ADR-001: Use Entity Framework Core for ORM

## Context
We need an Object-Relational Mapping (ORM) solution for database access.

## Decision
We will use Entity Framework Core 8 for all database operations.

## Rationale
- Native .NET integration with async/await
- LINQ support for type-safe queries
- Database migrations built-in
- Supports both PostgreSQL and SQLite
- Large community and excellent documentation

## Consequences
- Team must learn EF Core patterns and best practices
- Some complex queries may require raw SQL
- Performance requires careful query optimization

## Alternatives Considered
1. **Dapper** - Too low-level, more boilerplate
2. **NHibernate** - More mature but steeper learning curve
3. **Raw ADO.NET** - Too much boilerplate code

## Related ADRs
- ADR-002: Repository Pattern for Data Access
```

---

## Code Examples

### README examples should be runnable
```csharp
// ✓ Good - complete, copy-paste-able
var measurements = new[] { 20.1, 20.3, 19.9, 20.2, 20.0 };
var specification = new ProcessSpecification
{
    LowerSpecificationLimit = 19.5,
    UpperSpecificationLimit = 20.5,
    TargetValue = 20.0
};

var analyzer = new ProcessAnalyzer(logger);
var cpk = analyzer.CalculateCapabilityIndex(measurements, specification);

Console.WriteLine($"Process Cpk: {cpk:F2}");
```

### Example files
Create `samples/` directory with complete examples:
```
samples/
├── BasicAnalysis/
│   ├── BasicAnalysis.csproj
│   └── Program.cs
├── DashboardExample/
│   └── ...
└── README.md
```

### Tutorial documentation
Create step-by-step guides for common tasks:

```markdown
# Tutorial: Analyzing a Manufacturing Process

## Step 1: Collect Data
Gather measurement data from your process...

## Step 2: Create a ProcessSpecification
Define the acceptable limits...

## Step 3: Use ProcessAnalyzer
Calculate capability metrics...

## Step 4: Interpret Results
Cpk > 1.33 indicates an acceptable process...
```

---

## Changelog

Maintain a `CHANGELOG.md` following Keep a Changelog format:

```markdown
# Changelog

All notable changes to BenchLibrary are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/),
and this project adheres to [Semantic Versioning](https://semver.org/).

## [1.2.0] - 2024-02-15

### Added
- Real-time dashboard updates via WebSocket
- Support for SCPI instrument communication (initial phase)
- Measurement export to CSV and JSON formats

### Changed
- Improved control chart rendering performance
- Updated UI components to use MudBlazor 6.0

### Fixed
- Null reference exception in capability calculation
- DateTime serialization issue with SQLite

### Deprecated
- Legacy API endpoints (v1.*) - use v2.* instead

## [1.1.0] - 2024-01-20

### Added
- Process control charts (Shewhart)
- Batch measurement import
- Role-based access control

### Fixed
- Database migration issues with PostgreSQL
```

---

## Inline Comments

### When to add comments
Only comment when the code **isn't self-explanatory**:

```csharp
// ✗ Avoid - obvious what the code does
// Add 1 to count
count++;

// Initialize customer list
var customers = new List<Customer>();

// ✓ Good - explains why, not what
// Use exponential moving average for real-time trending
// instead of simple average to weight recent values more heavily
var ema = CalculateEMA(recentMeasurements, 0.3);

// PostgreSQL requires explicit cast for string comparison
// to ensure case-sensitive matching per ISO specification
var caseSensitiveResults = _context.Processes
    .Where(p => EF.Functions.Collate(p.Name, "C") == searchTerm)
    .ToList();
```

### Comment style
```csharp
// Single-line comment for brief notes
// Use sentence case with proper punctuation.

/*
 * Multi-line comment for longer explanations.
 * Keep it concise but complete.
 */

/// <summary>
/// XML documentation comments for public APIs.
/// </summary>
```

### TODO comments
```csharp
// TODO: [Enhancement] Implement caching for frequently accessed data (Issue #42)
// TODO: [Performance] Optimize measurement query with better indexing (Issue #89)
// TODO: [Bug] Fix datetime conversion for SQLite edge case (Issue #15)
```

---

## Best Practices

### 1. Documentation is code
- Keep it up-to-date as code changes
- Review documentation changes in PRs
- Use spell check

### 2. Write for your audience
- Developers: Clear, technical, includes examples
- API users: Focus on behavior, not implementation
- Contributors: Include setup and contribution guidelines

### 3. Use clear structure
- Clear headings and sections
- Table of contents for longer documents
- Code syntax highlighting

### 4. Make it discoverable
- Link related documents
- Use cross-references with `<see cref/>`
- Maintain a docs index/TOC

### 5. Examples must work
- Test code examples
- Keep them current
- Include realistic data

---

## Documentation Checklist

Before considering a feature complete:

- [ ] All public APIs have XML documentation
- [ ] Key classes have detailed summaries
- [ ] All parameters documented with types
- [ ] All exceptions documented
- [ ] Usage examples provided
- [ ] Related types cross-referenced
- [ ] README updated (if user-facing)
- [ ] Architecture documentation updated (if architectural)
- [ ] CHANGELOG.md updated
- [ ] Spelling and grammar checked

