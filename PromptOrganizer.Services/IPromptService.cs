using PromptOrganizer.Domain.Entities;

namespace PromptOrganizer.Services;

/// <summary>
/// Service interface for prompt business logic operations.
/// </summary>
public interface IPromptService
{
  /// <summary>
  /// Creates a new prompt with validation and business rules.
  /// </summary>
  /// <param name="title">The prompt title.</param>
  /// <param name="content">The prompt content.</param>
  /// <param name="description">The prompt description.</param>
  /// <param name="categoryId">The category ID.</param>
  /// <param name="tags">The tags as a comma-separated string.</param>
  /// <returns>The created prompt.</returns>
  Task<Prompt> CreatePromptAsync(string title, string content, string description, int? categoryId, string tags);

  /// <summary>
  /// Updates an existing prompt with validation and business rules.
  /// </summary>
  /// <param name="id">The prompt ID.</param>
  /// <param name="title">The new title.</param>
  /// <param name="content">The new content.</param>
  /// <param name="description">The new description.</param>
  /// <param name="categoryId">The new category ID.</param>
  /// <param name="tags">The new tags.</param>
  /// <returns>The updated prompt.</returns>
  Task<Prompt> UpdatePromptAsync(int id, string title, string content, string description, int? categoryId, string tags);

  /// <summary>
  /// Updates an existing prompt using the prompt entity itself.
  /// </summary>
  /// <param name="prompt">The prompt to update.</param>
  /// <returns>The updated prompt.</returns>
  Task<Prompt> UpdatePromptAsync(Prompt prompt);

  /// <summary>
  /// Gets a prompt by ID.
  /// </summary>
  /// <param name="id">The prompt ID.</param>
  /// <returns>The prompt or null if not found.</returns>
  Task<Prompt?> GetPromptByIdAsync(int id);

  /// <summary>
  /// Gets all active prompts.
  /// </summary>
  /// <returns>A collection of active prompts.</returns>
  Task<IEnumerable<Prompt>> GetAllActivePromptsAsync();

  /// <summary>
  /// Gets all inactive (deleted) prompts.
  /// </summary>
  /// <returns>A collection of inactive prompts.</returns>
  Task<IEnumerable<Prompt>> GetInactivePromptsAsync();

  /// <summary>
  /// Gets prompts by category.
  /// </summary>
  /// <param name="categoryId">The category ID.</param>
  /// <returns>A collection of prompts in the category.</returns>
  Task<IEnumerable<Prompt>> GetPromptsByCategoryAsync(int categoryId);

  /// <summary>
  /// Gets prompts by tags.
  /// </summary>
  /// <param name="tags">The tags to search for.</param>
  /// <returns>A collection of prompts with matching tags.</returns>
  Task<IEnumerable<Prompt>> GetPromptsByTagsAsync(IEnumerable<string> tags);

  /// <summary>
  Task<bool> ActivatePromptAsync(int id);

  /// <summary>
  /// Validates prompt data before creation or update.
  /// </summary>
  /// <param name="title">The prompt title.</param>
  /// <param name="content">The prompt content.</param>
  /// <param name="description">The prompt description.</param>
  /// <param name="categoryId">The category ID.</param>
  /// <param name="tags">The tags.</param>
  /// <returns>A collection of validation errors.</returns>
  Task<IEnumerable<string>> ValidatePromptDataAsync(string title, string content, string description, int? categoryId, string tags);

  /// <summary>
  /// Gets prompts created within a date range.
  /// </summary>
  /// <param name="fromDate">The from date.</param>
  /// <param name="toDate">The to date.</param>
  /// <returns>A collection of prompts created within the date range.</returns>
  Task<IEnumerable<Prompt>> GetPromptsByDateRangeAsync(DateTime fromDate, DateTime toDate);

  /// <summary>
  /// Gets the total count of prompts.
  /// </summary>
  /// <returns>The total count of prompts.</returns>
  Task<int> GetTotalPromptCountAsync();

  /// <summary>
  /// Gets the count of prompts by category.
  /// </summary>
  /// <param name="categoryId">The category ID.</param>
  /// <returns>The count of prompts in the category.</returns>
  Task<int> GetPromptCountByCategoryAsync(int categoryId);
}