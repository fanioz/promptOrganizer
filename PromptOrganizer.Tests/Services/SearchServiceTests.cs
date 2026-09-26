using FluentAssertions;
using Moq;
using PromptOrganizer.Domain.Entities;
using PromptOrganizer.Domain.Repositories;
using PromptOrganizer.Services;
using PromptOrganizer.Tests.TestHelpers;
using Xunit;

namespace PromptOrganizer.Tests.Services;

/// <summary>
/// TDD tests for SearchService with advanced search algorithms and business logic.
/// </summary>
public class SearchServiceTests : BaseTest
{
  private readonly Mock<IPromptRepository> _mockPromptRepository;
  private readonly Mock<ICategoryRepository> _mockCategoryRepository;
  private readonly Mock<ITagRepository> _mockTagRepository;
  private readonly ISearchService _searchService;

  public SearchServiceTests()
  {
    _mockPromptRepository = new Mock<IPromptRepository>();
    _mockCategoryRepository = new Mock<ICategoryRepository>();
    _mockTagRepository = new Mock<ITagRepository>();
    _searchService = new SearchService(_mockPromptRepository.Object, _mockCategoryRepository.Object, _mockTagRepository.Object);
  }

  #region SearchAsync Tests

  [Fact]
  public async Task SearchAsync_WithValidSearchTerm_ShouldReturnComprehensiveResults()
  {
    // Arrange
    var searchTerm = "test";
    var prompts = new List<Prompt>
        {
            new Prompt(1, "Test Prompt", "Content with test", "Desc", 1, "tag1"),
            new Prompt(2, "Another Test", "Content", "Test description", 1, "tag2")
        };
    var categories = new List<Category>
        {
            new Category(1, "Test Category", "Test category description"),
            new Category(2, "Another Category", "Another description")
        };
    var tags = new List<Tag>
        {
            new Tag(1, "test-tag", "Test tag", "#FF0000"),
            new Tag(2, "another-tag", "Another tag", "#00FF00")
        };

    _mockPromptRepository.Setup(x => x.SearchAsync(searchTerm))
        .ReturnsAsync(prompts);
    _mockCategoryRepository.Setup(x => x.SearchAsync(searchTerm))
        .ReturnsAsync(categories);
    _mockTagRepository.Setup(x => x.SearchAsync(searchTerm))
        .ReturnsAsync(tags);

    // Act
    var result = await _searchService.SearchAsync(searchTerm, SearchType.All);

    // Assert
    result.Should().NotBeNull();
    result.SearchTerm.Should().Be(searchTerm);
    result.SearchType.Should().Be(SearchType.All);
    result.IsSuccessful.Should().BeTrue();
    result.Prompts.Should().HaveCount(2);
    result.Categories.Should().HaveCount(2);
    result.Tags.Should().HaveCount(2);
    result.TotalCount.Should().Be(6);
    result.ErrorMessage.Should().BeNull();
  }

  [Fact]
  public async Task SearchAsync_WithEmptySearchTerm_ShouldReturnEmptyResults()
  {
    // Arrange
    var searchTerm = "";

    // Act
    var result = await _searchService.SearchAsync(searchTerm, SearchType.All);

    // Assert
    result.Should().NotBeNull();
    result.SearchTerm.Should().Be(searchTerm);
    result.SearchType.Should().Be(SearchType.All);
    result.IsSuccessful.Should().BeTrue();
    result.Prompts.Should().BeEmpty();
    result.Categories.Should().BeEmpty();
    result.Tags.Should().BeEmpty();
    result.TotalCount.Should().Be(0);
    result.ErrorMessage.Should().BeNull();
  }

  [Fact]
  public async Task SearchAsync_WithPromptsOnly_ShouldReturnOnlyPrompts()
  {
    // Arrange
    var searchTerm = "test";
    var prompts = new List<Prompt>
        {
            new Prompt(1, "Test Prompt", "Content with test", "Desc", 1, "tag1"),
            new Prompt(2, "Another Test", "Content", "Test description", 1, "tag2")
        };

    _mockPromptRepository.Setup(x => x.SearchAsync(searchTerm))
        .ReturnsAsync(prompts);

    // Act
    var result = await _searchService.SearchAsync(searchTerm, SearchType.Prompts);

    // Assert
    result.Should().NotBeNull();
    result.SearchTerm.Should().Be(searchTerm);
    result.SearchType.Should().Be(SearchType.Prompts);
    result.IsSuccessful.Should().BeTrue();
    result.Prompts.Should().HaveCount(2);
    result.Categories.Should().BeEmpty();
    result.Tags.Should().BeEmpty();
    result.TotalCount.Should().Be(2);
  }

  [Fact]
  public async Task SearchAsync_WithCategoriesOnly_ShouldReturnOnlyCategories()
  {
    // Arrange
    var searchTerm = "test";
    var categories = new List<Category>
        {
            new Category(1, "Test Category", "Test category description"),
            new Category(2, "Another Category", "Another description")
        };

    _mockCategoryRepository.Setup(x => x.SearchAsync(searchTerm))
        .ReturnsAsync(categories);

    // Act
    var result = await _searchService.SearchAsync(searchTerm, SearchType.Categories);

    // Assert
    result.Should().NotBeNull();
    result.SearchTerm.Should().Be(searchTerm);
    result.SearchType.Should().Be(SearchType.Categories);
    result.IsSuccessful.Should().BeTrue();
    result.Prompts.Should().BeEmpty();
    result.Categories.Should().HaveCount(2);
    result.Tags.Should().BeEmpty();
    result.TotalCount.Should().Be(2);
  }

  [Fact]
  public async Task SearchAsync_WithTagsOnly_ShouldReturnOnlyTags()
  {
    // Arrange
    var searchTerm = "test";
    var tags = new List<Tag>
        {
            new Tag(1, "test-tag", "Test tag", "#FF0000"),
            new Tag(2, "another-tag", "Another tag", "#00FF00")
        };

    _mockTagRepository.Setup(x => x.SearchAsync(searchTerm))
        .ReturnsAsync(tags);

    // Act
    var result = await _searchService.SearchAsync(searchTerm, SearchType.Tags);

    // Assert
    result.Should().NotBeNull();
    result.SearchTerm.Should().Be(searchTerm);
    result.SearchType.Should().Be(SearchType.Tags);
    result.IsSuccessful.Should().BeTrue();
    result.Prompts.Should().BeEmpty();
    result.Categories.Should().BeEmpty();
    result.Tags.Should().HaveCount(2);
    result.TotalCount.Should().Be(2);
  }

  #endregion

  #region SearchPromptsAsync Tests

  [Fact]
  public async Task SearchPromptsAsync_WithCriteria_ShouldReturnFilteredPrompts()
  {
    // Arrange
    var criteria = new PromptSearchCriteria
    {
      SearchTerm = "test",
      CategoryIds = new[] { 1, 2 },
      Tags = new[] { "tag1", "tag2" },
      IncludeInactive = false,
      MaxResults = 10,
      SortBy = PromptSortOption.Title,
      SortDescending = false
    };

    var prompts = new List<Prompt>
        {
            new Prompt(1, "Test Prompt", "Content with test", "Desc", 1, "tag1"),
            new Prompt(2, "Another Test", "Content", "Test description", 2, "tag2")
        };

    _mockPromptRepository.Setup(x => x.SearchAsync(criteria.SearchTerm))
        .ReturnsAsync(prompts);
    _mockPromptRepository.Setup(x => x.GetByCategoryIdAsync(It.IsAny<int>()))
        .ReturnsAsync((int id) => prompts.Where(p => p.CategoryId == id));
    _mockPromptRepository.Setup(x => x.GetByTagsAsync(It.IsAny<IEnumerable<string>>()))
        .ReturnsAsync((IEnumerable<string> tags) => prompts.Where(p => tags.Any(tag => p.Tags.Contains(tag))));

    // Act
    var result = await _searchService.SearchPromptsAsync(criteria);

    // Assert
    result.Should().NotBeNull();
    result.Should().HaveCount(2);
  }

  [Fact]
  public async Task SearchPromptsAsync_WithDateRange_ShouldFilterByDate()
  {
    // Arrange
    var fromDate = DateTime.UtcNow.AddDays(-7);
    var toDate = DateTime.UtcNow;

    var criteria = new PromptSearchCriteria
    {
      DateRange = new DateRange { FromDate = fromDate, ToDate = toDate }
    };

    var prompts = new List<Prompt>
        {
            new Prompt(1, "Recent Prompt", "Content", "Desc", 1, "tag1"),
            new Prompt(2, "Old Prompt", "Content", "Desc", 1, "tag2")
        };

    _mockPromptRepository.Setup(x => x.GetByDateRangeAsync(fromDate, toDate))
        .ReturnsAsync(prompts);
    _mockPromptRepository.Setup(x => x.GetActivePromptsAsync())
        .ReturnsAsync(prompts);

    // Act
    var result = await _searchService.SearchPromptsAsync(criteria);

    // Assert
    result.Should().NotBeNull();
    result.Should().HaveCount(2);
    _mockPromptRepository.Verify(x => x.GetByDateRangeAsync(fromDate, toDate), Times.Once);
  }

  #endregion

  #region SearchCategoriesAsync Tests

  [Fact]
  public async Task SearchCategoriesAsync_WithCriteria_ShouldReturnFilteredCategories()
  {
    // Arrange
    var criteria = new CategorySearchCriteria
    {
      SearchTerm = "test",
      IncludeInactive = false,
      MaxResults = 5,
      SortBy = CategorySortOption.Name,
      SortDescending = false
    };

    var categories = new List<Category>
        {
            new Category(1, "Test Category", "Test category description"),
            new Category(2, "Another Category", "Another description")
        };

    _mockCategoryRepository.Setup(x => x.SearchAsync(criteria.SearchTerm))
        .ReturnsAsync(categories);

    // Act
    var result = await _searchService.SearchCategoriesAsync(criteria);

    // Assert
    result.Should().NotBeNull();
    result.Should().HaveCount(2);
  }

  #endregion

  #region SearchTagsAsync Tests

  [Fact]
  public async Task SearchTagsAsync_WithCriteria_ShouldReturnFilteredTags()
  {
    // Arrange
    var criteria = new TagSearchCriteria
    {
      SearchTerm = "test",
      Color = "#FF0000",
      IncludeInactive = false,
      MaxResults = 5,
      SortBy = TagSortOption.Name,
      SortDescending = false
    };

    var tags = new List<Tag>
        {
            new Tag(1, "test-tag", "Test tag", "#FF0000"),
            new Tag(2, "another-tag", "Another tag", "#FF0000")
        };

    _mockTagRepository.Setup(x => x.SearchAsync(criteria.SearchTerm))
        .ReturnsAsync(tags);
    _mockTagRepository.Setup(x => x.GetByColorAsync(criteria.Color))
        .ReturnsAsync(tags);

    // Act
    var result = await _searchService.SearchTagsAsync(criteria);

    // Assert
    result.Should().NotBeNull();
    result.Should().HaveCount(2);
  }

  #endregion

  #region GetSearchSuggestionsAsync Tests

  [Fact]
  public async Task GetSearchSuggestionsAsync_WithPartialTerm_ShouldReturnSuggestions()
  {
    // Arrange
    var partialTerm = "tes";
    var suggestions = new List<string> { "test", "testing", "test prompt" };

    // Mock implementation would return suggestions based on partial term
    // For now, we'll test the basic structure

    // Act
    var result = await _searchService.GetSearchSuggestionsAsync(partialTerm, 10);

    // Assert
    result.Should().NotBeNull();
    result.Should().HaveCountLessOrEqualTo(10);
  }

  [Fact]
  public async Task GetSearchSuggestionsAsync_WithEmptyTerm_ShouldReturnEmptySuggestions()
  {
    // Arrange
    var partialTerm = "";

    // Act
    var result = await _searchService.GetSearchSuggestionsAsync(partialTerm, 10);

    // Assert
    result.Should().NotBeNull();
    result.Should().BeEmpty();
  }

  #endregion

  #region GetPopularSearchTermsAsync Tests

  [Fact]
  public async Task GetPopularSearchTermsAsync_ShouldReturnPopularTerms()
  {
    // Arrange
    var limit = 5;

    // Act
    var result = await _searchService.GetPopularSearchTermsAsync(limit);

    // Assert
    result.Should().NotBeNull();
    result.Should().HaveCountLessOrEqualTo(limit);
  }

  #endregion

  #region GetRecentSearchesAsync Tests

  [Fact]
  public async Task GetRecentSearchesAsync_ShouldReturnRecentSearches()
  {
    // Arrange
    var limit = 10;

    // Act
    var result = await _searchService.GetRecentSearchesAsync(limit);

    // Assert
    result.Should().NotBeNull();
    result.Should().HaveCountLessOrEqualTo(limit);
  }

  #endregion

  #region ClearSearchHistoryAsync Tests

  [Fact]
  public async Task ClearSearchHistoryAsync_ShouldClearHistory()
  {
    // Act
    await _searchService.ClearSearchHistoryAsync();

    // Assert
    // Verify that history is cleared by checking recent searches
    var recentSearches = await _searchService.GetRecentSearchesAsync(10);
    recentSearches.Should().BeEmpty();
  }

  #endregion

  #region Business Rule Tests

  [Fact]
  public async Task SearchAsync_ShouldMeasureExecutionTime()
  {
    // Arrange
    var searchTerm = "test";
    var prompts = new List<Prompt>
      {
          new Prompt(1, "Test Prompt", "Content with test", "Desc", 1, "tag1")
      };

    _mockPromptRepository.Setup(x => x.SearchAsync(searchTerm))
        .ReturnsAsync(prompts);

    // Act
    var result = await _searchService.SearchAsync(searchTerm, SearchType.Prompts);

    // Assert
    result.Should().NotBeNull();
    result.ExecutionTimeMs.Should().BeGreaterOrEqualTo(0); // Allow 0ms for very fast operations
    result.ExecutionTimeMs.Should().BeLessThan(1000); // Should be very fast
  }

  [Fact]
  public async Task SearchAsync_WithException_ShouldReturnErrorResult()
  {
    // Arrange
    var searchTerm = "test";
    var exception = new Exception("Database error");

    _mockPromptRepository.Setup(x => x.SearchAsync(searchTerm))
        .ThrowsAsync(exception);

    // Act
    var result = await _searchService.SearchAsync(searchTerm, SearchType.Prompts);

    // Assert
    result.Should().NotBeNull();
    result.IsSuccessful.Should().BeFalse();
    result.ErrorMessage.Should().Contain("Database error");
    result.Prompts.Should().BeEmpty();
  }

  [Fact]
  public async Task SearchPromptsAsync_WithNullCriteria_ShouldThrowArgumentException()
  {
    // Arrange
    PromptSearchCriteria? criteria = null;

    // Act & Assert
    await Assert.ThrowsAsync<ArgumentException>(() =>
        _searchService.SearchPromptsAsync(criteria!));
  }

  [Fact]
  public async Task SearchCategoriesAsync_WithNullCriteria_ShouldThrowArgumentException()
  {
    // Arrange
    CategorySearchCriteria? criteria = null;

    // Act & Assert
    await Assert.ThrowsAsync<ArgumentException>(() =>
        _searchService.SearchCategoriesAsync(criteria!));
  }

  [Fact]
  public async Task SearchTagsAsync_WithNullCriteria_ShouldThrowArgumentException()
  {
    // Arrange
    TagSearchCriteria? criteria = null;

    // Act & Assert
    await Assert.ThrowsAsync<ArgumentException>(() =>
        _searchService.SearchTagsAsync(criteria!));
  }

  #endregion
}