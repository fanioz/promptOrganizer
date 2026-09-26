using LiteDB;
using PromptOrganizer.Data.Repositories;
using PromptOrganizer.Domain.Entities;
using PromptOrganizer.Domain.Repositories;
using Xunit;

namespace PromptOrganizer.Tests.TestHelpers;

/// <summary>
/// Base class for repository tests that provides common setup and teardown.
/// </summary>
public abstract class RepositoryTestBase : BaseTest
{
  protected ILiteDatabase Database { get; private set; }
  protected IPromptRepository PromptRepository { get; private set; }
  protected ICategoryRepository CategoryRepository { get; private set; }
  protected ITagRepository TagRepository { get; private set; }

  protected override void Setup()
  {
    base.Setup();

    // Create fresh database for each test to ensure complete isolation
    Database = LiteDbTestFactory.CreateInMemoryDatabase();

    // Create repositories with fresh database
    PromptRepository = new PromptRepository(Database);
    CategoryRepository = new CategoryRepository(Database);
    TagRepository = new TagRepository(Database);
  }

  protected override void Cleanup()
  {
    // Dispose of the database to clean up resources and ensure no data leaks between tests
    Database?.Dispose();

    base.Cleanup();
  }

  /// <summary>
  /// Clears all data from the database to ensure test isolation.
  /// </summary>
  private void ClearDatabase()
  {
    // Drop all collections to ensure clean state
    var collectionNames = Database.GetCollectionNames().ToList();
    foreach (var collectionName in collectionNames)
    {
      Database.DropCollection(collectionName);
    }
  }

  /// <summary>
  /// Creates a test category.
  /// </summary>
  protected async Task<Category> CreateTestCategoryAsync(string name = "Test Category", string description = "Test description")
  {
    var category = new Category(0, name, description);
    return await CategoryRepository.AddAsync(category);
  }

  /// <summary>
  /// Creates a test tag.
  /// </summary>
  protected async Task<Tag> CreateTestTagAsync(string name = "Test Tag", string description = "Test description", string color = "#FF0000")
  {
    var tag = new Tag(0, name, description, color);
    return await TagRepository.AddAsync(tag);
  }

  /// <summary>
  /// Creates a test prompt.
  /// </summary>
  protected async Task<Prompt> CreateTestPromptAsync(string title = "Test Prompt", string content = "Test content", int? categoryId = null)
  {
    var prompt = new Prompt(0, title, content, "Test description", categoryId, "test,tag");
    return await PromptRepository.AddAsync(prompt);
  }
}