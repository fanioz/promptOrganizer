using FluentAssertions;
using Moq;
using PromptOrganizer.Domain.Entities;
using PromptOrganizer.Domain.Repositories;
using PromptOrganizer.Services;
using PromptOrganizer.Tests.TestHelpers;
using Xunit;

namespace PromptOrganizer.Tests.Services;

/// <summary>
/// TDD tests for CategoryService business logic and management operations.
/// </summary>
public class CategoryServiceTests : BaseTest
{
  private readonly Mock<ICategoryRepository> _mockCategoryRepository;
  private readonly Mock<IPromptRepository> _mockPromptRepository;
  private readonly ICategoryService _categoryService;

  public CategoryServiceTests()
  {
    _mockCategoryRepository = new Mock<ICategoryRepository>();
    _mockPromptRepository = new Mock<IPromptRepository>();
    _categoryService = new CategoryService(_mockCategoryRepository.Object, _mockPromptRepository.Object);
  }

  #region CreateCategoryAsync Tests

  [Fact]
  public async Task CreateCategoryAsync_WithValidData_ShouldCreateCategory()
  {
    // Arrange
    var name = "Test Category";
    var description = "Test description";

    var expectedCategory = new Category(1, name, description);
    _mockCategoryRepository.Setup(x => x.AddAsync(It.IsAny<Category>()))
        .ReturnsAsync(expectedCategory);

    // Act
    var result = await _categoryService.CreateCategoryAsync(name, description);

    // Assert
    result.Should().NotBeNull();
    result.Name.Should().Be(name);
    result.Description.Should().Be(description);
    result.IsActive.Should().BeTrue();

    _mockCategoryRepository.Verify(x => x.AddAsync(It.IsAny<Category>()), Times.Once);
  }

  [Fact]
  public async Task CreateCategoryAsync_WithEmptyName_ShouldThrowArgumentException()
  {
    // Arrange
    var name = "";
    var description = "Test description";

    // Act & Assert
    await Assert.ThrowsAsync<ArgumentException>(() =>
        _categoryService.CreateCategoryAsync(name, description));
  }

  [Fact]
  public async Task CreateCategoryAsync_WithNullName_ShouldThrowArgumentException()
  {
    // Arrange
    string name = null!;
    var description = "Test description";

    // Act & Assert
    await Assert.ThrowsAsync<ArgumentException>(() =>
        _categoryService.CreateCategoryAsync(name, description));
  }

  [Fact]
  public async Task CreateCategoryAsync_WithNameExceedingMaxLength_ShouldThrowArgumentException()
  {
    // Arrange
    var name = new string('A', 101); // Max length is 100
    var description = "Test description";

    // Act & Assert
    await Assert.ThrowsAsync<ArgumentException>(() =>
        _categoryService.CreateCategoryAsync(name, description));
  }

  [Fact]
  public async Task CreateCategoryAsync_WithDuplicateName_ShouldThrowInvalidOperationException()
  {
    // Arrange
    var name = "Existing Category";
    var description = "Test description";

    _mockCategoryRepository.Setup(x => x.ExistsByNameAsync(name))
        .ReturnsAsync(true);

    // Act & Assert
    await Assert.ThrowsAsync<InvalidOperationException>(() =>
        _categoryService.CreateCategoryAsync(name, description));
  }

  #endregion

  #region UpdateCategoryAsync Tests

  [Fact]
  public async Task UpdateCategoryAsync_WithValidData_ShouldUpdateCategory()
  {
    // Arrange
    var categoryId = 1;
    var name = "Updated Category";
    var description = "Updated description";

    var existingCategory = new Category(categoryId, "Original Name", "Original Description");
    var updatedCategory = new Category(categoryId, name, description);

    _mockCategoryRepository.Setup(x => x.GetByIdAsync(categoryId))
        .ReturnsAsync(existingCategory);
    _mockCategoryRepository.Setup(x => x.UpdateAsync(It.IsAny<Category>()))
        .ReturnsAsync(updatedCategory);
    _mockCategoryRepository.Setup(x => x.ExistsByNameAsync(name))
        .ReturnsAsync(false);

    // Act
    var result = await _categoryService.UpdateCategoryAsync(categoryId, name, description);

    // Assert
    result.Should().NotBeNull();
    result.Name.Should().Be(name);
    result.Description.Should().Be(description);

    _mockCategoryRepository.Verify(x => x.GetByIdAsync(categoryId), Times.Once);
    _mockCategoryRepository.Verify(x => x.UpdateAsync(It.IsAny<Category>()), Times.Once);
  }

  [Fact]
  public async Task UpdateCategoryAsync_WithNonExistentCategory_ShouldThrowKeyNotFoundException()
  {
    // Arrange
    var categoryId = 999;
    var name = "Updated Category";
    var description = "Updated description";

    _mockCategoryRepository.Setup(x => x.GetByIdAsync(categoryId))
        .ReturnsAsync((Category?)null);

    // Act & Assert
    await Assert.ThrowsAsync<KeyNotFoundException>(() =>
        _categoryService.UpdateCategoryAsync(categoryId, name, description));
  }

  [Fact]
  public async Task UpdateCategoryAsync_WithInvalidData_ShouldThrowArgumentException()
  {
    // Arrange
    var categoryId = 1;
    var name = ""; // Invalid name
    var description = "Updated description";

    var existingCategory = new Category(categoryId, "Original Name", "Original Description");
    _mockCategoryRepository.Setup(x => x.GetByIdAsync(categoryId))
        .ReturnsAsync(existingCategory);

    // Act & Assert
    await Assert.ThrowsAsync<ArgumentException>(() =>
        _categoryService.UpdateCategoryAsync(categoryId, name, description));
  }

  #endregion

  #region GetCategoryByIdAsync Tests

  [Fact]
  public async Task GetCategoryByIdAsync_WithExistingCategory_ShouldReturnCategory()
  {
    // Arrange
    var categoryId = 1;
    var expectedCategory = new Category(categoryId, "Test Category", "Test description");

    _mockCategoryRepository.Setup(x => x.GetByIdAsync(categoryId))
        .ReturnsAsync(expectedCategory);

    // Act
    var result = await _categoryService.GetCategoryByIdAsync(categoryId);

    // Assert
    result.Should().NotBeNull();
    result.Should().Be(expectedCategory);
    _mockCategoryRepository.Verify(x => x.GetByIdAsync(categoryId), Times.Once);
  }

  [Fact]
  public async Task GetCategoryByIdAsync_WithNonExistentCategory_ShouldReturnNull()
  {
    // Arrange
    var categoryId = 999;

    _mockCategoryRepository.Setup(x => x.GetByIdAsync(categoryId))
        .ReturnsAsync((Category?)null);

    // Act
    var result = await _categoryService.GetCategoryByIdAsync(categoryId);

    // Assert
    result.Should().BeNull();
    _mockCategoryRepository.Verify(x => x.GetByIdAsync(categoryId), Times.Once);
  }

  #endregion

  #region GetCategoryByNameAsync Tests

  [Fact]
  public async Task GetCategoryByNameAsync_WithExistingCategory_ShouldReturnCategory()
  {
    // Arrange
    var categoryName = "Test Category";
    var expectedCategory = new Category(1, categoryName, "Test description");

    _mockCategoryRepository.Setup(x => x.GetByNameAsync(categoryName))
        .ReturnsAsync(expectedCategory);

    // Act
    var result = await _categoryService.GetCategoryByNameAsync(categoryName);

    // Assert
    result.Should().NotBeNull();
    result.Should().Be(expectedCategory);
    _mockCategoryRepository.Verify(x => x.GetByNameAsync(categoryName), Times.Once);
  }

  [Fact]
  public async Task GetCategoryByNameAsync_WithNonExistentCategory_ShouldReturnNull()
  {
    // Arrange
    var categoryName = "NonExistent";

    _mockCategoryRepository.Setup(x => x.GetByNameAsync(categoryName))
        .ReturnsAsync((Category?)null);

    // Act
    var result = await _categoryService.GetCategoryByNameAsync(categoryName);

    // Assert
    result.Should().BeNull();
    _mockCategoryRepository.Verify(x => x.GetByNameAsync(categoryName), Times.Once);
  }

  #endregion

  #region GetAllActiveCategoriesAsync Tests

  [Fact]
  public async Task GetAllActiveCategoriesAsync_ShouldReturnOnlyActiveCategories()
  {
    // Arrange
    var activeCategories = new List<Category>
    {
        CreateActiveCategory(1, "Active 1", "Desc 1"),
        CreateActiveCategory(2, "Active 2", "Desc 2")
    };

    _mockCategoryRepository.Setup(x => x.GetActiveCategoriesAsync())
        .ReturnsAsync(activeCategories);

    // Act
    var result = await _categoryService.GetAllActiveCategoriesAsync();

    // Assert
    result.Should().NotBeNull();
    result.Should().HaveCount(2);
    result.All(c => c.IsActive).Should().BeTrue();
    _mockCategoryRepository.Verify(x => x.GetActiveCategoriesAsync(), Times.Once);
  }

  [Fact]
  public async Task GetAllActiveCategoriesAsync_WhenNoActiveCategories_ShouldReturnEmptyCollection()
  {
    // Arrange
    _mockCategoryRepository.Setup(x => x.GetActiveCategoriesAsync())
        .ReturnsAsync(Enumerable.Empty<Category>());

    // Act
    var result = await _categoryService.GetAllActiveCategoriesAsync();

    // Assert
    result.Should().NotBeNull();
    result.Should().BeEmpty();
    _mockCategoryRepository.Verify(x => x.GetActiveCategoriesAsync(), Times.Once);
  }

  #endregion

  #region GetCategoriesWithPromptCountsAsync Tests

  [Fact]
  public async Task GetCategoriesWithPromptCountsAsync_ShouldReturnCategoriesWithCounts()
  {
    // Arrange
    var categories = new List<Category>
        {
            new Category(1, "Category 1", "Desc 1"),
            new Category(2, "Category 2", "Desc 2")
        };

    var categoriesWithCounts = new List<(Category Category, int PromptCount)>
        {
            (categories[0], 5),
            (categories[1], 3)
        };

    _mockCategoryRepository.Setup(x => x.GetCategoriesWithPromptCountsAsync())
        .ReturnsAsync(categoriesWithCounts);

    // Act
    var result = await _categoryService.GetCategoriesWithPromptCountsAsync();

    // Assert
    result.Should().NotBeNull();
    result.Should().HaveCount(2);
    result.First().Category.Should().Be(categories[0]);
    result.First().PromptCount.Should().Be(5);
    result.Last().Category.Should().Be(categories[1]);
    result.Last().PromptCount.Should().Be(3);
  }

  #endregion

  #region SearchCategoriesAsync Tests

  [Fact]
  public async Task SearchCategoriesAsync_WithMatchingTerm_ShouldReturnMatchingCategories()
  {
    // Arrange
    var searchTerm = "test";
    var matchingCategories = new List<Category>
        {
            new Category(1, "Test Category", "Desc with test"),
            new Category(2, "Another Test", "Desc")
        };

    _mockCategoryRepository.Setup(x => x.SearchAsync(searchTerm))
        .ReturnsAsync(matchingCategories);

    // Act
    var result = await _categoryService.SearchCategoriesAsync(searchTerm);

    // Assert
    result.Should().NotBeNull();
    result.Should().HaveCount(2);
    _mockCategoryRepository.Verify(x => x.SearchAsync(searchTerm), Times.Once);
  }

  [Fact]
  public async Task SearchCategoriesAsync_WithEmptySearchTerm_ShouldReturnEmptyCollection()
  {
    // Arrange
    var searchTerm = "";

    // Act
    var result = await _categoryService.SearchCategoriesAsync(searchTerm);

    // Assert
    result.Should().NotBeNull();
    result.Should().BeEmpty();
    _mockCategoryRepository.Verify(x => x.SearchAsync(It.IsAny<string>()), Times.Never);
  }

  #endregion

  #region DeactivateCategoryAsync Tests

  [Fact]
  public async Task DeactivateCategoryAsync_WithExistingCategory_ShouldDeactivateAndReturnTrue()
  {
    // Arrange
    var categoryId = 1;
    var category = new Category(categoryId, "Test Category", "Test description");

    _mockCategoryRepository.Setup(x => x.GetByIdAsync(categoryId))
        .ReturnsAsync(category);
    _mockCategoryRepository.Setup(x => x.UpdateAsync(It.IsAny<Category>()))
        .ReturnsAsync(category);

    // Act
    var result = await _categoryService.DeactivateCategoryAsync(categoryId);

    // Assert
    result.Should().BeTrue();
    category.IsActive.Should().BeFalse();
    _mockCategoryRepository.Verify(x => x.GetByIdAsync(categoryId), Times.Once);
    _mockCategoryRepository.Verify(x => x.UpdateAsync(It.IsAny<Category>()), Times.Once);
  }

  [Fact]
  public async Task DeactivateCategoryAsync_WithNonExistentCategory_ShouldReturnFalse()
  {
    // Arrange
    var categoryId = 999;

    _mockCategoryRepository.Setup(x => x.GetByIdAsync(categoryId))
        .ReturnsAsync((Category?)null);

    // Act
    var result = await _categoryService.DeactivateCategoryAsync(categoryId);

    // Assert
    result.Should().BeFalse();
    _mockCategoryRepository.Verify(x => x.GetByIdAsync(categoryId), Times.Once);
    _mockCategoryRepository.Verify(x => x.UpdateAsync(It.IsAny<Category>()), Times.Never);
  }

  #endregion

  #region ActivateCategoryAsync Tests

  [Fact]
  public async Task ActivateCategoryAsync_WithExistingCategory_ShouldActivateAndReturnTrue()
  {
    // Arrange
    var categoryId = 1;
    var category = CreateInactiveCategory(categoryId, "Test Category", "Test description");

    _mockCategoryRepository.Setup(x => x.GetByIdAsync(categoryId))
        .ReturnsAsync(category);
    _mockCategoryRepository.Setup(x => x.UpdateAsync(It.IsAny<Category>()))
        .ReturnsAsync(category);

    // Act
    var result = await _categoryService.ActivateCategoryAsync(categoryId);

    // Assert
    result.Should().BeTrue();
    category.IsActive.Should().BeTrue();
    _mockCategoryRepository.Verify(x => x.GetByIdAsync(categoryId), Times.Once);
    _mockCategoryRepository.Verify(x => x.UpdateAsync(It.IsAny<Category>()), Times.Once);
  }

  [Fact]
  public async Task ActivateCategoryAsync_WithNonExistentCategory_ShouldReturnFalse()
  {
    // Arrange
    var categoryId = 999;

    _mockCategoryRepository.Setup(x => x.GetByIdAsync(categoryId))
        .ReturnsAsync((Category?)null);

    // Act
    var result = await _categoryService.ActivateCategoryAsync(categoryId);

    // Assert
    result.Should().BeFalse();
    _mockCategoryRepository.Verify(x => x.GetByIdAsync(categoryId), Times.Once);
    _mockCategoryRepository.Verify(x => x.UpdateAsync(It.IsAny<Category>()), Times.Never);
  }

  #endregion

  #region CategoryNameExistsAsync Tests

  [Fact]
  public async Task CategoryNameExistsAsync_WithExistingName_ShouldReturnTrue()
  {
    // Arrange
    var categoryName = "Existing Category";

    _mockCategoryRepository.Setup(x => x.ExistsByNameAsync(categoryName))
        .ReturnsAsync(true);

    // Act
    var result = await _categoryService.CategoryNameExistsAsync(categoryName);

    // Assert
    result.Should().BeTrue();
    _mockCategoryRepository.Verify(x => x.ExistsByNameAsync(categoryName), Times.Once);
  }

  [Fact]
  public async Task CategoryNameExistsAsync_WithNonExistingName_ShouldReturnFalse()
  {
    // Arrange
    var categoryName = "New Category";

    _mockCategoryRepository.Setup(x => x.ExistsByNameAsync(categoryName))
        .ReturnsAsync(false);

    // Act
    var result = await _categoryService.CategoryNameExistsAsync(categoryName);

    // Assert
    result.Should().BeFalse();
    _mockCategoryRepository.Verify(x => x.ExistsByNameAsync(categoryName), Times.Once);
  }

  [Fact]
  public async Task CategoryNameExistsAsync_WithExistingNameButDifferentId_ShouldReturnTrue()
  {
    // Arrange
    var categoryName = "Existing Category";
    var excludeId = 1;

    _mockCategoryRepository.Setup(x => x.GetByNameAsync(categoryName))
        .ReturnsAsync(new Category(2, categoryName, "Description"));

    // Act
    var result = await _categoryService.CategoryNameExistsAsync(categoryName, excludeId);

    // Assert
    result.Should().BeTrue();
    _mockCategoryRepository.Verify(x => x.GetByNameAsync(categoryName), Times.Once);
  }

  [Fact]
  public async Task CategoryNameExistsAsync_WithSameNameAndId_ShouldReturnFalse()
  {
    // Arrange
    var categoryName = "Existing Category";
    var excludeId = 1;

    _mockCategoryRepository.Setup(x => x.GetByNameAsync(categoryName))
        .ReturnsAsync(new Category(excludeId, categoryName, "Description"));

    // Act
    var result = await _categoryService.CategoryNameExistsAsync(categoryName, excludeId);

    // Assert
    result.Should().BeFalse();
    _mockCategoryRepository.Verify(x => x.GetByNameAsync(categoryName), Times.Once);
  }

  #endregion

  #region ValidateCategoryDataAsync Tests

  [Fact]
  public async Task ValidateCategoryDataAsync_WithValidData_ShouldReturnEmptyValidationErrors()
  {
    // Arrange
    var name = "Test Category";
    var description = "Test description";

    _mockCategoryRepository.Setup(x => x.ExistsByNameAsync(name))
        .ReturnsAsync(false);

    // Act
    var result = await _categoryService.ValidateCategoryDataAsync(name, description);

    // Assert
    result.Should().NotBeNull();
    result.Should().BeEmpty();
  }

  [Fact]
  public async Task ValidateCategoryDataAsync_WithInvalidData_ShouldReturnValidationErrors()
  {
    // Arrange
    var name = ""; // Invalid
    var description = new string('A', 501); // Too long

    // Act
    var result = await _categoryService.ValidateCategoryDataAsync(name, description);

    // Assert
    result.Should().NotBeNull();
    result.Should().Contain("Name cannot be null or empty");
    result.Should().Contain("Description cannot exceed 500 characters");
  }

  [Fact]
  public async Task ValidateCategoryDataAsync_WithDuplicateName_ShouldReturnValidationError()
  {
    // Arrange
    var name = "Existing Category";
    var description = "Test description";

    _mockCategoryRepository.Setup(x => x.ExistsByNameAsync(name))
        .ReturnsAsync(true);

    // Act
    var result = await _categoryService.ValidateCategoryDataAsync(name, description);

    // Assert
    result.Should().NotBeNull();
    result.Should().Contain("Category name already exists");
  }

  #endregion

  #region GetDeletableCategoriesAsync Tests

  [Fact]
  public async Task GetDeletableCategoriesAsync_ShouldReturnCategoriesWithNoPrompts()
  {
    // Arrange
    var categories = new List<Category>
        {
            new Category(1, "Empty Category", "No prompts"),
            new Category(2, "Used Category", "Has prompts")
        };

    _mockCategoryRepository.Setup(x => x.GetAllAsync())
        .ReturnsAsync(categories);
    _mockPromptRepository.Setup(x => x.CountByCategoryAsync(1))
        .ReturnsAsync(0); // No prompts
    _mockPromptRepository.Setup(x => x.CountByCategoryAsync(2))
        .ReturnsAsync(5); // Has prompts

    // Act
    var result = await _categoryService.GetDeletableCategoriesAsync();

    // Assert
    result.Should().NotBeNull();
    result.Should().HaveCount(1);
    result.First().Id.Should().Be(1);
  }

  #endregion

  #region CanDeleteCategoryAsync Tests

  [Fact]
  public async Task CanDeleteCategoryAsync_WithNoPrompts_ShouldReturnTrue()
  {
    // Arrange
    var categoryId = 1;

    _mockPromptRepository.Setup(x => x.CountByCategoryAsync(categoryId))
        .ReturnsAsync(0);

    // Act
    var result = await _categoryService.CanDeleteCategoryAsync(categoryId);

    // Assert
    result.Should().BeTrue();
    _mockPromptRepository.Verify(x => x.CountByCategoryAsync(categoryId), Times.Once);
  }

  [Fact]
  public async Task CanDeleteCategoryAsync_WithPrompts_ShouldReturnFalse()
  {
    // Arrange
    var categoryId = 1;

    _mockPromptRepository.Setup(x => x.CountByCategoryAsync(categoryId))
        .ReturnsAsync(5);

    // Act
    var result = await _categoryService.CanDeleteCategoryAsync(categoryId);

    // Assert
    result.Should().BeFalse();
    _mockPromptRepository.Verify(x => x.CountByCategoryAsync(categoryId), Times.Once);
  }

  #endregion

  #region GetTotalCategoryCountAsync Tests

  [Fact]
  public async Task GetTotalCategoryCountAsync_ShouldReturnTotalCount()
  {
    // Arrange
    var categories = new List<Category>
        {
            new Category(1, "Category 1", "Desc 1"),
            new Category(2, "Category 2", "Desc 2"),
            new Category(3, "Category 3", "Desc 3")
        };

    _mockCategoryRepository.Setup(x => x.GetAllAsync())
        .ReturnsAsync(categories);

    // Act
    var result = await _categoryService.GetTotalCategoryCountAsync();

    // Assert
    result.Should().Be(3);
    _mockCategoryRepository.Verify(x => x.GetAllAsync(), Times.Once);
  }

  #endregion

  #region GetActiveCategoryCountAsync Tests

  [Fact]
  public async Task GetActiveCategoryCountAsync_ShouldReturnActiveCount()
  {
    // Arrange
    var activeCategories = new List<Category>
    {
        CreateActiveCategory(1, "Active 1", "Desc 1"),
        CreateActiveCategory(2, "Active 2", "Desc 2")
    };

    _mockCategoryRepository.Setup(x => x.GetActiveCategoriesAsync())
        .ReturnsAsync(activeCategories);

    // Act
    var result = await _categoryService.GetActiveCategoryCountAsync();

    // Assert
    result.Should().Be(2);
    _mockCategoryRepository.Verify(x => x.GetActiveCategoriesAsync(), Times.Once);
  }

  #endregion

  #region Business Rule Tests

  [Fact]
  public async Task CreateCategoryAsync_ShouldTrimWhitespaceFromNameAndDescription()
  {
    // Arrange
    var name = "  Test Category  ";
    var description = "  Test description  ";

    var expectedCategory = new Category(1, name.Trim(), description.Trim());
    _mockCategoryRepository.Setup(x => x.AddAsync(It.IsAny<Category>()))
        .ReturnsAsync(expectedCategory);
    _mockCategoryRepository.Setup(x => x.ExistsByNameAsync(name.Trim()))
        .ReturnsAsync(false);

    // Act
    var result = await _categoryService.CreateCategoryAsync(name, description);

    // Assert
    result.Name.Should().Be("Test Category");
    result.Description.Should().Be("Test description");
  }

  [Fact]
  public async Task CreateCategoryAsync_ShouldSetCreatedAndUpdatedDatesToUtcNow()
  {
    // Arrange
    var name = "Test Category";
    var description = "Test description";
    var beforeCreation = DateTime.UtcNow;

    var expectedCategory = new Category(1, name, description);
    _mockCategoryRepository.Setup(x => x.AddAsync(It.IsAny<Category>()))
        .ReturnsAsync(expectedCategory);
    _mockCategoryRepository.Setup(x => x.ExistsByNameAsync(name))
        .ReturnsAsync(false);

    // Act
    var result = await _categoryService.CreateCategoryAsync(name, description);

    // Assert
    result.CreatedAt.Should().BeCloseTo(beforeCreation, TimeSpan.FromSeconds(1));
    result.UpdatedAt.Should().BeCloseTo(beforeCreation, TimeSpan.FromSeconds(1));
  }

  private Category CreateActiveCategory(int id, string name, string description)
  {
    var category = new Category(id, name, description);
    // Use reflection to set IsActive since the setter is private
    var isActiveProperty = typeof(Category).GetProperty("IsActive");
    if (isActiveProperty != null && isActiveProperty.CanWrite)
    {
      isActiveProperty.SetValue(category, true);
    }
    return category;
  }

  private Category CreateInactiveCategory(int id, string name, string description)
  {
    var category = new Category(id, name, description);
    // Use reflection to set IsActive since the setter is private
    var isActiveProperty = typeof(Category).GetProperty("IsActive");
    if (isActiveProperty != null && isActiveProperty.CanWrite)
    {
      isActiveProperty.SetValue(category, false);
    }
    return category;
  }

  [Fact]
  public async Task UpdateCategoryAsync_ShouldUpdateUpdatedAtTimestamp()
  {
    // Arrange
    var categoryId = 1;
    var name = "Updated Category";
    var description = "Updated description";
    var beforeUpdate = DateTime.UtcNow;

    var existingCategory = new Category(categoryId, "Original Name", "Original Description");
    _mockCategoryRepository.Setup(x => x.GetByIdAsync(categoryId))
        .ReturnsAsync(existingCategory);
    _mockCategoryRepository.Setup(x => x.UpdateAsync(It.IsAny<Category>()))
        .ReturnsAsync((Category c) => c);
    _mockCategoryRepository.Setup(x => x.ExistsByNameAsync(name))
        .ReturnsAsync(false);

    // Act
    var result = await _categoryService.UpdateCategoryAsync(categoryId, name, description);

    // Assert
    result.UpdatedAt.Should().BeAfter(beforeUpdate);
  }

  #endregion
}