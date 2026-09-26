using PromptOrganizer.Domain.Entities;
using System.Linq.Expressions;

namespace PromptOrganizer.Domain.Repositories;

/// <summary>
/// Repository interface for Prompt entities with specific query methods.
/// </summary>
public interface IPromptRepository : IRepository<Prompt>
{
  /// <summary>
  /// Gets prompts by category ID.
  /// </summary>
  /// <param name="categoryId">The category ID.</param>
  /// <returns>A collection of prompts in the specified category.</returns>
  Task<IEnumerable<Prompt>> GetByCategoryIdAsync(int categoryId);

  /// <summary>
  /// Gets active prompts.
  /// </summary>
  /// <returns>A collection of active prompts.</returns>
  Task<IEnumerable<Prompt>> GetActivePromptsAsync();

  /// <summary>
  /// Gets prompts by tags.
  /// </summary>
  /// <param name="tags">The tags to search for.</param>
  /// <returns>A collection of prompts with matching tags.</returns>
  Task<IEnumerable<Prompt>> GetByTagsAsync(IEnumerable<string> tags);

  /// <summary>
  /// Searches prompts by title or content.
  /// </summary>
  /// <param name="searchTerm">The search term.</param>
  /// <returns>A collection of prompts matching the search term.</returns>
  Task<IEnumerable<Prompt>> SearchAsync(string searchTerm);

  /// <summary>
  /// Gets prompts created within a date range.
  /// </summary>
  /// <param name="fromDate">The from date.</param>
  /// <param name="toDate">The to date.</param>
  /// <returns>A collection of prompts created within the date range.</returns>
  Task<IEnumerable<Prompt>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate);

  /// <summary>
  /// Gets the total count of prompts by category.
  /// </summary>
  /// <param name="categoryId">The category ID.</param>
  /// <returns>The count of prompts in the category.</returns>
  Task<int> CountByCategoryAsync(int categoryId);
}