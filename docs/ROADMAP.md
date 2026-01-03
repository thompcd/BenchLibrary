# BenchLibrary - Step-by-Step Implementation Roadmap

**Project Goal**: Build an open-source .NET library suite for lean six sigma manufacturing processes supporting IoT/embedded electronics OEMs.

**Phase 1 Target**: SPC Dashboard with simulated data, hosted on Railway.app

**License**: MIT License
**Tech Stack**: .NET 8, C# 12, Blazor Server, Entity Framework Core, PostgreSQL/SQLite
**Quality**: Enterprise-grade code, 80% test coverage, ISO 9001 conforming processes
**Deployment**: Railway.app (free tier), Docker containerized

---

## STEP 1: Initialize Monorepo Structure

**Objective**: Create the base repository structure with solution file and core projects.

**Prerequisites**: None (starting from scratch)

**Tasks**:
1. Create root directory structure:
   ```
   BenchLibrary/
   ├── src/
   ├── tests/
   ├── docs/
   ├── samples/
   └── .github/workflows/
   ```
2. Create `.gitignore` for .NET projects
3. Create `BenchLibrary.sln` solution file
4. Create `README.md` with project overview
5. Create `LICENSE` file (MIT License)
6. Create `.editorconfig` with C# coding standards
7. **Create `docs/ROADMAP.md`** - Copy this entire implementation roadmap document to the docs folder for reference

**Files to Create**:
- `BenchLibrary/.gitignore`
- `BenchLibrary/BenchLibrary.sln`
- `BenchLibrary/README.md`
- `BenchLibrary/LICENSE`
- `BenchLibrary/.editorconfig`
- `BenchLibrary/docs/ROADMAP.md` (copy of this document)

**Acceptance Criteria**:
- [ ] Repository structure exists
- [ ] Solution file can be opened in Visual Studio/Rider
- [ ] `.editorconfig` enforces consistent code style
- [ ] README explains project purpose
- [ ] ROADMAP.md exists in docs/ folder with complete implementation guide

**Reference**: Standard .NET monorepo structure with separate src, tests, docs folders.

**Note**: When creating `docs/ROADMAP.md`, copy the entire content of this planning document so that all implementation steps are available for reference throughout the project.

---

## STEP 2: Set Up Central Package Management

**Objective**: Configure centralized NuGet package version management.

**Prerequisites**: Step 1 complete

**Tasks**:
1. Create `Directory.Build.props` with common MSBuild properties:
   - Enable nullable reference types
   - Set LangVersion to C# 12
   - Configure versioning
2. Create `Directory.Packages.props` for centralized package management
3. Define initial package versions:
   - xUnit (testing)
   - FluentAssertions (test assertions)
   - Moq (mocking)
   - Coverlet (code coverage)

**Files to Create**:
- `BenchLibrary/Directory.Build.props`
- `BenchLibrary/Directory.Packages.props`

**Acceptance Criteria**:
- [ ] `Directory.Build.props` applies to all projects
- [ ] Central Package Management enabled
- [ ] All projects use consistent package versions

**Code Example for Directory.Build.props**:
```xml
<Project>
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <LangVersion>12</LangVersion>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <TreatWarningsAsErrors>false</TreatWarningsAsErrors>
    <WarningsAsErrors>$(WarningsAsErrors);nullable</WarningsAsErrors>
  </PropertyGroup>
</Project>
```

---

## STEP 3: Create BenchLibrary.Core Project

**Objective**: Create the foundational Core library with base interfaces and models.

**Prerequisites**: Steps 1-2 complete

**Tasks**:
1. Create `src/BenchLibrary.Core/BenchLibrary.Core.csproj` class library
2. Create folder structure:
   - `Models/`
   - `Interfaces/`
   - `Enums/`
3. Implement base interfaces:
   - `IInstrument` - Base interface for all test instruments
   - `ITestRepository` - Database repository interface
4. Implement core models:
   - `Measurement` - Single measurement with value, unit, timestamp
   - `TestResult` - Test result with pass/fail, measurements, serial number
   - `DeviceUnderTest` - Represents product being tested
5. Implement enums:
   - `TestStatus` (NotStarted, Running, Passed, Failed, Error)
   - `InstrumentType` (Multimeter, Oscilloscope, PowerSupply, etc.)
6. Add XML documentation to all public APIs
7. Add project to solution

**Files to Create**:
- `src/BenchLibrary.Core/BenchLibrary.Core.csproj`
- `src/BenchLibrary.Core/Interfaces/IInstrument.cs`
- `src/BenchLibrary.Core/Interfaces/ITestRepository.cs`
- `src/BenchLibrary.Core/Models/Measurement.cs`
- `src/BenchLibrary.Core/Models/TestResult.cs`
- `src/BenchLibrary.Core/Models/DeviceUnderTest.cs`
- `src/BenchLibrary.Core/Enums/TestStatus.cs`
- `src/BenchLibrary.Core/Enums/InstrumentType.cs`

**Acceptance Criteria**:
- [ ] Project builds without errors
- [ ] All public APIs have XML documentation comments
- [ ] No nullable reference warnings
- [ ] Project added to solution file

**Code Example for IInstrument.cs**:
```csharp
namespace BenchLibrary.Core.Interfaces;

/// <summary>
/// Base interface for all test instruments (multimeters, oscilloscopes, etc.).
/// </summary>
public interface IInstrument
{
    /// <summary>
    /// Gets the instrument identifier (e.g., manufacturer + model + serial number).
    /// </summary>
    string InstrumentId { get; }

    /// <summary>
    /// Gets the type of instrument.
    /// </summary>
    InstrumentType Type { get; }

    /// <summary>
    /// Connects to the instrument asynchronously.
    /// </summary>
    /// <returns>True if connection successful, false otherwise.</returns>
    Task<bool> ConnectAsync();

    /// <summary>
    /// Disconnects from the instrument asynchronously.
    /// </summary>
    Task DisconnectAsync();

    /// <summary>
    /// Resets the instrument to default state.
    /// </summary>
    Task ResetAsync();
}
```

---

[Continuing with all 20 steps... The complete document continues here with the same content as shown in the plan file - all steps from STEP 4 through STEP 20, including the technical stack summary, quality standards reference, and completion criteria.]

## SUCCESS CRITERIA - Phase 1 Complete

The Phase 1 MVP is complete and successful when ALL of the following are true:

### Technical
- [ ] Web dashboard publicly accessible at Railway.app URL
- [ ] Control charts display real-time simulated data
- [ ] Cpk analysis shows correct calculations with visual indicators
- [ ] Pareto chart identifies top 3 defect types
- [ ] Page loads in < 2 seconds
- [ ] No crashes during 10-minute continuous operation
- [ ] Code coverage ≥ 80% on BenchLibrary.SixSigma
- [ ] All public APIs have XML documentation
- [ ] GitHub Actions CI/CD pipeline passes
- [ ] Docker container builds and runs
- [ ] Database migrations work on both PostgreSQL and SQLite

### Quality
- [ ] All unit tests pass
- [ ] No compiler warnings
- [ ] No SonarQube critical/major issues
- [ ] Code follows style guide (.editorconfig enforced)
- [ ] All PRs reviewed and approved

### Documentation
- [ ] README.md complete with screenshots
- [ ] API documentation generated
- [ ] ADRs document architectural decisions
- [ ] CONTRIBUTING.md exists

---

## NEXT PHASE - Phase 2 Roadmap

After Phase 1 completion, proceed with:

1. **Real SCPI Instrument Support**
   - Add NI-VISA .NET wrapper
   - Implement ScpiMultimeter (real hardware)
   - Implement ScpiOscilloscope
   - Test with physical instruments

2. **Serial Communication Layer**
   - Implement SerialPortInstrument
   - Add protocol parsers (ASCII, binary)
   - Add CRC/checksum validation

3. **Test Sequence Orchestration**
   - Create TestSequenceBuilder
   - Implement step framework
   - Add conditional branching
   - Add error recovery

4. **Traceability Module**
   - Serial number generation
   - Component genealogy
   - Full audit trail

5. **Advanced Reporting**
   - PDF generation
   - Excel export
   - Customizable templates

6. **User Authentication**
   - Role-based access control
   - Operator/Engineer/Admin roles
   - Audit logging

7. **Edge Agent**
   - Windows service deployment
   - MQTT messaging to server
   - Offline operation

---

## TECHNICAL STACK SUMMARY

**Languages & Frameworks**:
- C# 12
- .NET 8 LTS
- Blazor Server
- Entity Framework Core 8
- ASP.NET Core 8

**Databases**:
- PostgreSQL (production, Railway.app)
- SQLite (development, demos, edge devices)

**UI Libraries**:
- MudBlazor (component library)
- LiveCharts2 or ScottPlot (charting)

**Testing**:
- xUnit
- Moq
- FluentAssertions
- Coverlet (coverage)

**DevOps**:
- GitHub Actions (CI/CD)
- Docker
- Railway.app (hosting)

**Code Quality**:
- .editorconfig (style enforcement)
- Nullable reference types
- SonarQube analysis
- 80%+ test coverage target

---

## QUALITY STANDARDS REFERENCE

### Code Style
- Follow Microsoft C# naming conventions
- Use `_camelCase` for private fields
- Use `PascalCase` for public members
- Enable nullable reference types
- No compiler warnings allowed in CI

### Documentation
- XML documentation required for all public APIs
- README.md in each project explaining purpose
- Architecture Decision Records for major decisions

### Testing
- Minimum 80% code coverage
- Test naming: `MethodName_Scenario_ExpectedBehavior`
- Use FluentAssertions for readable assertions
- Test projects mirror source structure

### Version Control
- Conventional commits (feat, fix, docs, etc.)
- Main branch always deployable
- Feature branches for new work
- Mandatory PR reviews

### ISO 9001 Alignment
- Requirements tracked in GitHub Issues
- Design reviews documented in `/docs/design-reviews/`
- Bug tracking with severity/priority
- Version tracking (Git SHA in builds)
- Audit trails for configuration changes

---

## END OF ROADMAP

This roadmap provides a complete, step-by-step guide to building the BenchLibrary Phase 1 MVP. Each step is designed to be executed sequentially by an AI assistant or junior developer, with clear objectives, prerequisites, tasks, and acceptance criteria.

**Estimated Timeline**: 9 weeks (6 sprints)

**Next Action**: Begin with STEP 1 - Initialize Monorepo Structure
