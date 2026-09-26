using PromptOrganizer.Domain.Entities;
using System.Linq.Expressions;

namespace PromptOrganizer.Domain.Repositories;

/// <summary>
/// Repository interface for Tag entities with specific query methods.
/// </summary>
public interface ITagRepository : IRepository<Tag>
{
  /// <summary>
  /// Gets a tag by its name.
  /// </summary>
  /// <param name="name">The tag name.</param>
  /// <returns>The tag or null if not found.</returns>
  Task<Tag?> GetByNameAsync(string name);

  /// <summary>
  /// Gets active tags.
  /// </summary>
  /// <returns>A collection of active tags.</returns>
  Task<IEnumerable<Tag>> GetActiveTagsAsync();

  /// <summary>
  /// Gets tags by color.
  /// </summary>
  /// <param name="color">The color (hex format).</param>
  /// <returns>A collection of tags with the specified color.</returns>
  Task<IEnumerable<Tag>> GetByColorAsync(string color);

  /// <summary>
  /// Searches tags by name or description.
  /// </summary>
  /// <param name="searchTerm">The search term.</param>
  /// <returns>A collection of tags matching the search term.</returns>
  Task<IEnumerable<Tag>> SearchAsync(string searchTerm);

  /// <summary>
  /// Checks if a tag with the specified name exists.
  /// </summary>
  /// <param name="name">The tag name.</param>
  /// <returns>True if the tag exists; otherwise, false.</returns>
  Task<bool> ExistsByNameAsync(string name);

  /// <summary>
  /// Gets tags by names.
  /// </summary>
  /// <param name="names">The tag names.</param>
  /// <returns>A collection of tags with matching names.</returns>
  Task<IEnumerable<Tag>> GetByNamesAsync(IEnumerable<string> names);
}