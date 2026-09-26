# PromptOrganizer Project Memory Bank

## Project Overview
1. Project Overview

PromptOrganizer is an offline-first productivity application for organizing, managing, and quickly retrieving AI prompts using a visual board + columns + cards system. It is designed for AI engineers, prompt engineers, developers, researchers, and creators who work heavily with LLMs and need structured prompt management.

Version R1 targets Windows desktop first, with planned expansion to Android and iOS through cloud synchronization in future releases.

2. Technology Stack

Framework: Uno Platform (.NET 10.0)

Primary Platform (R1): Windows Desktop (packaged app) - CURRENT BUILD TARGET

Build Configuration: Desktop-only (net10.0-desktop)

Future Platforms: Android (read+sync), iOS (read+sync), Web (read-only viewer) - Disabled for R1 focus

UI Technology: XAML with WinUI visual layer

Architecture: Clean Architecture + MVVM + Repository Pattern

Database: LiteDB (encrypted local storage)

Testing: xUnit

Language: C# with nullable reference types and implicit usings enabled

3. Solution Architecture
PromptOrganizer.sln
├── PromptOrganizer.App/          # Presentation layer (Uno UI)
├── PromptOrganizer.Domain/       # Domain logic and core entities
├── PromptOrganizer.Data/         # Data access layer + LiteDB repositories
├── PromptOrganizer.Services/     # Business logic + use cases
└── PromptOrganizer.Tests/        # Unit tests

Dependency Flow

App → Services, Domain

Services → Data, Domain

Data → Domain

Tests → Domain (isolation)

4. Application Features
Core Functionalities (R1)

Visual Board System — Multiple columns for organizing prompt groups

Prompt Cards — title, icon, label color, tags, preview body, quick actions

Markdown Prompt Editor Modal — full prompt editing + advanced settings

Global Search Overlay (Ctrl+K)

Copy to Clipboard (Ctrl+Enter / Ctrl+C)

Favorites / Pin

Soft Delete + Trash system with restore and 30-day auto-purge

Undo snackbar

Encrypted offline database (LiteDB)

Light / Dark mode switching

Future Enhancements (Roadmap)

Drag & Drop reordering across columns (R1.1 polish)

Import / Export JSON

Cloud Sync (Pro upgrade)

Android & iOS companion apps

Version history

Advanced search filters

AI-assisted auto-categorization

Templates + plugin ecosystem

5. Search Strategy
Hybrid Search Model
Phase	Implementation
R1	Simple Contains() search (title, tags, body)
R2+	Full-Text Search indexing (auto-enabled for >300 prompts)
6. UI Design System
Visual Style

Minimal, calm, focused (Notion + Linear + Raycast tone)

Medium density layout; productivity over decoration

Subtle elevation and motion

Color Palette
Purpose	Value
Light background	#F7F7F8
Dark background	#111316
Accent colors	Blue #3B82F6, Green #22C55E, Yellow #FACC15, Pink #EC4899, Red #EF4444, Violet #8B5CF6
Typography

UI: Segoe UI

Editor: Cascadia Mono (Markdown content only)

Spacing & Geometry

Base spacing unit: 4px

Corners: Cards 8px, Chips 10px, Buttons 6px, Modals 12px

Motion & Microinteractions

Hover: 120–140ms

Modal open/close: 240–260ms scale + fade

Drag lift: soft elevation + ease-out curve

Snackbar slide-up notification

7. Interaction Model
Keyboard Shortcuts
Action	Shortcut
Search	Ctrl + K
New Prompt	Ctrl + N
Copy prompt	Ctrl + Enter / Ctrl + C
Save	Ctrl + S
Favorite	F
Close modal/overlay	Esc
8. Trash / Deletion Lifecycle
Delete → moves to Trash (soft delete)
↓ Snackbar: “Prompt moved to Trash — Undo”
↓ Trash retains items for 30 days
↓ Items can be restored or permanently removed
↓ Auto purge runs daily

9. Development Guidelines
Code Standards

Nullable reference types enabled

Clean naming & .NET conventions

XML docs for public APIs

UI Guidelines

XAML bindings and state management

ResourceDictionary for themes

VisualStateManager for interactions

Keyboard-first workflow

Testing Strategy

Domain testing only in xUnit

Repository tests with temporary LiteDB instance

10. Current Status

Functional base elements are completed:

Initial UI structure with board layout

Prompt card component shell

Modal editor shell structure

Global search overlay structure

Light/Dark theme foundation

Interaction controls and hover behaviors

11. Release Roadmap
R1 – Public Preview (Windows)

Visual board system

Prompt CRUD + editor modal

Search overlay

Soft delete + Trash

Drag-and-drop

Import / Export JSON (optional R1.1)

R2 – Productivity Expansion

Cloud Sync (Pro upgrade)

Android & iOS companion viewer

Template library

R3 – Power Features

Version history

Marketplace & plugins

AI auto-categorization

12. CI/CD & Deployment

MSBuild multi-target builds

Packaging for Microsoft Store

Hot reload enabled

Lightweight logging in production

## Build Configuration

### Current Build Target
- **Primary Target**: Desktop (Windows) - `net10.0-desktop`
- **Build Scripts**:
  - `build-desktop.bat` (Windows Command Line)
  - `build-desktop.ps1` (PowerShell/Cross-platform)
- **Build Command**: `dotnet build -f net10.0-desktop`

### Build Process
1. Clean previous builds
2. Restore dependencies
3. Build for desktop target framework
4. Output location: `PromptOrganizer.App\PromptOrganizer.App\bin\Debug\net10.0-desktop\`

### Development Notes
- Multi-platform targets disabled for R1 focus on Windows Desktop
- LiteDB used for local encrypted storage
- SkiaRenderer enabled for consistent cross-platform rendering
- Hot reload supported for development

## Test-Driven Development (TDD) Implementation

### Testing Architecture
- **Framework**: xUnit with FluentAssertions for readable assertions
- **Coverage Tools**: Coverlet for coverage collection, ReportGenerator for reports
- **Mocking**: Moq framework for creating test doubles
- **Test Data**: Builder pattern with fluent interfaces for test data creation
- **Database Testing**: LiteDB in-memory databases for isolated testing

### Test Organization
```
PromptOrganizer.Tests/
├── Domain/Entities/          # Domain entity unit tests
├── Data/Repositories/        # Repository pattern tests
├── Services/                 # Business logic service tests
├── TestHelpers/              # Base classes, builders, fixtures
├── Integration/              # Cross-layer integration tests
└── Performance/              # Performance and load tests
```

### Test Coverage Requirements
- **Minimum Thresholds**: 80% line coverage, 75% branch coverage, 80% method coverage
- **Coverage Reports**: HTML, JSON, and Cobertura formats with risk hotspot analysis
- **CI/CD Integration**: Automated coverage collection and threshold enforcement
- **Mutation Testing**: Stryker.NET for advanced test quality verification

### TDD Workflow
1. **Red**: Write failing test that defines expected behavior
2. **Green**: Write minimal code to make test pass
3. **Refactor**: Improve code structure while keeping tests green
4. **Repeat**: Continue cycle for each new feature/behavior

### Test Naming Conventions
- **Pattern**: `[MethodName]_[Scenario]_[ExpectedResult]`
- **Examples**: `Constructor_WithValidParameters_ShouldCreatePromptSuccessfully`
- **Organization**: Unit vs Integration test separation
- **Categories**: Use traits for test categorization (Unit, Integration, Performance)

### Test Data Management
- **Builders**: Fluent builders for creating test entities
- **Fixtures**: Pre-configured test data scenarios
- **Factories**: Test database and context creation
- **Scenarios**: Complete workflow test scenarios

### Business Logic Testing
- **Domain Entities**: 36 comprehensive tests for Prompt entity
- **Repository Pattern**: 58 tests with 94% coverage for data access
- **Service Layer**: 119 tests with 100% coverage for business logic
- **Integration Tests**: Cross-layer workflow validation

### Test Execution
- **Scripts**: PowerShell and Bash scripts for coverage reporting
- **Commands**: `dotnet test` with coverage collection
- **Reports**: Generated in `coverage/` directory with detailed analysis
- **Quality Gates**: Automated enforcement in CI/CD pipeline

### Performance Testing
- **Benchmarking**: BenchmarkDotNet integration for performance metrics
- **Load Testing**: Simulated user load scenarios
- **Memory Testing**: Memory leak detection and profiling

### Development Guidelines
- **TDD Principles**: Comprehensive guidelines in [`docs/TESTING-TDD-GUIDELINES.md`](docs/TESTING-TDD-GUIDELINES.md)
- **Best Practices**: Test organization, naming, and maintenance guidelines
- **Anti-patterns**: Common testing mistakes to avoid
- **Code Quality**: Maintainability and readability standards

### CI/CD Integration
- **GitHub Actions**: Automated test execution with coverage reporting
- **PR Comments**: Coverage metrics posted to pull requests
- **Quality Enforcement**: Build failures on coverage threshold violations
- **Documentation**: Auto-generated coverage reports deployed to GitHub Pages

### Current Test Statistics
- **Total Tests**: 213+ tests across all layers
- **Coverage**: 94%+ overall coverage with comprehensive business logic testing
- **Success Rate**: 100% passing tests in service layer, 94% in repository layer
- **Test Types**: Unit, Integration, Performance, and Mutation tests

This memory bank serves as a comprehensive reference for understanding the PromptOrganizer project's architecture, design system, implementation details, and testing practices.