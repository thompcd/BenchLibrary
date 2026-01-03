# BenchLibrary

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![Build Status](https://img.shields.io/badge/status-Phase%201%20MVP-blue)](docs/ROADMAP.md)
[![.NET 8](https://img.shields.io/badge/.NET-8.0%20LTS-512BD4)](https://dotnet.microsoft.com/)

Open-source .NET library suite for **Lean Six Sigma** manufacturing processes. Build statistical process control dashboards, capability analysis tools, and Six Sigma analysis features with enterprise-grade quality.

## Status: Phase 1 MVP in Development

BenchLibrary is in active development with an ambitious roadmap. Currently building interactive demo pages that showcase framework capabilities.

### 🎯 Demo Pages Coming Soon

Four demo scenarios featuring the BenchLibrary framework:

1. **Statistical Process Control (SPC) Dashboard**
   - Real-time XBar-R control charts
   - Out-of-control detection
   - Trend analysis and visualization

2. **Process Capability Analysis**
   - Cpk, Cp, Ppk, Pp calculations
   - Specification limit visualization
   - Pass/fail assessment

3. **Measurement Data Visualization**
   - Distribution analysis (histograms)
   - Trend charts and time-series analysis
   - Data filtering and export

4. **Six Sigma Analysis Tools**
   - Pareto analysis for defect categorization
   - Sigma level calculations
   - Root cause prioritization

**Demos deploying to Railway.app** - Live URL coming soon

## 🚀 Quick Start

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)
- Git

### Getting Started
```bash
# Clone repository (coming soon when pushed to GitHub)
git clone https://github.com/[YOUR-USERNAME]/BenchLibrary.git
cd BenchLibrary

# Review development standards
cat claude.md

# Review style guides
cat docs/STYLE_GUIDES.md

# Build solution (coming in Phase 1)
dotnet build

# Run tests (coming in Phase 1)
dotnet test

# Run web app (coming in Phase 1)
dotnet run --project src/BenchLibrary.Web
```

## 📚 Documentation

### For Developers
- **[claude.md](claude.md)** - AI-assisted development guide with quick reference
- **[docs/STYLE_GUIDES.md](docs/STYLE_GUIDES.md)** - Master index of all style guides
- **[docs/ROADMAP.md](docs/ROADMAP.md)** - 20-step implementation roadmap for Phase 1 MVP

### Style Guides by Topic
- **[C# Code Style](docs/STYLE_GUIDE_CSHARP.md)** - C# 12 naming, patterns, async/await
- **[Blazor/Web UI](docs/STYLE_GUIDE_BLAZOR_UI.md)** - Blazor Server and MudBlazor standards
- **[Database](docs/STYLE_GUIDE_DATABASE.md)** - EF Core and PostgreSQL patterns
- **[Testing](docs/STYLE_GUIDE_TESTING.md)** - xUnit, Testcontainers, regression tests
- **[Git Workflow](docs/STYLE_GUIDE_GIT.md)** - Conventional commits and branching
- **[Documentation](docs/STYLE_GUIDE_DOCUMENTATION.md)** - API docs and code comments
- **[Project Structure](docs/STYLE_GUIDE_PROJECT_STRUCTURE.md)** - Solution organization

## 🛠️ Tech Stack

| Component | Technology | Version |
|-----------|-----------|---------|
| **Language** | C# | 12 |
| **Framework** | .NET | 8 LTS |
| **Web Framework** | ASP.NET Core | 8 |
| **Web UI** | Blazor Server | 8 |
| **UI Components** | MudBlazor | 6.x |
| **ORM** | Entity Framework Core | 8 |
| **Database** | PostgreSQL / SQLite | 13+ / 3.40+ |
| **Testing** | xUnit + Testcontainers | 2.6+ / 3.7+ |
| **Deployment** | Railway.app | — |

## 📋 Development Standards

This project follows **strict development standards** documented in the `docs/` folder:

### Code Quality
- **Language:** C# 12 with nullable reference types
- **Testing:** 80% minimum code coverage with Testcontainers for integration tests
- **Regression Tests:** Created for all customer issues/warranty returns
- **Documentation:** XML comments on all public APIs
- **Git:** Conventional commits with atomic, logical changes

### Architecture
- **Domain-Driven Design (DDD)** with layered architecture
- **Dependency Injection** throughout
- **Repository Pattern** for data access
- **Service-based** business logic
- **Testcontainers** for real database integration testing

### UI Standards
- **Blazor Server** with MudBlazor components only
- **Scoped CSS** for styling
- **Accessibility** (ARIA labels, keyboard navigation)
- **Responsive** design (mobile-friendly)

## 🎓 Learning & Contributions

### For First-Time Contributors
1. Read [claude.md](claude.md) - Quick reference for all standards
2. Review relevant style guide from `docs/`
3. Check [ROADMAP](docs/ROADMAP.md) for current priorities
4. See [CONTRIBUTING.md](CONTRIBUTING.md) (coming soon)

### Using Claude Code Online
All documentation is formatted for use with Claude Code web interface. Reference specific style guides while developing to maintain consistency.

## 📊 Project Metrics

**Current Status:** Phase 1 MVP in Development

| Metric | Target | Status |
|--------|--------|--------|
| Demo Pages | 4 | In Development |
| Code Coverage | 80%+ | Planned for Phase 2 |
| Tests | Comprehensive | Planned for Phase 2 |
| Documentation | Complete | ✅ Done |
| Style Guides | 8 areas | ✅ Done |

## 🔗 Important Links

- **Live Demos:** [Coming soon - Railway.app]
- **GitHub Issues:** [To be enabled]
- **Discussions:** [To be enabled]
- **LinkedIn:** Share your feedback!

## 📝 License

This project is licensed under the **MIT License** - see the [LICENSE](LICENSE) file for details.

MIT License allows:
- ✅ Commercial use
- ✅ Modification
- ✅ Distribution
- ✅ Private use

With only a requirement to include the original license and copyright notice.

## 🤝 Support

- **Questions?** Check [docs/STYLE_GUIDES.md](docs/STYLE_GUIDES.md)
- **Development Help?** See [claude.md](claude.md)
- **Architecture Questions?** See [docs/ROADMAP.md](docs/ROADMAP.md)

## 🎯 Next Steps

1. **Demos going live** - Week of [Date TBD]
2. **LinkedIn announcements** - When demos are ready
3. **Phase 2 development** - Core framework implementation
4. **v1.0 Release** - Planned for [Date TBD]

---

**Made with ❤️ for manufacturing excellence**

