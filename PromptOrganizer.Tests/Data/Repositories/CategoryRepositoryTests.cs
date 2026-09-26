using FluentAssertions;
using PromptOrganizer.Domain.Entities;
using PromptOrganizer.Domain.Repositories;
using PromptOrganizer.Tests.TestHelpers;
using Xunit;

using PromptOrganizer.Tests.TestHelpers;

namespace PromptOrganizer.Tests.Data.Repositories;

[Collection("DatabaseCollection")]
public class CategoryRepositoryTests : RepositoryTestBase
{
  [Fact]
  public async Task AddAsync_WithValidCategory_ShouldAddSuccessfully()
  {
    // Arrange
    var category = new Category(0, "Test Category", "Test description");

    // Act
    var result = await CategoryRepository.AddAsync(category);

    // Assert
    result.Should().NotBeNull();
    result.Id.Should().BeGreaterThan(0);
    result.Name.Should().Be("Test Category");
    result.Description.Should().Be("Test description");
    result.IsActive.Should().BeTrue();
  }

  [Fact]
  public async Task GetByIdAsync_WithExistingCategory_ShouldReturnCategory()
  {
    // Arrange
    var category = await CreateTestCategoryAsync();

    // Act
    var result = await CategoryRepository.GetByIdAsync(category.Id);

    // Assert
    result.Should().NotBeNull();
    result.Id.Should().Be(category.Id);
    result.Name.Should().Be(category.Name);
  }

  [Fact]
  public async Task GetByIdAsync_WithNonExistingCategory_ShouldReturnNull()
  {
    // Act
    var result = await CategoryRepository.GetByIdAsync(999);

    // Assert
    result.Should().BeNull();
  }

  [Fact]
  public async Task GetAllAsync_WithCategories_ShouldReturnAllCategories()
  {
    // Arrange
    await CreateTestCategoryAsync("Category 1");
    await CreateTestCategoryAsync("Category 2");
    await CreateTestCategoryAsync("Category 3");

    // Act
    var result = await CategoryRepository.GetAllAsync();

    // Assert
    result.Should().NotBeNull();
    result.Should().HaveCount(3);
  }

  [Fact]
  public async Task GetAllAsync_WithNoCategories_ShouldReturnEmptyList()
  {
    // Act
    var result = await CategoryRepository.GetAllAsync();

    // Assert
    result.Should().NotBeNull();
    result.Should().BeEmpty();
  }

  [Fact]
  public async Task UpdateAsync_WithValidCategory_ShouldUpdateSuccessfully()
  {
    // Arrange
    var category = await CreateTestCategoryAsync();
    var updatedDescription = "Updated description";

    // Act
    category.UpdateDescription(updatedDescription);
    var result = await CategoryRepository.UpdateAsync(category);

    // Assert
    result.Should().NotBeNull();
    result.Description.Should().Be(updatedDescription);
    result.UpdatedAt.Should().BeAfter(result.CreatedAt);
  }

  [Fact]
  public async Task DeleteAsync_WithExistingCategory_ShouldDeleteSuccessfully()
  {
    // Arrange
    var category = await CreateTestCategoryAsync();

    // Act
    var result = await CategoryRepository.DeleteAsync(category.Id);

    // Assert
    result.Should().BeTrue();
    var deletedCategory = await CategoryRepository.GetByIdAsync(category.Id);
    deletedCategory.Should().BeNull();
  }

  [Fact]
  public async Task DeleteAsync_WithNonExistingCategory_ShouldReturnFalse()
  {
    // Act
    var result = await CategoryRepository.DeleteAsync(999);

    // Assert
    result.Should().BeFalse();
  }

  [Fact]
  public async Task ExistsAsync_WithExistingCategory_ShouldReturnTrue()
  {
    // Arrange
    var category = await CreateTestCategoryAsync();

    // Act
    var result = await CategoryRepository.ExistsAsync(category.Id);

    // Assert
    result.Should().BeTrue();
  }

  [Fact]
  public async Task ExistsAsync_WithNonExistingCategory_ShouldReturnFalse()
  {
    // Act
    var result = await CategoryRepository.ExistsAsync(999);

    // Assert
    result.Should().BeFalse();
  }

  [Fact]
  public async Task CountAsync_WithCategories_ShouldReturnCorrectCount()
  {
    // Arrange
    await CreateTestCategoryAsync("Category 1");
    await CreateTestCategoryAsync("Category 2");

    // Act
    var result = await CategoryRepository.CountAsync();

    // Assert
    result.Should().Be(2);
  }

  [Fact]
  public async Task CountAsync_WithNoCategories_ShouldReturnZero()
  {
    // Act
    var result = await CategoryRepository.CountAsync();

    // Assert
    result.Should().Be(0);
  }

  [Fact]
  public async Task GetByNameAsync_WithExistingCategory_ShouldReturnCategory()
  {
    // Arrange
    var categoryName = "Unique Category";
    await CreateTestCategoryAsync(categoryName);

    // Act
    var result = await CategoryRepository.GetByNameAsync(categoryName);

    // Assert
    result.Should().NotBeNull();
    result.Name.Should().Be(categoryName);
  }

  [Fact]
  public async Task GetByNameAsync_WithNonExistingCategory_ShouldReturnNull()
  {
    // Act
    var result = await CategoryRepository.GetByNameAsync("Non-existent Category");

    // Assert
    result.Should().BeNull();
  }

  [Fact]
  public async Task GetByNameAsync_WithNullName_ShouldReturnNull()
  {
    // Act
    var result = await CategoryRepository.GetByNameAsync(null);

    // Assert
    result.Should().BeNull();
  }

  [Fact]
  public async Task GetActiveCategoriesAsync_WithMixedCategories_ShouldReturnOnlyActive()
  {
    // Arrange
    var activeCategory = await CreateTestCategoryAsync("Active Category");
    var inactiveCategory = await CreateTestCategoryAsync("Inactive Category");
    inactiveCategory.Deactivate();
    await CategoryRepository.UpdateAsync(inactiveCategory);

    // Act
    var result = await CategoryRepository.GetActiveCategoriesAsync();

    // Assert
    result.Should().HaveCount(1);
    result.Should().ContainSingle(c => c.Id == activeCategory.Id);
  }

  [Fact]
  public async Task SearchAsync_WithMatchingName_ShouldReturnCategories()
  {
    // Arrange
    await CreateTestCategoryAsync("Test Category");
    await CreateTestCategoryAsync("Another Category");
    await CreateTestCategoryAsync("Different Name");

    // Act
    var result = await CategoryRepository.SearchAsync("Test");

    // Assert
    result.Should().HaveCount(1);
    result.Should().ContainSingle(c => c.Name.Contains("Test"));
  }

  [Fact]
  public async Task SearchAsync_WithMatchingDescription_ShouldReturnCategories()
  {
    // Arrange
    await CreateTestCategoryAsync("Category 1", "This is a test description");
    await CreateTestCategoryAsync("Category 2", "Different description");

    // Act
    var result = await CategoryRepository.SearchAsync("test");

    // Assert
    result.Should().HaveCount(1);
    result.Should().ContainSingle(c => c.Description.Contains("test"));
  }

  [Fact]
  public async Task SearchAsync_WithEmptySearchTerm_ShouldReturnAllCategories()
  {
    // Arrange
    await CreateTestCategoryAsync("Category 1");
    await CreateTestCategoryAsync("Category 2");

    // Act
    var result = await CategoryRepository.SearchAsync("");

    // Assert
    result.Should().HaveCount(2);
  }

  [Fact]
  public async Task ExistsByNameAsync_WithExistingName_ShouldReturnTrue()
  {
    // Arrange
    var categoryName = "Existing Category";
    await CreateTestCategoryAsync(categoryName);

    // Act
    var result = await CategoryRepository.ExistsByNameAsync(categoryName);

    // Assert
    result.Should().BeTrue();
  }

  [Fact]
  public async Task ExistsByNameAsync_WithNonExistingName_ShouldReturnFalse()
  {
    // Act
    var result = await CategoryRepository.ExistsByNameAsync("Non-existent Category");

    // Assert
    result.Should().BeFalse();
  }

  [Fact]
  public async Task ExistsByNameAsync_WithNullName_ShouldReturnFalse()
  {
    // Act
    var result = await CategoryRepository.ExistsByNameAsync(null);

    // Assert
    result.Should().BeFalse();
  }

  [Fact]
  public async Task GetCategoriesWithPromptCountsAsync_WithCategories_ShouldReturnCounts()
  {
    // Arrange
    var category1 = await CreateTestCategoryAsync("Category 1");
    var category2 = await CreateTestCategoryAsync("Category 2");

    // Create prompts in categories
    await CreateTestPromptAsync("Prompt 1", "Content 1", category1.Id);
    await CreateTestPromptAsync("Prompt 2", "Content 2", category1.Id);
    await CreateTestPromptAsync("Prompt 3", "Content 3", category2.Id);

    // Act
    var result = await CategoryRepository.GetCategoriesWithPromptCountsAsync();

    // Assert
    result.Should().HaveCount(2);

    var category1Result = result.Should().ContainSingle(x => x.Category.Id == category1.Id).Subject;
    category1Result.PromptCount.Should().Be(2);

    var category2Result = result.Should().ContainSingle(x => x.Category.Id == category2.Id).Subject;
    category2Result.PromptCount.Should().Be(1);
  }

  [Fact]
  public async Task GetCategoriesWithPromptCountsAsync_WithNoCategories_ShouldReturnEmpty()
  {
    // Act
    var result = await CategoryRepository.GetCategoriesWithPromptCountsAsync();

    // Assert
    result.Should().BeEmpty();
  }
}