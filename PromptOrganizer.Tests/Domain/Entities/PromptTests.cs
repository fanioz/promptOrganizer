using FluentAssertions;
using PromptOrganizer.Domain.Entities;
using PromptOrganizer.Tests.TestHelpers;
using Xunit;

namespace PromptOrganizer.Tests.Domain.Entities;

public class PromptTests : BaseTest
{
  private const string ValidTitle = "Test Prompt";
  private const string ValidContent = "This is a test prompt content";
  private const string ValidDescription = "Test prompt description";
  private const string ValidCategory = "Test Category";
  private const string ValidTags = "test,prompt,demo";

  [Fact]
  public void Constructor_WithValidParameters_ShouldCreatePromptSuccessfully()
  {
    // Act
    var prompt = new Prompt(ValidTitle, ValidContent, ValidDescription, ValidCategory, ValidTags);

    // Assert
    prompt.Should().NotBeNull();
    prompt.Title.Should().Be(ValidTitle);
    prompt.Content.Should().Be(ValidContent);
    prompt.Description.Should().Be(ValidDescription);
    prompt.Category.Should().Be(ValidCategory);
    prompt.Tags.Should().Be(ValidTags);
    prompt.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    prompt.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    prompt.IsActive.Should().BeTrue();
  }

  [Fact]
  public void Constructor_WithNullTitle_ShouldThrowArgumentException()
  {
    // Act & Assert
    AssertThrows<ArgumentException>(() =>
        new Prompt(null!, ValidContent, ValidDescription, ValidCategory, ValidTags),
        "Title cannot be null or empty");
  }

  [Fact]
  public void Constructor_WithEmptyTitle_ShouldThrowArgumentException()
  {
    // Act & Assert
    AssertThrows<ArgumentException>(() =>
        new Prompt(string.Empty, ValidContent, ValidDescription, ValidCategory, ValidTags),
        "Title cannot be null or empty");
  }

  [Fact]
  public void Constructor_WithWhitespaceTitle_ShouldThrowArgumentException()
  {
    // Act & Assert
    AssertThrows<ArgumentException>(() =>
        new Prompt("   ", ValidContent, ValidDescription, ValidCategory, ValidTags),
        "Title cannot be null or empty");
  }

  [Fact]
  public void Constructor_WithNullContent_ShouldThrowArgumentException()
  {
    // Act & Assert
    AssertThrows<ArgumentException>(() =>
        new Prompt(ValidTitle, null!, ValidDescription, ValidCategory, ValidTags),
        "Content cannot be null or empty");
  }

  [Fact]
  public void Constructor_WithEmptyContent_ShouldThrowArgumentException()
  {
    // Act & Assert
    AssertThrows<ArgumentException>(() =>
        new Prompt(ValidTitle, string.Empty, ValidDescription, ValidCategory, ValidTags),
        "Content cannot be null or empty");
  }

  [Fact]
  public void Constructor_WithWhitespaceContent_ShouldThrowArgumentException()
  {
    // Act & Assert
    AssertThrows<ArgumentException>(() =>
        new Prompt(ValidTitle, "   ", ValidDescription, ValidCategory, ValidTags),
        "Content cannot be null or empty");
  }

  [Fact]
  public void Constructor_WithTitleExceedingMaxLength_ShouldThrowArgumentException()
  {
    // Arrange
    var longTitle = new string('a', 201);

    // Act & Assert
    AssertThrows<ArgumentException>(() =>
        new Prompt(longTitle, ValidContent, ValidDescription, ValidCategory, ValidTags),
        "Title cannot exceed 200 characters");
  }

  [Fact]
  public void Constructor_WithContentExceedingMaxLength_ShouldThrowArgumentException()
  {
    // Arrange
    var longContent = new string('a', 5001);

    // Act & Assert
    AssertThrows<ArgumentException>(() =>
        new Prompt(ValidTitle, longContent, ValidDescription, ValidCategory, ValidTags),
        "Content cannot exceed 5000 characters");
  }

  [Fact]
  public void UpdateContent_WithValidContent_ShouldUpdateContentAndTimestamp()
  {
    // Arrange
    var prompt = new Prompt(ValidTitle, ValidContent, ValidDescription, ValidCategory, ValidTags);
    var originalUpdatedAt = prompt.UpdatedAt;
    var newContent = "Updated prompt content";

    // Act
    prompt.UpdateContent(newContent);

    // Assert
    prompt.Content.Should().Be(newContent);
    prompt.UpdatedAt.Should().BeAfter(originalUpdatedAt);
  }

  [Fact]
  public void UpdateContent_WithNullContent_ShouldThrowArgumentException()
  {
    // Arrange
    var prompt = new Prompt(ValidTitle, ValidContent, ValidDescription, ValidCategory, ValidTags);

    // Act & Assert
    AssertThrows<ArgumentException>(() => prompt.UpdateContent(null!),
        "Content cannot be null or empty");
  }

  [Fact]
  public void UpdateContent_WithEmptyContent_ShouldThrowArgumentException()
  {
    // Arrange
    var prompt = new Prompt(ValidTitle, ValidContent, ValidDescription, ValidCategory, ValidTags);

    // Act & Assert
    AssertThrows<ArgumentException>(() => prompt.UpdateContent(string.Empty),
        "Content cannot be null or empty");
  }

  [Fact]
  public void UpdateDescription_WithValidDescription_ShouldUpdateDescriptionAndTimestamp()
  {
    // Arrange
    var prompt = new Prompt(ValidTitle, ValidContent, ValidDescription, ValidCategory, ValidTags);
    var originalUpdatedAt = prompt.UpdatedAt;
    var newDescription = "Updated prompt description";

    // Act
    prompt.UpdateDescription(newDescription);

    // Assert
    prompt.Description.Should().Be(newDescription);
    prompt.UpdatedAt.Should().BeAfter(originalUpdatedAt);
  }

  [Fact]
  public void UpdateCategory_WithValidCategory_ShouldUpdateCategoryAndTimestamp()
  {
    // Arrange
    var prompt = new Prompt(ValidTitle, ValidContent, ValidDescription, ValidCategory, ValidTags);
    var originalUpdatedAt = prompt.UpdatedAt;
    var newCategory = "Updated Category";

    // Act
    prompt.UpdateCategory(newCategory);

    // Assert
    prompt.Category.Should().Be(newCategory);
    prompt.UpdatedAt.Should().BeAfter(originalUpdatedAt);
  }

  [Fact]
  public void UpdateTags_WithValidTags_ShouldUpdateTagsAndTimestamp()
  {
    // Arrange
    var prompt = new Prompt(ValidTitle, ValidContent, ValidDescription, ValidCategory, ValidTags);
    var originalUpdatedAt = prompt.UpdatedAt;
    var newTags = "updated,tags,new";

    // Act
    prompt.UpdateTags(newTags);

    // Assert
    prompt.Tags.Should().Be(newTags);
    prompt.UpdatedAt.Should().BeAfter(originalUpdatedAt);
  }

  [Fact]
  public void Deactivate_ShouldSetIsActiveToFalseAndUpdateTimestamp()
  {
    // Arrange
    var prompt = new Prompt(ValidTitle, ValidContent, ValidDescription, ValidCategory, ValidTags);
    var originalUpdatedAt = prompt.UpdatedAt;

    // Act
    prompt.Deactivate();

    // Assert
    prompt.IsActive.Should().BeFalse();
    prompt.UpdatedAt.Should().BeAfter(originalUpdatedAt);
  }

  [Fact]
  public void Activate_ShouldSetIsActiveToTrueAndUpdateTimestamp()
  {
    // Arrange
    var prompt = new Prompt(ValidTitle, ValidContent, ValidDescription, ValidCategory, ValidTags);
    prompt.Deactivate(); // First deactivate
    var originalUpdatedAt = prompt.UpdatedAt;

    // Act
    prompt.Activate();

    // Assert
    prompt.IsActive.Should().BeTrue();
    prompt.UpdatedAt.Should().BeAfter(originalUpdatedAt);
  }

  [Fact]
  public void Title_Property_ShouldReturnCorrectValue()
  {
    // Arrange
    var prompt = new Prompt(ValidTitle, ValidContent, ValidDescription, ValidCategory, ValidTags);

    // Act & Assert
    prompt.Title.Should().Be(ValidTitle);
  }

  [Fact]
  public void Content_Property_ShouldReturnCorrectValue()
  {
    // Arrange
    var prompt = new Prompt(ValidTitle, ValidContent, ValidDescription, ValidCategory, ValidTags);

    // Act & Assert
    prompt.Content.Should().Be(ValidContent);
  }

  [Fact]
  public void Description_Property_ShouldReturnCorrectValue()
  {
    // Arrange
    var prompt = new Prompt(ValidTitle, ValidContent, ValidDescription, ValidCategory, ValidTags);

    // Act & Assert
    prompt.Description.Should().Be(ValidDescription);
  }

  [Fact]
  public void Category_Property_ShouldReturnCorrectValue()
  {
    // Arrange
    var prompt = new Prompt(ValidTitle, ValidContent, ValidDescription, ValidCategory, ValidTags);

    // Act & Assert
    prompt.Category.Should().Be(ValidCategory);
  }

  [Fact]
  public void Tags_Property_ShouldReturnCorrectValue()
  {
    // Arrange
    var prompt = new Prompt(ValidTitle, ValidContent, ValidDescription, ValidCategory, ValidTags);

    // Act & Assert
    prompt.Tags.Should().Be(ValidTags);
  }

  [Fact]
  public void CreatedAt_Property_ShouldReturnCorrectValue()
  {
    // Arrange
    var beforeCreation = DateTime.UtcNow;
    var prompt = new Prompt(ValidTitle, ValidContent, ValidDescription, ValidCategory, ValidTags);
    var afterCreation = DateTime.UtcNow;

    // Act & Assert
    prompt.CreatedAt.Should().BeOnOrAfter(beforeCreation);
    prompt.CreatedAt.Should().BeOnOrBefore(afterCreation);
  }

  [Fact]
  public void UpdatedAt_Property_ShouldReturnCorrectValue()
  {
    // Arrange
    var beforeCreation = DateTime.UtcNow;
    var prompt = new Prompt(ValidTitle, ValidContent, ValidDescription, ValidCategory, ValidTags);
    var afterCreation = DateTime.UtcNow;

    // Act & Assert
    prompt.UpdatedAt.Should().BeOnOrAfter(beforeCreation);
    prompt.UpdatedAt.Should().BeOnOrBefore(afterCreation);
  }

  [Fact]
  public void IsActive_Property_ShouldReturnCorrectValue()
  {
    // Arrange
    var prompt = new Prompt(ValidTitle, ValidContent, ValidDescription, ValidCategory, ValidTags);

    // Act & Assert
    prompt.IsActive.Should().BeTrue();
  }
}