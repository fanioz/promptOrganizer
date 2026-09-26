using PromptOrganizer.Domain.Common;
using LiteDB;

namespace PromptOrganizer.Domain.Entities;

/// <summary>
/// Represents a category entity in the PromptOrganizer domain.
/// </summary>
public class Category : IEntity
{
  private const int MaxNameLength = 100;
  private const int MaxDescriptionLength = 500;

  /// <summary>
  /// Gets or sets the unique identifier of the category.
  /// </summary>
  [BsonId]
  public int Id { get; set; }

  /// <summary>
  /// Gets the name of the category.
  /// </summary>
  public string Name { get; }

  /// <summary>
  /// Gets the description of the category.
  /// </summary>
  public string Description { get; private set; }

  /// <summary>
  /// Gets the date and time when the category was created.
  /// </summary>
  public DateTime CreatedAt { get; }

  /// <summary>
  /// Gets the date and time when the category was last updated.
  /// </summary>
  public DateTime UpdatedAt { get; private set; }

  /// <summary>
  /// Gets a value indicating whether the category is active.
  /// </summary>
  public bool IsActive { get; private set; }

  /// <summary>
  /// Initializes a new instance of the <see cref="Category"/> class.
  /// </summary>
  /// <param name="id">The unique identifier of the category.</param>
  /// <param name="name">The name of the category.</param>
  /// <param name="description">The description of the category.</param>
  /// <exception cref="ArgumentException">Thrown when validation fails.</exception>
  public Category(int id, string name, string description)
  {
    ValidateName(name);
    ValidateDescription(description);

    Id = id;
    Name = name.Trim();
    Description = description?.Trim() ?? string.Empty;
    CreatedAt = DateTime.UtcNow;
    UpdatedAt = CreatedAt;
    IsActive = true;
  }

  /// <summary>
  /// Updates the description of the category.
  /// </summary>
  /// <param name="newDescription">The new description.</param>
  /// <exception cref="ArgumentException">Thrown when description is invalid.</exception>
  public void UpdateDescription(string newDescription)
  {
    ValidateDescription(newDescription);
    Description = newDescription?.Trim() ?? string.Empty;
    UpdatedAt = DateTime.UtcNow;
  }

  /// <summary>
  /// Deactivates the category.
  /// </summary>
  public void Deactivate()
  {
    IsActive = false;
    UpdatedAt = DateTime.UtcNow;
  }

  /// <summary>
  /// Activates the category.
  /// </summary>
  public void Activate()
  {
    IsActive = true;
    UpdatedAt = DateTime.UtcNow;
  }

  private static void ValidateName(string name)
  {
    if (string.IsNullOrWhiteSpace(name))
    {
      throw new ArgumentException("Name cannot be null or empty");
    }

    if (name.Length > MaxNameLength)
    {
      throw new ArgumentException($"Name cannot exceed {MaxNameLength} characters");
    }
  }

  private static void ValidateDescription(string description)
  {
    if (description != null && description.Length > MaxDescriptionLength)
    {
      throw new ArgumentException($"Description cannot exceed {MaxDescriptionLength} characters");
    }
  }
}