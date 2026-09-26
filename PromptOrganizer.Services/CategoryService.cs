using PromptOrganizer.Domain.Entities;
using PromptOrganizer.Domain.Repositories;

namespace PromptOrganizer.Services;

/// <summary>
/// Service implementation for category business logic operations.
/// </summary>
public class CategoryService : ICategoryService
{
  private readonly ICategoryRepository _categoryRepository;
  private readonly IPromptRepository _promptRepository;

  public CategoryService(ICategoryRepository categoryRepository, IPromptRepository promptRepository)
  {
    _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
    _promptRepository = promptRepository ?? throw new ArgumentNullException(nameof(promptRepository));
  }

  public async Task<Category> CreateCategoryAsync(string name, string description)
  {
    // Validate input data (excluding duplicate name check)
    var validationErrors = await ValidateCategoryDataAsync(name, description, null, false);
    if (validationErrors.Any())
    {
      throw new ArgumentException(string.Join("; ", validationErrors));
    }

    // Check for duplicate name
    if (await _categoryRepository.ExistsByNameAsync(name))
    {
      throw new InvalidOperationException("Category with this name already exists");
    }

    // Create the category
    var category = new Category(0, name, description);
    return await _categoryRepository.AddAsync(category);
  }

  public async Task<Category> UpdateCategoryAsync(int id, string name, string description)
  {
    // Get existing category
    var existingCategory = await _categoryRepository.GetByIdAsync(id);
    if (existingCategory == null)
    {
      throw new KeyNotFoundException($"Category with ID {id} not found.");
    }

    // Validate input data
    var validationErrors = await ValidateCategoryDataAsync(name, description, id);
    if (validationErrors.Any())
    {
      throw new ArgumentException(string.Join("; ", validationErrors));
    }

    // Update category properties
    existingCategory.UpdateDescription(description);

    return await _categoryRepository.UpdateAsync(existingCategory);
  }

  public async Task<Category?> GetCategoryByIdAsync(int id)
  {
    return await _categoryRepository.GetByIdAsync(id);
  }

  public async Task<Category?> GetCategoryByNameAsync(string name)
  {
    return await _categoryRepository.GetByNameAsync(name);
  }

  public async Task<IEnumerable<Category>> GetAllActiveCategoriesAsync()
  {
    return await _categoryRepository.GetActiveCategoriesAsync();
  }

  public async Task<IEnumerable<CategoryWithPromptCount>> GetCategoriesWithPromptCountsAsync()
  {
    var categoriesWithCounts = await _categoryRepository.GetCategoriesWithPromptCountsAsync();
    return categoriesWithCounts.Select(tuple => new CategoryWithPromptCount
    {
      Category = tuple.Category,
      PromptCount = tuple.PromptCount
    });
  }

  public async Task<IEnumerable<Category>> SearchCategoriesAsync(string searchTerm)
  {
    if (string.IsNullOrWhiteSpace(searchTerm))
    {
      return Enumerable.Empty<Category>();
    }

    return await _categoryRepository.SearchAsync(searchTerm);
  }

  public async Task<bool> DeactivateCategoryAsync(int id)
  {
    var category = await _categoryRepository.GetByIdAsync(id);
    if (category == null)
    {
      return false;
    }

    // Check if category can be deactivated (has no active prompts)
    var promptCount = await _promptRepository.CountByCategoryAsync(id);
    if (promptCount > 0)
    {
      throw new InvalidOperationException("Cannot deactivate category that has associated prompts");
    }

    category.Deactivate();
    await _categoryRepository.UpdateAsync(category);
    return true;
  }

  public async Task<bool> ActivateCategoryAsync(int id)
  {
    var category = await _categoryRepository.GetByIdAsync(id);
    if (category == null)
    {
      return false;
    }

    category.Activate();
    await _categoryRepository.UpdateAsync(category);
    return true;
  }

  public async Task<bool> CategoryNameExistsAsync(string name, int? excludeId = null)
  {
    if (excludeId.HasValue)
    {
      var existingCategory = await _categoryRepository.GetByNameAsync(name);
      return existingCategory != null && existingCategory.Id != excludeId.Value;
    }

    return await _categoryRepository.ExistsByNameAsync(name);
  }

  public async Task<IEnumerable<string>> ValidateCategoryDataAsync(string name, string description, int? excludeId = null, bool checkDuplicateName = true)
  {
    var errors = new List<string>();

    // Validate name
    if (string.IsNullOrWhiteSpace(name))
    {
      errors.Add("Name cannot be null or empty");
    }
    else if (name.Length > 100)
    {
      errors.Add("Name cannot exceed 100 characters");
    }

    // Validate description
    if (description != null && description.Length > 500)
    {
      errors.Add("Description cannot exceed 500 characters");
    }

    // Check for duplicate name
    if (checkDuplicateName && !string.IsNullOrWhiteSpace(name) && await CategoryNameExistsAsync(name, excludeId))
    {
      errors.Add("Category name already exists");
    }

    return errors;
  }

  public async Task<IEnumerable<Category>> GetDeletableCategoriesAsync()
  {
    var allCategories = await _categoryRepository.GetAllAsync();
    var deletableCategories = new List<Category>();

    foreach (var category in allCategories)
    {
      if (await CanDeleteCategoryAsync(category.Id))
      {
        deletableCategories.Add(category);
      }
    }

    return deletableCategories;
  }

  public async Task<bool> CanDeleteCategoryAsync(int categoryId)
  {
    var promptCount = await _promptRepository.CountByCategoryAsync(categoryId);
    return promptCount == 0;
  }

  public async Task<int> GetTotalCategoryCountAsync()
  {
    var allCategories = await _categoryRepository.GetAllAsync();
    return allCategories.Count();
  }

  public async Task<int> GetActiveCategoryCountAsync()
  {
    var activeCategories = await _categoryRepository.GetActiveCategoriesAsync();
    return activeCategories.Count();
  }
}