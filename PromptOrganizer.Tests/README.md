# PromptOrganizer TDD Architecture

This document describes the comprehensive Test-Driven Development (TDD) architecture implemented for the PromptOrganizer domain layer.

## Overview

The TDD architecture provides a robust foundation for testing domain entities with:
- **Base test classes** with common assertion helpers
- **Test data builders** for creating test entities
- **Test fixtures** for providing consistent test data
- **Mock repositories** for testing with dependencies
- **FluentAssertions** for readable test assertions
- **Proper test naming conventions** and organization

## Project Structure

```
PromptOrganizer.Tests/
├── Domain/
│   ├── Entities/           # Domain entity tests
│   └── Repositories/       # Repository tests (future)
├── TestHelpers/
│   ├── Builders/          # Test data builders
│   ├── Fixtures/          # Test data fixtures
│   ├── Mocks/             # Mock repositories and test doubles
│   └── BaseTest.cs        # Base test class
└── README.md              # This file
```

## Core Components

### 1. BaseTest Class (`TestHelpers/BaseTest.cs`)

The foundation for all test classes, providing:
- **Common assertion helpers** (AssertThrows, AssertEqual, AssertNotNull, etc.)
- **IDisposable pattern** for proper cleanup
- **Setup and Cleanup methods** for test lifecycle management
- **FluentAssertions integration** for readable assertions

**Usage:**
```csharp
public class MyEntityTests : BaseTest
{
    [Fact]
    public void MyTest_ShouldBehaveAsExpected()
    {
        // Arrange
        var entity = new MyEntity("test");
        
        // Act
        var result = entity.DoSomething();
        
        // Assert
        AssertEqual("expected", result);
        AssertNotNull(entity);
    }
}
```

### 2. Builder Pattern (`TestHelpers/Builders/`)

Provides fluent interfaces for creating test entities with specific configurations.

**Base Builder (`BuilderBase.cs`):**
- Generic base class for all builders
- Provides `Build()`, `BuildList()`, and `Reset()` methods
- Supports fluent method chaining

**Example Usage:**
```csharp
// Create a prompt with specific properties
var prompt = PromptBuilder.New()
    .WithTitle("Test Prompt")
    .WithContent("Test content")
    .WithCategory("Testing")
    .WithTags("test,unit")
    .Build();

// Create multiple prompts
var prompts = PromptBuilder.New()
    .WithCategory("Testing")
    .BuildList(5);
```

### 3. Test Fixtures (`TestHelpers/Fixtures/`)

Provide pre-configured test data for common scenarios.

**Base Fixture (`FixtureBase.cs`):**
- Ensures consistent initialization
- Provides `Reset()` method for test isolation

**Example Usage:**
```csharp
public class MyTests : BaseTest
{
    private readonly PromptFixture _fixture;
    
    public MyTests()
    {
        _fixture = new PromptFixture();
    }
    
    [Fact]
    public void Test_WithFixtureData()
    {
        var defaultPrompt = _fixture.DefaultPrompt;
        var minimalPrompt = _fixture.MinimalPrompt;
        var promptList = _fixture.PromptList;
        
        // Use the fixture data in tests
    }
}
```

### 4. Mock Repositories (`TestHelpers/Mocks/`)

Base classes for creating mock repositories using Moq.

**MockRepositoryBase (`MockRepositoryBase.cs`):**
- Generic base for repository mocks
- Provides common setup methods (SetupGetById, SetupGetAll, etc.)
- Includes verification methods for testing interactions

**Example Usage:**
```csharp
var mockRepo = new MockPromptRepository()
    .SetupGetById(1, testPrompt)
    .SetupGetAll(promptList)
    .Build();

// Use in tests
var service = new PromptService(mockRepo.Object);
```

## Test Naming Conventions

### Test Method Naming
Follow the pattern: `[MethodName]_[Scenario]_[ExpectedResult]`

**Examples:**
- `Constructor_WithValidParameters_ShouldCreatePromptSuccessfully`
- `UpdateContent_WithNullContent_ShouldThrowArgumentException`
- `Deactivate_ShouldSetIsActiveToFalseAndUpdateTimestamp`

### Test Class Organization
- **Unit Tests**: `[EntityName]Tests` (e.g., `PromptTests`)
- **Integration Tests**: `[EntityName]IntegrationTests` (e.g., `PromptIntegrationTests`)
- **Repository Tests**: `[EntityName]RepositoryTests`

## Test Categories

### 1. Constructor Tests
- Valid parameter combinations
- Null/empty parameter validation
- Maximum length validation
- Business rule validation

### 2. Property Tests
- Getter functionality
- Immutable properties
- Calculated properties

### 3. Method Tests
- Command methods (changing state)
- Query methods (returning data)
- Validation methods
- Business logic methods

### 4. Lifecycle Tests
- Activation/deactivation
- State transitions
- Timestamp updates

## Best Practices

### 1. Arrange-Act-Assert (AAA) Pattern
```csharp
[Fact]
public void Method_WithScenario_ShouldResult()
{
    // Arrange
    var entity = EntityBuilder.New().WithProperty("value").Build();
    
    // Act
    var result = entity.Method();
    
    // Assert
    result.Should().Be("expected");
}
```

### 2. Use Builders for Complex Setup
```csharp
[Fact]
public void ComplexScenario_ShouldWork()
{
    // Arrange
    var entity = EntityBuilder.New()
        .WithProperty1("value1")
        .WithProperty2("value2")
        .WithRelatedEntity(relatedEntity)
        .Build();
    
    // Act & Assert
    // ...
}
```

### 3. Use Fixtures for Common Data
```csharp
[Fact]
public void Test_WithCommonData_ShouldWork()
{
    // Arrange
    var commonEntity = _fixture.CommonEntity;
    var entityList = _fixture.EntityList;
    
    // Act & Assert
    // ...
}
```

### 4. Test Edge Cases
- Null values
- Empty strings/collections
- Maximum lengths
- Boundary conditions
- Invalid states

### 5. Test Naming
- Be descriptive about the scenario
- Include the expected result
- Use consistent formatting

## Running Tests

### Run All Tests
```bash
dotnet test PromptOrganizer.Tests/PromptOrganizer.Tests.csproj
```

### Run Specific Test Category
```bash
dotnet test --filter "FullyQualifiedName~PromptTests"
dotnet test --filter "FullyQualifiedName~IntegrationTests"
```

### Run with Coverage
```bash
dotnet test --collect:"XPlat Code Coverage"
```

## Adding New Tests

### 1. Create Test Class
```csharp
public class NewEntityTests : BaseTest
{
    // Tests go here
}
```

### 2. Create Builder (if needed)
```csharp
public class NewEntityBuilder : BuilderBase<NewEntity, NewEntityBuilder>
{
    protected override NewEntity CreateDefault()
    {
        return new NewEntity("default");
    }
    
    public NewEntityBuilder WithProperty(string value)
    {
        // Set property logic
        return this;
    }
}
```

### 3. Create Fixture (if needed)
```csharp
public class NewEntityFixture : FixtureBase
{
    public NewEntity DefaultEntity { get; private set; }
    
    protected override void Initialize()
    {
        DefaultEntity = NewEntityBuilder.New().Build();
    }
}
```

## Example: Complete Test Implementation

```csharp
using FluentAssertions;
using PromptOrganizer.Domain.Entities;
using PromptOrganizer.Tests.TestHelpers;
using PromptOrganizer.Tests.TestHelpers.Builders;
using PromptOrganizer.Tests.TestHelpers.Fixtures;
using Xunit;

namespace PromptOrganizer.Tests.Domain.Entities;

public class PromptAdvancedTests : BaseTest
{
    private readonly PromptFixture _fixture;

    public PromptAdvancedTests()
    {
        _fixture = new PromptFixture();
    }

    [Fact]
    public void PromptBuilder_ShouldCreateValidPrompts_WithDifferentConfigurations()
    {
        // Arrange & Act
        var minimalPrompt = PromptBuilder.New()
            .WithMinimalData()
            .Build();

        var fullPrompt = PromptBuilder.New()
            .WithTitle("Full Featured Prompt")
            .WithContent("Comprehensive content")
            .WithDescription("Full description")
            .WithCategory("Testing")
            .WithTags("test,full,featured")
            .Build();

        // Assert
        minimalPrompt.Should().NotBeNull();
        minimalPrompt.Title.Should().Be("Minimal Prompt");
        minimalPrompt.Description.Should().BeEmpty();

        fullPrompt.Should().NotBeNull();
        fullPrompt.Title.Should().Be("Full Featured Prompt");
        fullPrompt.Category.Should().Be("Testing");
        fullPrompt.Tags.Should().Be("test,full,featured");
    }

    [Fact]
    public void PromptFixture_ShouldProvideConsistentTestData()
    {
        // Arrange
        var defaultPrompt = _fixture.DefaultPrompt;
        var minimalPrompt = _fixture.MinimalPrompt;
        var promptList = _fixture.PromptList;

        // Act & Assert
        defaultPrompt.Should().NotBeNull();
        defaultPrompt.Title.Should().Be("Default Test Prompt");
        defaultPrompt.IsActive.Should().BeTrue();

        minimalPrompt.Should().NotBeNull();
        minimalPrompt.Title.Should().Be("Minimal Prompt");
        minimalPrompt.Description.Should().BeEmpty();

        promptList.Should().HaveCount(7);
        promptList.Should().Contain(p => p.Title == "Default Test Prompt");
    }
}
```

## Conclusion

This TDD architecture provides a solid foundation for testing the PromptOrganizer domain layer. It promotes:
- **Consistency** across all tests
- **Maintainability** through reusable components
- **Readability** with clear naming and organization
- **Flexibility** to handle various testing scenarios
- **Best practices** in test design and implementation

The architecture is extensible and can be easily adapted for new domain entities and testing requirements.