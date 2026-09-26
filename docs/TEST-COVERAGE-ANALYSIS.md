# Test Coverage Analysis Guide for PromptOrganizer

## Overview

This document provides comprehensive guidance on analyzing and interpreting test coverage reports for the PromptOrganizer project. It covers coverage metrics, analysis techniques, and improvement strategies.

## Table of Contents

1. [Coverage Metrics](#coverage-metrics)
2. [Coverage Report Structure](#coverage-report-structure)
3. [Analysis Techniques](#analysis-techniques)
4. [Coverage Thresholds](#coverage-thresholds)
5. [Identifying Coverage Gaps](#identifying-coverage-gaps)
6. [Improving Coverage](#improving-coverage)
7. [Coverage Anti-Patterns](#coverage-anti-patterns)
8. [Tools and Scripts](#tools-and-scripts)
9. [CI/CD Integration](#cicd-integration)
10. [Best Practices](#best-practices)

## Coverage Metrics

### Line Coverage
- **Definition**: Percentage of executable lines of code that have been executed
- **Target**: 80% minimum
- **Formula**: (Lines Executed / Total Executable Lines) × 100

### Branch Coverage
- **Definition**: Percentage of decision branches that have been executed
- **Target**: 75% minimum
- **Formula**: (Branches Executed / Total Branches) × 100

### Method Coverage
- **Definition**: Percentage of methods that have been executed
- **Target**: 80% minimum
- **Formula**: (Methods Executed / Total Methods) × 100

### Class Coverage
- **Definition**: Percentage of classes that have been executed
- **Target**: 95% minimum
- **Formula**: (Classes Executed / Total Classes) × 100

### Cyclomatic Complexity
- **Definition**: Measure of code complexity based on decision points
- **Target**: Keep below 10 for individual methods
- **Formula**: Number of decision points + 1

## Coverage Report Structure

### ReportGenerator Output Structure

```
coverage/
├── report/
│   ├── index.html                    # Main dashboard
│   ├── PromptOrganizer.Services.html # Service coverage
│   ├── PromptOrganizer.Data.html     # Data layer coverage
│   ├── PromptOrganizer.Domain.html   # Domain coverage
│   └── PromptOrganizer.App.html      # App layer coverage
├── summary/
│   └── Summary.txt                   # Text summary
├── history/                          # Historical data
└── coverage.cobertura.xml           # Raw coverage data
```

### Key Report Sections

1. **Dashboard Overview**
   - Overall coverage percentages
   - Trend analysis (if history enabled)
   - Risk hotspots identification

2. **Assembly Coverage**
   - Coverage by project/assembly
   - Detailed class and method coverage
   - Uncovered code highlighting

3. **File Coverage**
   - Line-by-line coverage visualization
   - Branch coverage details
   - Complexity metrics

4. **Risk Hotspots**
   - Methods with low coverage and high complexity
   - Classes with many uncovered lines
   - Critical paths without tests

## Analysis Techniques

### 1. Coverage Trend Analysis

```powershell
# Generate coverage with history
.\scripts\coverage-report.ps1 -IncludeHistory -Configuration Release

# Compare coverage between builds
# Review historical trends in coverage/report/history.html
```

### 2. Risk Hotspot Analysis

```csharp
// High-risk methods (low coverage + high complexity)
public class RiskAnalysis
{
    public void ComplexMethodWithLowCoverage() // Risk Hotspot
    {
        if (condition1) // Branch not covered
        {
            if (condition2) // Nested branch not covered
            {
                // Complex logic not tested
            }
        }
        
        switch (value) // Switch statement not covered
        {
            case 1: // Case not covered
                break;
            case 2: // Case not covered
                break;
            default: // Default not covered
                break;
        }
    }
}
```

### 3. Branch Coverage Analysis

```csharp
// Analyze branch coverage gaps
public void ProcessPrompt(Prompt prompt)
{
    if (prompt == null) // Branch 1: Covered
        throw new ArgumentNullException();
    
    if (string.IsNullOrEmpty(prompt.Title)) // Branch 2: Not covered
        throw new ValidationException();
    
    if (prompt.Title.Length > 100) // Branch 3: Not covered
        prompt.Title = prompt.Title.Substring(0, 100);
    
    // Need tests for:
    // - prompt with null title
    // - prompt with empty title
    // - prompt with title > 100 characters
}
```

### 4. Path Coverage Analysis

```csharp
// Multiple execution paths
public async Task<PromptResult> SearchPromptsAsync(SearchCriteria criteria)
{
    var query = _repository.GetAll();
    
    if (!string.IsNullOrEmpty(criteria.Query)) // Path 1
    {
        query = query.Where(p => p.Title.Contains(criteria.Query));
    }
    
    if (criteria.CategoryId.HasValue) // Path 2
    {
        query = query.Where(p => p.CategoryId == criteria.CategoryId);
    }
    
    if (criteria.DateFrom.HasValue) // Path 3
    {
        query = query.Where(p => p.CreatedAt >= criteria.DateFrom);
    }
    
    // Need tests for all path combinations:
    // - Only query
    // - Only category
    // - Only date
    // - Query + category
    // - Query + date
    // - Category + date
    // - All three
    // - None
}
```

## Coverage Thresholds

### Project-Wide Thresholds

```xml
<!-- Directory.Build.props -->
<PropertyGroup>
  <LineCoverageThreshold>80</LineCoverageThreshold>
  <BranchCoverageThreshold>75</BranchCoverageThreshold>
  <MethodCoverageThreshold>80</MethodCoverageThreshold>
  <ClassCoverageThreshold>95</ClassCoverageThreshold>
</PropertyGroup>
```

### Assembly-Specific Thresholds

```xml
<!-- Different thresholds for different assemblies -->
<PropertyGroup Condition="'$(MSBuildProjectName)' == 'PromptOrganizer.Services'">
  <LineCoverageThreshold>85</LineCoverageThreshold>
  <BranchCoverageThreshold>80</BranchCoverageThreshold>
</PropertyGroup>

<PropertyGroup Condition="'$(MSBuildProjectName)' == 'PromptOrganizer.App'">
  <LineCoverageThreshold>70</LineCoverageThreshold>
  <BranchCoverageThreshold>60</LineCoverageThreshold>
</PropertyGroup>
```

### Quality Gates

```yaml
# CI/CD Quality Gates
- task: DotNetCoreCLI@2
  displayName: 'Test with coverage'
  inputs:
    command: 'test'
    projects: '**/*Tests.csproj'
    arguments: '--configuration Release /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:Threshold=80 /p:ThresholdType=line /p:FailBuildOnCoverageBelowThreshold=true'
```

## Identifying Coverage Gaps

### 1. Uncovered Code Analysis

```powershell
# Find uncovered files
Get-ChildItem -Path "coverage" -Recurse -Filter "*.html" | 
    Select-String -Pattern "0%" |
    Select-Object Filename, Line |
    Sort-Object Filename
```

### 2. Critical Path Analysis

```csharp
// Identify critical business logic without coverage
public class CriticalBusinessLogic
{
    public decimal CalculatePromptPriority(Prompt prompt)
    {
        // Business-critical calculation
        // Should have comprehensive coverage
        var age = DateTime.UtcNow - prompt.CreatedAt;
        var complexity = CalculateComplexity(prompt.Content);
        var usage = GetUsageStatistics(prompt.Id);
        
        return (age.TotalDays * 0.3) + (complexity * 0.5) + (usage * 0.2);
    }
}
```

### 3. Edge Case Analysis

```csharp
// Look for edge cases not covered
public void ValidatePrompt(Prompt prompt)
{
    if (prompt == null) throw new ArgumentNullException();
    if (string.IsNullOrWhiteSpace(prompt.Title)) throw new ValidationException();
    if (prompt.Title.Length > 200) throw new ValidationException();
    if (prompt.Content?.Length > 10000) throw new ValidationException();
    if (prompt.CategoryId <= 0) throw new ValidationException();
    
    // Need tests for:
    // - Null prompt
    // - Empty title
    // - Whitespace title
    // - Title exactly 200 characters
    // - Title 201 characters
    // - Content exactly 10000 characters
    // - Content 10001 characters
    // - CategoryId 0
    // - CategoryId negative
}
```

## Improving Coverage

### 1. Systematic Approach

```csharp
// Before: Low coverage
public class PromptService
{
    public async Task<Prompt> CreatePromptAsync(PromptData data)
    {
        Validate(data);
        var prompt = MapToPrompt(data);
        await _repository.CreateAsync(prompt);
        await _eventBus.PublishAsync(new PromptCreatedEvent(prompt.Id));
        return prompt;
    }
}

// After: Comprehensive tests
public class PromptServiceTests
{
    [Theory]
    [InlineData(null, "content")] // Null title
    [InlineData("", "content")]   // Empty title
    [InlineData("title", null)]   // Null content
    public async Task CreatePrompt_WithInvalidData_ShouldThrow(string title, string content)
    {
        var data = new PromptData { Title = title, Content = content };
        
        await Assert.ThrowsAsync<ValidationException>(
            () => _service.CreatePromptAsync(data)
        );
    }
    
    [Fact]
    public async Task CreatePrompt_WithValidData_ShouldCreateAndPublishEvent()
    {
        var data = new PromptData { Title = "Test", Content = "Content" };
        
        var result = await _service.CreatePromptAsync(data);
        
        result.Should().NotBeNull();
        _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<Prompt>()), Times.Once);
        _eventBusMock.Verify(e => e.PublishAsync(It.IsAny<PromptCreatedEvent>()), Times.Once);
    }
}
```

### 2. Parameterized Tests for Coverage

```csharp
[Theory]
[MemberData(nameof(GetPromptValidationTestData))]
public void ValidatePrompt_WithVariousInputs_ShouldValidateCorrectly(
    Prompt prompt, bool expectedValid, string expectedError)
{
    if (!expectedValid)
    {
        var exception = Assert.Throws<ValidationException>(
            () => _validator.Validate(prompt)
        );
        exception.Message.Should().Contain(expectedError);
    }
    else
    {
        _validator.Validate(prompt); // Should not throw
    }
}

public static IEnumerable<object[]> GetPromptValidationTestData()
{
    yield return new object[] { null, false, "Prompt cannot be null" };
    yield return new object[] { new Prompt { Title = null }, false, "Title is required" };
    yield return new object[] { new Prompt { Title = "" }, false, "Title is required" };
    yield return new object[] { new Prompt { Title = "Valid" }, true, "" };
    yield return new object[] { new Prompt { Title = "A" }, false, "minimum length" };
    yield return new object[] { new Prompt { Title = new string('A', 201) }, false, "maximum length" };
}
```

### 3. Boundary Value Testing

```csharp
[Theory]
[InlineData(0)]        // Boundary: minimum
[InlineData(1)]        // Boundary: just above minimum
[InlineData(99)]       // Boundary: just below maximum
[InlineData(100)]      // Boundary: maximum
[InlineData(101)]      // Boundary: just above maximum
public void GetPrompts_WithPageSize_ShouldReturnCorrectNumber(int pageSize)
{
    var prompts = TestDataFactory.CreatePrompts(200);
    _repository.Setup(r => r.GetAll()).Returns(prompts.AsQueryable());
    
    var result = _service.GetPrompts(pageSize: pageSize);
    
    result.Items.Should().HaveCount(Math.Min(pageSize, 200));
}
```

## Coverage Anti-Patterns

### 1. Testing Trivial Code

```csharp
// ❌ Don't test simple property getters/setters
[Fact]
public void Prompt_TitleGetter_ShouldReturnTitle()
{
    var prompt = new Prompt { Title = "Test" };
    prompt.Title.Should().Be("Test"); // Trivial test
}

// ✅ Focus on business logic
[Fact]
public void Prompt_Validate_ShouldEnforceBusinessRules()
{
    var prompt = new Prompt { Title = "A" };
    
    var exception = Assert.Throws<ValidationException>(
        () => prompt.Validate()
    );
    
    exception.Message.Should().Contain("minimum length");
}
```

### 2. Testing Implementation Details

```csharp
// ❌ Don't test private methods or internal state
[Fact]
public void PromptService_GenerateId_ShouldReturnUniqueId()
{
    // Testing private implementation detail
    var id = CallPrivateMethod(_service, "GenerateId");
    id.Should().NotBe(0);
}

// ✅ Test public behavior
[Fact]
public void CreatePrompt_ShouldReturnPromptWithUniqueId()
{
    var result1 = _service.CreatePrompt(data);
    var result2 = _service.CreatePrompt(data);
    
    result1.Id.Should().NotBe(result2.Id);
}
```

### 3. False Coverage

```csharp
// ❌ Tests that don't actually verify behavior
[Fact]
public void ProcessPrompt_ShouldNotThrow()
{
    // This test passes even if the method does nothing
    _service.ProcessPrompt(prompt); // No assertions
}

// ✅ Verify actual behavior
[Fact]
public void ProcessPrompt_ShouldUpdatePromptStatus()
{
    var result = _service.ProcessPrompt(prompt);
    
    result.Status.Should().Be(PromptStatus.Processed);
    result.ProcessedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
}
```

## Tools and Scripts

### Coverage Analysis Script

```powershell
# scripts/analyze-coverage.ps1
param(
    [string]$CoverageFile = "./coverage/coverage.cobertura.xml",
    [int]$MinLineCoverage = 80,
    [int]$MinBranchCoverage = 75
)

[xml]$coverage = Get-Content $CoverageFile

# Analyze coverage by assembly
$assemblies = $coverage.coverage.packages.package
foreach ($assembly in $assemblies)
{
    $name = $assembly.name
    $lineRate = [math]::Round([double]$assembly.'line-rate' * 100, 2)
    $branchRate = [math]::Round([double]$assembly.'branch-rate' * 100, 2)
    
    Write-Host "Assembly: $name"
    Write-Host "  Line Coverage: $lineRate%"
    Write-Host "  Branch Coverage: $branchRate%"
    
    if ($lineRate -lt $MinLineCoverage)
    {
        Write-Host "  ⚠️  Line coverage below threshold!" -ForegroundColor Yellow
    }
    
    if ($branchRate -lt $MinBranchCoverage)
    {
        Write-Host "  ⚠️  Branch coverage below threshold!" -ForegroundColor Yellow
    }
}

# Find methods with low coverage
$lowCoverageMethods = $coverage.coverage.packages.package.classes.class.methods.method |
    Where-Object { [double]$_.'line-rate' -lt 0.5 } |
    Select-Object -Property name, @{Name='Coverage';Expression={[math]::Round([double]$_.'line-rate' * 100, 2)}}

Write-Host "`nMethods with < 50% coverage:"
$lowCoverageMethods | Format-Table -AutoSize
```

### Coverage Comparison Script

```powershell
# scripts/compare-coverage.ps1
param(
    [string]$BaselineFile,
    [string]$CurrentFile
)

[xml]$baseline = Get-Content $BaselineFile
[xml]$current = Get-Content $CurrentFile

$baselineLineRate = [math]::Round([double]$baseline.coverage.'line-rate' * 100, 2)
$currentLineRate = [math]::Round([double]$current.coverage.'line-rate' * 100, 2)

$baselineBranchRate = [math]::Round([double]$baseline.coverage.'branch-rate' * 100, 2)
$currentBranchRate = [math]::Round([double]$current.coverage.'branch-rate' * 100, 2)

Write-Host "Coverage Comparison:"
Write-Host "Line Coverage: $baselineLineRate% → $currentLineRate% ($([math]::Round($currentLineRate - $baselineLineRate, 2))%)"
Write-Host "Branch Coverage: $baselineBranchRate% → $currentBranchRate% ($([math]::Round($currentBranchRate - $baselineBranchRate, 2))%)"

if ($currentLineRate -lt $baselineLineRate)
{
    Write-Host "⚠️  Line coverage decreased!" -ForegroundColor Red
}
else
{
    Write-Host "✅ Line coverage maintained or improved!" -ForegroundColor Green
}
```

## CI/CD Integration

### GitHub Actions Coverage Check

```yaml
- name: Check Coverage Thresholds
  run: |
    $coverage = [xml](Get-Content ./coverage/coverage.cobertura.xml)
    $lineRate = [math]::Round([double]$coverage.coverage.'line-rate' * 100, 2)
    $branchRate = [math]::Round([double]$coverage.coverage.'branch-rate' * 100, 2)
    
    Write-Host "Line Coverage: $lineRate%"
    Write-Host "Branch Coverage: $branchRate%"
    
    if ($lineRate -lt 80) { exit 1 }
    if ($branchRate -lt 75) { exit 1 }
```

### Azure DevOps Coverage Policy

```yaml
- task: BuildQualityChecks@8
  displayName: 'Check Coverage'
  inputs:
    checkCoverage: true
    coverageFailOption: 'fixed'
    coverageType: 'line'
    coverageThreshold: '80'
```

## Best Practices

### 1. Regular Coverage Reviews
- Review coverage reports weekly
- Identify trends and patterns
- Address coverage gaps promptly

### 2. Focus on Business Logic
- Prioritize core business logic coverage
- Don't obsess over trivial code coverage
- Focus on edge cases and error conditions

### 3. Maintain Coverage Quality
- Write meaningful tests, not just for coverage numbers
- Ensure tests actually verify behavior
- Avoid false coverage

### 4. Use Coverage as a Guide
- Coverage is a tool, not a goal
- 100% coverage doesn't guarantee bug-free code
- Focus on test quality over quantity

### 5. Continuous Improvement
- Set realistic coverage goals
- Gradually improve coverage over time
- Balance coverage with development velocity

---

## Conclusion

Effective coverage analysis requires understanding what the numbers mean and using them as a guide for improving test quality. Focus on testing critical business logic, edge cases, and error conditions rather than achieving arbitrary coverage percentages.

Regular analysis and improvement of test coverage ensures the PromptOrganizer project maintains high quality and reliability.