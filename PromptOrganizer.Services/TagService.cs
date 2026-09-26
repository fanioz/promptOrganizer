using PromptOrganizer.Domain.Entities;
using PromptOrganizer.Domain.Repositories;

namespace PromptOrganizer.Services;

/// <summary>
/// Service implementation for tag business logic operations.
/// </summary>
public class TagService : ITagService
{
  private readonly ITagRepository _tagRepository;
  private readonly IPromptRepository _promptRepository;

  public TagService(ITagRepository tagRepository, IPromptRepository promptRepository)
  {
    _tagRepository = tagRepository ?? throw new ArgumentNullException(nameof(tagRepository));
    _promptRepository = promptRepository ?? throw new ArgumentNullException(nameof(promptRepository));
  }

  public async Task<Tag> CreateTagAsync(string name, string description, string color)
  {
    // Validate input data (excluding duplicate name check)
    var validationErrors = await ValidateTagDataAsync(name, description, color, null, false);
    if (validationErrors.Any())
    {
      throw new ArgumentException(string.Join("; ", validationErrors));
    }

    // Check for duplicate name
    if (await _tagRepository.ExistsByNameAsync(name))
    {
      throw new InvalidOperationException("Tag with this name already exists");
    }

    // Create the tag
    var tag = new Tag(0, name, description, color);
    return await _tagRepository.AddAsync(tag);
  }

  public async Task<Tag> UpdateTagAsync(int id, string name, string description, string color)
  {
    // Get existing tag
    var existingTag = await _tagRepository.GetByIdAsync(id);
    if (existingTag == null)
    {
      throw new KeyNotFoundException($"Tag with ID {id} not found.");
    }

    // Validate input data
    var validationErrors = await ValidateTagDataAsync(name, description, color, id);
    if (validationErrors.Any())
    {
      throw new ArgumentException(string.Join("; ", validationErrors));
    }

    // Update tag properties
    existingTag.UpdateDescription(description);
    existingTag.UpdateColor(color);

    return await _tagRepository.UpdateAsync(existingTag);
  }

  public async Task<Tag?> GetTagByIdAsync(int id)
  {
    return await _tagRepository.GetByIdAsync(id);
  }

  public async Task<Tag?> GetTagByNameAsync(string name)
  {
    return await _tagRepository.GetByNameAsync(name);
  }

  public async Task<IEnumerable<Tag>> GetAllActiveTagsAsync()
  {
    return await _tagRepository.GetActiveTagsAsync();
  }

  public async Task<IEnumerable<Tag>> GetTagsByColorAsync(string color)
  {
    return await _tagRepository.GetByColorAsync(color);
  }

  public async Task<IEnumerable<Tag>> SearchTagsAsync(string searchTerm)
  {
    if (string.IsNullOrWhiteSpace(searchTerm))
    {
      return Enumerable.Empty<Tag>();
    }

    return await _tagRepository.SearchAsync(searchTerm);
  }

  public async Task<bool> DeactivateTagAsync(int id)
  {
    var tag = await _tagRepository.GetByIdAsync(id);
    if (tag == null)
    {
      return false;
    }

    // Check if tag can be deactivated (has no active prompts)
    var promptsWithTag = await _promptRepository.GetByTagsAsync(new[] { tag.Name });
    if (promptsWithTag.Any())
    {
      throw new InvalidOperationException("Cannot deactivate tag that has associated prompts");
    }

    tag.Deactivate();
    await _tagRepository.UpdateAsync(tag);
    return true;
  }

  public async Task<bool> ActivateTagAsync(int id)
  {
    var tag = await _tagRepository.GetByIdAsync(id);
    if (tag == null)
    {
      return false;
    }

    tag.Activate();
    await _tagRepository.UpdateAsync(tag);
    return true;
  }

  public async Task<bool> TagNameExistsAsync(string name, int? excludeId = null)
  {
    if (excludeId.HasValue)
    {
      var existingTag = await _tagRepository.GetByNameAsync(name);
      return existingTag != null && existingTag.Id != excludeId.Value;
    }

    return await _tagRepository.ExistsByNameAsync(name);
  }

  public async Task<IEnumerable<string>> ValidateTagDataAsync(string name, string description, string color, int? excludeId = null, bool checkDuplicateName = true)
  {
    var errors = new List<string>();

    // Validate name
    if (string.IsNullOrWhiteSpace(name))
    {
      errors.Add("Name cannot be null or empty");
    }
    else if (name.Length > 50)
    {
      errors.Add("Name cannot exceed 50 characters");
    }

    // Validate description
    if (description != null && description.Length > 200)
    {
      errors.Add("Description cannot exceed 200 characters");
    }

    // Validate color
    if (!string.IsNullOrWhiteSpace(color) && !System.Text.RegularExpressions.Regex.IsMatch(color, @"^#[0-9A-Fa-f]{6}$"))
    {
      errors.Add("Color must be in hex format (#RRGGBB)");
    }

    // Check for duplicate name
    if (checkDuplicateName && !string.IsNullOrWhiteSpace(name) && await TagNameExistsAsync(name, excludeId))
    {
      errors.Add("Tag name already exists");
    }

    return errors;
  }

  public async Task<IEnumerable<Tag>> GetDeletableTagsAsync()
  {
    var allTags = await _tagRepository.GetAllAsync();
    var deletableTags = new List<Tag>();

    foreach (var tag in allTags)
    {
      if (await CanDeleteTagAsync(tag.Id))
      {
        deletableTags.Add(tag);
      }
    }

    return deletableTags;
  }

  public async Task<bool> CanDeleteTagAsync(int tagId)
  {
    var tag = await _tagRepository.GetByIdAsync(tagId);
    if (tag == null)
    {
      return false;
    }

    var promptsWithTag = await _promptRepository.GetByTagsAsync(new[] { tag.Name });
    return !promptsWithTag.Any();
  }

  public async Task<int> GetTotalTagCountAsync()
  {
    var allTags = await _tagRepository.GetAllAsync();
    return allTags.Count();
  }

  public async Task<int> GetActiveTagCountAsync()
  {
    var activeTags = await _tagRepository.GetActiveTagsAsync();
    return activeTags.Count();
  }

  public async Task<IEnumerable<Tag>> GetTagsByNamesAsync(IEnumerable<string> names)
  {
    return await _tagRepository.GetByNamesAsync(names);
  }

  public async Task<IEnumerable<Tag>> GetPopularTagsAsync(int limit = 10)
  {
    // For now, return all active tags sorted by name
    // In a real implementation, this would be based on usage statistics
    var allTags = await _tagRepository.GetAllAsync();
    return allTags
        .Where(t => t.IsActive)
        .OrderBy(t => t.Name)
        .Take(limit);
  }

  public async Task<IEnumerable<Tag>> GetSimilarTagsAsync(int tagId, int limit = 5)
  {
    // Get the source tag
    var sourceTag = await _tagRepository.GetByIdAsync(tagId);
    if (sourceTag == null)
    {
      return Enumerable.Empty<Tag>();
    }

    // For now, return tags with the same color
    // In a real implementation, this would be based on semantic similarity
    var similarTags = await _tagRepository.GetByColorAsync(sourceTag.Color);
    return similarTags
        .Where(t => t.Id != tagId && t.IsActive)
        .Take(limit);
  }
}