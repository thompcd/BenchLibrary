# Claude Development Guide for BenchLibrary

**Purpose:** Context and standards for Claude to reference while developing BenchLibrary.
**Last Updated:** 2026-01-03
**Comprehensive Guides:** See `docs/STYLE_GUIDES.md`

---

## Quick Reference

### Tech Stack
- **Language:** C# 12 with nullable reference types enabled
- **Framework:** .NET 8 LTS
- **Web:** Blazor Server + MudBlazor
- **Database:** EF Core 8 (PostgreSQL + SQLite)
- **Testing:** xUnit + Moq + FluentAssertions + **Testcontainers**
- **Version Control:** Git + Conventional Commits

### Essential Standards

#### 1. C# Code Style
- **Naming:** `PascalCase` (public), `camelCase` (local), `_camelCase` (private fields)
- **Required keyword:** Use for mandatory fields/parameters
- **Nullable refs:** Globally enabled - no null reference warnings allowed
- **Async:** Always use `Async` suffix, `ConfigureAwait(false)` in libraries
- **Documentation:** XML comments on all public APIs (`<summary>`, `<param>`, `<returns>`, `<exception>`)
- **String interpolation:** Use `$"text {variable}"`, not concatenation
- **LINQ:** Method syntax preferred, `AsNoTracking()` for read-only

**Full Guide:** `docs/STYLE_GUIDE_CSHARP.md`

#### 2. Testing Standards (CRITICAL)
**Default Approach:**
- **Unit Tests:** xUnit with Moq for mocks, FluentAssertions for assertions
- **Integration Tests:** **Testcontainers for real databases** (NOT in-memory)
- **Naming:** `MethodName_Scenario_ExpectedResult`
- **Pattern:** AAA (Arrange-Act-Assert)
- **Coverage:** Minimum 80%

**Regression Tests (REQUIRED):**
When customer complaint/warranty return/root cause identified:
1. Create failing regression test that reproduces issue
2. Fix the bug
3. Tag test with Traits:
   ```csharp
   [Trait("Category", "Regression")]
   [Trait("Issue", "GH-234")]
   [Trait("Severity", "High")]
   [Trait("Customer", "Company Name")]
   ```
4. Test prevents future regressions

**Testcontainers Integration:**
```csharp
// Always use real PostgreSQL in integration tests
public class PostgreSqlFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase("benchlibrary_test")
        .Build();
    // ... initialize and run migrations
}

[Collection("PostgreSQL Database collection")]
public class ProcessRepositoryIntegrationTests
{
    private readonly PostgreSqlFixture _fixture;
    // Tests run against real PostgreSQL instance
}
```

**Full Guide:** `docs/STYLE_GUIDE_TESTING.md`

#### 3. Database (EF Core)
- **Primary Key:** Always use `Id` (int)
- **Timestamps:** `CreatedAt`, `UpdatedAt` (auto-updated in SaveChangesAsync)
- **Required fields:** Use `required` keyword
- **Relationships:** Explicit `.Include()` for eager loading (never lazy loading)
- **Migrations:** Descriptive names: `AddProcessMeasurementsTable`, not `UpdateDatabase`
- **Repositories:** Generic base + specialized per entity
- **Indexes:** Add for frequently filtered columns

**Full Guide:** `docs/STYLE_GUIDE_DATABASE.md`

#### 4. Blazor/Web UI
- **Component naming:** `PascalCasePage.razor`, `PascalCaseComponent.razor`
- **Code-behind:** Single file for simple components, `.razor.cs` for complex
- **Styling:** Always scoped CSS with `.razor` file or `<style scoped>`
- **Components:** Always use MudBlazor, not raw HTML
- **Binding:** `@bind-Value` for two-way, `ValueChanged` for logic
- **Error handling:** Implement error boundaries, loading states
- **Accessibility:** ARIA labels, keyboard navigation

**Full Guide:** `docs/STYLE_GUIDE_BLAZOR_UI.md`

#### 5. Git & Commits
- **Format:** `type(scope): description` (Conventional Commits)
- **Types:** `feat`, `fix`, `docs`, `style`, `refactor`, `perf`, `test`, `chore`, `ci`
- **Branch naming:** `feature/short-name`, `fix/short-name`, `hotfix/short-name`
- **Merge strategy:** Squash merge to main
- **Commits:** Atomic, logical, one concern each
- **Issues:** Link with "Fixes #123" syntax

**Full Guide:** `docs/STYLE_GUIDE_GIT.md`

#### 6. Documentation
- **XML Comments:** Required on all public members
- **Format:** Include `<summary>`, `<param>`, `<returns>`, `<exception>`
- **Examples:** Provide copy-paste-able code examples
- **Comments:** Explain "why", not "what"
- **README:** Each project should have a README explaining purpose/usage
- **API Docs:** Use Swagger annotations for REST endpoints

**Full Guide:** `docs/STYLE_GUIDE_DOCUMENTATION.md`

#### 7. Project Structure
- **Solution Layout:** `src/` for source, `tests/` for tests, `docs/` for documentation
- **Namespaces:** Match folder structure with file-scoped namespaces
- **One public type per file** (file name matches type name)
- **Projects:**
  - `BenchLibrary.Core` - Domain models, no external deps
  - `BenchLibrary.SixSigma` - SPC/statistical analysis
  - `BenchLibrary.Data` - EF Core + repositories
  - `BenchLibrary.Web` - Blazor Server UI
- **Test Projects:** Mirror source structure

**Full Guide:** `docs/STYLE_GUIDE_PROJECT_STRUCTURE.md`

---

## Development Workflow

### Before Writing Code
1. Check if this relates to a customer issue/warranty return
2. If yes, plan regression test first
3. Review relevant style guide
4. Check `.editorconfig` is configured in IDE

### While Coding
1. Write tests first (TDD preferred)
2. Follow appropriate style guide
3. Use `required` keyword for mandatory fields
4. Document all public APIs with XML comments
5. Keep commits atomic and small

### Before Committing
1. Run `dotnet build` - zero warnings
2. Run `dotnet test` - all tests pass
3. Check code coverage: `dotnet test /p:CollectCoverage=true`
4. Verify XML documentation on public APIs
5. If fixing customer issue: ensure regression test exists and passes
6. Follow Conventional Commits format

### Before Creating PR
1. Squash related commits
2. Write clear PR title/description
3. Link related issues
4. All tests passing
5. Coverage meets 80% target

---

## Key Rules to Remember

### Always Do This
✅ Use `required` keyword for mandatory fields
✅ Enable nullable reference types (`#nullable enable`)
✅ Use Testcontainers for integration tests (real databases)
✅ Create regression tests for customer issues
✅ Tag regression tests with Traits
✅ Write XML documentation for public APIs
✅ Use Conventional Commits
✅ Follow AAA pattern in tests
✅ Use MudBlazor components (not raw HTML)
✅ Use `await` for async operations

### Never Do This
✗ Use generic exception types (`throw new Exception()`)
✗ Block on async (`.Result`, `.Wait()`)
✗ Create in-memory databases for integration tests (use Testcontainers)
✗ Fix customer issues without regression tests
✗ Use `async void` (except event handlers)
✗ Leave public APIs undocumented
✗ Over-mock in tests (brittle)
✗ Create types larger than 400 lines
✗ Use global state or singletons
✗ Force-push to main/develop

---

## Common Scenarios

### Scenario: Bug Report from Customer
1. Create GitHub issue (reference warranty return if applicable)
2. **Create failing regression test** that reproduces the issue
3. Implement fix
4. Tag regression test with issue info and customer name
5. Verify all tests pass
6. Create PR linking to issue

### Scenario: Adding New Feature
1. Create feature branch: `feature/short-description`
2. Write tests first (TDD)
3. Implement feature
4. Document public APIs (XML comments)
5. Create PR with Conventional Commit title
6. Link related issue if applicable

### Scenario: Database Change
1. Create migration: `dotnet ef migrations add DescriptiveNameOfChange`
2. Create integration test with Testcontainers
3. Test actual migration and data preservation
4. Verify indexes are created for filtered columns

### Scenario: UI Component
1. Use scoped CSS for styling
2. Use MudBlazor components
3. Implement error boundaries
4. Add ARIA labels for accessibility
5. Test with integration tests (WebApplicationFactory + Testcontainers)

### Scenario: API Endpoint
1. Add Swagger attributes for documentation
2. Return DTOs (not entities)
3. Validate inputs at system boundaries
4. Create integration tests with Testcontainers database
5. Implement proper error handling

---

## Testing Commands

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true /p:CoverageFormat=opencover

# Run only regression tests
dotnet test --filter "Category=Regression"

# Run regressions for specific issue
dotnet test --filter "Issue=GH-234"

# Run critical regressions
dotnet test --filter "Severity=Critical"

# Run with verbose output
dotnet test --verbosity detailed

# Run specific test class
dotnet test --filter "FullyQualifiedName~ProcessRepositoryTests"
```

---

## Useful File Locations

| Purpose | Location |
|---------|----------|
| **Style Guides Index** | `docs/STYLE_GUIDES.md` |
| **C# Standards** | `docs/STYLE_GUIDE_CSHARP.md` |
| **Testing Standards** | `docs/STYLE_GUIDE_TESTING.md` |
| **Database Standards** | `docs/STYLE_GUIDE_DATABASE.md` |
| **Blazor/UI Standards** | `docs/STYLE_GUIDE_BLAZOR_UI.md` |
| **Git Standards** | `docs/STYLE_GUIDE_GIT.md` |
| **Documentation Standards** | `docs/STYLE_GUIDE_DOCUMENTATION.md` |
| **Project Structure** | `docs/STYLE_GUIDE_PROJECT_STRUCTURE.md` |
| **Roadmap** | `docs/ROADMAP.md` |
| **Architecture** | `docs/ARCHITECTURE.md` |
| **Contributing** | `CONTRIBUTING.md` |
| **EditorConfig** | `.editorconfig` |

---

## Code Style Quick Examples

### C# Naming
```csharp
// ✓ Correct
public class ProcessAnalyzer { }                 // PascalCase
public int MeasurementCount { get; set; }        // PascalCase
private string _processName;                     // _camelCase for private
var sampleCount = measurements.Count;            // camelCase for local
public required string ProcessName { get; set; } // required keyword

// ✗ Wrong
public class processAnalyzer { }
public int _measurement_count { get; set; }
private String ProcessName;
```

### Test Naming
```csharp
// ✓ Correct
public void CalculateCapabilityIndex_WithValidMeasurements_ReturnsPositiveValue()
public void SaveProcess_WithNullName_ThrowsArgumentNullException()

// ✗ Wrong
public void Test1()
public void TestCalculateCapabilityIndex()
```

### Database Queries
```csharp
// ✓ Correct
var process = await _context.Processes
    .Include(p => p.Measurements)
    .AsNoTracking()
    .FirstOrDefaultAsync(p => p.Id == id);

// ✗ Wrong
var process = _context.Processes.FirstOrDefault(p => p.Id == id);
var measurements = process.Measurements; // lazy loading!
```

### Integration Tests
```csharp
// ✓ Correct - uses Testcontainers
[Collection("PostgreSQL Database collection")]
public class ProcessRepositoryIntegrationTests
{
    private readonly PostgreSqlFixture _fixture;
    // Uses real PostgreSQL
}

// ✗ Wrong - uses in-memory
var options = new DbContextOptionsBuilder<BenchLibraryDbContext>()
    .UseInMemoryDatabase("test")
    .Options;
```

### Regression Test
```csharp
// ✓ Correct - tagged and documented
[Fact]
[Trait("Category", "Regression")]
[Trait("Issue", "GH-234")]
[Trait("Severity", "High")]
public void CapabilityIndex_WithZeroStdDev_ReturnsValidValue()
{
    // Test reproduces customer issue
    // Prevents regression
}

// ✗ Wrong - not tagged
[Fact]
public void TestCapabilityIndexBug()
{
}
```

---

## Important Notes for Claude

1. **Testcontainers is the default** for integration tests - do not use in-memory databases
2. **Regression tests are mandatory** - every customer issue needs a permanent test
3. **Nullable reference types are enabled globally** - use `?` and `!` appropriately
4. **100% XML documentation required** on all public APIs
5. **No compiler warnings allowed** in release builds
6. **80% code coverage minimum** - measure before committing
7. **Conventional Commits required** - automated tooling may depend on this
8. **MudBlazor only** for UI components - consistency across the app
9. **Repository pattern** for all data access - never direct DbContext in services
10. **Immutable records preferred** for DTOs and data transfer objects

---

## Quick Checklist When Starting Development

- [ ] Read relevant style guide from `docs/`
- [ ] Check if this is related to customer issue (if yes, plan regression test)
- [ ] Verify `.editorconfig` is in IDE settings
- [ ] Create feature branch with proper naming
- [ ] Write tests first (TDD)
- [ ] Follow appropriate style guide
- [ ] Document all public APIs
- [ ] Use Testcontainers for integration tests
- [ ] Tag regression tests with Traits
- [ ] Run full test suite before commit
- [ ] Follow Conventional Commits format
- [ ] Ensure all tests pass before PR

---

## Getting Help

1. **Style question?** → Check relevant guide in `docs/STYLE_GUIDES.md`
2. **Testing question?** → See `docs/STYLE_GUIDE_TESTING.md`
3. **Git workflow?** → See `docs/STYLE_GUIDE_GIT.md`
4. **Architecture question?** → See `docs/ARCHITECTURE.md`
5. **Contribution guidelines?** → See `CONTRIBUTING.md`

---

**Reference this file when developing. All standards and patterns are documented in the `docs/` folder.**

