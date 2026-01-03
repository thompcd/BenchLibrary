# Project Structure & Folder Organization

**Convention:** Domain-Driven Design (DDD) with layered architecture
**Solution Structure:** Multi-project with clear separation of concerns
**Last Updated:** 2026-01-03

---

## Table of Contents

1. [Solution-Level Structure](#solution-level-structure)
2. [Project Organization](#project-organization)
3. [Namespace Conventions](#namespace-conventions)
4. [File Organization Within Projects](#file-organization-within-projects)
5. [Naming Conventions](#naming-conventions)
6. [Configuration Files](#configuration-files)
7. [Build Artifacts](#build-artifacts)
8. [Package Organization](#package-organization)

---

## Solution-Level Structure

### Root directory layout
```
BenchLibrary/
├── src/                          # Source code projects
│   ├── BenchLibrary.Core/        # Domain models and core abstractions
│   ├── BenchLibrary.SixSigma/    # Six Sigma and SPC calculations
│   ├── BenchLibrary.Hardware/    # Instrument integration (Phase 2)
│   ├── BenchLibrary.Data/        # EF Core and repository layer
│   └── BenchLibrary.Web/         # Blazor Server web application
│
├── tests/                        # Test projects
│   ├── BenchLibrary.Core.Tests/
│   ├── BenchLibrary.SixSigma.Tests/
│   ├── BenchLibrary.Hardware.Tests/
│   ├── BenchLibrary.Data.Tests/
│   └── BenchLibrary.Web.Tests/
│
├── samples/                      # Example projects and tutorials
│   ├── BasicAnalysis/
│   ├── DashboardExample/
│   └── README.md
│
├── docs/                         # Documentation
│   ├── STYLE_GUIDE_CSHARP.md
│   ├── STYLE_GUIDE_BLAZOR_UI.md
│   ├── STYLE_GUIDE_DATABASE.md
│   ├── STYLE_GUIDE_TESTING.md
│   ├── STYLE_GUIDE_GIT.md
│   ├── STYLE_GUIDE_DOCUMENTATION.md
│   ├── STYLE_GUIDE_PROJECT_STRUCTURE.md
│   ├── ARCHITECTURE.md
│   ├── ROADMAP.md
│   ├── API.md
│   ├── GETTING_STARTED.md
│   ├── CONTRIBUTING.md
│   └── adr/                      # Architecture Decision Records
│       ├── ADR-001-use-efcore.md
│       ├── ADR-002-repository-pattern.md
│       └── ADR-003-layered-architecture.md
│
├── .github/                      # GitHub configuration
│   ├── workflows/                # CI/CD pipelines
│   │   ├── build.yml
│   │   ├── tests.yml
│   │   └── coverage.yml
│   ├── ISSUE_TEMPLATE/
│   └── PULL_REQUEST_TEMPLATE.md
│
├── .editorconfig                 # Code style enforcement
├── .gitignore                    # Git ignore rules
├── BenchLibrary.sln              # Solution file
├── Directory.Build.props          # MSBuild properties
├── Directory.Packages.props        # NuGet package versions
├── CHANGELOG.md                  # Version history
├── CONTRIBUTING.md               # Contribution guidelines
├── LICENSE                       # MIT License
└── README.md                     # Project overview
```

### Solution file (.sln)
Always include all projects in the solution:
```xml
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio Version 17
VisualStudioVersion = 17.0.31919.166
Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "BenchLibrary.Core", "src\BenchLibrary.Core\BenchLibrary.Core.csproj", "{...}"
EndProject
Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "BenchLibrary.SixSigma", "src\BenchLibrary.SixSigma\BenchLibrary.SixSigma.csproj", "{...}"
EndProject
...
```

---

## Project Organization

### Core project (BenchLibrary.Core)
Domain models and core abstractions - **no external dependencies** except logging:

```
BenchLibrary.Core/
├── Models/                       # Domain entities
│   ├── Process.cs
│   ├── Measurement.cs
│   ├── ProcessSpecification.cs
│   └── ProcessResult.cs
│
├── ValueObjects/                 # Immutable value types
│   ├── MeasurementValue.cs
│   ├── ProcessStatus.cs
│   └── CapabilityIndex.cs
│
├── Exceptions/                   # Domain exceptions
│   ├── ProcessException.cs
│   ├── InvalidMeasurementException.cs
│   └── SpecificationException.cs
│
├── Abstractions/                 # Interfaces and contracts
│   ├── IProcessAnalyzer.cs
│   ├── IStatisticalCalculator.cs
│   └── IRepository.cs
│
├── Extensions/                   # Extension methods
│   ├── EnumerableExtensions.cs
│   ├── StringExtensions.cs
│   └── DateTimeExtensions.cs
│
└── BenchLibrary.Core.csproj
```

### Business logic project (BenchLibrary.SixSigma)
Statistical analysis and SPC algorithms:

```
BenchLibrary.SixSigma/
├── Analyzers/                    # Main analysis classes
│   ├── ProcessAnalyzer.cs
│   ├── CapabilityAnalyzer.cs
│   └── ControlChartCalculator.cs
│
├── Calculators/                  # Specific calculation utilities
│   ├── MeanCalculator.cs
│   ├── StandardDeviationCalculator.cs
│   ├── CapabilityIndexCalculator.cs
│   └── ProcessCapabilityCalculator.cs
│
├── Models/                       # Domain models specific to SPC
│   ├── ControlLimits.cs
│   ├── StatisticalMetrics.cs
│   └── ControlChartData.cs
│
├── Validators/                   # Input validation
│   ├── MeasurementValidator.cs
│   └── SpecificationValidator.cs
│
├── Extensions/                   # Extension methods
│   └── MeasurementExtensions.cs
│
└── BenchLibrary.SixSigma.csproj
```

### Data access project (BenchLibrary.Data)
EF Core configuration and repositories:

```
BenchLibrary.Data/
├── Context/
│   └── BenchLibraryDbContext.cs
│
├── EntityConfigurations/         # EF Core Fluent API configs
│   ├── ProcessConfiguration.cs
│   ├── MeasurementConfiguration.cs
│   └── ProcessSpecificationConfiguration.cs
│
├── Repositories/                 # Data access layer
│   ├── Repository.cs             # Generic base
│   ├── ProcessRepository.cs
│   ├── MeasurementRepository.cs
│   └── Abstractions/
│       └── IRepository.cs
│
├── Migrations/                   # EF Core migrations (auto-generated)
│   ├── 20240115_InitialCreate.cs
│   ├── 20240120_AddTimestamps.cs
│   └── BenchLibraryDbContextModelSnapshot.cs
│
├── Specifications/               # Query specifications (optional)
│   ├── Specification.cs
│   ├── ProcessByNameSpecification.cs
│   └── MeasurementsByProcessIdSpecification.cs
│
├── Seeds/                        # Data seeding
│   ├── InitialDataSeeder.cs
│   └── SampleDataSeeder.cs
│
└── BenchLibrary.Data.csproj
```

### Web project (BenchLibrary.Web)
Blazor Server UI:

```
BenchLibrary.Web/
├── Pages/                        # Main pages
│   ├── Dashboard.razor
│   ├── ProcessesPage.razor
│   ├── ProcessesPage.razor.cs
│   ├── MeasurementsPage.razor
│   ├── AnalysisPage.razor
│   ├── AdminPage.razor
│   └── Error.razor
│
├── Shared/                       # Shared layout components
│   ├── MainLayout.razor
│   ├── AdminLayout.razor
│   ├── NavMenu.razor
│   └── SuspenseContent.razor
│
├── Components/                   # Reusable components
│   ├── Charts/
│   │   ├── CapabilityChart.razor
│   │   ├── ControlChart.razor
│   │   ├── HistogramChart.razor
│   │   └── TrendChart.razor
│   │
│   ├── Forms/
│   │   ├── ProcessForm.razor
│   │   ├── ProcessForm.razor.cs
│   │   ├── MeasurementInput.razor
│   │   ├── FilterPanel.razor
│   │   └── UploadMeasurements.razor
│   │
│   ├── Tables/
│   │   ├── ProcessTable.razor
│   │   ├── MeasurementTable.razor
│   │   └── ResultsTable.razor
│   │
│   └── Common/
│       ├── ConfirmationDialog.razor
│       ├── ErrorAlert.razor
│       ├── LoadingSpinner.razor
│       ├── PageTitle.razor
│       └── EmptyState.razor
│
├── Layouts/
│   ├── DefaultLayout.razor
│   └── AdminLayout.razor
│
├── Services/                     # Blazor-specific services
│   ├── IProcessService.cs
│   ├── ProcessService.cs
│   ├── IMeasurementService.cs
│   └── MeasurementService.cs
│
├── DTOs/                         # Data Transfer Objects
│   ├── ProcessDto.cs
│   ├── MeasurementDto.cs
│   ├── AnalysisResultDto.cs
│   └── CreateProcessRequest.cs
│
├── Mapping/                      # AutoMapper profiles
│   └── MappingProfile.cs
│
├── wwwroot/                      # Static files
│   ├── css/
│   │   ├── app.css
│   │   └── custom-components.css
│   ├── js/
│   │   └── interop.js
│   └── images/
│
├── appsettings.json              # Configuration
├── appsettings.Development.json
├── launchSettings.json            # Debug settings
├── Program.cs                    # Startup configuration
├── App.razor                     # Root component
└── BenchLibrary.Web.csproj
```

### Test projects
Mirror source structure:

```
BenchLibrary.Core.Tests/
├── Models/
│   ├── ProcessTests.cs
│   └── MeasurementTests.cs
│
├── Exceptions/
│   └── ProcessExceptionTests.cs
│
├── Extensions/
│   └── EnumerableExtensionsTests.cs
│
├── Fixtures/
│   ├── ProcessFixture.cs
│   └── MeasurementFixture.cs
│
├── Helpers/
│   ├── ProcessBuilder.cs
│   └── TestData.cs
│
└── BenchLibrary.Core.Tests.csproj
```

---

## Namespace Conventions

### Namespace structure mirrors folder structure:
```csharp
// File: src/BenchLibrary.Core/Models/Process.cs
namespace BenchLibrary.Core.Models;

public class Process { }

// File: src/BenchLibrary.Core/Exceptions/ProcessException.cs
namespace BenchLibrary.Core.Exceptions;

public class ProcessException : Exception { }

// File: src/BenchLibrary.SixSigma/Analyzers/CapabilityAnalyzer.cs
namespace BenchLibrary.SixSigma.Analyzers;

public class CapabilityAnalyzer { }

// File: src/BenchLibrary.Data/Repositories/ProcessRepository.cs
namespace BenchLibrary.Data.Repositories;

public class ProcessRepository { }
```

### File-scoped namespaces (preferred)
```csharp
// ✓ Good - modern, concise
namespace BenchLibrary.Core.Models;

public class Process { }

// ✗ Avoid - older style
namespace BenchLibrary.Core.Models
{
    public class Process { }
}
```

### Namespace nesting
Avoid deep nesting; keep to 3-4 levels:
```
BenchLibrary.SixSigma.Analyzers.CapabilityAnalysis.Validators  // ✗ Too deep

BenchLibrary.SixSigma.Analyzers          // ✓ Better
BenchLibrary.SixSigma.Validators.Capability // ✓ Or this
```

---

## File Organization Within Projects

### One public type per file
```csharp
// ✓ Good - single public class
// File: Process.cs
namespace BenchLibrary.Core.Models;

public class Process { }

// Nested private types are OK
private class ProcessInternal { }

// ✗ Avoid - multiple public types
// File: Models.cs
public class Process { }
public class Measurement { }
public class Specification { }
```

### File naming matches class names
```
File: ProcessAnalyzer.cs          → class ProcessAnalyzer { }
File: IProcessRepository.cs       → interface IProcessRepository { }
File: ProcessException.cs         → class ProcessException : Exception { }
File: ProcessValidator.cs         → class ProcessValidator { }
```

### Class member organization
Order members logically:
```csharp
public class Process
{
    // 1. Constants
    public const int MaxNameLength = 255;

    // 2. Static fields
    private static readonly Dictionary<int, Process> Cache = new();

    // 3. Instance fields
    private string? _internalCache;

    // 4. Properties
    public int Id { get; set; }
    public required string Name { get; set; }
    public ICollection<Measurement> Measurements { get; set; } = new List<Measurement>();

    // 5. Constructors
    public Process() { }
    public Process(int id, string name) => (Id, Name) = (id, name);

    // 6. Public methods
    public void AddMeasurement(Measurement measurement) { }

    // 7. Private/internal methods
    private void ValidateName() { }

    // 8. Nested types
    private class InternalHelper { }
}
```

### Partial classes
Use partial only for code generation or very large classes:
```csharp
// File: Process.cs
namespace BenchLibrary.Core.Models;

public partial class Process
{
    public int Id { get; set; }
    public required string Name { get; set; }
}

// File: Process.Calculated.cs - computed properties only
public partial class Process
{
    public double AverageMeasurement =>
        Measurements.Any() ? Measurements.Average(m => m.Value) : 0;
}
```

---

## Naming Conventions

### Project/Assembly names
```
BenchLibrary.Core              # Core domain
BenchLibrary.SixSigma          # Domain-specific (SPC/6-sigma)
BenchLibrary.Hardware          # Infrastructure integration
BenchLibrary.Data              # Data access layer
BenchLibrary.Web               # User interface

# Not:
BenchLibraryCore
BenchLibrary_Core
benchlibrary.core
```

### Folder names
```
Models/                # Domain entities
ValueObjects/         # Immutable value types
Exceptions/           # Custom exceptions
Abstractions/         # Interfaces
Repositories/         # Data access implementations
Services/             # Business logic services
Analyzers/            # Analysis classes
Calculators/          # Calculation utilities
Validators/           # Input validation
Extensions/           # Extension methods
DTOs/                 # Data transfer objects
Mapping/              # Serialization/mapping
Specifications/       # Query specifications
Migrations/           # Database migrations (auto)
wwwroot/              # Static web assets
Pages/                # Web pages
Components/           # Reusable components
Layouts/              # Layout components
Fixtures/             # Test fixtures
Helpers/              # Test helpers
```

### File names
- Match the public class name exactly
- Use PascalCase: `ProcessAnalyzer.cs`, not `processAnalyzer.cs`
- Keep names concise but descriptive

```
ProcessAnalyzer.cs         # class ProcessAnalyzer
IProcessService.cs         # interface IProcessService
ProcessException.cs        # class ProcessException
ProcessBuilder.cs          # class ProcessBuilder (test helper)
```

---

## Configuration Files

### .editorconfig
Enforce code style across IDEs:
```ini
# .editorconfig
root = true

[*.cs]
indent_size = 4
insert_final_newline = true
charset = utf-8-bom
trim_trailing_whitespace = true

# C# specific
csharp_indent_braces = false
csharp_new_line_before_open_brace = all
csharp_new_line_before_else = true
csharp_prefer_braces = true:suggestion

# Naming rules
dotnet_naming_style.private_field_style.capitalization = camel_case
dotnet_naming_style.private_field_style.required_prefix = _
```

### Directory.Build.props
Centralized MSBuild configuration:
```xml
<Project>
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <LangVersion>12</LangVersion>
    <Nullable>enable</Nullable>
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
    <NoWarn>$(NoWarn);1591</NoWarn>
    <WarningsNotAsErrors>1591</WarningsNotAsErrors>
  </PropertyGroup>

  <PropertyGroup Condition="'$(Configuration)' == 'Release'">
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.VisualStudio.Threading.Analyzers" Version="17.8.14" />
  </ItemGroup>
</Project>
```

### Directory.Packages.props
Centralized NuGet versioning:
```xml
<Project>
  <ItemGroup>
    <!-- Framework -->
    <PackageReference Update="Microsoft.EntityFrameworkCore" Version="8.0.0" />
    <PackageReference Update="Microsoft.EntityFrameworkCore.Tools" Version="8.0.0" />
    <PackageReference Update="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.0.0" />

    <!-- Testing -->
    <PackageReference Update="xunit" Version="2.6.4" />
    <PackageReference Update="Moq" Version="4.20.69" />
    <PackageReference Update="FluentAssertions" Version="6.12.0" />
  </ItemGroup>
</Project>
```

### appsettings.json
Application configuration:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=BenchLibrary.db"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "BenchLibrary": "Debug"
    }
  },
  "Kestrel": {
    "EndpointDefaults": {
      "Protocols": "Http2AndHttp11"
    }
  }
}
```

---

## Build Artifacts

### .gitignore standards
Exclude build outputs, never commit generated files:
```
# Build results
bin/
obj/
dist/
*.o
*.a

# IDE/Editor
.vs/
.vscode/
.idea/
*.sln.iml
*.user
*.suo

# Dependencies
packages/
.nuget/

# Secrets
appsettings.Production.json
.env
.env.local

# Tests
coverage/
.nyc_output/

# Generated files (keep migrations, exclude others)
Generated/
```

### Output directory structure (after build)
```
bin/
├── Debug/
│   └── net8.0/
│       ├── BenchLibrary.Core.dll
│       ├── BenchLibrary.SixSigma.dll
│       └── ...
└── Release/
    └── net8.0/
        └── ...
```

---

## Package Organization

### NuGet package structure
When publishing to NuGet:

```
BenchLibrary.Core
├── lib/
│   └── net8.0/
│       └── BenchLibrary.Core.dll
├── BenchLibrary.Core.nuspec
└── README.md

BenchLibrary.SixSigma
├── lib/
│   └── net8.0/
│       └── BenchLibrary.SixSigma.dll
└── README.md
```

### Package naming
```
BenchLibrary.Core                # Foundation types
BenchLibrary.SixSigma            # Statistical analysis
BenchLibrary.Hardware            # Instrument integration
BenchLibrary.Data.EntityFramework # EF Core context
```

---

## Project Size Guidelines

### When to split a project
Split a project if it:
- Exceeds 10-15 classes
- Has dependencies others don't need
- Represents a distinct domain/layer
- Would be reused independently

### When projects are too large
If a project exceeds 20-30 types, consider:
```
Before (one large project):
BenchLibrary.SixSigma/
├── Analyzers/
│   ├── ProcessAnalyzer.cs
│   ├── CapabilityAnalyzer.cs
│   ├── TrendAnalyzer.cs
│   ├── ForecastingAnalyzer.cs
│   └── ... (50+ files)

After (split into domain areas):
BenchLibrary.SixSigma/
├── Analyzers/
├── Validators/
└── Extensions/

BenchLibrary.SixSigma.Forecasting/
└── Analyzers/
```

---

## Key Rules Summary

✓ **DO:**
- One public type per file
- Match file names to type names
- Use file-scoped namespaces
- Mirror folder structure in namespaces
- Keep projects focused (3-4 namespace levels max)
- Store tests in parallel project structure
- Use PascalCase for file and folder names
- Organize class members logically
- Create clear separation of concerns

✗ **DON'T:**
- Mix multiple public types in one file
- Use deep folder hierarchies (>4 levels)
- Create "Util" or "Helper" folders at top level
- Put unrelated classes in same namespace
- Create overly large projects (>30 classes)
- Mix concerns (business logic with data access)
- Use generic folder names like "Common"
- Create circular dependencies between projects

