# PromptOrganizer Testing and Coverage Guide

## Overview

This document provides a comprehensive guide to testing, test coverage, and Test-Driven Development (TDD) practices for the PromptOrganizer project.

## Quick Start

### Running Tests with Coverage

```bash
# Run all tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Generate detailed coverage report
.\scripts\coverage-report.ps1

# Run with coverage thresholds
.\scripts\coverage-report.ps1 -FailBelowThreshold -LineCoverageThreshold 80 -BranchCoverageThreshold 75
```

### Viewing Coverage Reports

```bash
# Open coverage report in browser
start coverage/report/index.html

# View coverage summary
cat coverage/summary/Summary.txt
```

## Test Structure

```
PromptOrganizer.Tests/
├── Unit/                           # Unit tests
│   ├── Services/                   # Service layer tests
│   ├── Data/                       # Data layer tests
│   └── Domain/                     # Domain entity tests
├── Integration/                    # Integration tests
├── Performance/                    # Performance tests
├── TestHelpers/                    # Test utilities
│   ├── TestDataBuilder.cs          # Test data builders
│   ├── LiteDbTestFactory.cs        # Database test factory
│   └── DatabaseCollection.cs       # Test collection fixtures
└── Fixtures/                       # Test fixtures and data
```

## Coverage Requirements

### Minimum Thresholds

- **Line Coverage**: 80%
- **Branch Coverage**: 75%
- **Method Coverage**: 80%
- **Class Coverage**: 95%

### Quality Gates

Coverage thresholds are enforced in CI/CD pipelines. Builds will fail if coverage falls below these thresholds.

## TDD Guidelines

### Red-Green-Refactor Cycle

1. **Red**: Write a failing test that defines desired behavior
2. **Green**: Write minimum code to make the test pass
3. **Refactor**: Improve code while keeping tests green

### Test Naming Conventions

Use the **Given-When-Then** pattern:
```csharp
public void GivenValidPromptData_WhenCreatingPrompt_ThenReturnsPromptWithId()
```

Or **MethodName_StateUnderTest_ExpectedBehavior**:
```csharp
public void CreatePrompt_WithValidData_ShouldReturnPromptWithId()
```

## Test Data Management

### Using Test Data Builders

```csharp
// Create a prompt with specific properties
var prompt = new PromptBuilder()
    .WithId(1)
    .WithTitle("Test Prompt")
    .WithContent("Test content")
    .WithCategory(developmentCategory)
    .AsActive()
    .Build();

// Create multiple prompts
var prompts = TestDataFactory.CreatePrompts(10);
```

### Test Data Scenarios

```csharp
// Create complete test scenario
var scenario = ScenarioBuilder.CreateCompleteScenario();
var categories = scenario.Categories;
var prompts = scenario.Prompts;
```

## Mocking and Test Doubles

### When to Use Mocks

- External dependencies (database, file system, web services)
- Time-dependent code
- Configuration and settings
- Third-party services

### Mocking Best Practices

```csharp
[Fact]
public void CreatePrompt_WithRepositoryError_ShouldLogError()
{
    // Arrange
    var repositoryMock = new Mock<IPromptRepository>();
    var loggerMock = new Mock<ILogger<PromptService>>();
    var service = new PromptService(repositoryMock.Object, loggerMock.Object);
    
    repositoryMock.Setup(r => r.CreateAsync(It.IsAny<Prompt>()))
                  .ThrowsAsync(new DatabaseException("Error"));
    
    // Act & Assert
    Assert.ThrowsAsync<DatabaseException>(
        async () => await service.CreatePromptAsync(data)
    );
    
    loggerMock.Verify(
        x => x.LogError(It.IsAny<DatabaseException>(), It.IsAny<string>()),
        Times.Once
    );
}
```

## Performance Testing

### Benchmark Tests

```csharp
[MemoryDiagnoser]
public class PromptServiceBenchmarks
{
    [Benchmark]
    public void SearchPrompts_Benchmark()
    {
        _service.SearchPrompts("test");
    }
}
```

### Load Testing

```csharp
[Test]
public async Task CreatePrompt_ConcurrentRequests_ShouldHandleLoad()
{
    var tasks = Enumerable.Range(0, 100)
        .Select(i => service.CreatePromptAsync(data));
    
    await Task.WhenAll(tasks);
}
```

## CI/CD Integration

### GitHub Actions

The project includes comprehensive CI/CD workflows:

- **Test Execution**: Runs all tests with coverage collection
- **Coverage Analysis**: Generates and uploads coverage reports
- **Quality Gates**: Enforces coverage thresholds
- **PR Comments**: Automatically comments on pull requests with coverage metrics
- **Mutation Testing**: Runs mutation tests to verify test quality
- **Performance Testing**: Executes performance benchmarks
- **Security Scanning**: Performs security analysis

### Coverage Reports in CI

Coverage reports are automatically:
- Generated for every build
- Uploaded as artifacts
- Published to GitHub Pages
- Commented on pull requests

## Test Maintenance

### Regular Review Process

1. **Monthly Reviews**: Review test coverage and quality
2. **Refactoring**: Update tests alongside production code
3. **Documentation**: Keep test documentation current
4. **Performance**: Monitor and optimize slow tests
5. **Coverage**: Maintain coverage thresholds

### Test Evolution

When requirements change:
1. Update tests first (TDD principle)
2. Ensure tests reflect new behavior
3. Remove obsolete tests
4. Add tests for new functionality

## Tools and Scripts

### Coverage Scripts

- **`scripts/coverage-report.ps1`**: PowerShell script for Windows
- **`scripts/coverage-report.sh`**: Bash script for Unix/Linux/macOS
- **`.github/workflows/test-coverage.yml`**: CI/CD pipeline configuration

### Analysis Tools

- **ReportGenerator**: Creates detailed HTML coverage reports
- **Stryker.NET**: Mutation testing framework
- **BenchmarkDotNet**: Performance benchmarking
- **Security Code Scan**: Security analysis

## Best Practices

### DOs

✅ Write tests first (TDD)  
✅ Keep tests simple and focused  
✅ Use descriptive test names  
✅ Test edge cases and error conditions  
✅ Use test data builders  
✅ Mock external dependencies  
✅ Run tests frequently  
✅ Maintain coverage thresholds  
✅ Document complex test scenarios  
✅ Use consistent patterns  

### DON'Ts

❌ Test private methods  
❌ Write complex test logic  
❌ Depend on test order  
❌ Use production data in tests  
❌ Ignore failing tests  
❌ Over-mock dependencies  
❌ Test framework code  
❌ Use magic numbers  
❌ Catch exceptions silently  
❌ Duplicate test code  

## Troubleshooting

### Common Issues

1. **Low Coverage**: Use coverage analysis to identify gaps
2. **Slow Tests**: Optimize test data setup and teardown
3. **Flaky Tests**: Ensure test isolation and independence
4. **Build Failures**: Check coverage thresholds and quality gates

### Getting Help

- Review the comprehensive TDD guidelines in [`docs/TESTING-TDD-GUIDELINES.md`](docs/TESTING-TDD-GUIDELINES.md)
- Check coverage analysis guide in [`docs/TEST-COVERAGE-ANALYSIS.md`](docs/TEST-COVERAGE-ANALYSIS.md)
- Examine existing test examples in the codebase
- Consult the development team for specific questions

## Resources

### Documentation

- [TDD Guidelines](docs/TESTING-TDD-GUIDELINES.md) - Comprehensive TDD practices
- [Coverage Analysis](docs/TEST-COVERAGE-ANALYSIS.md) - Coverage analysis techniques
- [CI/CD Workflows](.github/workflows/test-coverage.yml) - Automated testing pipeline

### External Resources

- [xUnit Documentation](https://xunit.net/)
- [Moq Documentation](https://github.com/moq/moq)
- [FluentAssertions Documentation](https://fluentassertions.com/)
- [ReportGenerator Documentation](https://github.com/danielpalme/ReportGenerator)
- [Stryker.NET Documentation](https://stryker-mutator.io/docs/stryker-net/)

---

For questions or suggestions regarding testing practices, please create an issue in the project repository or contact the development team.