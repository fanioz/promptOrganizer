using FluentAssertions;
using PromptOrganizer.Domain.Entities;
using PromptOrganizer.Tests.TestHelpers;
using PromptOrganizer.Tests.TestHelpers.Builders;
using PromptOrganizer.Tests.TestHelpers.Fixtures;
using Xunit;

namespace PromptOrganizer.Tests.Domain.Entities;

public class PromptIntegrationTests : BaseTest
{
  private readonly PromptFixture _fixture;

  public PromptIntegrationTests()
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
        .WithContent("This is a comprehensive prompt with detailed content")
        .WithDescription("A fully featured prompt")
        .WithCategory("Testing")
        .WithTags("test,full,featured")
        .Build();

    var maxLengthPrompt = PromptBuilder.New()
        .WithMaximumLengthData()
        .Build();

    // Assert
    minimalPrompt.Should().NotBeNull();
    minimalPrompt.Title.Should().Be("Minimal Prompt");
    minimalPrompt.Description.Should().BeEmpty();

    fullPrompt.Should().NotBeNull();
    fullPrompt.Title.Should().Be("Full Featured Prompt");
    fullPrompt.CategoryName.Should().Be("Testing");
    fullPrompt.Tags.Should().Be("test,full,featured");

    maxLengthPrompt.Should().NotBeNull();
    maxLengthPrompt.Title.Should().HaveLength(200);
    maxLengthPrompt.Content.Should().HaveLength(5000);
  }

  [Fact]
  public void PromptFixture_ShouldProvideConsistentTestData()
  {
    // Arrange
    var defaultPrompt = _fixture.DefaultPrompt;
    var minimalPrompt = _fixture.MinimalPrompt;
    var fullPrompt = _fixture.FullPrompt;
    var inactivePrompt = _fixture.InactivePrompt;

    // Act & Assert
    defaultPrompt.Should().NotBeNull();
    defaultPrompt.Title.Should().Be("Default Test Prompt");
    defaultPrompt.IsActive.Should().BeTrue();

    minimalPrompt.Should().NotBeNull();
    minimalPrompt.Title.Should().Be("Minimal Prompt");
    minimalPrompt.Description.Should().BeEmpty();

    fullPrompt.Should().NotBeNull();
    fullPrompt.Title.Should().Be("Full Featured Prompt");
    fullPrompt.IsActive.Should().BeTrue();

    inactivePrompt.Should().NotBeNull();
    inactivePrompt.Title.Should().Be("Inactive Test Prompt");
    inactivePrompt.IsActive.Should().BeFalse();
  }

  [Fact]
  public void PromptList_ShouldContainExpectedNumberOfPrompts()
  {
    // Arrange
    var promptList = _fixture.PromptList;

    // Act & Assert
    promptList.Should().NotBeNull();
    promptList.Should().HaveCount(7);
    promptList.Should().Contain(p => p.Title == "Default Test Prompt");
    promptList.Should().Contain(p => p.Title == "Minimal Prompt");
    promptList.Should().Contain(p => p.Title == "Full Featured Prompt");
    promptList.Should().Contain(p => p.Title == "Inactive Test Prompt");
  }

  [Fact]
  public void PromptsWithDifferentCategories_ShouldBeProperlyCategorized()
  {
    // Arrange
    var categorizedPrompts = _fixture.GetPromptsWithDifferentCategories();

    // Act & Assert
    categorizedPrompts.Should().HaveCount(4);
    categorizedPrompts.Should().Contain(p => p.CategoryName == "Development");
    categorizedPrompts.Should().Contain(p => p.CategoryName == "Testing");
    categorizedPrompts.Should().Contain(p => p.CategoryName == "Documentation");
    categorizedPrompts.Should().Contain(p => p.CategoryName == "Design");
  }

  [Fact]
  public void PromptsWithMixedActivityStates_ShouldHaveCorrectActivityStatus()
  {
    // Arrange
    var mixedActivityPrompts = _fixture.GetPromptsWithMixedActivityStates();

    // Act & Assert
    mixedActivityPrompts.Should().HaveCount(2);
    mixedActivityPrompts.Should().Contain(p => p.IsActive == true);
    mixedActivityPrompts.Should().Contain(p => p.IsActive == false);
  }

  [Fact]
  public void PromptBuilder_ShouldCreateMultiplePrompts_Efficiently()
  {
    // Arrange
    var builder = PromptBuilder.New();
    var prompts = new List<Prompt>();

    // Act
    for (int i = 1; i <= 5; i++)
    {
      var prompt = builder
          .WithTitle($"Test Prompt {i}")
          .WithContent($"Content for test prompt {i}")
          .WithCategory("Testing")
          .Build();

      prompts.Add(prompt);
    }

    // Assert
    prompts.Should().HaveCount(5);
    prompts.Should().OnlyContain(p => p.CategoryName == "Testing");
    prompts.Should().Contain(p => p.Title == "Test Prompt 1");
    prompts.Should().Contain(p => p.Title == "Test Prompt 5");
  }

  [Fact]
  public void PromptFixture_Reset_ShouldRecreateAllFixtures()
  {
    // Arrange
    var originalDefaultPrompt = _fixture.DefaultPrompt;
    var originalMinimalPrompt = _fixture.MinimalPrompt;

    // Act
    _fixture.Reset();

    // Assert
    _fixture.DefaultPrompt.Should().NotBeSameAs(originalDefaultPrompt);
    _fixture.MinimalPrompt.Should().NotBeSameAs(originalMinimalPrompt);
    _fixture.DefaultPrompt.Title.Should().Be("Default Test Prompt");
    _fixture.MinimalPrompt.Title.Should().Be("Minimal Prompt");
  }

  [Fact]
  public void PromptBuilder_WithInvalidData_ShouldCreateInvalidPrompt()
  {
    // Arrange & Act
    var invalidPromptBuilder = PromptBuilder.New().WithInvalidData();

    // Assert that building with invalid data should throw when we try to build
    AssertThrows<ArgumentException>(() => invalidPromptBuilder.Build(),
        "because invalid data should not be allowed");
  }

  [Fact]
  public void PromptLifecycle_ShouldHandleActivationAndDeactivation()
  {
    // Arrange
    var prompt = PromptBuilder.New()
        .WithTitle("Lifecycle Test Prompt")
        .Build();

    // Act & Assert
    prompt.IsActive.Should().BeTrue("because prompts are active by default");

    // Deactivate
    prompt.Deactivate();
    prompt.IsActive.Should().BeFalse();
    var deactivatedTime = prompt.UpdatedAt;

    // Activate
    prompt.Activate();
    prompt.IsActive.Should().BeTrue();
    prompt.UpdatedAt.Should().BeAfter(deactivatedTime);
  }

  [Fact]
  public void PromptUpdates_ShouldUpdateTimestamps()
  {
    // Arrange
    var prompt = _fixture.DefaultPrompt;
    var originalUpdatedAt = prompt.UpdatedAt;

    // Wait a moment to ensure timestamp difference
    System.Threading.Thread.Sleep(10);

    // Act
    prompt.UpdateContent("Updated content");
    var afterContentUpdate = prompt.UpdatedAt;

    prompt.UpdateDescription("Updated description");
    var afterDescriptionUpdate = prompt.UpdatedAt;

    prompt.UpdateCategory("Updated category");
    var afterCategoryUpdate = prompt.UpdatedAt;

    prompt.UpdateTags("updated,tags");
    var afterTagsUpdate = prompt.UpdatedAt;

    // Assert
    afterContentUpdate.Should().BeAfter(originalUpdatedAt);
    afterDescriptionUpdate.Should().BeAfter(afterContentUpdate);
    afterCategoryUpdate.Should().BeAfter(afterDescriptionUpdate);
    afterTagsUpdate.Should().BeAfter(afterCategoryUpdate);
  }
}