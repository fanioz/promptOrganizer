using System;
using System.Collections.Generic;
using System.Linq;
using PromptOrganizer.Domain.Entities;

namespace PromptOrganizer.Tests.TestHelpers
{
  /// <summary>
  /// Builder pattern for creating test data with fluent interface
  /// </summary>
  internal class PromptBuilder
  {
    private int _id = 1;
    private string _title = "Default Test Prompt";
    private string _content = "Default test prompt content";
    private string _description = "Default description";
    private int? _categoryId = 1;
    private string _categoryName = "";
    private string _tags = "test,default";
    private bool _isActive = true;
    private DateTime _createdAt = DateTime.UtcNow;
    private DateTime? _updatedAt;
    private Category? _category;

    public PromptBuilder WithId(int id)
    {
      _id = id;
      return this;
    }

    public PromptBuilder WithTitle(string title)
    {
      _title = title;
      return this;
    }

    public PromptBuilder WithContent(string content)
    {
      _content = content;
      return this;
    }

    public PromptBuilder WithDescription(string description)
    {
      _description = description;
      return this;
    }

    public PromptBuilder WithCategory(int? categoryId)
    {
      _categoryId = categoryId;
      return this;
    }

    public PromptBuilder WithCategory(Category category)
    {
      _category = category;
      _categoryId = category?.Id;
      return this;
    }

    public PromptBuilder WithCategoryName(string categoryName)
    {
      _categoryName = categoryName;
      return this;
    }

    public PromptBuilder WithTags(string tags)
    {
      _tags = tags;
      return this;
    }

    public PromptBuilder AsActive()
    {
      _isActive = true;
      return this;
    }

    public PromptBuilder AsInactive()
    {
      _isActive = false;
      return this;
    }

    public PromptBuilder WithCreatedAt(DateTime createdAt)
    {
      _createdAt = createdAt;
      return this;
    }

    public PromptBuilder WithUpdatedAt(DateTime? updatedAt)
    {
      _updatedAt = updatedAt;
      return this;
    }

    public Prompt Build()
    {
      if (_category != null)
      {
        return new Prompt(_id, _title, _content, _description, _category, _tags)
        {
          Id = _id
        };
      }
      else if (!string.IsNullOrEmpty(_categoryName))
      {
        return new Prompt(_id, _title, _content, _description, _categoryName, _tags);
      }
      else
      {
        return new Prompt(_id, _title, _content, _description, _categoryId, _tags);
      }
    }

    public object ToPromptData()
    {
      return new
      {
        Title = _title,
        Content = _content,
        Description = _description,
        CategoryId = _categoryId,
        Tags = _tags
      };
    }
  }

  /// <summary>
  /// Builder for Category entities
  /// </summary>
  internal class CategoryBuilder
  {
    private int _id = 1;
    private string _name = "Default Category";
    private string _description = "Default category description";
    private bool _isActive = true;
    private DateTime _createdAt = DateTime.UtcNow;

    public CategoryBuilder WithId(int id)
    {
      _id = id;
      return this;
    }

    public CategoryBuilder WithName(string name)
    {
      _name = name;
      return this;
    }

    public CategoryBuilder WithDescription(string description)
    {
      _description = description;
      return this;
    }

    public CategoryBuilder AsInactive()
    {
      _isActive = false;
      return this;
    }

    public CategoryBuilder WithCreatedAt(DateTime createdAt)
    {
      _createdAt = createdAt;
      return this;
    }

    public Category Build()
    {
      var category = new Category(_id, _name, _description)
      {
        Id = _id
      };

      if (!_isActive)
      {
        category.Deactivate();
      }

      return category;
    }
  }

  /// <summary>
  /// Builder for Tag entities
  /// </summary>
  internal class TagBuilder
  {
    private int _id = 1;
    private string _name = "default-tag";
    private string _description = "";
    private string _color = "#000000";
    private bool _isActive = true;
    private DateTime _createdAt = DateTime.UtcNow;

    public TagBuilder WithId(int id)
    {
      _id = id;
      return this;
    }

    public TagBuilder WithName(string name)
    {
      _name = name?.ToUpperInvariant().Replace(" ", "-", StringComparison.Ordinal) ?? "default-tag";
      return this;
    }

    public TagBuilder WithDescription(string description)
    {
      _description = description ?? "";
      return this;
    }

    public TagBuilder WithColor(string color)
    {
      _color = color;
      return this;
    }

    public TagBuilder AsInactive()
    {
      _isActive = false;
      return this;
    }

    public TagBuilder WithCreatedAt(DateTime createdAt)
    {
      _createdAt = createdAt;
      return this;
    }

    public Tag Build()
    {
      var tag = new Tag(_id, _name, _description, _color)
      {
        Id = _id
      };

      if (!_isActive)
      {
        tag.Deactivate();
      }

      return tag;
    }
  }

  /// <summary>
  /// Factory for creating common test data scenarios
  /// </summary>
  internal static class TestDataFactory
  {
    /// <summary>
    /// Creates a valid prompt with default values
    /// </summary>
    public static Prompt CreateValidPrompt(int id = 1)
    {
      return new PromptBuilder()
          .WithId(id)
          .WithTitle($"Test Prompt {id}")
          .WithContent($"Test content for prompt {id}")
          .WithDescription($"Description for prompt {id}")
          .Build();
    }

    /// <summary>
    /// Creates a list of valid prompts
    /// </summary>
    public static List<Prompt> CreatePrompts(int count, int startId = 1)
    {
      return Enumerable.Range(startId, count)
          .Select(id => CreateValidPrompt(id))
          .ToList();
    }

    /// <summary>
    /// Creates prompts with different statuses
    /// </summary>
    public static List<Prompt> CreatePromptsWithMixedStatuses(int countPerStatus = 5)
    {
      var prompts = new List<Prompt>();

      // Active prompts
      prompts.AddRange(Enumerable.Range(1, countPerStatus)
          .Select(i => new PromptBuilder()
              .WithId(i)
              .WithTitle($"Active Prompt {i}")
              .AsActive()
              .Build()));

      // Inactive prompts
      prompts.AddRange(Enumerable.Range(countPerStatus + 1, countPerStatus)
          .Select(i => new PromptBuilder()
              .WithId(i)
              .WithTitle($"Inactive Prompt {i}")
              .AsInactive()
              .Build()));

      return prompts;
    }

    /// <summary>
    /// Creates a valid category
    /// </summary>
    public static Category CreateValidCategory(int id = 1, string? name = null)
    {
      return new CategoryBuilder()
          .WithId(id)
          .WithName(name ?? $"Category {id}")
          .WithDescription($"Description for category {id}")
          .Build();
    }

    /// <summary>
    /// Creates a list of categories
    /// </summary>
    public static List<Category> CreateCategories(int count, int startId = 1)
    {
      return Enumerable.Range(startId, count)
          .Select(id => CreateValidCategory(id))
          .ToList();
    }

    /// <summary>
    /// Creates a valid tag
    /// </summary>
    public static Tag CreateValidTag(int id = 1, string? name = null)
    {
      return new TagBuilder()
          .WithId(id)
          .WithName(name ?? $"tag-{id}")
          .Build();
    }

    /// <summary>
    /// Creates a list of tags
    /// </summary>
    public static List<Tag> CreateTags(int count, int startId = 1)
    {
      return Enumerable.Range(startId, count)
          .Select(id => CreateValidTag(id))
          .ToList();
    }

    /// <summary>
    /// Creates invalid prompt data for testing validation
    /// </summary>
    public static IEnumerable<object> CreateInvalidPromptData()
    {
      // Null title
      yield return new { Title = (string?)null, Content = "Content" };

      // Empty title
      yield return new { Title = "", Content = "Content" };

      // Whitespace title
      yield return new { Title = "   ", Content = "Content" };

      // Title too long
      yield return new { Title = new string('A', 201), Content = "Content" };

      // Null content
      yield return new { Title = "Valid Title", Content = (string?)null };

      // Content too long
      yield return new { Title = "Valid Title", Content = new string('X', 5001) };

      // Invalid category ID
      yield return new { Title = "Valid Title", Content = "Content", CategoryId = -1 };
    }

    /// <summary>
    /// Creates edge case data for boundary testing
    /// </summary>
    public static IEnumerable<object> CreateEdgeCasePromptData()
    {
      // Minimum valid title (1 character after trim)
      yield return new { Title = "A", Content = "Content" };

      // Maximum valid title (200 characters)
      yield return new { Title = new string('A', 200), Content = "Content" };

      // Minimum valid content (1 character)
      yield return new { Title = "Valid Title", Content = "X" };

      // Maximum valid content (5000 characters)
      yield return new { Title = "Valid Title", Content = new string('X', 5000) };

      // Title with special characters
      yield return new { Title = "Title with @#$%^&*()", Content = "Content" };

      // Content with special characters and newlines
      yield return new
      {
        Title = "Valid Title",
        Content = "Line 1\nLine 2\r\nLine 3\tTabbed"
      };
    }
  }

  /// <summary>
  /// Helper for creating test data with specific scenarios
  /// </summary>
  internal static class ScenarioBuilder
  {
    /// <summary>
    /// Creates a complete scenario with categories, prompts, and tags
    /// </summary>
    public static TestScenario CreateCompleteScenario()
    {
      var categories = new List<Category>
      {
          TestDataFactory.CreateValidCategory(1, "Development"),
          TestDataFactory.CreateValidCategory(2, "Testing")
      };

      var prompts = new List<Prompt>
            {
                new PromptBuilder()
                    .WithId(1)
                    .WithTitle("React Component Template")
                    .WithContent("Create a React component with props and state...")
                    .WithCategory(categories[0])
                    .Build(),

                new PromptBuilder()
                    .WithId(2)
                    .WithTitle("API Endpoint Template")
                    .WithContent("Create a REST API endpoint with validation...")
                    .WithCategory(categories[0])
                    .Build()
            };

      var tags = TestDataFactory.CreateTags(5);

      return new TestScenario
      {
        Categories = categories,
        Prompts = prompts,
        Tags = tags
      };
    }
  }

  internal class TestScenario
  {
    public IReadOnlyList<Category> Categories { get; set; } = Array.Empty<Category>();
    public IReadOnlyList<Prompt> Prompts { get; set; } = Array.Empty<Prompt>();
    public IReadOnlyList<Tag> Tags { get; set; } = Array.Empty<Tag>();
  }
}