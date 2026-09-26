using PromptOrganizer.Domain.Entities;
using System.Linq.Expressions;

namespace PromptOrganizer.Domain.Repositories;

/// <summary>
/// Repository interface for Category entities with specific query methods.
/// </summary>
public interface ICategoryRepository : IRepository<Category>
{
  /// <summary>
  /// Gets a category by its name.
  /// </summary>
  /// <param name="name">The category name.</param>
  /// <returns>The category or null if not found.</returns>
  Task<Category?> GetByNameAsync(string name);

  /// <summary>
  /// Gets active categories.
  /// </summary>
  /// <returns>A collection of active categories.</returns>
  Task<IEnumerable<Category>> GetActiveCategoriesAsync();

  /// <summary>
  /// Searches categories by name or description.
  /// </summary>
  /// <param name="searchTerm">The search term.</param>
  /// <returns>A collection of categories matching the search term.</returns>
  Task<IEnumerable<Category>> SearchAsync(string searchTerm);

  /// <summary>
  /// Checks if a category with the specified name exists.
  /// </summary>
  /// <param name="name">The category name.</param>
  /// <returns>True if the category exists; otherwise, false.</returns>
  Task<bool> ExistsByNameAsync(string name);

  /// <summary>
  /// Gets categories with their associated prompt counts.
  /// </summary>
  /// <returns>A collection of categories with prompt counts.</returns>
  Task<IEnumerable<(Category Category, int PromptCount)>> GetCategoriesWithPromptCountsAsync();
}