using PromptOrganizer.Domain.Common;

namespace PromptOrganizer.Domain.Entities;

/// <summary>
/// Represents a tag entity in the PromptOrganizer domain.
/// </summary>
public class Tag : IEntity
{
  private const int MaxNameLength = 50;
  private const int MaxDescriptionLength = 200;

  /// <summary>
  /// Gets or sets the unique identifier of the tag.
  /// </summary>
  public int Id { get; set; }

  /// <summary>
  /// Gets the name of the tag.
  /// </summary>
  public string Name { get; }

  /// <summary>
  /// Gets the description of the tag.
  /// </summary>
  public string Description { get; private set; }

  /// <summary>
  /// Gets the color associated with the tag (hex format).
  /// </summary>
  public string Color { get; private set; }

  /// <summary>
  /// Gets the date and time when the tag was created.
  /// </summary>
  public DateTime CreatedAt { get; }

  /// <summary>
  /// Gets the date and time when the tag was last updated.
  /// </summary>
  public DateTime UpdatedAt { get; private set; }

  /// <summary>
  /// Gets a value indicating whether the tag is active.
  /// </summary>
  public bool IsActive { get; private set; }

  /// <summary>
  /// Initializes a new instance of the <see cref="Tag"/> class.
  /// </summary>
  /// <param name="id">The unique identifier of the tag.</param>
  /// <param name="name">The name of the tag.</param>
  /// <param name="description">The description of the tag.</param>
  /// <param name="color">The color associated with the tag (hex format).</param>
  /// <exception cref="ArgumentException">Thrown when validation fails.</exception>
  public Tag(int id, string name, string description, string color)
  {
    ValidateName(name);
    ValidateDescription(description);
    ValidateColor(color);

    Id = id;
    Name = name.Trim();
    Description = description?.Trim() ?? string.Empty;
    Color = color?.Trim() ?? "#000000";
    CreatedAt = DateTime.UtcNow;
    UpdatedAt = CreatedAt;
    IsActive = true;
  }

  /// <summary>
  /// Updates the description of the tag.
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
  /// Updates the color of the tag.
  /// </summary>
  /// <param name="newColor">The new color (hex format).</param>
  /// <exception cref="ArgumentException">Thrown when color is invalid.</exception>
  public void UpdateColor(string newColor)
  {
    ValidateColor(newColor);
    Color = newColor?.Trim() ?? "#000000";
    UpdatedAt = DateTime.UtcNow;
  }

  /// <summary>
  /// Deactivates the tag.
  /// </summary>
  public void Deactivate()
  {
    IsActive = false;
    UpdatedAt = DateTime.UtcNow;
  }

  /// <summary>
  /// Activates the tag.
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

  private static void ValidateColor(string color)
  {
    if (string.IsNullOrWhiteSpace(color))
    {
      return; // Default color will be used
    }

    if (!System.Text.RegularExpressions.Regex.IsMatch(color, @"^#[0-9A-Fa-f]{6}$"))
    {
      throw new ArgumentException("Color must be in hex format (#RRGGBB)");
    }
  }
}