using FluentAssertions;
using Moq;
using PromptOrganizer.Domain.Entities;
using PromptOrganizer.Domain.Repositories;
using PromptOrganizer.Services;
using PromptOrganizer.Tests.TestHelpers;
using Xunit;

namespace PromptOrganizer.Tests.Services;

/// <summary>
/// TDD tests for TagService business logic and management operations.
/// </summary>
public class TagServiceTests : BaseTest
{
  private readonly Mock<ITagRepository> _mockTagRepository;
  private readonly Mock<IPromptRepository> _mockPromptRepository;
  private readonly ITagService _tagService;

  public TagServiceTests()
  {
    _mockTagRepository = new Mock<ITagRepository>();
    _mockPromptRepository = new Mock<IPromptRepository>();
    _tagService = new TagService(_mockTagRepository.Object, _mockPromptRepository.Object);
  }

  #region CreateTagAsync Tests

  [Fact]
  public async Task CreateTagAsync_WithValidData_ShouldCreateTag()
  {
    // Arrange
    var name = "Test Tag";
    var description = "Test description";
    var color = "#FF0000";

    var expectedTag = new Tag(1, name, description, color);
    _mockTagRepository.Setup(x => x.AddAsync(It.IsAny<Tag>()))
        .ReturnsAsync(expectedTag);

    // Act
    var result = await _tagService.CreateTagAsync(name, description, color);

    // Assert
    result.Should().NotBeNull();
    result.Name.Should().Be(name);
    result.Description.Should().Be(description);
    result.Color.Should().Be(color);
    result.IsActive.Should().BeTrue();

    _mockTagRepository.Verify(x => x.AddAsync(It.IsAny<Tag>()), Times.Once);
  }

  [Fact]
  public async Task CreateTagAsync_WithEmptyName_ShouldThrowArgumentException()
  {
    // Arrange
    var name = "";
    var description = "Test description";
    var color = "#FF0000";

    // Act & Assert
    await Assert.ThrowsAsync<ArgumentException>(() =>
        _tagService.CreateTagAsync(name, description, color));
  }

  [Fact]
  public async Task CreateTagAsync_WithNullName_ShouldThrowArgumentException()
  {
    // Arrange
    string name = null!;
    var description = "Test description";
    var color = "#FF0000";

    // Act & Assert
    await Assert.ThrowsAsync<ArgumentException>(() =>
        _tagService.CreateTagAsync(name, description, color));
  }

  [Fact]
  public async Task CreateTagAsync_WithNameExceedingMaxLength_ShouldThrowArgumentException()
  {
    // Arrange
    var name = new string('A', 51); // Max length is 50
    var description = "Test description";
    var color = "#FF0000";

    // Act & Assert
    await Assert.ThrowsAsync<ArgumentException>(() =>
        _tagService.CreateTagAsync(name, description, color));
  }

  [Fact]
  public async Task CreateTagAsync_WithDuplicateName_ShouldThrowInvalidOperationException()
  {
    // Arrange
    var name = "Existing Tag";
    var description = "Test description";
    var color = "#FF0000";

    _mockTagRepository.Setup(x => x.ExistsByNameAsync(name))
        .ReturnsAsync(true);

    // Act & Assert
    await Assert.ThrowsAsync<InvalidOperationException>(() =>
        _tagService.CreateTagAsync(name, description, color));
  }

  [Fact]
  public async Task CreateTagAsync_WithInvalidColor_ShouldThrowArgumentException()
  {
    // Arrange
    var name = "Test Tag";
    var description = "Test description";
    var color = "invalid-color";

    // Act & Assert
    await Assert.ThrowsAsync<ArgumentException>(() =>
        _tagService.CreateTagAsync(name, description, color));
  }

  #endregion

  #region UpdateTagAsync Tests

  [Fact]
  public async Task UpdateTagAsync_WithValidData_ShouldUpdateTag()
  {
    // Arrange
    var tagId = 1;
    var name = "Updated Tag";
    var description = "Updated description";
    var color = "#00FF00";

    var existingTag = new Tag(tagId, "Original Name", "Original Description", "#FF0000");
    var updatedTag = new Tag(tagId, name, description, color);

    _mockTagRepository.Setup(x => x.GetByIdAsync(tagId))
        .ReturnsAsync(existingTag);
    _mockTagRepository.Setup(x => x.UpdateAsync(It.IsAny<Tag>()))
        .ReturnsAsync(updatedTag);
    _mockTagRepository.Setup(x => x.ExistsByNameAsync(name))
        .ReturnsAsync(false);

    // Act
    var result = await _tagService.UpdateTagAsync(tagId, name, description, color);

    // Assert
    result.Should().NotBeNull();
    result.Name.Should().Be(name);
    result.Description.Should().Be(description);
    result.Color.Should().Be(color);

    _mockTagRepository.Verify(x => x.GetByIdAsync(tagId), Times.Once);
    _mockTagRepository.Verify(x => x.UpdateAsync(It.IsAny<Tag>()), Times.Once);
  }

  [Fact]
  public async Task UpdateTagAsync_WithNonExistentTag_ShouldThrowKeyNotFoundException()
  {
    // Arrange
    var tagId = 999;
    var name = "Updated Tag";
    var description = "Updated description";
    var color = "#00FF00";

    _mockTagRepository.Setup(x => x.GetByIdAsync(tagId))
        .ReturnsAsync((Tag?)null);

    // Act & Assert
    await Assert.ThrowsAsync<KeyNotFoundException>(() =>
        _tagService.UpdateTagAsync(tagId, name, description, color));
  }

  [Fact]
  public async Task UpdateTagAsync_WithInvalidData_ShouldThrowArgumentException()
  {
    // Arrange
    var tagId = 1;
    var name = ""; // Invalid name
    var description = "Updated description";
    var color = "#00FF00";

    var existingTag = new Tag(tagId, "Original Name", "Original Description", "#FF0000");
    _mockTagRepository.Setup(x => x.GetByIdAsync(tagId))
        .ReturnsAsync(existingTag);

    // Act & Assert
    await Assert.ThrowsAsync<ArgumentException>(() =>
        _tagService.UpdateTagAsync(tagId, name, description, color));
  }

  #endregion

  #region GetTagByIdAsync Tests

  [Fact]
  public async Task GetTagByIdAsync_WithExistingTag_ShouldReturnTag()
  {
    // Arrange
    var tagId = 1;
    var expectedTag = new Tag(tagId, "Test Tag", "Test description", "#FF0000");

    _mockTagRepository.Setup(x => x.GetByIdAsync(tagId))
        .ReturnsAsync(expectedTag);

    // Act
    var result = await _tagService.GetTagByIdAsync(tagId);

    // Assert
    result.Should().NotBeNull();
    result.Should().Be(expectedTag);
    _mockTagRepository.Verify(x => x.GetByIdAsync(tagId), Times.Once);
  }

  [Fact]
  public async Task GetTagByIdAsync_WithNonExistentTag_ShouldReturnNull()
  {
    // Arrange
    var tagId = 999;

    _mockTagRepository.Setup(x => x.GetByIdAsync(tagId))
        .ReturnsAsync((Tag?)null);

    // Act
    var result = await _tagService.GetTagByIdAsync(tagId);

    // Assert
    result.Should().BeNull();
    _mockTagRepository.Verify(x => x.GetByIdAsync(tagId), Times.Once);
  }

  #endregion

  #region GetTagByNameAsync Tests

  [Fact]
  public async Task GetTagByNameAsync_WithExistingTag_ShouldReturnTag()
  {
    // Arrange
    var tagName = "Test Tag";
    var expectedTag = new Tag(1, tagName, "Test description", "#FF0000");

    _mockTagRepository.Setup(x => x.GetByNameAsync(tagName))
        .ReturnsAsync(expectedTag);

    // Act
    var result = await _tagService.GetTagByNameAsync(tagName);

    // Assert
    result.Should().NotBeNull();
    result.Should().Be(expectedTag);
    _mockTagRepository.Verify(x => x.GetByNameAsync(tagName), Times.Once);
  }

  [Fact]
  public async Task GetTagByNameAsync_WithNonExistentTag_ShouldReturnNull()
  {
    // Arrange
    var tagName = "NonExistent";

    _mockTagRepository.Setup(x => x.GetByNameAsync(tagName))
        .ReturnsAsync((Tag?)null);

    // Act
    var result = await _tagService.GetTagByNameAsync(tagName);

    // Assert
    result.Should().BeNull();
    _mockTagRepository.Verify(x => x.GetByNameAsync(tagName), Times.Once);
  }

  #endregion

  #region GetAllActiveTagsAsync Tests

  [Fact]
  public async Task GetAllActiveTagsAsync_ShouldReturnOnlyActiveTags()
  {
    // Arrange
    var activeTags = new List<Tag>
    {
        CreateActiveTag(1, "Active 1", "Desc 1", "#FF0000"),
        CreateActiveTag(2, "Active 2", "Desc 2", "#00FF00")
    };

    _mockTagRepository.Setup(x => x.GetActiveTagsAsync())
        .ReturnsAsync(activeTags);

    // Act
    var result = await _tagService.GetAllActiveTagsAsync();

    // Assert
    result.Should().NotBeNull();
    result.Should().HaveCount(2);
    result.All(t => t.IsActive).Should().BeTrue();
    _mockTagRepository.Verify(x => x.GetActiveTagsAsync(), Times.Once);
  }

  [Fact]
  public async Task GetAllActiveTagsAsync_WhenNoActiveTags_ShouldReturnEmptyCollection()
  {
    // Arrange
    _mockTagRepository.Setup(x => x.GetActiveTagsAsync())
        .ReturnsAsync(Enumerable.Empty<Tag>());

    // Act
    var result = await _tagService.GetAllActiveTagsAsync();

    // Assert
    result.Should().NotBeNull();
    result.Should().BeEmpty();
    _mockTagRepository.Verify(x => x.GetActiveTagsAsync(), Times.Once);
  }

  #endregion

  #region GetTagsByColorAsync Tests

  [Fact]
  public async Task GetTagsByColorAsync_WithExistingColor_ShouldReturnMatchingTags()
  {
    // Arrange
    var color = "#FF0000";
    var matchingTags = new List<Tag>
    {
        new Tag(1, "Red Tag 1", "Desc 1", color),
        new Tag(2, "Red Tag 2", "Desc 2", color)
    };

    _mockTagRepository.Setup(x => x.GetByColorAsync(color))
        .ReturnsAsync(matchingTags);

    // Act
    var result = await _tagService.GetTagsByColorAsync(color);

    // Assert
    result.Should().NotBeNull();
    result.Should().HaveCount(2);
    result.All(t => t.Color == color).Should().BeTrue();
    _mockTagRepository.Verify(x => x.GetByColorAsync(color), Times.Once);
  }

  #endregion

  #region SearchTagsAsync Tests

  [Fact]
  public async Task SearchTagsAsync_WithMatchingTerm_ShouldReturnMatchingTags()
  {
    // Arrange
    var searchTerm = "test";
    var matchingTags = new List<Tag>
    {
        new Tag(1, "Test Tag", "Desc with test", "#FF0000"),
        new Tag(2, "Another Test", "Desc", "#00FF00")
    };

    _mockTagRepository.Setup(x => x.SearchAsync(searchTerm))
        .ReturnsAsync(matchingTags);

    // Act
    var result = await _tagService.SearchTagsAsync(searchTerm);

    // Assert
    result.Should().NotBeNull();
    result.Should().HaveCount(2);
    _mockTagRepository.Verify(x => x.SearchAsync(searchTerm), Times.Once);
  }

  [Fact]
  public async Task SearchTagsAsync_WithEmptySearchTerm_ShouldReturnEmptyCollection()
  {
    // Arrange
    var searchTerm = "";

    // Act
    var result = await _tagService.SearchTagsAsync(searchTerm);

    // Assert
    result.Should().NotBeNull();
    result.Should().BeEmpty();
    _mockTagRepository.Verify(x => x.SearchAsync(It.IsAny<string>()), Times.Never);
  }

  #endregion

  #region DeactivateTagAsync Tests

  [Fact]
  public async Task DeactivateTagAsync_WithExistingTag_ShouldDeactivateAndReturnTrue()
  {
    // Arrange
    var tagId = 1;
    var tag = new Tag(tagId, "Test Tag", "Test description", "#FF0000");

    _mockTagRepository.Setup(x => x.GetByIdAsync(tagId))
        .ReturnsAsync(tag);
    _mockTagRepository.Setup(x => x.UpdateAsync(It.IsAny<Tag>()))
        .ReturnsAsync(tag);
    _mockPromptRepository.Setup(x => x.GetByTagsAsync(new[] { tag.Name }))
        .ReturnsAsync(Enumerable.Empty<Prompt>());

    // Act
    var result = await _tagService.DeactivateTagAsync(tagId);

    // Assert
    result.Should().BeTrue();
    tag.IsActive.Should().BeFalse();
    _mockTagRepository.Verify(x => x.GetByIdAsync(tagId), Times.Once);
    _mockTagRepository.Verify(x => x.UpdateAsync(It.IsAny<Tag>()), Times.Once);
  }

  [Fact]
  public async Task DeactivateTagAsync_WithNonExistentTag_ShouldReturnFalse()
  {
    // Arrange
    var tagId = 999;

    _mockTagRepository.Setup(x => x.GetByIdAsync(tagId))
        .ReturnsAsync((Tag?)null);

    // Act
    var result = await _tagService.DeactivateTagAsync(tagId);

    // Assert
    result.Should().BeFalse();
    _mockTagRepository.Verify(x => x.GetByIdAsync(tagId), Times.Once);
    _mockTagRepository.Verify(x => x.UpdateAsync(It.IsAny<Tag>()), Times.Never);
  }

  [Fact]
  public async Task DeactivateTagAsync_WithPrompts_ShouldThrowInvalidOperationException()
  {
    // Arrange
    var tagId = 1;
    var tag = new Tag(tagId, "Test Tag", "Test description", "#FF0000");
    var promptsWithTag = new List<Prompt>
    {
        new Prompt(1, "Prompt 1", "Content 1", "Description 1", 1, "Test Tag")
    };

    _mockTagRepository.Setup(x => x.GetByIdAsync(tagId))
        .ReturnsAsync(tag);
    _mockPromptRepository.Setup(x => x.GetByTagsAsync(new[] { tag.Name }))
        .ReturnsAsync(promptsWithTag);

    // Act & Assert
    await Assert.ThrowsAsync<InvalidOperationException>(() =>
        _tagService.DeactivateTagAsync(tagId));
  }

  #endregion

  #region ActivateTagAsync Tests

  [Fact]
  public async Task ActivateTagAsync_WithExistingTag_ShouldActivateAndReturnTrue()
  {
    // Arrange
    var tagId = 1;
    var tag = CreateInactiveTag(tagId, "Test Tag", "Test description", "#FF0000");

    _mockTagRepository.Setup(x => x.GetByIdAsync(tagId))
        .ReturnsAsync(tag);
    _mockTagRepository.Setup(x => x.UpdateAsync(It.IsAny<Tag>()))
        .ReturnsAsync(tag);

    // Act
    var result = await _tagService.ActivateTagAsync(tagId);

    // Assert
    result.Should().BeTrue();
    tag.IsActive.Should().BeTrue();
    _mockTagRepository.Verify(x => x.GetByIdAsync(tagId), Times.Once);
    _mockTagRepository.Verify(x => x.UpdateAsync(It.IsAny<Tag>()), Times.Once);
  }

  [Fact]
  public async Task ActivateTagAsync_WithNonExistentTag_ShouldReturnFalse()
  {
    // Arrange
    var tagId = 999;

    _mockTagRepository.Setup(x => x.GetByIdAsync(tagId))
        .ReturnsAsync((Tag?)null);

    // Act
    var result = await _tagService.ActivateTagAsync(tagId);

    // Assert
    result.Should().BeFalse();
    _mockTagRepository.Verify(x => x.GetByIdAsync(tagId), Times.Once);
    _mockTagRepository.Verify(x => x.UpdateAsync(It.IsAny<Tag>()), Times.Never);
  }

  #endregion

  #region TagNameExistsAsync Tests

  [Fact]
  public async Task TagNameExistsAsync_WithExistingName_ShouldReturnTrue()
  {
    // Arrange
    var tagName = "Existing Tag";

    _mockTagRepository.Setup(x => x.ExistsByNameAsync(tagName))
        .ReturnsAsync(true);

    // Act
    var result = await _tagService.TagNameExistsAsync(tagName);

    // Assert
    result.Should().BeTrue();
    _mockTagRepository.Verify(x => x.ExistsByNameAsync(tagName), Times.Once);
  }

  [Fact]
  public async Task TagNameExistsAsync_WithNonExistingName_ShouldReturnFalse()
  {
    // Arrange
    var tagName = "New Tag";

    _mockTagRepository.Setup(x => x.ExistsByNameAsync(tagName))
        .ReturnsAsync(false);

    // Act
    var result = await _tagService.TagNameExistsAsync(tagName);

    // Assert
    result.Should().BeFalse();
    _mockTagRepository.Verify(x => x.ExistsByNameAsync(tagName), Times.Once);
  }

  [Fact]
  public async Task TagNameExistsAsync_WithExistingNameButDifferentId_ShouldReturnTrue()
  {
    // Arrange
    var tagName = "Existing Tag";
    var excludeId = 1;

    _mockTagRepository.Setup(x => x.GetByNameAsync(tagName))
        .ReturnsAsync(new Tag(2, tagName, "Description", "#FF0000"));

    // Act
    var result = await _tagService.TagNameExistsAsync(tagName, excludeId);

    // Assert
    result.Should().BeTrue();
    _mockTagRepository.Verify(x => x.GetByNameAsync(tagName), Times.Once);
  }

  [Fact]
  public async Task TagNameExistsAsync_WithSameNameAndId_ShouldReturnFalse()
  {
    // Arrange
    var tagName = "Existing Tag";
    var excludeId = 1;

    _mockTagRepository.Setup(x => x.GetByNameAsync(tagName))
        .ReturnsAsync(new Tag(excludeId, tagName, "Description", "#FF0000"));

    // Act
    var result = await _tagService.TagNameExistsAsync(tagName, excludeId);

    // Assert
    result.Should().BeFalse();
    _mockTagRepository.Verify(x => x.GetByNameAsync(tagName), Times.Once);
  }

  #endregion

  #region ValidateTagDataAsync Tests

  [Fact]
  public async Task ValidateTagDataAsync_WithValidData_ShouldReturnEmptyValidationErrors()
  {
    // Arrange
    var name = "Test Tag";
    var description = "Test description";
    var color = "#FF0000";

    _mockTagRepository.Setup(x => x.ExistsByNameAsync(name))
        .ReturnsAsync(false);

    // Act
    var result = await _tagService.ValidateTagDataAsync(name, description, color);

    // Assert
    result.Should().NotBeNull();
    result.Should().BeEmpty();
  }

  [Fact]
  public async Task ValidateTagDataAsync_WithInvalidData_ShouldReturnValidationErrors()
  {
    // Arrange
    var name = ""; // Invalid
    var description = new string('A', 201); // Too long
    var color = "invalid-color";

    // Act
    var result = await _tagService.ValidateTagDataAsync(name, description, color);

    // Assert
    result.Should().NotBeNull();
    result.Should().Contain("Name cannot be null or empty");
    result.Should().Contain("Description cannot exceed 200 characters");
    result.Should().Contain("Color must be in hex format (#RRGGBB)");
  }

  [Fact]
  public async Task ValidateTagDataAsync_WithDuplicateName_ShouldReturnValidationError()
  {
    // Arrange
    var name = "Existing Tag";
    var description = "Test description";
    var color = "#FF0000";

    _mockTagRepository.Setup(x => x.ExistsByNameAsync(name))
        .ReturnsAsync(true);

    // Act
    var result = await _tagService.ValidateTagDataAsync(name, description, color);

    // Assert
    result.Should().NotBeNull();
    result.Should().Contain("Tag name already exists");
  }

  #endregion

  #region GetDeletableTagsAsync Tests

  [Fact]
  public async Task GetDeletableTagsAsync_ShouldReturnTagsWithNoPrompts()
  {
    // Arrange
    var tags = new List<Tag>
    {
        new Tag(1, "Empty Tag", "No prompts", "#FF0000"),
        new Tag(2, "Used Tag", "Has prompts", "#00FF00")
    };

    _mockTagRepository.Setup(x => x.GetAllAsync())
        .ReturnsAsync(tags);
    _mockTagRepository.Setup(x => x.GetByIdAsync(1))
        .ReturnsAsync(tags[0]);
    _mockTagRepository.Setup(x => x.GetByIdAsync(2))
        .ReturnsAsync(tags[1]);
    _mockPromptRepository.Setup(x => x.GetByTagsAsync(It.Is<string[]>(names => names.Contains("Empty Tag"))))
        .ReturnsAsync(Enumerable.Empty<Prompt>());
    _mockPromptRepository.Setup(x => x.GetByTagsAsync(It.Is<string[]>(names => names.Contains("Used Tag"))))
        .ReturnsAsync(new List<Prompt> { new Prompt(1, "Prompt", "Content", "Description", 1, "Used Tag") });

    // Act
    var result = await _tagService.GetDeletableTagsAsync();

    // Assert
    result.Should().NotBeNull();
    result.Should().HaveCount(1);
    result.First().Id.Should().Be(1);
  }

  #endregion

  #region CanDeleteTagAsync Tests

  [Fact]
  public async Task CanDeleteTagAsync_WithNoPrompts_ShouldReturnTrue()
  {
    // Arrange
    var tagId = 1;
    var tag = new Tag(tagId, "Empty Tag", "No prompts", "#FF0000");

    _mockTagRepository.Setup(x => x.GetByIdAsync(tagId))
        .ReturnsAsync(tag);
    _mockPromptRepository.Setup(x => x.GetByTagsAsync(new[] { tag.Name }))
        .ReturnsAsync(Enumerable.Empty<Prompt>());

    // Act
    var result = await _tagService.CanDeleteTagAsync(tagId);

    // Assert
    result.Should().BeTrue();
    _mockPromptRepository.Verify(x => x.GetByTagsAsync(new[] { tag.Name }), Times.Once);
  }

  [Fact]
  public async Task CanDeleteTagAsync_WithPrompts_ShouldReturnFalse()
  {
    // Arrange
    var tagId = 1;
    var tag = new Tag(tagId, "Used Tag", "Has prompts", "#FF0000");

    _mockTagRepository.Setup(x => x.GetByIdAsync(tagId))
        .ReturnsAsync(tag);
    _mockPromptRepository.Setup(x => x.GetByTagsAsync(new[] { tag.Name }))
        .ReturnsAsync(new List<Prompt> { new Prompt(1, "Prompt", "Content", "Description", 1, "Used Tag") });

    // Act
    var result = await _tagService.CanDeleteTagAsync(tagId);

    // Assert
    result.Should().BeFalse();
    _mockPromptRepository.Verify(x => x.GetByTagsAsync(new[] { tag.Name }), Times.Once);
  }

  #endregion

  #region GetTotalTagCountAsync Tests

  [Fact]
  public async Task GetTotalTagCountAsync_ShouldReturnTotalCount()
  {
    // Arrange
    var tags = new List<Tag>
    {
        new Tag(1, "Tag 1", "Desc 1", "#FF0000"),
        new Tag(2, "Tag 2", "Desc 2", "#00FF00"),
        new Tag(3, "Tag 3", "Desc 3", "#0000FF")
    };

    _mockTagRepository.Setup(x => x.GetAllAsync())
        .ReturnsAsync(tags);

    // Act
    var result = await _tagService.GetTotalTagCountAsync();

    // Assert
    result.Should().Be(3);
    _mockTagRepository.Verify(x => x.GetAllAsync(), Times.Once);
  }

  #endregion

  #region GetActiveTagCountAsync Tests

  [Fact]
  public async Task GetActiveTagCountAsync_ShouldReturnActiveCount()
  {
    // Arrange
    var activeTags = new List<Tag>
    {
        CreateActiveTag(1, "Active 1", "Desc 1", "#FF0000"),
        CreateActiveTag(2, "Active 2", "Desc 2", "#00FF00")
    };

    _mockTagRepository.Setup(x => x.GetActiveTagsAsync())
        .ReturnsAsync(activeTags);

    // Act
    var result = await _tagService.GetActiveTagCountAsync();

    // Assert
    result.Should().Be(2);
    _mockTagRepository.Verify(x => x.GetActiveTagsAsync(), Times.Once);
  }

  #endregion

  #region GetTagsByNamesAsync Tests

  [Fact]
  public async Task GetTagsByNamesAsync_WithExistingNames_ShouldReturnMatchingTags()
  {
    // Arrange
    var names = new[] { "Tag 1", "Tag 2" };
    var matchingTags = new List<Tag>
    {
        new Tag(1, "Tag 1", "Desc 1", "#FF0000"),
        new Tag(2, "Tag 2", "Desc 2", "#00FF00")
    };

    _mockTagRepository.Setup(x => x.GetByNamesAsync(names))
        .ReturnsAsync(matchingTags);

    // Act
    var result = await _tagService.GetTagsByNamesAsync(names);

    // Assert
    result.Should().NotBeNull();
    result.Should().HaveCount(2);
    _mockTagRepository.Verify(x => x.GetByNamesAsync(names), Times.Once);
  }

  #endregion

  #region GetPopularTagsAsync Tests

  [Fact]
  public async Task GetPopularTagsAsync_ShouldReturnLimitedActiveTags()
  {
    // Arrange
    var allTags = new List<Tag>
    {
        CreateActiveTag(1, "Alpha", "Desc 1", "#FF0000"),
        CreateActiveTag(2, "Beta", "Desc 2", "#00FF00"),
        CreateActiveTag(3, "Gamma", "Desc 3", "#0000FF"),
        CreateInactiveTag(4, "Delta", "Desc 4", "#FFFF00")
    };

    _mockTagRepository.Setup(x => x.GetAllAsync())
        .ReturnsAsync(allTags);

    // Act
    var result = await _tagService.GetPopularTagsAsync(2);

    // Assert
    result.Should().NotBeNull();
    result.Should().HaveCount(2);
    result.All(t => t.IsActive).Should().BeTrue();
    result.Select(t => t.Name).Should().BeInAscendingOrder();
  }

  #endregion

  #region GetSimilarTagsAsync Tests

  [Fact]
  public async Task GetSimilarTagsAsync_WithExistingTag_ShouldReturnSimilarTags()
  {
    // Arrange
    var tagId = 1;
    var sourceTag = new Tag(tagId, "Source Tag", "Description", "#FF0000");
    var similarTags = new List<Tag>
    {
        new Tag(2, "Similar 1", "Desc 1", "#FF0000"),
        new Tag(3, "Similar 2", "Desc 2", "#FF0000")
    };

    _mockTagRepository.Setup(x => x.GetByIdAsync(tagId))
        .ReturnsAsync(sourceTag);
    _mockTagRepository.Setup(x => x.GetByColorAsync("#FF0000"))
        .ReturnsAsync(similarTags);

    // Act
    var result = await _tagService.GetSimilarTagsAsync(tagId, 2);

    // Assert
    result.Should().NotBeNull();
    result.Should().HaveCount(2);
    result.All(t => t.Color == "#FF0000").Should().BeTrue();
    result.All(t => t.Id != tagId).Should().BeTrue();
  }

  [Fact]
  public async Task GetSimilarTagsAsync_WithNonExistentTag_ShouldReturnEmptyCollection()
  {
    // Arrange
    var tagId = 999;

    _mockTagRepository.Setup(x => x.GetByIdAsync(tagId))
        .ReturnsAsync((Tag?)null);

    // Act
    var result = await _tagService.GetSimilarTagsAsync(tagId);

    // Assert
    result.Should().NotBeNull();
    result.Should().BeEmpty();
  }

  #endregion

  #region Business Rule Tests

  [Fact]
  public async Task CreateTagAsync_ShouldTrimWhitespaceFromNameAndDescription()
  {
    // Arrange
    var name = "  Test Tag  ";
    var description = "  Test description  ";
    var color = "#FF0000";

    var expectedTag = new Tag(1, name.Trim(), description.Trim(), color);
    _mockTagRepository.Setup(x => x.AddAsync(It.IsAny<Tag>()))
        .ReturnsAsync(expectedTag);
    _mockTagRepository.Setup(x => x.ExistsByNameAsync(name.Trim()))
        .ReturnsAsync(false);

    // Act
    var result = await _tagService.CreateTagAsync(name, description, color);

    // Assert
    result.Name.Should().Be("Test Tag");
    result.Description.Should().Be("Test description");
  }

  [Fact]
  public async Task CreateTagAsync_ShouldSetCreatedAndUpdatedDatesToUtcNow()
  {
    // Arrange
    var name = "Test Tag";
    var description = "Test description";
    var color = "#FF0000";
    var beforeCreation = DateTime.UtcNow;

    var expectedTag = new Tag(1, name, description, color);
    _mockTagRepository.Setup(x => x.AddAsync(It.IsAny<Tag>()))
        .ReturnsAsync(expectedTag);
    _mockTagRepository.Setup(x => x.ExistsByNameAsync(name))
        .ReturnsAsync(false);

    // Act
    var result = await _tagService.CreateTagAsync(name, description, color);

    // Assert
    result.CreatedAt.Should().BeCloseTo(beforeCreation, TimeSpan.FromSeconds(1));
    result.UpdatedAt.Should().BeCloseTo(beforeCreation, TimeSpan.FromSeconds(1));
  }

  [Fact]
  public async Task UpdateTagAsync_ShouldUpdateUpdatedAtTimestamp()
  {
    // Arrange
    var tagId = 1;
    var name = "Updated Tag";
    var description = "Updated description";
    var color = "#00FF00";
    var beforeUpdate = DateTime.UtcNow;

    var existingTag = new Tag(tagId, "Original Name", "Original Description", "#FF0000");
    _mockTagRepository.Setup(x => x.GetByIdAsync(tagId))
        .ReturnsAsync(existingTag);
    _mockTagRepository.Setup(x => x.UpdateAsync(It.IsAny<Tag>()))
        .ReturnsAsync((Tag t) => t);
    _mockTagRepository.Setup(x => x.ExistsByNameAsync(name))
        .ReturnsAsync(false);

    // Act
    var result = await _tagService.UpdateTagAsync(tagId, name, description, color);

    // Assert
    result.UpdatedAt.Should().BeAfter(beforeUpdate);
  }

  private Tag CreateActiveTag(int id, string name, string description, string color)
  {
    var tag = new Tag(id, name, description, color);
    // Use reflection to set IsActive since the setter is private
    var isActiveProperty = typeof(Tag).GetProperty("IsActive");
    if (isActiveProperty != null && isActiveProperty.CanWrite)
    {
      isActiveProperty.SetValue(tag, true);
    }
    return tag;
  }

  private Tag CreateInactiveTag(int id, string name, string description, string color)
  {
    var tag = new Tag(id, name, description, color);
    // Use reflection to set IsActive since the setter is private
    var isActiveProperty = typeof(Tag).GetProperty("IsActive");
    if (isActiveProperty != null && isActiveProperty.CanWrite)
    {
      isActiveProperty.SetValue(tag, false);
    }
    return tag;
  }

  #endregion
}