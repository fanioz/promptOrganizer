using PromptOrganizer.Domain.Entities;

namespace PromptOrganizer.Services;

/// <summary>
/// Service interface for advanced search operations with business logic.
/// </summary>
public interface ISearchService
{
  /// <summary>
  /// Performs a comprehensive search across prompts, categories, and tags.
  /// </summary>
  /// <param name="searchTerm">The search term.</param>
  /// <param name="searchType">The type of search to perform.</param>
  /// <returns>A comprehensive search result.</returns>
  Task<SearchResult> SearchAsync(string searchTerm, SearchType searchType = SearchType.All);

  /// <summary>
  /// Searches prompts with advanced filtering options.
  /// </summary>
  /// <param name="criteria">The search criteria.</param>
  /// <returns>A collection of matching prompts.</returns>
  Task<IEnumerable<Prompt>> SearchPromptsAsync(PromptSearchCriteria criteria);

  /// <summary>
  /// Searches categories with advanced filtering options.
  /// </summary>
  /// <param name="criteria">The search criteria.</param>
  /// <returns>A collection of matching categories.</returns>
  Task<IEnumerable<Category>> SearchCategoriesAsync(CategorySearchCriteria criteria);

  /// <summary>
  /// Searches tags with advanced filtering options.
  /// </summary>
  /// <param name="criteria">The search criteria.</param>
  /// <returns>A collection of matching tags.</returns>
  Task<IEnumerable<Tag>> SearchTagsAsync(TagSearchCriteria criteria);

  /// <summary>
  /// Gets search suggestions based on a partial search term.
  /// </summary>
  /// <param name="partialTerm">The partial search term.</param>
  /// <param name="maxSuggestions">The maximum number of suggestions.</param>
  /// <returns>A collection of search suggestions.</returns>
  Task<IEnumerable<string>> GetSearchSuggestionsAsync(string partialTerm, int maxSuggestions = 10);

  /// <summary>
  /// Gets popular search terms.
  /// </summary>
  /// <param name="limit">The maximum number of terms to return.</param>
  /// <returns>A collection of popular search terms.</returns>
  Task<IEnumerable<string>> GetPopularSearchTermsAsync(int limit = 10);

  /// <summary>
  /// Clears the search history.
  /// </summary>
  Task ClearSearchHistoryAsync();

  /// <summary>
  /// Gets recent searches.
  /// </summary>
  /// <param name="limit">The maximum number of recent searches.</param>
  /// <returns>A collection of recent search terms.</returns>
  Task<IEnumerable<string>> GetRecentSearchesAsync(int limit = 10);
}

/// <summary>
/// Represents the type of search to perform.
/// </summary>
public enum SearchType
{
  /// <summary>
  /// Search all entities.
  /// </summary>
  All,

  /// <summary>
  /// Search only prompts.
  /// </summary>
  Prompts,

  /// <summary>
  /// Search only categories.
  /// </summary>
  Categories,

  /// <summary>
  /// Search only tags.
  /// </summary>
  Tags
}

/// <summary>
/// Represents comprehensive search results.
/// </summary>
public class SearchResult
{
  /// <summary>
  /// Gets or sets the search term used.
  /// </summary>
  public string SearchTerm { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the search type.
  /// </summary>
  public SearchType SearchType { get; set; }

  /// <summary>
  /// Gets or sets the matching prompts.
  /// </summary>
  public IEnumerable<Prompt> Prompts { get; set; } = Enumerable.Empty<Prompt>();

  /// <summary>
  /// Gets or sets the matching categories.
  /// </summary>
  public IEnumerable<Category> Categories { get; set; } = Enumerable.Empty<Category>();

  /// <summary>
  /// Gets or sets the matching tags.
  /// </summary>
  public IEnumerable<Tag> Tags { get; set; } = Enumerable.Empty<Tag>();

  /// <summary>
  /// Gets or sets the total count of results.
  /// </summary>
  public int TotalCount { get; set; }

  /// <summary>
  /// Gets or sets the execution time in milliseconds.
  /// </summary>
  public long ExecutionTimeMs { get; set; }

  /// <summary>
  /// Gets or sets whether the search was successful.
  /// </summary>
  public bool IsSuccessful { get; set; }

  /// <summary>
  /// Gets or sets any error messages.
  /// </summary>
  public string? ErrorMessage { get; set; }
}

/// <summary>
/// Represents criteria for searching prompts.
/// </summary>
public class PromptSearchCriteria
{
  /// <summary>
  /// Gets or sets the search term.
  /// </summary>
  public string? SearchTerm { get; set; }

  /// <summary>
  /// Gets or sets the category IDs to filter by.
  /// </summary>
  public IEnumerable<int>? CategoryIds { get; set; }

  /// <summary>
  /// Gets or sets the tags to filter by.
  /// </summary>
  public IEnumerable<string>? Tags { get; set; }

  /// <summary>
  /// Gets or sets whether to include inactive prompts.
  /// </summary>
  public bool IncludeInactive { get; set; }

  /// <summary>
  /// Gets or sets the date range filter.
  /// </summary>
  public DateRange? DateRange { get; set; }

  /// <summary>
  /// Gets or sets the maximum number of results.
  /// </summary>
  public int? MaxResults { get; set; }

  /// <summary>
  /// Gets or sets the sort option.
  /// </summary>
  public PromptSortOption SortBy { get; set; } = PromptSortOption.Title;

  /// <summary>
  /// Gets or sets whether to sort in descending order.
  /// </summary>
  public bool SortDescending { get; set; }
}

/// <summary>
/// Represents criteria for searching categories.
/// </summary>
public class CategorySearchCriteria
{
  /// <summary>
  /// Gets or sets the search term.
  /// </summary>
  public string? SearchTerm { get; set; }

  /// <summary>
  /// Gets or sets whether to include inactive categories.
  /// </summary>
  public bool IncludeInactive { get; set; }

  /// <summary>
  /// Gets or sets the maximum number of results.
  /// </summary>
  public int? MaxResults { get; set; }

  /// <summary>
  /// Gets or sets the sort option.
  /// </summary>
  public CategorySortOption SortBy { get; set; } = CategorySortOption.Name;

  /// <summary>
  /// Gets or sets whether to sort in descending order.
  /// </summary>
  public bool SortDescending { get; set; }
}

/// <summary>
/// Represents criteria for searching tags.
/// </summary>
public class TagSearchCriteria
{
  /// <summary>
  /// Gets or sets the search term.
  /// </summary>
  public string? SearchTerm { get; set; }

  /// <summary>
  /// Gets or sets the color to filter by.
  /// </summary>
  public string? Color { get; set; }

  /// <summary>
  /// Gets or sets whether to include inactive tags.
  /// </summary>
  public bool IncludeInactive { get; set; }

  /// <summary>
  /// Gets or sets the maximum number of results.
  /// </summary>
  public int? MaxResults { get; set; }

  /// <summary>
  /// Gets or sets the sort option.
  /// </summary>
  public TagSortOption SortBy { get; set; } = TagSortOption.Name;

  /// <summary>
  /// Gets or sets whether to sort in descending order.
  /// </summary>
  public bool SortDescending { get; set; }
}

/// <summary>
/// Represents a date range filter.
/// </summary>
public class DateRange
{
  /// <summary>
  /// Gets or sets the start date.
  /// </summary>
  public DateTime FromDate { get; set; }

  /// <summary>
  /// Gets or sets the end date.
  /// </summary>
  public DateTime ToDate { get; set; }
}

/// <summary>
/// Represents sort options for prompts.
/// </summary>
public enum PromptSortOption
{
  /// <summary>
  /// Sort by title.
  /// </summary>
  Title,

  /// <summary>
  /// Sort by creation date.
  /// </summary>
  CreatedDate,

  /// <summary>
  /// Sort by update date.
  /// </summary>
  UpdatedDate,

  /// <summary>
  /// Sort by category.
  /// </summary>
  Category
}

/// <summary>
/// Represents sort options for categories.
/// </summary>
public enum CategorySortOption
{
  /// <summary>
  /// Sort by name.
  /// </summary>
  Name,

  /// <summary>
  /// Sort by creation date.
  /// </summary>
  CreatedDate,

  /// <summary>
  /// Sort by prompt count.
  /// </summary>
  PromptCount
}

/// <summary>
/// Represents sort options for tags.
/// </summary>
public enum TagSortOption
{
  /// <summary>
  /// Sort by name.
  /// </summary>
  Name,

  /// <summary>
  /// Sort by creation date.
  /// </summary>
  CreatedDate,

  /// <summary>
  /// Sort by color.
  /// </summary>
  Color
}