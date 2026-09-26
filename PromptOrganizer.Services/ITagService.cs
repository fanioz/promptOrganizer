using PromptOrganizer.Domain.Entities;

namespace PromptOrganizer.Services;

/// <summary>
/// Service interface for tag business logic operations.
/// </summary>
public interface ITagService
{
  /// <summary>
  /// Creates a new tag with validation and business rules.
  /// </summary>
  /// <param name="name">The tag name.</param>
  /// <param name="description">The tag description.</param>
  /// <param name="color">The tag color in hex format.</param>
  /// <returns>The created tag.</returns>
  Task<Tag> CreateTagAsync(string name, string description, string color);

  /// <summary>
  /// Updates an existing tag with validation and business rules.
  /// </summary>
  /// <param name="id">The tag ID.</param>
  /// <param name="name">The new name.</param>
  /// <param name="description">The new description.</param>
  /// <param name="color">The new color.</param>
  /// <returns>The updated tag.</returns>
  Task<Tag> UpdateTagAsync(int id, string name, string description, string color);

  /// <summary>
  /// Gets a tag by ID.
  /// </summary>
  /// <param name="id">The tag ID.</param>
  /// <returns>The tag or null if not found.</returns>
  Task<Tag?> GetTagByIdAsync(int id);

  /// <summary>
  /// Gets a tag by name.
  /// </summary>
  /// <param name="name">The tag name.</param>
  /// <returns>The tag or null if not found.</returns>
  Task<Tag?> GetTagByNameAsync(string name);

  /// <summary>
  /// Gets all active tags.
  /// </summary>
  /// <returns>A collection of active tags.</returns>
  Task<IEnumerable<Tag>> GetAllActiveTagsAsync();

  /// <summary>
  /// Gets tags by color.
  /// </summary>
  /// <param name="color">The color in hex format.</param>
  /// <returns>A collection of tags with the specified color.</returns>
  Task<IEnumerable<Tag>> GetTagsByColorAsync(string color);

  /// <summary>
  /// Searches tags by name or description.
  /// </summary>
  /// <param name="searchTerm">The search term.</param>
  /// <returns>A collection of matching tags.</returns>
  Task<IEnumerable<Tag>> SearchTagsAsync(string searchTerm);

  /// <summary>
  /// Deactivates a tag.
  /// </summary>
  /// <param name="id">The tag ID.</param>
  /// <returns>True if successful; otherwise, false.</returns>
  Task<bool> DeactivateTagAsync(int id);

  /// <summary>
  /// Activates a tag.
  /// </summary>
  /// <param name="id">The tag ID.</param>
  /// <returns>True if successful; otherwise, false.</returns>
  Task<bool> ActivateTagAsync(int id);

  /// <summary>
  /// Checks if a tag name already exists.
  /// </summary>
  /// <param name="name">The tag name.</param>
  /// <param name="excludeId">Optional tag ID to exclude from the check.</param>
  /// <returns>True if the name exists; otherwise, false.</returns>
  Task<bool> TagNameExistsAsync(string name, int? excludeId = null);

  /// <summary>
  /// Validates tag data before creation or update.
  /// </summary>
  /// <param name="name">The tag name.</param>
  /// <param name="description">The tag description.</param>
  /// <param name="color">The tag color.</param>
  /// <param name="excludeId">Optional tag ID to exclude from name uniqueness check.</param>
  /// <returns>A collection of validation errors.</returns>
  Task<IEnumerable<string>> ValidateTagDataAsync(string name, string description, string color, int? excludeId = null, bool checkDuplicateName = true);

  /// <summary>
  /// Gets tags that can be safely deleted (have no associated prompts).
  /// </summary>
  /// <returns>A collection of deletable tags.</returns>
  Task<IEnumerable<Tag>> GetDeletableTagsAsync();

  /// <summary>
  /// Checks if a tag can be deleted.
  /// </summary>
  /// <param name="tagId">The tag ID.</param>
  /// <returns>True if the tag can be deleted; otherwise, false.</returns>
  Task<bool> CanDeleteTagAsync(int tagId);

  /// <summary>
  /// Gets the total count of tags.
  /// </summary>
  /// <returns>The total count of tags.</returns>
  Task<int> GetTotalTagCountAsync();

  /// <summary>
  /// Gets the count of active tags.
  /// </summary>
  /// <returns>The count of active tags.</returns>
  Task<int> GetActiveTagCountAsync();

  /// <summary>
  /// Gets tags by names.
  /// </summary>
  /// <param name="names">The tag names.</param>
  /// <returns>A collection of tags with matching names.</returns>
  Task<IEnumerable<Tag>> GetTagsByNamesAsync(IEnumerable<string> names);

  /// <summary>
  /// Gets the most popular tags based on usage.
  /// </summary>
  /// <param name="limit">The maximum number of tags to return.</param>
  /// <returns>A collection of popular tags.</returns>
  Task<IEnumerable<Tag>> GetPopularTagsAsync(int limit = 10);

  /// <summary>
  /// Gets tags that are similar to the specified tag.
  /// </summary>
  /// <param name="tagId">The tag ID.</param>
  /// <param name="limit">The maximum number of similar tags.</param>
  /// <returns>A collection of similar tags.</returns>
  Task<IEnumerable<Tag>> GetSimilarTagsAsync(int tagId, int limit = 5);
}