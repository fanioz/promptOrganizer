using PromptOrganizer.Domain.Entities;
using PromptOrganizer.Domain.Repositories;

namespace PromptOrganizer.Services;

/// <summary>
/// Service implementation for advanced search operations with business logic.
/// </summary>
public class SearchService : ISearchService
{
  private readonly IPromptRepository _promptRepository;
  private readonly ICategoryRepository _categoryRepository;
  private readonly ITagRepository _tagRepository;
  private readonly List<string> _searchHistory;
  private readonly List<string> _popularSearches;

  public SearchService(IPromptRepository promptRepository, ICategoryRepository categoryRepository, ITagRepository tagRepository)
  {
    _promptRepository = promptRepository ?? throw new ArgumentNullException(nameof(promptRepository));
    _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
    _tagRepository = tagRepository ?? throw new ArgumentNullException(nameof(tagRepository));
    _searchHistory = new List<string>();
    _popularSearches = new List<string> { "prompt", "template", "code", "design", "workflow" };
  }

  public async Task<SearchResult> SearchAsync(string searchTerm, SearchType searchType = SearchType.All)
  {
    var stopwatch = System.Diagnostics.Stopwatch.StartNew();
    var result = new SearchResult
    {
      SearchTerm = searchTerm,
      SearchType = searchType,
      IsSuccessful = true
    };

    try
    {
      if (string.IsNullOrWhiteSpace(searchTerm))
      {
        result.Prompts = Enumerable.Empty<Prompt>();
        result.Categories = Enumerable.Empty<Category>();
        result.Tags = Enumerable.Empty<Tag>();
        result.TotalCount = 0;
        return result;
      }

      // Add to search history
      AddToSearchHistory(searchTerm);

      // Perform searches based on type
      if (searchType == SearchType.All || searchType == SearchType.Prompts)
      {
        result.Prompts = await _promptRepository.SearchAsync(searchTerm);
      }

      if (searchType == SearchType.All || searchType == SearchType.Categories)
      {
        result.Categories = await _categoryRepository.SearchAsync(searchTerm);
      }

      if (searchType == SearchType.All || searchType == SearchType.Tags)
      {
        result.Tags = await _tagRepository.SearchAsync(searchTerm);
      }

      result.TotalCount = result.Prompts.Count() + result.Categories.Count() + result.Tags.Count();
    }
    catch (Exception ex)
    {
      result.IsSuccessful = false;
      result.ErrorMessage = ex.Message;
      result.Prompts = Enumerable.Empty<Prompt>();
      result.Categories = Enumerable.Empty<Category>();
      result.Tags = Enumerable.Empty<Tag>();
      result.TotalCount = 0;
    }
    finally
    {
      stopwatch.Stop();
      result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
    }

    return result;
  }

  public async Task<IEnumerable<Prompt>> SearchPromptsAsync(PromptSearchCriteria criteria)
  {
    if (criteria == null)
    {
      throw new ArgumentException("Search criteria cannot be null");
    }

    var prompts = new List<Prompt>();

    // If we have a search term, start with that
    if (!string.IsNullOrWhiteSpace(criteria.SearchTerm))
    {
      prompts.AddRange(await _promptRepository.SearchAsync(criteria.SearchTerm));
    }
    else
    {
      // Otherwise, get all active prompts as base
      prompts.AddRange(await _promptRepository.GetActivePromptsAsync());
    }

    // Apply category filter
    if (criteria.CategoryIds?.Any() == true)
    {
      var filteredByCategory = new List<Prompt>();
      foreach (var categoryId in criteria.CategoryIds)
      {
        var categoryPrompts = await _promptRepository.GetByCategoryIdAsync(categoryId);
        filteredByCategory.AddRange(categoryPrompts);
      }
      prompts = prompts.Intersect(filteredByCategory).ToList();
    }

    // Apply tag filter
    if (criteria.Tags?.Any() == true)
    {
      var tagPrompts = await _promptRepository.GetByTagsAsync(criteria.Tags);
      prompts = prompts.Intersect(tagPrompts).ToList();
    }

    // Apply date range filter
    if (criteria.DateRange != null)
    {
      var dateRangePrompts = await _promptRepository.GetByDateRangeAsync(criteria.DateRange.FromDate, criteria.DateRange.ToDate);
      prompts = prompts.Intersect(dateRangePrompts).ToList();
    }

    // Apply active filter
    if (!criteria.IncludeInactive)
    {
      prompts = prompts.Where(p => p.IsActive).ToList();
    }

    // Apply sorting
    prompts = ApplyPromptSorting(prompts, criteria.SortBy, criteria.SortDescending);

    // Apply max results limit
    if (criteria.MaxResults.HasValue)
    {
      prompts = prompts.Take(criteria.MaxResults.Value).ToList();
    }

    return prompts;
  }

  public async Task<IEnumerable<Category>> SearchCategoriesAsync(CategorySearchCriteria criteria)
  {
    if (criteria == null)
    {
      throw new ArgumentException("Search criteria cannot be null");
    }

    var categories = new List<Category>();

    // If we have a search term, start with that
    if (!string.IsNullOrWhiteSpace(criteria.SearchTerm))
    {
      categories.AddRange(await _categoryRepository.SearchAsync(criteria.SearchTerm));
    }
    else
    {
      // Otherwise, get all active categories as base
      categories.AddRange(await _categoryRepository.GetActiveCategoriesAsync());
    }

    // Apply active filter
    if (!criteria.IncludeInactive)
    {
      categories = categories.Where(c => c.IsActive).ToList();
    }

    // Apply sorting
    categories = ApplyCategorySorting(categories, criteria.SortBy, criteria.SortDescending);

    // Apply max results limit
    if (criteria.MaxResults.HasValue)
    {
      categories = categories.Take(criteria.MaxResults.Value).ToList();
    }

    return categories;
  }

  public async Task<IEnumerable<Tag>> SearchTagsAsync(TagSearchCriteria criteria)
  {
    if (criteria == null)
    {
      throw new ArgumentException("Search criteria cannot be null");
    }

    var tags = new List<Tag>();

    // If we have a search term, start with that
    if (!string.IsNullOrWhiteSpace(criteria.SearchTerm))
    {
      tags.AddRange(await _tagRepository.SearchAsync(criteria.SearchTerm));
    }
    else
    {
      // Otherwise, get all active tags as base
      tags.AddRange(await _tagRepository.GetActiveTagsAsync());
    }

    // Apply color filter
    if (!string.IsNullOrWhiteSpace(criteria.Color))
    {
      var colorTags = await _tagRepository.GetByColorAsync(criteria.Color);
      tags = tags.Intersect(colorTags).ToList();
    }

    // Apply active filter
    if (!criteria.IncludeInactive)
    {
      tags = tags.Where(t => t.IsActive).ToList();
    }

    // Apply sorting
    tags = ApplyTagSorting(tags, criteria.SortBy, criteria.SortDescending);

    // Apply max results limit
    if (criteria.MaxResults.HasValue)
    {
      tags = tags.Take(criteria.MaxResults.Value).ToList();
    }

    return tags;
  }

  public async Task<IEnumerable<string>> GetSearchSuggestionsAsync(string partialTerm, int maxSuggestions = 10)
  {
    if (string.IsNullOrWhiteSpace(partialTerm))
    {
      return Enumerable.Empty<string>();
    }

    // Simple implementation - in a real system, this would be more sophisticated
    var suggestions = new List<string>();

    // Get suggestions from recent searches
    var recentSuggestions = _searchHistory
        .Where(term => term.Contains(partialTerm, StringComparison.OrdinalIgnoreCase))
        .Take(maxSuggestions / 2)
        .ToList();

    suggestions.AddRange(recentSuggestions);

    // Get suggestions from popular searches
    var popularSuggestions = _popularSearches
        .Where(term => term.Contains(partialTerm, StringComparison.OrdinalIgnoreCase))
        .Take(maxSuggestions - suggestions.Count)
        .ToList();

    suggestions.AddRange(popularSuggestions);

    return suggestions.Distinct().Take(maxSuggestions);
  }

  public async Task<IEnumerable<string>> GetPopularSearchTermsAsync(int limit = 10)
  {
    return _popularSearches.Take(limit);
  }

  public async Task ClearSearchHistoryAsync()
  {
    _searchHistory.Clear();
  }

  public async Task<IEnumerable<string>> GetRecentSearchesAsync(int limit = 10)
  {
    return _searchHistory.TakeLast(limit).Reverse();
  }

  #region Private Helper Methods

  private void AddToSearchHistory(string searchTerm)
  {
    if (!string.IsNullOrWhiteSpace(searchTerm))
    {
      _searchHistory.Add(searchTerm);

      // Keep only the last 100 searches
      if (_searchHistory.Count > 100)
      {
        _searchHistory.RemoveAt(0);
      }
    }
  }

  private List<Prompt> ApplyPromptSorting(List<Prompt> prompts, PromptSortOption sortBy, bool sortDescending)
  {
    return sortBy switch
    {
      PromptSortOption.Title => sortDescending
          ? prompts.OrderByDescending(p => p.Title).ToList()
          : prompts.OrderBy(p => p.Title).ToList(),
      PromptSortOption.CreatedDate => sortDescending
          ? prompts.OrderByDescending(p => p.CreatedAt).ToList()
          : prompts.OrderBy(p => p.CreatedAt).ToList(),
      PromptSortOption.UpdatedDate => sortDescending
          ? prompts.OrderByDescending(p => p.UpdatedAt).ToList()
          : prompts.OrderBy(p => p.UpdatedAt).ToList(),
      PromptSortOption.Category => sortDescending
          ? prompts.OrderByDescending(p => p.CategoryId).ToList()
          : prompts.OrderBy(p => p.CategoryId).ToList(),
      _ => prompts
    };
  }

  private List<Category> ApplyCategorySorting(List<Category> categories, CategorySortOption sortBy, bool sortDescending)
  {
    return sortBy switch
    {
      CategorySortOption.Name => sortDescending
          ? categories.OrderByDescending(c => c.Name).ToList()
          : categories.OrderBy(c => c.Name).ToList(),
      CategorySortOption.CreatedDate => sortDescending
          ? categories.OrderByDescending(c => c.CreatedAt).ToList()
          : categories.OrderBy(c => c.CreatedAt).ToList(),
      CategorySortOption.PromptCount => sortDescending
          ? categories.OrderByDescending(c => GetCategoryPromptCount(c.Id).Result).ToList()
          : categories.OrderBy(c => GetCategoryPromptCount(c.Id).Result).ToList(),
      _ => categories
    };
  }

  private List<Tag> ApplyTagSorting(List<Tag> tags, TagSortOption sortBy, bool sortDescending)
  {
    return sortBy switch
    {
      TagSortOption.Name => sortDescending
          ? tags.OrderByDescending(t => t.Name).ToList()
          : tags.OrderBy(t => t.Name).ToList(),
      TagSortOption.CreatedDate => sortDescending
          ? tags.OrderByDescending(t => t.CreatedAt).ToList()
          : tags.OrderBy(t => t.CreatedAt).ToList(),
      TagSortOption.Color => sortDescending
          ? tags.OrderByDescending(t => t.Color).ToList()
          : tags.OrderBy(t => t.Color).ToList(),
      _ => tags
    };
  }

  private async Task<int> GetCategoryPromptCount(int categoryId)
  {
    return await _promptRepository.CountByCategoryAsync(categoryId);
  }

  #endregion
}