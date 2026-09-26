using FluentAssertions;
using PromptOrganizer.Domain.Entities;
using PromptOrganizer.Tests.TestHelpers;
using Xunit;

namespace PromptOrganizer.Tests.Data.Repositories;

public class DebugTests : RepositoryTestBase
{
  [Fact]
  public async Task Debug_BasicCategoryOperations()
  {
    // Arrange
    var category = new Category(0, "Test Category", "Test description");

    // Act - Add
    var addedCategory = await CategoryRepository.AddAsync(category);
    Console.WriteLine($"Added category ID: {addedCategory.Id}");

    // Act - Get by ID
    var retrievedCategory = await CategoryRepository.GetByIdAsync(addedCategory.Id);
    Console.WriteLine($"Retrieved category: {retrievedCategory?.Name ?? "null"}");

    // Act - Get all
    var allCategories = await CategoryRepository.GetAllAsync();
    Console.WriteLine($"Total categories: {allCategories.Count()}");

    // Assert
    addedCategory.Should().NotBeNull();
    addedCategory.Id.Should().BeGreaterThan(0);
    retrievedCategory.Should().NotBeNull();
    retrievedCategory.Name.Should().Be("Test Category");
    allCategories.Should().HaveCount(1);
  }

  [Fact]
  public async Task Debug_TestIsolation()
  {
    // Act - Get all (should be empty after previous test)
    var allCategories = await CategoryRepository.GetAllAsync();
    Console.WriteLine($"Categories at start of test: {allCategories.Count()}");

    // Assert - Should be empty due to test isolation
    allCategories.Should().BeEmpty();
  }
}