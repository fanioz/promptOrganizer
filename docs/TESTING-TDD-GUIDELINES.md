# Test-Driven Development (TDD) Guidelines for PromptOrganizer

## Table of Contents
1. [Overview](#overview)
2. [TDD Principles](#tdd-principles)
3. [Test Organization](#test-organization)
4. [Naming Conventions](#naming-conventions)
5. [Test Structure](#test-structure)
6. [Test Data Management](#test-data-management)
7. [Mocking and Test Doubles](#mocking-and-test-doubles)
8. [Coverage Requirements](#coverage-requirements)
9. [Performance Testing](#performance-testing)
10. [CI/CD Integration](#cicd-integration)
11. [Test Maintenance](#test-maintenance)
12. [Best Practices](#best-practices)
13. [Common Patterns](#common-patterns)
14. [Anti-Patterns to Avoid](#anti-patterns-to-avoid)

## Overview

This document provides comprehensive guidelines for implementing Test-Driven Development (TDD) in the PromptOrganizer project. Following these guidelines ensures high-quality, maintainable, and reliable code through systematic testing practices.

### TDD Cycle (Red-Green-Refactor)

1. **Red**: Write a failing test that defines the desired behavior
2. **Green**: Write the minimum code necessary to make the test pass
3. **Refactor**: Improve the code while keeping tests green

## TDD Principles

### Core Principles

1. **Test First**: Always write tests before implementation code
2. **Small Steps**: Make incremental changes with frequent test runs
3. **Fast Feedback**: Tests should run quickly (< 1 second per test)
4. **Isolated Tests**: Each test should be independent and isolated
5. **Descriptive Names**: Test names should clearly describe the behavior
6. **One Assertion per Test**: Focus each test on a single behavior
7. **No Logic in Tests**: Tests should be simple and straightforward

### TDD Workflow

```csharp
// 1. Write failing test (Red)
[Fact]
public void CreatePrompt_WithValidData_ShouldReturnPromptWithId()
{
    // Arrange
    var promptService = new PromptService(repository);
    var promptData = new PromptData { Title = "Test", Content = "Content" };
    
    // Act
    var result = promptService.CreatePrompt(promptData);
    
    // Assert
    result.Should().NotBeNull();
    result.Id.Should().BeGreaterThan(0);
}

// 2. Write minimal implementation (Green)
public Prompt CreatePrompt(PromptData data)
{
    return new Prompt { Id = 1, Title = data.Title, Content = data.Content };
}

// 3. Refactor while keeping tests green
```

## Test Organization

### Project Structure

```
PromptOrganizer.Tests/
├── Unit/
│   ├── Services/
│   │   ├── PromptServiceTests.cs
│   │   ├── CategoryServiceTests.cs
│   │   └── SearchServiceTests.cs
│   ├── Data/
│   │   ├── Repositories/
│   │   │   ├── PromptRepositoryTests.cs
│   │   │   └── CategoryRepositoryTests.cs
│   │   └── Models/
│   └── Domain/
│       └── Entities/
├── Integration/
│   ├── Database/
│   │   ├── LiteDbIntegrationTests.cs
│   │   └── RepositoryIntegrationTests.cs
│   └── Services/
│       └── ServiceIntegrationTests.cs
├── Performance/
│   ├── Benchmarks/
│   └── LoadTests/
├── TestHelpers/
│   ├── TestDataBuilder.cs
│   ├── MockFactory.cs
│   ├── DatabaseCollection.cs
│   └── LiteDbTestFactory.cs
└── Fixtures/
    ├── TestData/
    └── Configurations/
```

### Test Categories

1. **Unit Tests**: Test individual components in isolation
2. **Integration Tests**: Test component interactions
3. **Performance Tests**: Measure performance characteristics
4. **End-to-End Tests**: Test complete user workflows

## Naming Conventions

### Test Method Naming

Use the **Given-When-Then** or **MethodName_StateUnderTest_ExpectedBehavior** pattern:

```csharp
// Given-When-Then pattern
public void GivenValidPromptData_WhenCreatingPrompt_ThenReturnsPromptWithGeneratedId()

// MethodName_StateUnderTest_ExpectedBehavior pattern
public void CreatePrompt_WithValidData_ShouldReturnPromptWithId()

// BDD-style naming
public void Should_CreatePrompt_WithGeneratedId_When_ValidDataProvided()
```

### Test Class Naming

```csharp
// Service tests
public class PromptServiceTests
public class CategoryServiceTests

// Repository tests
public class PromptRepositoryTests
public class CategoryRepositoryTests

// Entity tests
public class PromptTests
public class CategoryTests
```

### Test Data Naming

```csharp
// Constants for test data
private const string ValidPromptTitle = "Test Prompt Title";
private const string ValidPromptContent = "This is test prompt content";
private const int ValidPromptId = 1;

// Test data builders
var promptBuilder = new PromptBuilder()
    .WithTitle("Test Title")
    .WithContent("Test Content")
    .WithCategory("Test Category");
```

## Test Structure

### Standard Test Structure (AAA Pattern)

```csharp
[Fact]
public void CreatePrompt_WithValidData_ShouldReturnPromptWithId()
{
    // Arrange
    var repository = new Mock<IPromptRepository>();
    var service = new PromptService(repository.Object);
    var promptData = new PromptData 
    { 
        Title = "Test Title", 
        Content = "Test Content" 
    };
    
    repository.Setup(r => r.Create(It.IsAny<Prompt>()))
              .ReturnsAsync(new Prompt { Id = 1 });
    
    // Act
    var result = service.CreatePrompt(promptData);
    
    // Assert
    result.Should().NotBeNull();
    result.Id.Should().BeGreaterThan(0);
    result.Title.Should().Be(promptData.Title);
    result.Content.Should().Be(promptData.Content);
}
```

### Test Setup and Teardown

```csharp
public class PromptServiceTests : IDisposable
{
    private readonly Mock<IPromptRepository> _repository;
    private readonly PromptService _service;
    private readonly Mock<ILogger<PromptService>> _logger;
    
    public PromptServiceTests()
    {
        // Setup (runs before each test)
        _repository = new Mock<IPromptRepository>();
        _logger = new Mock<ILogger<PromptService>>();
        _service = new PromptService(_repository.Object, _logger.Object);
    }
    
    public void Dispose()
    {
        // Teardown (runs after each test)
        _repository.VerifyAll();
    }
}
```

## Test Data Management

### Test Data Builders

```csharp
public class PromptBuilder
{
    private int _id = 1;
    private string _title = "Default Title";
    private string _content = "Default Content";
    private string _category = "Default Category";
    private DateTime _createdAt = DateTime.UtcNow;
    
    public PromptBuilder WithId(int id)
    {
        _id = id;
        return this;
    }
    
    public PromptBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }
    
    public PromptBuilder WithContent(string content)
    {
        _content = content;
        return this;
    }
    
    public PromptBuilder WithCategory(string category)
    {
        _category = category;
        return this;
    }
    
    public Prompt Build()
    {
        return new Prompt
        {
            Id = _id,
            Title = _title,
            Content = _content,
            Category = _category,
            CreatedAt = _createdAt
        };
    }
}
```

### Test Data Factories

```csharp
public static class TestDataFactory
{
    public static Prompt CreateValidPrompt()
    {
        return new PromptBuilder()
            .WithTitle("Valid Prompt Title")
            .WithContent("Valid prompt content")
            .WithCategory("Test Category")
            .Build();
    }
    
    public static List<Prompt> CreatePrompts(int count)
    {
        return Enumerable.Range(1, count)
            .Select(i => new PromptBuilder()
                .WithId(i)
                .WithTitle($"Prompt {i}")
                .WithContent($"Content {i}")
                .Build())
            .ToList();
    }
}
```

### Database Test Data

```csharp
public class DatabaseSeeder
{
    public static void SeedTestData(LiteDatabase database)
    {
        var prompts = database.GetCollection<Prompt>("prompts");
        var categories = database.GetCollection<Category>("categories");
        
        // Seed categories
        var testCategories = new[]
        {
            new Category { Id = 1, Name = "Development", Description = "Development prompts" },
            new Category { Id = 2, Name = "Testing", Description = "Testing prompts" }
        };
        categories.InsertBulk(testCategories);
        
        // Seed prompts
        var testPrompts = new[]
        {
            new Prompt 
            { 
                Id = 1, 
                Title = "Test Prompt 1", 
                Content = "Content 1",
                CategoryId = 1 
            },
            new Prompt 
            { 
                Id = 2, 
                Title = "Test Prompt 2", 
                Content = "Content 2",
                CategoryId = 2 
            }
        };
        prompts.InsertBulk(testPrompts);
    }
}
```

## Mocking and Test Doubles

### When to Use Mocks

1. **External Dependencies**: Database, file system, web services
2. **Time-Dependent Code**: DateTime, timers, delays
3. **Random Number Generation**: Guid, Random
4. **Configuration**: App settings, environment variables
5. **Third-Party Libraries**: External APIs, services

### Mocking Best Practices

```csharp
public class PromptServiceTests
{
    private readonly Mock<IPromptRepository> _repository;
    private readonly Mock<ILogger<PromptService>> _logger;
    private readonly Mock<IConfiguration> _configuration;
    private readonly PromptService _service;
    
    public PromptServiceTests()
    {
        _repository = new Mock<IPromptRepository>();
        _logger = new Mock<ILogger<PromptService>>();
        _configuration = new Mock<IConfiguration>();
        _service = new PromptService(_repository.Object, _logger.Object, _configuration.Object);
    }
    
    [Fact]
    public void CreatePrompt_WithRepositoryError_ShouldLogErrorAndThrow()
    {
        // Arrange
        var promptData = new PromptData { Title = "Test", Content = "Content" };
        var expectedException = new DatabaseException("Database error");
        
        _repository.Setup(r => r.Create(It.IsAny<Prompt>()))
                   .ThrowsAsync(expectedException);
        
        // Act & Assert
        var exception = Assert.ThrowsAsync<DatabaseException>(
            async () => await _service.CreatePrompt(promptData)
        );
        
        exception.Should().Be(expectedException);
        
        _logger.Verify(
            x => x.LogError(
                It.IsAny<DatabaseException>(),
                It.Is<string>(s => s.Contains("Error creating prompt"))
            ),
            Times.Once
        );
    }
}
```

### Test Doubles Types

1. **Fake**: Working implementation with simplified functionality
2. **Mock**: Object with expectations about interactions
3. **Stub**: Object with predefined responses
4. **Spy**: Object that records interactions for verification

```csharp
// Fake implementation
public class FakePromptRepository : IPromptRepository
{
    private readonly List<Prompt> _prompts = new();
    
    public Task<Prompt> CreateAsync(Prompt prompt)
    {
        prompt.Id = _prompts.Count + 1;
        _prompts.Add(prompt);
        return Task.FromResult(prompt);
    }
    
    public Task<Prompt?> GetByIdAsync(int id)
    {
        return Task.FromResult(_prompts.FirstOrDefault(p => p.Id == id));
    }
}

// Using fakes in tests
[Fact]
public void CreatePrompt_WithFakeRepository_ShouldWork()
{
    // Arrange
    var repository = new FakePromptRepository();
    var service = new PromptService(repository, logger);
    var promptData = new PromptData { Title = "Test", Content = "Content" };
    
    // Act
    var result = service.CreatePrompt(promptData);
    
    // Assert
    result.Should().NotBeNull();
    result.Id.Should().BeGreaterThan(0);
}
```

## Coverage Requirements

### Minimum Coverage Thresholds

- **Line Coverage**: 80%
- **Branch Coverage**: 75%
- **Method Coverage**: 80%

### Coverage Exclusions

```xml
<!-- In Directory.Build.props or .runsettings -->
<Exclude>
  [PromptOrganizer.Tests]*    <!-- Test projects -->
  [PromptOrganizer.Migrations]*  <!-- Database migrations -->
  [PromptOrganizer.Program]*     <!-- Program.cs files -->
  *.Generated.cs                 <!-- Generated code -->
</Exclude>

<ExcludeByAttribute>
  GeneratedCodeAttribute
  CompilerGeneratedAttribute
  ExcludeFromCodeCoverageAttribute
</ExcludeByAttribute>
```

### Coverage Analysis

```powershell
# Generate coverage report
.\scripts\coverage-report.ps1 -Configuration Release -FailBelowThreshold

# View coverage report
start coverage/report/index.html
```

## Performance Testing

### Benchmark Tests

```csharp
[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net80)]
public class PromptServiceBenchmarks
{
    private IPromptService _service;
    private List<Prompt> _testPrompts;
    
    [GlobalSetup]
    public void Setup()
    {
        _service = new PromptService(CreateRepository(), CreateLogger());
        _testPrompts = TestDataFactory.CreatePrompts(1000);
    }
    
    [Benchmark]
    public void SearchPrompts_Benchmark()
    {
        _service.SearchPrompts("test");
    }
    
    [Benchmark]
    public void CreatePrompt_Benchmark()
    {
        var data = new PromptData { Title = "Benchmark", Content = "Content" };
        _service.CreatePrompt(data);
    }
}
```

### Load Testing

```csharp
[Test]
public async Task CreatePrompt_ConcurrentRequests_ShouldHandleLoad()
{
    // Arrange
    var service = new PromptService(repository, logger);
    var tasks = new List<Task>();
    var successCount = 0;
    
    // Act
    for (int i = 0; i < 100; i++)
    {
        tasks.Add(Task.Run(async () =>
        {
            try
            {
                var data = new PromptData { Title = $"Prompt {i}", Content = "Content" };
                await service.CreatePrompt(data);
                Interlocked.Increment(ref successCount);
            }
            catch
            {
                // Handle concurrent access issues
            }
        }));
    }
    
    await Task.WhenAll(tasks);
    
    // Assert
    successCount.Should().BeGreaterThan(90);
}
```

## CI/CD Integration

### GitHub Actions Workflow

```yaml
name: Test and Coverage

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main, develop ]

jobs:
  test:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v4
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: '8.0.x'
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build
      run: dotnet build --no-restore
    
    - name: Test with coverage
      run: |
        dotnet test --no-build --collect:"XPlat Code Coverage" \
          --results-directory:./coverage \
          -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=cobertura
    
    - name: Generate coverage report
      run: |
        dotnet tool install -g dotnet-reportgenerator-globaltool
        reportgenerator -reports:./coverage/**/coverage.cobertura.xml \
          -targetdir:./coverage/report -reporttypes:Html
    
    - name: Upload coverage reports
      uses: actions/upload-artifact@v4
      with:
        name: coverage-report
        path: ./coverage/report
    
    - name: Comment PR with coverage
      if: github.event_name == 'pull_request'
      uses: 5monkeys/cobertura-action@master
      with:
        path: ./coverage/**/coverage.cobertura.xml
        minimum_coverage: 80
        fail_below_threshold: true
```

### Azure DevOps Pipeline

```yaml
trigger:
  branches:
    include:
    - main
    - develop

pool:
  vmImage: 'ubuntu-latest'

steps:
- task: DotNetCoreCLI@2
  displayName: 'Restore packages'
  inputs:
    command: 'restore'
    projects: '**/*.csproj'

- task: DotNetCoreCLI@2
  displayName: 'Build solution'
  inputs:
    command: 'build'
    projects: '**/*.csproj'
    arguments: '--configuration Release'

- task: DotNetCoreCLI@2
  displayName: 'Run tests with coverage'
  inputs:
    command: 'test'
    projects: '**/*Tests.csproj'
    arguments: '--configuration Release --collect:"XPlat Code Coverage"'
    publishTestResults: true

- task: PublishCodeCoverageResults@1
  displayName: 'Publish coverage results'
  inputs:
    codeCoverageTool: 'Cobertura'
    summaryFileLocation: '$(Agent.TempDirectory)/**/coverage.cobertura.xml'
    reportDirectory: '$(Agent.TempDirectory)/coverage'
    failIfCoverageEmpty: true
```

## Test Maintenance

### Test Review Process

1. **Regular Review**: Review tests monthly for relevance and quality
2. **Refactoring**: Refactor tests alongside production code
3. **Documentation**: Keep test documentation up to date
4. **Performance**: Monitor test execution time and optimize slow tests
5. **Coverage**: Maintain coverage thresholds and add tests for new code

### Test Evolution Guidelines

```csharp
// When requirements change, update tests first
[Fact]
public void UpdatePrompt_WithNewRequirements_ShouldValidateTitleLength()
{
    // Arrange
    var service = new PromptService(repository);
    var promptData = new PromptData 
    { 
        Title = "A", // Too short - should fail validation
        Content = "Content" 
    };
    
    // Act & Assert
    var exception = Assert.Throws<ValidationException>(
        () => service.UpdatePrompt(1, promptData)
    );
    
    exception.Message.Should().Contain("Title must be at least 3 characters");
}

// When adding new features, write tests first
[Fact]
public void SearchPrompts_WithFilters_ShouldReturnFilteredResults()
{
    // Arrange
    var service = new PromptService(repository);
    var searchCriteria = new SearchCriteria
    {
        Query = "test",
        Category = "Development",
        DateFrom = DateTime.Now.AddDays(-7)
    };
    
    // Act
    var results = service.SearchPrompts(searchCriteria);
    
    // Assert
    results.Should().OnlyContain(p => 
        p.Title.Contains("test") && 
        p.Category == "Development" &&
        p.CreatedAt >= searchCriteria.DateFrom
    );
}
```

### Test Cleanup

```csharp
// Remove obsolete tests
[Obsolete("Replaced by SearchPrompts_WithFilters_ShouldReturnFilteredResults")]
[Fact]
public void SearchPrompts_BasicSearch_ShouldReturnResults()
{
    // This test is obsolete and should be removed
}

// Update tests when refactoring
[Fact]
public void CreatePrompt_UsingNewFactory_ShouldCreateValidPrompt()
{
    // Updated to use new factory pattern
    var factory = new PromptFactory();
    var prompt = factory.Create("Title", "Content");
    
    prompt.Should().NotBeNull();
    prompt.IsValid().Should().BeTrue();
}
```

## Best Practices

### DOs

1. **Write tests first**: Always start with a failing test
2. **Keep tests simple**: One concept per test
3. **Use descriptive names**: Test names should explain the behavior
4. **Test edge cases**: Null values, empty strings, boundary conditions
5. **Use test data builders**: Create reusable test data
6. **Mock external dependencies**: Keep tests fast and reliable
7. **Run tests frequently**: After every small change
8. **Maintain test coverage**: Keep coverage above thresholds
9. **Document test intent**: Use comments for complex scenarios
10. **Use consistent patterns**: Follow established conventions

### DON'Ts

1. **Don't test private methods**: Test public behavior
2. **Don't write complex test logic**: Tests should be simple
3. **Don't depend on test order**: Tests should be independent
4. **Don't use production data**: Use controlled test data
5. **Don't ignore failing tests**: Fix or remove them
6. **Don't over-mock**: Mock only external dependencies
7. **Don't test framework code**: Focus on your business logic
8. **Don't use magic numbers**: Use named constants
9. **Don't catch exceptions silently**: Let tests fail appropriately
10. **Don't duplicate test code**: Use helper methods and builders

## Common Patterns

### Repository Pattern Testing

```csharp
public class PromptRepositoryTests : IDisposable
{
    private readonly LiteDatabase _database;
    private readonly PromptRepository _repository;
    
    public PromptRepositoryTests()
    {
        _database = new LiteDatabase(":memory:");
        _repository = new PromptRepository(_database);
    }
    
    [Fact]
    public void Create_WithValidPrompt_ShouldPersistToDatabase()
    {
        // Arrange
        var prompt = new PromptBuilder().Build();
        
        // Act
        var result = _repository.Create(prompt);
        
        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        
        // Verify persistence
        var stored = _repository.GetById(result.Id);
        stored.Should().BeEquivalentTo(prompt, options => 
            options.Excluding(p => p.Id));
    }
    
    public void Dispose()
    {
        _database.Dispose();
    }
}
```

### Service Layer Testing

```csharp
public class PromptServiceTests
{
    [Fact]
    public async Task CreatePrompt_WithValidData_ShouldCallRepository()
    {
        // Arrange
        var repository = new Mock<IPromptRepository>();
        var service = new PromptService(repository.Object);
        var promptData = new PromptData { Title = "Test", Content = "Content" };
        
        repository.Setup(r => r.CreateAsync(It.IsAny<Prompt>()))
                  .ReturnsAsync(new Prompt { Id = 1 });
        
        // Act
        var result = await service.CreatePromptAsync(promptData);
        
        // Assert
        result.Should().NotBeNull();
        repository.Verify(r => r.CreateAsync(It.Is<Prompt>(p => 
            p.Title == promptData.Title && 
            p.Content == promptData.Content
        )), Times.Once);
    }
}
```

### Controller Testing

```csharp
public class PromptControllerTests
{
    [Fact]
    public async Task Create_WithValidModel_ShouldReturnCreatedResult()
    {
        // Arrange
        var service = new Mock<IPromptService>();
        var controller = new PromptController(service.Object);
        var model = new CreatePromptModel { Title = "Test", Content = "Content" };
        
        service.Setup(s => s.CreatePromptAsync(It.IsAny<PromptData>()))
               .ReturnsAsync(new Prompt { Id = 1, Title = model.Title });
        
        // Act
        var result = await controller.Create(model);
        
        // Assert
        var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be("Get");
        createdResult.RouteValues["id"].Should().Be(1);
    }
}
```

## Anti-Patterns to Avoid

### 1. The Giant Test

```csharp
// ❌ Bad: Too many assertions and setup
[Fact]
public void ProcessPrompt_ComplexScenario_ShouldWork()
{
    // 50+ lines of setup
    // 20+ lines of act
    // 30+ lines of assertions
}

// ✅ Good: Break into smaller tests
[Fact]
public void ProcessPrompt_WithValidData_ShouldCreatePrompt()
[Fact]
public void ProcessPrompt_WithInvalidData_ShouldThrowValidationException()
[Fact]
public void ProcessPrompt_WithExistingTitle_ShouldReturnDuplicateError()
```

### 2. The Mock Explosion

```csharp
// ❌ Bad: Too many mocks
[Fact]
public void CreatePrompt_WithManyDependencies_ShouldWork()
{
    var repoMock = new Mock<IPromptRepository>();
    var loggerMock = new Mock<ILogger<PromptService>>();
    var configMock = new Mock<IConfiguration>();
    var cacheMock = new Mock<ICacheService>();
    var validatorMock = new Mock<IValidator<Prompt>>();
    var mapperMock = new Mock<IMapper>();
    var eventBusMock = new Mock<IEventBus>();
    // ... 10 more mocks
}

// ✅ Good: Use facade or simplify dependencies
[Fact]
public void CreatePrompt_WithRequiredDependencies_ShouldWork()
{
    var repoMock = new Mock<IPromptRepository>();
    var service = new PromptService(repoMock.Object);
}
```

### 3. The Brittle Test

```csharp
// ❌ Bad: Tied to implementation details
[Fact]
public void CreatePrompt_ShouldCallRepositoryWithSpecificParameters()
{
    repository.Verify(r => r.Create(
        It.Is<Prompt>(p => 
            p.Title == "Test" && 
            p.Content == "Content" && 
            p.CreatedAt == new DateTime(2024, 1, 1, 12, 0, 0) // Too specific
        )
    ), Times.Once);
}

// ✅ Good: Test behavior, not implementation
[Fact]
public void CreatePrompt_ShouldPersistPromptData()
{
    repository.Verify(r => r.Create(
        It.Is<Prompt>(p => 
            p.Title == "Test" && 
            p.Content == "Content"
        )
    ), Times.Once);
}
```

### 4. The Slow Test

```csharp
// ❌ Bad: Using real database
[Fact]
public void CreatePrompt_WithRealDatabase_ShouldWork()
{
    using var context = new PromptDbContext("Server=localhost;Database=TestDB");
    var service = new PromptService(context);
    // Test takes 5+ seconds
}

// ✅ Good: Use in-memory or mock
[Fact]
public void CreatePrompt_WithInMemoryDatabase_ShouldWork()
{
    using var database = new LiteDatabase(":memory:");
    var service = new PromptService(database);
    // Test runs in milliseconds
}
```

### 5. The Mystery Test

```csharp
// ❌ Bad: No clear intent
[Fact]
public void Test1()
{
    var result = service.DoSomething();
    Assert.NotNull(result);
}

// ✅ Good: Clear intent and documentation
[Fact]
public void CreatePrompt_WithValidData_ShouldReturnPromptWithGeneratedId()
{
    // This test verifies that the CreatePrompt method generates
    // a unique ID for new prompts and returns the created prompt
    var result = service.CreatePrompt(validPromptData);
    
    result.Should().NotBeNull();
    result.Id.Should().BeGreaterThan(0);
}
```

---

## Conclusion

Following these TDD guidelines ensures that the PromptOrganizer project maintains high code quality, reliability, and maintainability. Regular review and updates of these guidelines help adapt to changing requirements and improve development practices.

For questions or suggestions regarding these guidelines, please create an issue in the project repository or contact the development team.