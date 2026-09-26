using FluentAssertions;
using Moq;
using PromptOrganizer.Domain.Entities;
using PromptOrganizer.Domain.Repositories;
using PromptOrganizer.Services;
using PromptOrganizer.Tests.TestHelpers;
using Xunit;

namespace PromptOrganizer.Tests.Services;

/// <summary>
/// TDD tests for PromptService business logic and validation rules.
/// </summary>
public class PromptServiceTests : BaseTest
{
  private readonly Mock<IPromptRepository> _mockPromptRepository;
  private readonly Mock<ICategoryRepository> _mockCategoryRepository;
  private readonly IPromptService _promptService;

  public PromptServiceTests()
  {
    _mockPromptRepository = new Mock<IPromptRepository>();
    _mockCategoryRepository = new Mock<ICategoryRepository>();
    _promptService = new PromptService(_mockPromptRepository.Object, _mockCategoryRepository.Object);
  }

  #region CreatePromptAsync Tests

  [Fact]
  public async Task CreatePromptAsync_WithValidData_ShouldCreatePrompt()
  {
    // Arrange
    var title = "Test Prompt";
    var content = "This is test content";
    var description = "Test description";
    var categoryId = 1;
    var tags = "test, prompt";

    var expectedPrompt = new Prompt(1, title, content, description, categoryId, tags);
    _mockPromptRepository.Setup(x => x.AddAsync(It.IsAny<Prompt>()))
        .ReturnsAsync(expectedPrompt);
    _mockCategoryRepository.Setup(x => x.GetByIdAsync(categoryId))
        .ReturnsAsync(new Category(categoryId, "Test Category", "Test category description"));

    // Act
    var result = await _promptService.CreatePromptAsync(title, content, description, categoryId, tags);

    // Assert
    result.Should().NotBeNull();
    result.Title.Should().Be(title);
    result.Content.Should().Be(content);
    result.Description.Should().Be(description);
    result.CategoryId.Should().Be(categoryId);
    result.Tags.Should().Be(tags);
    result.IsActive.Should().BeTrue();

    _mockPromptRepository.Verify(x => x.AddAsync(It.IsAny<Prompt>()), Times.Once);
  }

  [Fact]
  public async Task CreatePromptAsync_WithEmptyTitle_ShouldThrowArgumentException()
  {
    // Arrange
    var title = "";
    var content = "This is test content";
    var description = "Test description";
    var categoryId = 1;
    var tags = "test, prompt";

    // Act & Assert
    await Assert.ThrowsAsync<ArgumentException>(() =>
        _promptService.CreatePromptAsync(title, content, description, categoryId, tags));
  }

  [Fact]
  public async Task CreatePromptAsync_WithNullTitle_ShouldThrowArgumentException()
  {
    // Arrange
    string title = null!;
    var content = "This is test content";
    var description = "Test description";
    var categoryId = 1;
    var tags = "test, prompt";

    // Act & Assert
    await Assert.ThrowsAsync<ArgumentException>(() =>
        _promptService.CreatePromptAsync(title, content, description, categoryId, tags));
  }

  [Fact]
  public async Task CreatePromptAsync_WithTitleExceedingMaxLength_ShouldThrowArgumentException()
  {
    // Arrange
    var title = new string('A', 201); // Max length is 200
    var content = "This is test content";
    var description = "Test description";
    var categoryId = 1;
    var tags = "test, prompt";

    // Act & Assert
    await Assert.ThrowsAsync<ArgumentException>(() =>
        _promptService.CreatePromptAsync(title, content, description, categoryId, tags));
  }

  [Fact]
  public async Task CreatePromptAsync_WithEmptyContent_ShouldThrowArgumentException()
  {
    // Arrange
    var title = "Test Prompt";
    var content = "";
    var description = "Test description";
    var categoryId = 1;
    var tags = "test, prompt";

    // Act & Assert
    await Assert.ThrowsAsync<ArgumentException>(() =>
        _promptService.CreatePromptAsync(title, content, description, categoryId, tags));
  }

  [Fact]
  public async Task CreatePromptAsync_WithContentExceedingMaxLength_ShouldThrowArgumentException()
  {
    // Arrange
    var title = "Test Prompt";
    var content = new string('A', 5001); // Max length is 5000
    var description = "Test description";
    var categoryId = 1;
    var tags = "test, prompt";

    // Act & Assert
    await Assert.ThrowsAsync<ArgumentException>(() =>
        _promptService.CreatePromptAsync(title, content, description, categoryId, tags));
  }

  [Fact]
  public async Task CreatePromptAsync_WithNonExistentCategory_ShouldThrowArgumentException()
  {
    // Arrange
    var title = "Test Prompt";
    var content = "This is test content";
    var description = "Test description";
    var categoryId = 999; // Non-existent category
    var tags = "test, prompt";

    _mockCategoryRepository.Setup(x => x.GetByIdAsync(categoryId))
        .ReturnsAsync((Category?)null);

    // Act & Assert
    var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
        _promptService.CreatePromptAsync(title, content, description, categoryId, tags));

    exception.Message.Should().Contain("Category with ID 999 does not exist");
  }

  [Fact]
  public async Task CreatePromptAsync_WithNullCategory_ShouldCreatePrompt()
  {
    // Arrange
    var title = "Test Prompt";
    var content = "This is test content";
    var description = "Test description";
    int? categoryId = null;
    var tags = "test, prompt";

    var expectedPrompt = new Prompt(1, title, content, description, categoryId, tags);
    _mockPromptRepository.Setup(x => x.AddAsync(It.IsAny<Prompt>()))
        .ReturnsAsync(expectedPrompt);

    // Act
    var result = await _promptService.CreatePromptAsync(title, content, description, categoryId, tags);

    // Assert
    result.Should().NotBeNull();
    result.CategoryId.Should().BeNull();
    _mockPromptRepository.Verify(x => x.AddAsync(It.IsAny<Prompt>()), Times.Once);
  }

  #endregion

  #region UpdatePromptAsync Tests

  [Fact]
  public async Task UpdatePromptAsync_WithValidData_ShouldUpdatePrompt()
  {
    // Arrange
    var promptId = 1;
    var title = "Updated Prompt";
    var content = "Updated content";
    var description = "Updated description";
    var categoryId = 2;
    var tags = "updated, prompt";

    var existingPrompt = new Prompt(promptId, "Original Title", "Original Content", "Original Description", 1, "original, tag");
    var updatedPrompt = new Prompt(promptId, title, content, description, categoryId, tags);

    _mockPromptRepository.Setup(x => x.GetByIdAsync(promptId))
        .ReturnsAsync(existingPrompt);
    _mockCategoryRepository.Setup(x => x.GetByIdAsync(categoryId))
        .ReturnsAsync(new Category(categoryId, "Updated Category", "Updated category description"));
    _mockPromptRepository.Setup(x => x.UpdateAsync(It.IsAny<Prompt>()))
        .ReturnsAsync(updatedPrompt);

    // Act
    var result = await _promptService.UpdatePromptAsync(promptId, title, content, description, categoryId, tags);

    // Assert
    result.Should().NotBeNull();
    result.Title.Should().Be(title);
    result.Content.Should().Be(content);
    result.Description.Should().Be(description);
    result.CategoryId.Should().Be(categoryId);
    result.Tags.Should().Be(tags);

    _mockPromptRepository.Verify(x => x.GetByIdAsync(promptId), Times.Once);
    _mockPromptRepository.Verify(x => x.UpdateAsync(It.IsAny<Prompt>()), Times.Once);
  }

  [Fact]
  public async Task UpdatePromptAsync_WithNonExistentPrompt_ShouldThrowKeyNotFoundException()
  {
    // Arrange
    var promptId = 999;
    var title = "Updated Prompt";
    var content = "Updated content";
    var description = "Updated description";
    var categoryId = 2;
    var tags = "updated, prompt";

    _mockPromptRepository.Setup(x => x.GetByIdAsync(promptId))
        .ReturnsAsync((Prompt?)null);

    // Act & Assert
    await Assert.ThrowsAsync<KeyNotFoundException>(() =>
        _promptService.UpdatePromptAsync(promptId, title, content, description, categoryId, tags));
  }

  [Fact]
  public async Task UpdatePromptAsync_WithInvalidData_ShouldThrowArgumentException()
  {
    // Arrange
    var promptId = 1;
    var title = ""; // Invalid title
    var content = "Updated content";
    var description = "Updated description";
    var categoryId = 2;
    var tags = "updated, prompt";

    var existingPrompt = new Prompt(promptId, "Original Title", "Original Content", "Original Description", 1, "original, tag");
    _mockPromptRepository.Setup(x => x.GetByIdAsync(promptId))
        .ReturnsAsync(existingPrompt);

    // Act & Assert
    await Assert.ThrowsAsync<ArgumentException>(() =>
        _promptService.UpdatePromptAsync(promptId, title, content, description, categoryId, tags));
  }

  #endregion

  #region GetPromptByIdAsync Tests

  [Fact]
  public async Task GetPromptByIdAsync_WithExistingPrompt_ShouldReturnPrompt()
  {
    // Arrange
    var promptId = 1;
    var expectedPrompt = new Prompt(promptId, "Test Prompt", "Test Content", "Test Description", 1, "test, tag");

    _mockPromptRepository.Setup(x => x.GetByIdAsync(promptId))
        .ReturnsAsync(expectedPrompt);

    // Act
    var result = await _promptService.GetPromptByIdAsync(promptId);

    // Assert
    result.Should().NotBeNull();
    result.Should().Be(expectedPrompt);
    _mockPromptRepository.Verify(x => x.GetByIdAsync(promptId), Times.Once);
  }

  [Fact]
  public async Task GetPromptByIdAsync_WithNonExistentPrompt_ShouldReturnNull()
  {
    // Arrange
    var promptId = 999;

    _mockPromptRepository.Setup(x => x.GetByIdAsync(promptId))
        .ReturnsAsync((Prompt?)null);

    // Act
    var result = await _promptService.GetPromptByIdAsync(promptId);

    // Assert
    result.Should().BeNull();
    _mockPromptRepository.Verify(x => x.GetByIdAsync(promptId), Times.Once);
  }

  #endregion

  #region GetAllActivePromptsAsync Tests

  [Fact]
  public async Task GetAllActivePromptsAsync_ShouldReturnOnlyActivePrompts()
  {
    // Arrange
    var activePrompts = new List<Prompt>
    {
        CreateActivePrompt(1, "Active 1", "Content 1", "Desc 1", 1, "tag1"),
        CreateActivePrompt(2, "Active 2", "Content 2", "Desc 2", 1, "tag2")
    };

    _mockPromptRepository.Setup(x => x.GetActivePromptsAsync())
        .ReturnsAsync(activePrompts);

    // Act
    var result = await _promptService.GetAllActivePromptsAsync();

    // Assert
    result.Should().NotBeNull();
    result.Should().HaveCount(2);
    result.All(p => p.IsActive).Should().BeTrue();
    _mockPromptRepository.Verify(x => x.GetActivePromptsAsync(), Times.Once);
  }

  [Fact]
  public async Task GetAllActivePromptsAsync_WhenNoActivePrompts_ShouldReturnEmptyCollection()
  {
    // Arrange
    _mockPromptRepository.Setup(x => x.GetActivePromptsAsync())
        .ReturnsAsync(Enumerable.Empty<Prompt>());

    // Act
    var result = await _promptService.GetAllActivePromptsAsync();

    // Assert
    result.Should().NotBeNull();
    result.Should().BeEmpty();
    _mockPromptRepository.Verify(x => x.GetActivePromptsAsync(), Times.Once);
  }

  #endregion

  #region SearchPromptsAsync Tests

  [Fact]
  public async Task SearchPromptsAsync_WithMatchingTerm_ShouldReturnMatchingPrompts()
  {
    // Arrange
    var searchTerm = "test";
    var matchingPrompts = new List<Prompt>
        {
            new Prompt(1, "Test Prompt", "Content with test", "Desc", 1, "tag"),
            new Prompt(2, "Another Test", "Content", "Test description", 1, "tag")
        };

    _mockPromptRepository.Setup(x => x.SearchAsync(searchTerm))
        .ReturnsAsync(matchingPrompts);

    // Act
    var result = await _promptService.SearchPromptsAsync(searchTerm);

    // Assert
    result.Should().NotBeNull();
    result.Should().HaveCount(2);
    _mockPromptRepository.Verify(x => x.SearchAsync(searchTerm), Times.Once);
  }

  private Prompt CreateActivePrompt(int id, string title, string content, string description, int? categoryId, string tags)
  {
    var prompt = new Prompt(id, title, content, description, categoryId, tags);
    // Use reflection to set IsActive since the setter is private
    var isActiveProperty = typeof(Prompt).GetProperty("IsActive");
    if (isActiveProperty != null && isActiveProperty.CanWrite)
    {
      isActiveProperty.SetValue(prompt, true);
    }
    return prompt;
  }

  [Fact]
  public async Task SearchPromptsAsync_WithEmptySearchTerm_ShouldReturnEmptyCollection()
  {
    // Arrange
    var searchTerm = "";

    // Act
    var result = await _promptService.SearchPromptsAsync(searchTerm);

    // Assert
    result.Should().NotBeNull();
    result.Should().BeEmpty();
    _mockPromptRepository.Verify(x => x.SearchAsync(It.IsAny<string>()), Times.Never);
  }

  #endregion

  #region ValidatePromptDataAsync Tests

  [Fact]
  public async Task ValidatePromptDataAsync_WithValidData_ShouldReturnEmptyValidationErrors()
  {
    // Arrange
    var title = "Test Prompt";
    var content = "Test content";
    var description = "Test description";
    var categoryId = 1;
    var tags = "test, prompt";

    _mockCategoryRepository.Setup(x => x.GetByIdAsync(categoryId))
        .ReturnsAsync(new Category(categoryId, "Test Category", "Test description"));

    // Act
    var result = await _promptService.ValidatePromptDataAsync(title, content, description, categoryId, tags);

    // Assert
    result.Should().NotBeNull();
    result.Should().BeEmpty();
  }

  [Fact]
  public async Task ValidatePromptDataAsync_WithInvalidData_ShouldReturnValidationErrors()
  {
    // Arrange
    var title = ""; // Invalid
    var content = ""; // Invalid
    var description = "Test description";
    var categoryId = 999; // Non-existent
    var tags = "test, prompt";

    _mockCategoryRepository.Setup(x => x.GetByIdAsync(categoryId))
        .ReturnsAsync((Category?)null);

    // Act
    var result = await _promptService.ValidatePromptDataAsync(title, content, description, categoryId, tags);

    // Assert
    result.Should().NotBeNull();
    result.Should().Contain("Title cannot be null or empty");
    result.Should().Contain("Content cannot be null or empty");
    result.Should().Contain("Category with ID 999 does not exist");
  }

  #endregion

  #region Business Rule Tests

  [Fact]
  public async Task CreatePromptAsync_ShouldTrimWhitespaceFromTitleAndContent()
  {
    // Arrange
    var title = "  Test Prompt  ";
    var content = "  Test content  ";
    var description = "Test description";
    var categoryId = 1;
    var tags = "test, prompt";

    var expectedPrompt = new Prompt(1, title.Trim(), content.Trim(), description, categoryId, tags);
    _mockPromptRepository.Setup(x => x.AddAsync(It.IsAny<Prompt>()))
        .ReturnsAsync(expectedPrompt);
    _mockCategoryRepository.Setup(x => x.GetByIdAsync(categoryId))
        .ReturnsAsync(new Category(categoryId, "Test Category", "Test category description"));

    // Act
    var result = await _promptService.CreatePromptAsync(title, content, description, categoryId, tags);

    // Assert
    result.Title.Should().Be("Test Prompt");
    result.Content.Should().Be("Test content");
  }

  [Fact]
  public async Task CreatePromptAsync_ShouldSetCreatedAndUpdatedDatesToUtcNow()
  {
    // Arrange
    var title = "Test Prompt";
    var content = "Test content";
    var description = "Test description";
    var categoryId = 1;
    var tags = "test, prompt";

    var beforeCreation = DateTime.UtcNow;

    var expectedPrompt = new Prompt(1, title, content, description, categoryId, tags);
    _mockPromptRepository.Setup(x => x.AddAsync(It.IsAny<Prompt>()))
        .ReturnsAsync(expectedPrompt);
    _mockCategoryRepository.Setup(x => x.GetByIdAsync(categoryId))
        .ReturnsAsync(new Category(categoryId, "Test Category", "Test category description"));

    // Act
    var result = await _promptService.CreatePromptAsync(title, content, description, categoryId, tags);

    // Assert
    result.CreatedAt.Should().BeCloseTo(beforeCreation, TimeSpan.FromSeconds(1));
    result.UpdatedAt.Should().BeCloseTo(beforeCreation, TimeSpan.FromSeconds(1));
  }

  [Fact]
  public async Task UpdatePromptAsync_ShouldUpdateUpdatedAtTimestamp()
  {
    // Arrange
    var promptId = 1;
    var title = "Updated Prompt";
    var content = "Updated content";
    var description = "Updated description";
    var categoryId = 2;
    var tags = "updated, prompt";

    var existingPrompt = new Prompt(promptId, "Original Title", "Original Content", "Original Description", 1, "original, tag");
    var beforeUpdate = DateTime.UtcNow;

    _mockPromptRepository.Setup(x => x.GetByIdAsync(promptId))
        .ReturnsAsync(existingPrompt);
    _mockCategoryRepository.Setup(x => x.GetByIdAsync(categoryId))
        .ReturnsAsync(new Category(categoryId, "Updated Category", "Updated category description"));
    _mockPromptRepository.Setup(x => x.UpdateAsync(It.IsAny<Prompt>()))
        .ReturnsAsync((Prompt p) => p);

    // Act
    var result = await _promptService.UpdatePromptAsync(promptId, title, content, description, categoryId, tags);

    // Assert
    result.UpdatedAt.Should().BeAfter(beforeUpdate);
  }

  #endregion
}