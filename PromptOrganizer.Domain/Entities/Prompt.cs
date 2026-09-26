using PromptOrganizer.Domain.Common;

namespace PromptOrganizer.Domain.Entities;

/// <summary>
/// Represents a prompt entity in the PromptOrganizer domain.
/// </summary>
public class Prompt : IEntity
{
  private const int MaxTitleLength = 200;
  private const int MaxContentLength = 5000;

  /// <summary>
  /// Gets or sets the unique identifier of the prompt.
  /// </summary>
  public int Id { get; set; }

  /// <summary>
  /// Gets the title of the prompt.
  /// </summary>
  public string Title { get; }

  /// <summary>
  /// Gets the content of the prompt.
  /// </summary>
  public string Content { get; private set; }

  /// <summary>
  /// Gets the description of the prompt.
  /// </summary>
  public string Description { get; private set; }

  /// <summary>
  /// Gets the category ID of the prompt.
  /// </summary>
  public int? CategoryId { get; private set; }

  /// <summary>
  /// Gets the category of the prompt.
  /// </summary>
  public Category? Category { get; private set; }

  /// <summary>
  /// Gets the tags associated with the prompt.
  /// </summary>
  public string Tags { get; private set; }

  /// <summary>
  /// Gets the date and time when the prompt was created.
  /// </summary>
  public DateTime CreatedAt { get; }

  /// <summary>
  /// Gets the date and time when the prompt was last updated.
  /// </summary>
  public DateTime UpdatedAt { get; private set; }

  /// <summary>
  /// Gets a value indicating whether the prompt is active.
  /// </summary>
  public bool IsActive { get; private set; }

  /// <summary>
  /// Gets or sets a value indicating whether the prompt is a favorite.
  /// </summary>
  public bool IsFavorite { get; private set; }

  /// <summary>
  /// Gets the number of versions of this prompt.
  /// </summary>
  public int VersionCount { get; private set; } = 1;

  /// <summary>
  /// Gets the legacy category string for backward compatibility.
  /// </summary>
  public string CategoryName { get; private set; } = string.Empty;

  /// <summary>
  /// Initializes a new instance of the <see cref="Prompt"/> class.
  /// </summary>
  /// <param name="id">The unique identifier of the prompt.</param>
  /// <param name="title">The title of the prompt.</param>
  /// <param name="content">The content of the prompt.</param>
  /// <param name="description">The description of the prompt.</param>
  /// <param name="categoryId">The category ID of the prompt.</param>
  /// <param name="tags">The tags associated with the prompt.</param>
  /// <exception cref="ArgumentException">Thrown when validation fails.</exception>
  [LiteDB.BsonCtor]
  public Prompt(int id, string title, string content, string description, int? categoryId, string tags)
  {
    ValidateTitle(title);
    ValidateContent(content);

    Id = id;
    Title = title.Trim();
    Content = content.Trim();
    Description = description?.Trim() ?? string.Empty;
    CategoryId = categoryId;
    Tags = tags?.Trim() ?? string.Empty;

    CreatedAt = DateTime.UtcNow;
    UpdatedAt = CreatedAt;
    IsActive = true;
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="Prompt"/> class.
  /// </summary>
  /// <param name="title">The title of the prompt.</param>
  /// <param name="content">The content of the prompt.</param>
  /// <param name="description">The description of the prompt.</param>
  /// <param name="category">The category of the prompt.</param>
  /// <param name="tags">The tags associated with the prompt.</param>
  /// <exception cref="ArgumentException">Thrown when validation fails.</exception>
  public Prompt(string title, string content, string description, string category, string tags)
  {
    ValidateTitle(title);
    ValidateContent(content);

    Id = 0; // Will be set by repository
    Title = title.Trim();
    Content = content.Trim();
    Description = description?.Trim() ?? string.Empty;
    CategoryName = category?.Trim() ?? string.Empty;
    CategoryId = null; // Legacy support - string category stored in separate property
    Tags = tags?.Trim() ?? string.Empty;

    CreatedAt = DateTime.UtcNow;
    UpdatedAt = CreatedAt;
    IsActive = true;
  }

  /// <summary>
  /// Updates the content of the prompt.
  /// </summary>
  /// <param name="newContent">The new content.</param>
  /// <exception cref="ArgumentException">Thrown when content is invalid.</exception>
  public void UpdateContent(string newContent)
  {
    ValidateContent(newContent);
    Content = newContent.Trim();
    UpdatedAt = DateTime.UtcNow;
  }

  /// <summary>
  /// Updates the description of the prompt.
  /// </summary>
  /// <param name="newDescription">The new description.</param>
  public void UpdateDescription(string newDescription)
  {
    Description = newDescription?.Trim() ?? string.Empty;
    UpdatedAt = DateTime.UtcNow;
  }

  /// <summary>
  /// Updates the category of the prompt.
  /// </summary>
  /// <param name="newCategoryId">The new category ID.</param>
  public void UpdateCategory(int? newCategoryId)
  {
    CategoryId = newCategoryId;
    UpdatedAt = DateTime.UtcNow;
  }

  /// <summary>
  /// Updates the category of the prompt.
  /// </summary>
  /// <param name="newCategory">The new category.</param>
  public void UpdateCategory(Category? newCategory)
  {
    CategoryName = newCategory?.Name ?? string.Empty;
    CategoryId = newCategory?.Id;
    UpdatedAt = DateTime.UtcNow;
  }

  /// <summary>
  /// Updates the category of the prompt (legacy string version).
  /// </summary>
  /// <param name="newCategory">The new category.</param>
  public void UpdateCategory(string newCategory)
  {
    CategoryName = newCategory?.Trim() ?? string.Empty;
    CategoryId = null; // Clear category ID when using legacy string
    UpdatedAt = DateTime.UtcNow;
  }

  /// <summary>
  /// Updates the tags of the prompt.
  /// </summary>
  /// <param name="newTags">The new tags.</param>
  public void UpdateTags(string newTags)
  {
    Tags = newTags?.Trim() ?? string.Empty;
    UpdatedAt = DateTime.UtcNow;
  }

  /// <summary>
  /// Deactivates the prompt.
  /// </summary>
  public void Deactivate()
  {
    IsActive = false;
    UpdatedAt = DateTime.UtcNow;
  }

  /// <summary>
  /// Activates the prompt.
  /// </summary>
  public void Activate()
  {
    IsActive = true;
    UpdatedAt = DateTime.UtcNow;
  }

  /// <summary>
  /// Toggles the favorite status of the prompt.
  /// </summary>
  public void ToggleFavorite()
  {
    IsFavorite = !IsFavorite;
    UpdatedAt = DateTime.UtcNow;
  }

  /// <summary>
  /// Increments the version count of the prompt.
  /// </summary>
  public void IncrementVersion()
  {
    VersionCount++;
    UpdatedAt = DateTime.UtcNow;
  }

  private static void ValidateTitle(string title)
  {
    if (string.IsNullOrWhiteSpace(title))
    {
      throw new ArgumentException("Title cannot be null or empty");
    }

    if (title.Length > MaxTitleLength)
    {
      throw new ArgumentException($"Title cannot exceed {MaxTitleLength} characters");
    }
  }

  private static void ValidateContent(string content)
  {
    if (string.IsNullOrWhiteSpace(content))
    {
      throw new ArgumentException("Content cannot be null or empty");
    }

    if (content.Length > MaxContentLength)
    {
      throw new ArgumentException($"Content cannot exceed {MaxContentLength} characters");
    }
  }
}