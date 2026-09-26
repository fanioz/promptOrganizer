using PromptOrganizer.Domain.Entities;
using PromptOrganizer.Domain.Repositories;

namespace PromptOrganizer.Services;

/// <summary>
/// Service implementation for prompt business logic operations.
/// </summary>
public class PromptService : IPromptService
{
  private readonly IPromptRepository _promptRepository;
  private readonly ICategoryRepository _categoryRepository;

  public PromptService(IPromptRepository promptRepository, ICategoryRepository categoryRepository)
  {
    _promptRepository = promptRepository ?? throw new ArgumentNullException(nameof(promptRepository));
    _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
  }

  public async Task<Prompt> CreatePromptAsync(string title, string content, string description, int? categoryId, string tags)
  {
    // Validate input data
    var validationErrors = await ValidatePromptDataAsync(title, content, description, categoryId, tags);
    if (validationErrors.Any())
    {
      throw new ArgumentException(string.Join("; ", validationErrors));
    }

    // Create the prompt
    var prompt = new Prompt(0, title, content, description, categoryId, tags);
    return await _promptRepository.AddAsync(prompt);
  }

  public async Task<Prompt> UpdatePromptAsync(int id, string title, string content, string description, int? categoryId, string tags)
  {
    // Get existing prompt
    var existingPrompt = await _promptRepository.GetByIdAsync(id);
    if (existingPrompt == null)
    {
      throw new KeyNotFoundException($"Prompt with ID {id} not found.");
    }

    // Validate input data
    var validationErrors = await ValidatePromptDataAsync(title, content, description, categoryId, tags);
    if (validationErrors.Any())
    {
      throw new ArgumentException(string.Join("; ", validationErrors));
    }

    // Update prompt properties
    existingPrompt.UpdateContent(content);
    existingPrompt.UpdateDescription(description);
    existingPrompt.UpdateCategory(categoryId);
    existingPrompt.UpdateTags(tags);

    return await _promptRepository.UpdateAsync(existingPrompt);
  }

  public async Task<Prompt> UpdatePromptAsync(Prompt prompt)
  {
      return await _promptRepository.UpdateAsync(prompt);
  }

  public async Task<Prompt?> GetPromptByIdAsync(int id)
  {
    return await _promptRepository.GetByIdAsync(id);
  }

  public async Task<IEnumerable<Prompt>> GetAllActivePromptsAsync()
  {
    return await _promptRepository.GetActivePromptsAsync();
  }

  public async Task<IEnumerable<Prompt>> GetInactivePromptsAsync()
  {
    return await _promptRepository.FindAsync(p => !p.IsActive);
  }

  public async Task<IEnumerable<Prompt>> GetPromptsByCategoryAsync(int categoryId)
  {
    return await _promptRepository.GetByCategoryIdAsync(categoryId);
  }

  public async Task<IEnumerable<Prompt>> GetPromptsByTagsAsync(IEnumerable<string> tags)
  {
    return await _promptRepository.GetByTagsAsync(tags);
  }

  public async Task<IEnumerable<Prompt>> SearchPromptsAsync(string searchTerm)
  {
    if (string.IsNullOrWhiteSpace(searchTerm))
    {
      return Enumerable.Empty<Prompt>();
    }

    return await _promptRepository.SearchAsync(searchTerm);
  }

  public async Task<bool> DeactivatePromptAsync(int id)
  {
    var prompt = await _promptRepository.GetByIdAsync(id);
    if (prompt == null)
    {
      return false;
    }

    prompt.Deactivate();
    await _promptRepository.UpdateAsync(prompt);
    return true;
  }

  public async Task<bool> ActivatePromptAsync(int id)
  {
    var prompt = await _promptRepository.GetByIdAsync(id);
    if (prompt == null)
    {
      return false;
    }

    prompt.Activate();
    await _promptRepository.UpdateAsync(prompt);
    return true;
  }

  public async Task<bool> DeletePromptAsync(int id)
  {
    return await _promptRepository.DeleteAsync(id);
  }

  public async Task<IEnumerable<string>> ValidatePromptDataAsync(string title, string content, string description, int? categoryId, string tags)
  {
    var errors = new List<string>();

    // Validate title
    if (string.IsNullOrWhiteSpace(title))
    {
      errors.Add("Title cannot be null or empty");
    }
    else if (title.Length > 200)
    {
      errors.Add("Title cannot exceed 200 characters");
    }

    // Validate content
    if (string.IsNullOrWhiteSpace(content))
    {
      errors.Add("Content cannot be null or empty");
    }
    else if (content.Length > 5000)
    {
      errors.Add("Content cannot exceed 5000 characters");
    }

    // Validate category if provided
    if (categoryId.HasValue)
    {
      var category = await _categoryRepository.GetByIdAsync(categoryId.Value);
      if (category == null)
      {
        errors.Add($"Category with ID {categoryId.Value} does not exist");
      }
    }

    return await Task.FromResult(errors);
  }

  public async Task<IEnumerable<Prompt>> GetPromptsByDateRangeAsync(DateTime fromDate, DateTime toDate)
  {
    return await _promptRepository.GetByDateRangeAsync(fromDate, toDate);
  }

  public async Task<int> GetTotalPromptCountAsync()
  {
    var allPrompts = await _promptRepository.GetAllAsync();
    return allPrompts.Count();
  }

  public async Task<int> GetPromptCountByCategoryAsync(int categoryId)
  {
    return await _promptRepository.CountByCategoryAsync(categoryId);
  }
}