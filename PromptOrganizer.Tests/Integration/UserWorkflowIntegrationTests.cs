using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using PromptOrganizer.Data.Repositories;
using PromptOrganizer.Domain.Entities;
using PromptOrganizer.Services;
using PromptOrganizer.Tests.TestHelpers;
using Xunit;
using Xunit.Abstractions;

namespace PromptOrganizer.Tests.Integration;

/// <summary>
/// Integration test for the new user prompt creation workflow.
/// Tests the complete data flow from service layer to encrypted database storage.
/// </summary>
[Collection("DatabaseCollection")]
public class UserWorkflowIntegrationTests : IDisposable
{
  private readonly ITestOutputHelper _output;
  private readonly PromptService _promptService;
  private readonly CategoryService _categoryService;
  private readonly string _testDatabasePath;
  private bool _disposed;

  public UserWorkflowIntegrationTests(ITestOutputHelper output)
  {
    _output = output;
    _testDatabasePath = Path.Combine(Path.GetTempPath(), $"test_workflow_{Guid.NewGuid()}.db");

    // Initialize services with real database - use file-based database for encryption testing
    var connectionString = $"Filename={_testDatabasePath};Password=test_encryption_key;";
    var database = new LiteDatabase(connectionString);
    try
    {
      var promptRepository = new PromptRepository(database);
      var categoryRepository = new CategoryRepository(database);

      _promptService = new PromptService(promptRepository, categoryRepository);
      _categoryService = new CategoryService(categoryRepository, promptRepository);
    }
    catch
    {
      database.Dispose();
      throw;
    }
  }

  [Fact]
  public async Task NewUserCreatesFirstPromptCompleteDataWorkflowShouldSucceed()
  {
    // Arrange
    _output.WriteLine("=== Starting New User Prompt Creation Workflow Integration Test ===");

    // Verify initial state - no prompts exist
    var initialPrompts = await _promptService.SearchPromptsAsync("");
    Assert.Empty(initialPrompts);
    _output.WriteLine("✓ Initial state verified - no prompts in database");

    // Act & Assert - Test 1: Create prompt through service layer (simulating UI save)
    await TestPromptCreationWorkflow();

    // Act & Assert - Test 2: Verify encrypted storage in database
    await TestEncryptedStorage();

    // Act & Assert - Test 3: Test search functionality
    await TestSearchFunctionality();

    // Act & Assert - Test 4: Test error handling for invalid data
    await TestErrorHandling();

    // Act & Assert - Test 5: Test multiple prompt creation
    await TestMultiplePromptCreation();

    _output.WriteLine("=== All workflow integration tests completed successfully ===");
  }

  private async Task TestPromptCreationWorkflow()
  {
    _output.WriteLine("Testing prompt creation workflow...");

    // Simulate user creating a prompt through the UI
    var title = "My First AI Prompt";
    var content = "This is a comprehensive prompt for generating AI responses. It includes context, instructions, and expected output format.";
    var description = "A template for AI-generated content";
    var categoryName = "AI Templates";
    var tags = "ai, template, productivity, gpt";

    // Create category if it doesn't exist
    var category = await _categoryService.GetCategoryByNameAsync(categoryName).ConfigureAwait(false);
    if (category == null)
    {
      category = await _categoryService.CreateCategoryAsync(categoryName, "Templates for AI-related tasks").ConfigureAwait(false);
    }

    // Create the prompt (simulating Save button click)
    var createdPrompt = await _promptService.CreatePromptAsync(title, content, description, category.Id, tags).ConfigureAwait(false);

    // Verify the prompt was created successfully
    Assert.NotNull(createdPrompt);
    Assert.True(createdPrompt.Id > 0, "Prompt should have a valid ID");
    Assert.Equal(title, createdPrompt.Title);
    Assert.Equal(content, createdPrompt.Content);
    Assert.Equal(description, createdPrompt.Description);
    Assert.Equal(category.Id, createdPrompt.CategoryId);
    Assert.Equal(tags, createdPrompt.Tags);
    Assert.True(createdPrompt.CreatedAt > DateTime.MinValue, "CreatedAt should be set");
    Assert.True(createdPrompt.UpdatedAt > DateTime.MinValue, "UpdatedAt should be set");

    _output.WriteLine($"✓ Prompt created successfully with ID: {createdPrompt.Id}");
  }

  private async Task TestEncryptedStorage()
  {
    _output.WriteLine("Testing encrypted storage in database...");

    // Verify prompt exists in database
    var prompts = await _promptService.SearchPromptsAsync("").ConfigureAwait(false);
    Assert.Single(prompts);

    var savedPrompt = prompts.First();
    Assert.Equal("My First AI Prompt", savedPrompt.Title);

    // Read database file directly to verify encryption
    var databaseContent = await File.ReadAllTextAsync(_testDatabasePath).ConfigureAwait(false);

    // Verify sensitive data is encrypted (not visible in plain text)
    Assert.False(databaseContent.Contains("My First AI Prompt", StringComparison.Ordinal),
        "Prompt title should be encrypted in database");
    Assert.False(databaseContent.Contains("This is a comprehensive prompt", StringComparison.Ordinal),
        "Prompt content should be encrypted in database");
    Assert.False(databaseContent.Contains("ai, template, productivity, gpt", StringComparison.Ordinal),
        "Tags should be encrypted in database");

    // Verify we can still retrieve the data (decryption works)
    var retrievedPrompt = await _promptService.GetPromptByIdAsync(savedPrompt.Id).ConfigureAwait(false);
    Assert.NotNull(retrievedPrompt);
    Assert.Equal("My First AI Prompt", retrievedPrompt.Title);
    Assert.Equal("This is a comprehensive prompt", retrievedPrompt.Content);

    _output.WriteLine("✓ Data encryption verified - sensitive data is encrypted at rest");
  }

  private async Task TestSearchFunctionality()
  {
    _output.WriteLine("Testing search functionality...");

    // Test search by title
    var titleResults = await _promptService.SearchPromptsAsync("First AI").ConfigureAwait(false);
    Assert.Single(titleResults);
    Assert.Equal("My First AI Prompt", titleResults.First().Title);

    // Test search by content
    var contentResults = await _promptService.SearchPromptsAsync("comprehensive prompt").ConfigureAwait(false);
    Assert.Single(contentResults);
    Assert.Equal("My First AI Prompt", contentResults.First().Title);

    // Test search by tags
    var tagResults = await _promptService.SearchPromptsAsync("productivity").ConfigureAwait(false);
    Assert.Single(tagResults);
    Assert.Equal("My First AI Prompt", tagResults.First().Title);

    // Test search with no results
    var noResults = await _promptService.SearchPromptsAsync("nonexistent").ConfigureAwait(false);
    Assert.Empty(noResults);

    _output.WriteLine("✓ Search functionality working correctly");
  }

  private async Task TestErrorHandling()
  {
    _output.WriteLine("Testing error handling for invalid data...");

    // Test empty title
    var emptyTitlePrompt = new Prompt("", "Content", "Description", 1, "tags");
    await Assert.ThrowsAsync<ArgumentException>(() =>
        _promptService.CreatePromptAsync(emptyTitlePrompt));

    // Test null title
    var nullTitlePrompt = new Prompt(null!, "Content", "Description", 1, "tags");
    await Assert.ThrowsAsync<ArgumentException>(() =>
        _promptService.CreatePromptAsync(nullTitlePrompt));

    // Test whitespace title
    var whitespaceTitlePrompt = new Prompt("   ", "Content", "Description", 1, "tags");
    await Assert.ThrowsAsync<ArgumentException>(() =>
        _promptService.CreatePromptAsync(whitespaceTitlePrompt));

    // Test invalid category ID
    var invalidCategoryPrompt = new Prompt("Valid Title", "Content", "Description", -1, "tags");
    await Assert.ThrowsAsync<ArgumentException>(() =>
        _promptService.CreatePromptAsync(invalidCategoryPrompt));

    _output.WriteLine("✓ Error handling working correctly");
  }

  private async Task TestMultiplePromptCreation()
  {
    _output.WriteLine("Testing multiple prompt creation...");

    // Create additional prompts to simulate real usage
    var promptData = new[]
    {
        (Title: "Email Template", Content: "Draft a professional email for the following situation...", Description: "Email drafting template", CategoryId: 1, Tags: "email, communication, template"),
        (Title: "Code Review", Content: "Review the following code and provide feedback...", Description: "Code review checklist", CategoryId: 1, Tags: "code, review, development"),
        (Title: "Meeting Notes", Content: "Take comprehensive notes for the meeting...", Description: "Meeting documentation template", CategoryId: 1, Tags: "meeting, notes, productivity")
    };

    foreach (var data in promptData)
    {
      var created = await _promptService.CreatePromptAsync(
          data.Title,
          data.Content,
          data.Description,
          data.CategoryId,
          data.Tags).ConfigureAwait(false);
      Assert.NotNull(created);
      Assert.True(created.Id > 0);
    }

    // Verify all prompts were created
    var allPrompts = await _promptService.SearchPromptsAsync("").ConfigureAwait(false);
    Assert.Equal(4, allPrompts.Count()); // Original + 3 new ones

    // Verify each prompt has correct data
    var emailPrompt = allPrompts.FirstOrDefault(p => p.Title == "Email Template");
    Assert.NotNull(emailPrompt);
    Assert.Contains("professional email", emailPrompt.Content, StringComparison.OrdinalIgnoreCase);

    var codeReviewPrompt = allPrompts.FirstOrDefault(p => p.Title == "Code Review");
    Assert.NotNull(codeReviewPrompt);
    Assert.Contains("provide feedback", codeReviewPrompt.Content, StringComparison.OrdinalIgnoreCase);

    var meetingNotesPrompt = allPrompts.FirstOrDefault(p => p.Title == "Meeting Notes");
    Assert.NotNull(meetingNotesPrompt);
    Assert.Contains("comprehensive notes", meetingNotesPrompt.Content, StringComparison.OrdinalIgnoreCase);

    _output.WriteLine($"✓ Successfully created {promptData.Length} additional prompts");
  }

  #region IDisposable Implementation

  public void Dispose()
  {
    Dispose(true);
    GC.SuppressFinalize(this);
  }

  protected virtual void Dispose(bool disposing)
  {
    if (!_disposed)
    {
      if (disposing)
      {
        // Clean up database file
        try
        {
          if (File.Exists(_testDatabasePath))
          {
            File.Delete(_testDatabasePath);
            _output.WriteLine($"Cleaned up test database: {_testDatabasePath}");
          }
        }
        catch (IOException ex)
        {
          _output.WriteLine($"Warning: Could not clean up test database: {ex.Message}");
        }
      }

      _disposed = true;
    }
  }

  #endregion
}