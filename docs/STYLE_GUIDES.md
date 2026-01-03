# BenchLibrary Style Guides & Standards

Complete reference for code style, architecture, and development practices across the BenchLibrary tech stack.

**Last Updated:** 2026-01-03
**Target Audience:** All developers contributing to BenchLibrary

---

## Quick Navigation

### For a Quick Reference
- **Just starting?** → [Getting Started](#getting-started)
- **Writing code?** → [STYLE_GUIDE_CSHARP.md](STYLE_GUIDE_CSHARP.md)
- **Building UI?** → [STYLE_GUIDE_BLAZOR_UI.md](STYLE_GUIDE_BLAZOR_UI.md)
- **Database work?** → [STYLE_GUIDE_DATABASE.md](STYLE_GUIDE_DATABASE.md)
- **Writing tests?** → [STYLE_GUIDE_TESTING.md](STYLE_GUIDE_TESTING.md)
- **Committing code?** → [STYLE_GUIDE_GIT.md](STYLE_GUIDE_GIT.md)
- **Documenting?** → [STYLE_GUIDE_DOCUMENTATION.md](STYLE_GUIDE_DOCUMENTATION.md)
- **Organizing projects?** → [STYLE_GUIDE_PROJECT_STRUCTURE.md](STYLE_GUIDE_PROJECT_STRUCTURE.md)

---

## Style Guides Overview

### 1. **C# Code Style** ([STYLE_GUIDE_CSHARP.md](STYLE_GUIDE_CSHARP.md))
Language-level conventions and patterns for C# 12 development.

**Key Topics:**
- Naming conventions (PascalCase, camelCase, _camelCase)
- Code formatting and brace styles
- Modern C# features (records, init properties, collection expressions)
- Nullable reference types
- LINQ best practices
- XML documentation
- Error handling patterns
- Performance considerations

**Quick Rules:**
- Use `required` keyword for mandatory fields
- Use `_camelCase` for private fields
- Use `PascalCase` for everything public
- Enable nullable reference types globally
- Use `Async` suffix for async methods
- Use string interpolation, not concatenation

**When to Reference:** Whenever writing C# code

---

### 2. **Blazor & Web UI Standards** ([STYLE_GUIDE_BLAZOR_UI.md](STYLE_GUIDE_BLAZOR_UI.md))
Frontend development standards using Blazor Server and MudBlazor.

**Key Topics:**
- Component structure and organization
- File naming and directory layout
- Code-behind patterns
- Event handling
- State management
- Form and input handling
- CSS and styling (scoped CSS, BEM)
- MudBlazor component usage
- Data binding patterns
- Performance optimization
- Accessibility (ARIA, keyboard navigation)

**Quick Rules:**
- Use single-file components for simple features
- Use multi-file (code-behind) for complex logic
- Always use scoped CSS
- Use MudBlazor components, not raw HTML
- Use two-way binding with `@bind`
- Implement error boundaries

**When to Reference:** When developing Blazor components or pages

---

### 3. **Database & EF Core** ([STYLE_GUIDE_DATABASE.md](STYLE_GUIDE_DATABASE.md))
Entity Framework Core and database design standards.

**Key Topics:**
- Entity design and naming
- DbContext configuration
- Relationships (one-to-many, many-to-many)
- Value conversions
- Migrations and version control
- Querying best practices (LINQ)
- Performance optimization (indexes, lazy loading)
- Batch operations
- Data seeding
- Audit and timestamp patterns
- Repository pattern

**Quick Rules:**
- Use `Id` for primary keys
- Use file-scoped namespaces
- Use `Include()` for explicit eager loading
- Add indexes for frequently filtered columns
- Use `AsNoTracking()` for read-only queries
- Create migrations with descriptive names
- Use `required` keyword for mandatory columns

**When to Reference:** When working with databases or EF Core

---

### 4. **Testing Standards** ([STYLE_GUIDE_TESTING.md](STYLE_GUIDE_TESTING.md))
Unit testing, integration testing, and code coverage standards.

**Key Topics:**
- Test project structure (mirrors source)
- Test naming conventions (MethodName_Scenario_ExpectedResult)
- AAA pattern (Arrange-Act-Assert)
- xUnit fixtures and setup/teardown
- Mocking with Moq
- FluentAssertions usage
- Test data builders
- **Integration testing with Testcontainers (default)**
- **Regression tests for customer issues & warranty returns**
- Async tests
- Code coverage target (80% minimum)

**Quick Rules:**
- Use `_MethodName_Scenario_Result` naming
- Follow AAA pattern: Arrange, Act, Assert
- Mock only external dependencies
- Use builders for complex test data
- **Use Testcontainers for integration tests (not in-memory)**
- **Create regression tests for every customer issue**
- **Tag regression tests with Traits: Category, Issue, Severity**
- Aim for 80%+ code coverage
- Test happy paths AND error cases
- Use `async Task` for async tests

**When to Reference:** When writing tests

---

### 5. **Git & Commit Standards** ([STYLE_GUIDE_GIT.md](STYLE_GUIDE_GIT.md))
Version control workflow and commit message standards.

**Key Topics:**
- Conventional commits specification
- Commit types (feat, fix, docs, style, refactor, perf, test, chore, ci)
- Commit message format and structure
- Branching strategy (feature/fix/hotfix)
- Branch naming conventions
- Pull request process
- Code review guidelines
- Merge strategy (squash merge)
- Common git commands
- Handling conflicts

**Quick Rules:**
- Use conventional commits: `feat(scope): description`
- Create feature branches: `feature/short-description`
- Write imperative mood: "add" not "added"
- Squash commits before merging to main
- Link related issues: "Fixes #123"
- Delete branches after merging
- Keep commits atomic and logical

**When to Reference:** When committing code or creating pull requests

---

### 6. **Documentation Standards** ([STYLE_GUIDE_DOCUMENTATION.md](STYLE_GUIDE_DOCUMENTATION.md))
Code documentation, API docs, and knowledge sharing.

**Key Topics:**
- XML documentation comments (`<summary>`, `<param>`, `<returns>`, `<exception>`)
- README standards and structure
- Architecture documentation
- API documentation (REST, Swagger)
- Developer guides and tutorials
- Code examples
- Changelog maintenance (Keep a Changelog)
- Inline comments (when to use)

**Quick Rules:**
- Document all public APIs with XML comments
- Include `<summary>`, `<param>`, `<returns>`, `<exception>`
- Write code examples that are copy-paste-able
- Comment the "why", not the "what"
- Keep documentation in sync with code
- Use cross-references with `<see cref/>`
- Maintain CHANGELOG.md with every release

**When to Reference:** When writing documentation or public APIs

---

### 7. **Project Structure** ([STYLE_GUIDE_PROJECT_STRUCTURE.md](STYLE_GUIDE_PROJECT_STRUCTURE.md))
Solution layout, project organization, and folder conventions.

**Key Topics:**
- Solution-level directory structure
- Project organization (Core, SixSigma, Data, Web)
- Namespace conventions
- File organization within projects
- Naming conventions (projects, folders, files)
- Configuration files (.editorconfig, Directory.Build.props)
- Build artifacts and .gitignore
- Package organization
- Project size guidelines

**Quick Rules:**
- One public type per file
- Match file names to type names
- Use file-scoped namespaces
- Mirror folder structure in namespaces
- Keep projects focused (<30 classes per project)
- Use PascalCase for files and folders
- Create clear separation of concerns

**When to Reference:** When organizing code or starting new projects

---

## Development Workflow

### 1. Before You Start
- Read the [README.md](../README.md)
- Check the [ROADMAP.md](ROADMAP.md) for current priorities
- Review relevant style guides above

### 2. While Developing
- Reference appropriate style guide (see Quick Navigation above)
- Use `.editorconfig` for automatic formatting
- Write tests as you code (TDD when possible)
- Keep commits small and focused
- Document public APIs with XML comments

### 3. Before Committing
- Run `dotnet build` - ensure no compiler warnings
- Run `dotnet test` - ensure all tests pass
- Run code coverage check: `dotnet test /p:CollectCoverage=true`
- Verify XML documentation (no public undocumented APIs)
- **If fixing customer issue: verify regression test exists and passes**
- Follow conventional commit format

### 4. Before Creating PR
- Squash related commits into logical units
- Write clear PR title and description
- Link related issues
- Ensure all tests pass
- Verify code coverage meets 80% target

### 5. Code Review
- Address all feedback professionally
- Respond to comments
- Request re-review if major changes made

---

## Key Technologies & Versions

| Component | Technology | Version |
|-----------|-----------|---------|
| **Language** | C# | 12 |
| **Framework** | .NET | 8 LTS |
| **Web Framework** | ASP.NET Core | 8 |
| **ORM** | Entity Framework Core | 8 |
| **Web UI** | Blazor Server | ASP.NET Core 8 |
| **UI Components** | MudBlazor | 6.x |
| **Testing** | xUnit | 2.6+ |
| **Integration Testing** | Testcontainers | 3.7+ |
| **Mocking** | Moq | 4.20+ |
| **Assertions** | FluentAssertions | 6.12+ |
| **Database** | PostgreSQL / SQLite | 13+ / 3.40+ |
| **Version Control** | Git | Latest |

---

## Enforcement & Tooling

### Automated Enforcement

**`.editorconfig`** - Code formatting
```bash
# Automatically applied by most IDEs
# Configure in IDE settings to follow .editorconfig
```

**`Directory.Build.props`** - Build configuration
```bash
# Applied to all projects automatically
# Sets nullable reference types, language version, etc.
```

**Pre-commit hooks** - Optional local validation
```bash
# Set up in .git/hooks/ to run before commit
# Should validate: formatting, tests, coverage
```

**CI/CD Pipeline** - GitHub Actions
```bash
# Automatic validation on every PR
# Runs: build, tests, coverage, static analysis
```

### Manual Checks

- **Code Review** - Peer review of PRs
- **Architecture Review** - For major decisions
- **Security Review** - For sensitive operations

---

## Common Questions

### Q: Which style guide should I follow?
**A:** All of them. They're organized by area:
- Writing C#? → C# Style Guide
- Building UI? → Blazor UI Guide
- Database work? → Database Guide
- Etc.

### Q: What if the style guide conflicts with my IDE settings?
**A:** The `.editorconfig` file at the root takes precedence. Import its settings into your IDE, or let your IDE auto-format based on `.editorconfig`.

### Q: Can I deviate from the style guide?
**A:** Generally no, but if you have a justified reason:
1. Document your rationale clearly
2. Discuss in pull request
3. Get approval from maintainers
4. Update the relevant style guide if approved

### Q: How are these guidelines updated?
**A:** Style guides are treated as code:
- Changes go through pull request process
- Require discussion and consensus
- Updated in same PR that changes the code

### Q: What about performance vs. style?
**A:** Performance always wins. Document the tradeoff and get approval.

---

## Related Documentation

- **[ROADMAP.md](ROADMAP.md)** - Development roadmap and priorities
- **[ARCHITECTURE.md](ARCHITECTURE.md)** - System architecture overview
- **[GETTING_STARTED.md](GETTING_STARTED.md)** - Developer setup guide
- **[CONTRIBUTING.md](../CONTRIBUTING.md)** - Contribution guidelines
- **[CHANGELOG.md](../CHANGELOG.md)** - Version history

---

## Glossary

| Term | Definition |
|------|-----------|
| **AAA Pattern** | Arrange-Act-Assert - test structure pattern |
| **ADR** | Architecture Decision Record - documents major decisions |
| **DDD** | Domain-Driven Design - development approach |
| **EF Core** | Entity Framework Core - ORM library |
| **FDD** | Feature-Driven Development - planning methodology |
| **LINQ** | Language Integrated Query - C# query syntax |
| **SPC** | Statistical Process Control - quality control method |
| **TDD** | Test-Driven Development - write tests first |
| **Testcontainers** | Docker container library for integration testing (real databases in tests) |
| **UI/UX** | User Interface / User Experience |
| **XML Comments** | C# documentation comments |

---

## Quick Checklist for New Contributors

- [ ] Read README.md
- [ ] Review GETTING_STARTED.md
- [ ] Clone repository locally
- [ ] Set up IDE with .editorconfig settings
- [ ] Review relevant style guide(s)
- [ ] Run tests: `dotnet test`
- [ ] Build project: `dotnet build`
- [ ] Check code coverage: `dotnet test /p:CollectCoverage=true`
- [ ] Create feature branch: `git checkout -b feature/my-feature`
- [ ] Follow conventional commits for commits
- [ ] Submit PR with clear description
- [ ] Respond to code review feedback

---

## Style Guide Maintenance

These guides are living documents. They evolve as:
- The project matures
- Team learns best practices
- Technologies update
- Community standards change

**Keeping guides current:**
- Review annually
- Update when practices change
- Solicit feedback from team
- Document decisions that change
- Link related ADRs (Architecture Decision Records)

---

## Support & Questions

- **General questions:** Check the relevant style guide
- **Technical questions:** Create GitHub issue with `[question]` label
- **Style guide suggestions:** Create issue or PR to suggest changes
- **Build/setup issues:** Check GETTING_STARTED.md or ask in discussions

---

## License

These style guides are part of BenchLibrary, licensed under MIT License.

