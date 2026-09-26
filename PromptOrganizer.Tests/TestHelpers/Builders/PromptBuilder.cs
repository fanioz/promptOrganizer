using PromptOrganizer.Domain.Entities;

namespace PromptOrganizer.Tests.TestHelpers.Builders;

/// <summary>
/// Builder for creating test instances of <see cref="Prompt"/>.
/// </summary>
public class PromptBuilder : BuilderBase<Prompt, PromptBuilder>
{
  private string _title = "Default Test Prompt";
  private string _content = "Default test prompt content";
  private string _description = "Default test prompt description";
  private string _category = "Default Category";
  private string _tags = "test,default,prompt";

  /// <summary>
  /// Sets the title of the prompt.
  /// </summary>
  public PromptBuilder WithTitle(string title)
  {
    _title = title;
    return this;
  }

  /// <summary>
  /// Sets the content of the prompt.
  /// </summary>
  public PromptBuilder WithContent(string content)
  {
    _content = content;
    return this;
  }

  /// <summary>
  /// Sets the description of the prompt.
  /// </summary>
  public PromptBuilder WithDescription(string description)
  {
    _description = description;
    return this;
  }

  /// <summary>
  /// Sets the category of the prompt.
  /// </summary>
  public PromptBuilder WithCategory(string category)
  {
    _category = category;
    return this;
  }

  /// <summary>
  /// Sets the tags of the prompt.
  /// </summary>
  public PromptBuilder WithTags(string tags)
  {
    _tags = tags;
    return this;
  }

  /// <summary>
  /// Creates a prompt with minimal required fields.
  /// </summary>
  public PromptBuilder WithMinimalData()
  {
    _title = "Minimal Prompt";
    _content = "Minimal content";
    _description = string.Empty;
    _category = string.Empty;
    _tags = string.Empty;
    return this;
  }

  /// <summary>
  /// Creates a prompt with maximum length content.
  /// </summary>
  public PromptBuilder WithMaximumLengthData()
  {
    _title = new string('A', 200); // Max title length
    _content = new string('B', 5000); // Max content length
    _description = new string('C', 1000);
    _category = "Maximum Category";
    _tags = "max,length,data,test";
    return this;
  }

  /// <summary>
  /// Creates a prompt with invalid data for testing validation.
  /// </summary>
  public PromptBuilder WithInvalidData()
  {
    _title = string.Empty;
    _content = string.Empty;
    return this;
  }

  /// <summary>
  /// Creates a prompt with a specific creation date.
  /// </summary>
  public PromptBuilder WithCreationDate(DateTime createdAt)
  {
    // Note: This would require modifying the Prompt class to support dependency injection of DateTime
    // For now, this is a placeholder for future enhancement
    return this;
  }

  /// <summary>
  /// Creates an inactive prompt.
  /// </summary>
  public PromptBuilder Inactive()
  {
    // Note: This would require the builder to track state and apply it after build
    // For now, this is a placeholder for future enhancement
    return this;
  }

  /// <inheritdoc/>
  protected override Prompt CreateDefault()
  {
    return new Prompt(_title, _content, _description, _category, _tags);
  }

  /// <inheritdoc/>
  public override Prompt Build()
  {
    return new Prompt(_title, _content, _description, _category, _tags);
  }
}