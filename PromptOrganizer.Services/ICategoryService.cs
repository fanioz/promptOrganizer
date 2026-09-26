using PromptOrganizer.Domain.Entities;

namespace PromptOrganizer.Services;

/// <summary>
/// Service interface for category business logic operations.
/// </summary>
public interface ICategoryService
{
  /// <summary>
  /// Creates a new category with validation and business rules.
  /// </summary>
  /// <param name="name">The category name.</param>
  /// <param name="description">The category description.</param>
  /// <returns>The created category.</returns>
  Task<Category> CreateCategoryAsync(string name, string description);

  /// <summary>
  /// Updates an existing category with validation and business rules.
  /// </summary>
  /// <param name="id">The category ID.</param>
  /// <param name="name">The new name.</param>
  /// <param name="description">The new description.</param>
  /// <returns>The updated category.</returns>
  Task<Category> UpdateCategoryAsync(int id, string name, string description);

  /// <summary>
  /// Gets a category by ID.
  /// </summary>
  /// <param name="id">The category ID.</param>
  /// <returns>The category or null if not found.</returns>
  Task<Category?> GetCategoryByIdAsync(int id);

  /// <summary>
  /// Gets a category by name.
  /// </summary>
  /// <param name="name">The category name.</param>
  /// <returns>The category or null if not found.</returns>
  Task<Category?> GetCategoryByNameAsync(string name);

  /// <summary>
  /// Gets all active categories.
  /// </summary>
  /// <returns>A collection of active categories.</returns>
  Task<IEnumerable<Category>> GetAllActiveCategoriesAsync();

  /// <summary>
  /// Gets all categories with their associated prompt counts.
  /// </summary>
  /// <returns>A collection of categories with prompt counts.</returns>
  Task<IEnumerable<CategoryWithPromptCount>> GetCategoriesWithPromptCountsAsync();

  /// <summary>
  /// Searches categories by name or description.
  /// </summary>
  /// <param name="searchTerm">The search term.</param>
  /// <returns>A collection of matching categories.</returns>
  Task<IEnumerable<Category>> SearchCategoriesAsync(string searchTerm);

  /// <summary>
  /// Deactivates a category.
  /// </summary>
  /// <param name="id">The category ID.</param>
  /// <returns>True if successful; otherwise, false.</returns>
  Task<bool> DeactivateCategoryAsync(int id);

  /// <summary>
  /// Activates a category.
  /// </summary>
  /// <param name="id">The category ID.</param>
  /// <returns>True if successful; otherwise, false.</returns>
  Task<bool> ActivateCategoryAsync(int id);

  /// <summary>
  /// Checks if a category name already exists.
  /// </summary>
  /// <param name="name">The category name.</param>
  /// <param name="excludeId">Optional category ID to exclude from the check.</param>
  /// <returns>True if the name exists; otherwise, false.</returns>
  Task<bool> CategoryNameExistsAsync(string name, int? excludeId = null);

  /// <summary>
  /// Validates category data before creation or update.
  /// </summary>
  /// <param name="name">The category name.</param>
  /// <param name="description">The category description.</param>
  /// <param name="excludeId">Optional category ID to exclude from name uniqueness check.</param>
  /// <param name="checkDuplicateName">Whether to check for duplicate names. Default is true.</param>
  /// <returns>A collection of validation errors.</returns>
  Task<IEnumerable<string>> ValidateCategoryDataAsync(string name, string description, int? excludeId = null, bool checkDuplicateName = true);

  /// <summary>
  /// Gets categories that can be safely deleted (have no associated prompts).
  /// </summary>
  /// <returns>A collection of deletable categories.</returns>
  Task<IEnumerable<Category>> GetDeletableCategoriesAsync();

  /// <summary>
  /// Checks if a category can be deleted.
  /// </summary>
  /// <param name="categoryId">The category ID.</param>
  /// <returns>True if the category can be deleted; otherwise, false.</returns>
  Task<bool> CanDeleteCategoryAsync(int categoryId);

  /// <summary>
  /// Gets the total count of categories.
  /// </summary>
  /// <returns>The total count of categories.</returns>
  Task<int> GetTotalCategoryCountAsync();

  /// <summary>
  /// Gets the count of active categories.
  /// </summary>
  /// <returns>The count of active categories.</returns>
  Task<int> GetActiveCategoryCountAsync();
}

/// <summary>
/// Represents a category with its associated prompt count.
/// </summary>
public class CategoryWithPromptCount
{
  /// <summary>
  /// Gets or sets the category.
  /// </summary>
  public Category Category { get; set; } = null!;

  /// <summary>
  /// Gets or sets the prompt count.
  /// </summary>
  public int PromptCount { get; set; }
}