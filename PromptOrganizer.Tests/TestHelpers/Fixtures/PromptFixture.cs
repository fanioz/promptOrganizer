using PromptOrganizer.Domain.Entities;
using PromptOrganizer.Tests.TestHelpers.Builders;

namespace PromptOrganizer.Tests.TestHelpers.Fixtures;

/// <summary>
/// Test fixture providing pre-configured Prompt entities for testing.
/// </summary>
public class PromptFixture : FixtureBase
{
  public Prompt DefaultPrompt { get; private set; }
  public Prompt MinimalPrompt { get; private set; }
  public Prompt FullPrompt { get; private set; }
  public Prompt InactivePrompt { get; private set; }
  public List<Prompt> PromptList { get; private set; }

  /// <inheritdoc/>
  protected override void Initialize()
  {
    DefaultPrompt = PromptBuilder.New()
        .WithTitle("Default Test Prompt")
        .WithContent("This is a default test prompt content")
        .WithDescription("Default test description")
        .WithCategory("Test Category")
        .WithTags("test,default,prompt")
        .Build();

    MinimalPrompt = PromptBuilder.New()
        .WithMinimalData()
        .Build();

    FullPrompt = PromptBuilder.New()
        .WithTitle("Full Featured Prompt")
        .WithContent("This is a comprehensive prompt with detailed content that includes various features and capabilities for testing purposes.")
        .WithDescription("A fully featured prompt for comprehensive testing")
        .WithCategory("Comprehensive Testing")
        .WithTags("comprehensive,full,featured,testing,detailed")
        .Build();

    InactivePrompt = PromptBuilder.New()
        .WithTitle("Inactive Test Prompt")
        .WithContent("This prompt is inactive for testing purposes")
        .Build();

    // Deactivate the inactive prompt
    InactivePrompt.Deactivate();

    PromptList = new List<Prompt>
        {
            DefaultPrompt,
            MinimalPrompt,
            FullPrompt,
            InactivePrompt,
            PromptBuilder.New().WithTitle("Prompt 1").Build(),
            PromptBuilder.New().WithTitle("Prompt 2").Build(),
            PromptBuilder.New().WithTitle("Prompt 3").Build()
        };
  }

  /// <summary>
  /// Gets a prompt with specific characteristics.
  /// </summary>
  public Prompt GetPromptWithTitle(string title)
  {
    return PromptBuilder.New()
        .WithTitle(title)
        .Build();
  }

  /// <summary>
  /// Gets a prompt with specific content.
  /// </summary>
  public Prompt GetPromptWithContent(string content)
  {
    return PromptBuilder.New()
        .WithContent(content)
        .Build();
  }

  /// <summary>
  /// Gets a prompt with specific category.
  /// </summary>
  public Prompt GetPromptWithCategory(string category)
  {
    return PromptBuilder.New()
        .WithCategory(category)
        .Build();
  }

  /// <summary>
  /// Gets a prompt with specific tags.
  /// </summary>
  public Prompt GetPromptWithTags(string tags)
  {
    return PromptBuilder.New()
        .WithTags(tags)
        .Build();
  }

  /// <summary>
  /// Gets a list of prompts with different categories.
  /// </summary>
  public List<Prompt> GetPromptsWithDifferentCategories()
  {
    return new List<Prompt>
        {
            PromptBuilder.New().WithTitle("Development Prompt").WithCategory("Development").Build(),
            PromptBuilder.New().WithTitle("Testing Prompt").WithCategory("Testing").Build(),
            PromptBuilder.New().WithTitle("Documentation Prompt").WithCategory("Documentation").Build(),
            PromptBuilder.New().WithTitle("Design Prompt").WithCategory("Design").Build()
        };
  }

  /// <summary>
  /// Gets a list of prompts with different activity states.
  /// </summary>
  public List<Prompt> GetPromptsWithMixedActivityStates()
  {
    var activePrompt = PromptBuilder.New().WithTitle("Active Prompt").Build();
    var inactivePrompt = PromptBuilder.New().WithTitle("Inactive Prompt").Build();
    inactivePrompt.Deactivate();

    return new List<Prompt> { activePrompt, inactivePrompt };
  }

  /// <summary>
  /// Gets a list of prompts for pagination testing.
  /// </summary>
  public List<Prompt> GetPromptsForPagination(int count = 20)
  {
    var prompts = new List<Prompt>();
    for (int i = 1; i <= count; i++)
    {
      prompts.Add(PromptBuilder.New()
          .WithTitle($"Prompt {i}")
          .WithContent($"Content for prompt {i}")
          .Build());
    }
    return prompts;
  }
}