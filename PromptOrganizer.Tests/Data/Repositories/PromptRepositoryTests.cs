
using FluentAssertions;
using PromptOrganizer.Domain.Entities;
using PromptOrganizer.Domain.Repositories;
using PromptOrganizer.Tests.TestHelpers;
using Xunit;

namespace PromptOrganizer.Tests.Data.Repositories;

[Collection("DatabaseCollection")]
public class PromptRepositoryTests : RepositoryTestBase
{
  [Fact]
  public async Task AddAsync_WithValidPrompt_ShouldAddSuccessfully()
  {
    // Arrange
    var category = await CreateTestCategoryAsync();
    var prompt = new Prompt(0, "Test Prompt", "Test content", "Test description", category.Id, "test,tag");

    // Act
    var result = await PromptRepository.AddAsync(prompt);

    // Assert
    result.Should().NotBeNull();
    result.Id.Should().BeGreaterThan(0);
    result.Title.Should().Be("Test Prompt");
    result.Content.Should().Be("Test content");
    result.CategoryId.Should().Be(category.Id);
  }

  [Fact]
  public async Task GetByIdAsync_WithExistingPrompt_ShouldReturnPrompt()
  {
    // Arrange
    var prompt = await CreateTestPromptAsync();

    // Act
    var result = await PromptRepository.GetByIdAsync(prompt.Id);

    // Assert
    result.Should().NotBeNull();
    result.Id.Should().Be(prompt.Id);
    result.Title.Should().Be(prompt.Title);
  }

  [Fact]
  public async Task GetByCategoryIdAsync_WithExistingCategory_ShouldReturnPrompts()
  {
    // Arrange
    var category = await CreateTestCategoryAsync();
    await CreateTestPromptAsync("Prompt 1", "Content 1", category.Id);
    await CreateTestPromptAsync("Prompt 2", "Content 2", category.Id);

    // Act
    var result = await PromptRepository.GetByCategoryIdAsync(category.Id);

    // Assert
    result.Should().HaveCount(2);
    result.Should().OnlyContain(p => p.CategoryId == category.Id);
  }

  [Fact]
  public async Task GetActivePromptsAsync_WithMixedPrompts_ShouldReturnOnlyActive()
  {
    // Arrange
    var activePrompt = await CreateTestPromptAsync("Active Prompt");
    var inactivePrompt = await CreateTestPromptAsync("Inactive Prompt");
    inactivePrompt.Deactivate();
    await PromptRepository.UpdateAsync(inactivePrompt);

    // Act
    var result = await PromptRepository.GetActivePromptsAsync();

    // Assert
    result.Should().HaveCount(1);
    result.Should().ContainSingle(p => p.Id == activePrompt.Id);
  }

  [Fact]
  public async Task GetByTagsAsync_WithMatchingTags_ShouldReturnPrompts()
  {
    // Arrange
    await CreateTestPromptAsync("Prompt 1", "Content 1", null, "test,unit");
    await CreateTestPromptAsync("Prompt 2", "Content 2", null, "integration,test");
    await CreateTestPromptAsync("Prompt 3", "Content 3", null, "different,tags");

    // Act
    var result = await PromptRepository.GetByTagsAsync(new[] { "test" });

    // Assert
    result.Should().HaveCount(2);
    result.Should().Contain(p => p.Tags.Contains("test"));
  }

  [Fact]
  public async Task SearchAsync_WithMatchingTitle_ShouldReturnPrompts()
  {
    // Arrange
    await CreateTestPromptAsync("Test Prompt", "Content 1");
    await CreateTestPromptAsync("Another Prompt", "Content 2");
    await CreateTestPromptAsync("Different Title", "Content 3");

    // Act
    var result = await PromptRepository.SearchAsync("Test");

    // Assert
    result.Should().HaveCount(1);
    result.Should().ContainSingle(p => p.Title.Contains("Test"));
  }

  [Fact]
  public async Task SearchAsync_WithMatchingContent_ShouldReturnPrompts()
  {
    // Arrange
    await CreateTestPromptAsync("Prompt 1", "This is a test content");
    await CreateTestPromptAsync("Prompt 2", "Different content");

    // Act
    var result = await PromptRepository.SearchAsync("test");

    // Assert
    result.Should().HaveCount(1);
    result.Should().ContainSingle(p => p.Content.Contains("test"));
  }

  [Fact]
  public async Task CountByCategoryAsync_WithPrompts_ShouldReturnCorrectCount()
  {
    // Arrange
    var category = await CreateTestCategoryAsync();
