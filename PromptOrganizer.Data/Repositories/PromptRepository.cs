using LiteDB;
using PromptOrganizer.Domain.Entities;
using PromptOrganizer.Domain.Repositories;
using System.Linq.Expressions;

namespace PromptOrganizer.Data.Repositories;

/// <summary>
/// LiteDB implementation of IPromptRepository.
/// </summary>
public class PromptRepository : BaseRepository<Prompt>, IPromptRepository
{
  public PromptRepository(ILiteDatabase database) : base(database, "prompts")
  {
    // Ensure indexes for better query performance
    _collection.EnsureIndex(x => x.CategoryId);
    _collection.EnsureIndex(x => x.IsActive);
    _collection.EnsureIndex(x => x.CreatedAt);
    _collection.EnsureIndex(x => x.Tags);
  }

  public async Task<IEnumerable<Prompt>> GetByCategoryIdAsync(int categoryId)
  {
    return await FindAsync(x => x.CategoryId == categoryId);
  }

  public async Task<IEnumerable<Prompt>> GetActivePromptsAsync()
  {
    return await FindAsync(x => x.IsActive);
  }

  public async Task<IEnumerable<Prompt>> GetByTagsAsync(IEnumerable<string> tags)
  {
    var tagList = tags.ToList();
    if (!tagList.Any())
      return Enumerable.Empty<Prompt>();

    return await Task.Run(() =>
    {
      var prompts = _collection.FindAll().ToList();
      return prompts.Where(p =>
          {
          var promptTags = p.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                  .Select(t => t.Trim().ToLower());
          return tagList.Any(tag => promptTags.Contains(tag.ToLower()));
        }).ToList();
    });
  }

  public async Task<IEnumerable<Prompt>> SearchAsync(string searchTerm)
  {
    if (string.IsNullOrWhiteSpace(searchTerm))
      return await GetAllAsync();

    var lowerSearchTerm = searchTerm.ToLower();
    return await FindAsync(x =>
        x.Title.ToLower().Contains(lowerSearchTerm) ||
        x.Content.ToLower().Contains(lowerSearchTerm) ||
        x.Description.ToLower().Contains(lowerSearchTerm));
  }

  public async Task<IEnumerable<Prompt>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate)
  {
    return await FindAsync(x => x.CreatedAt >= fromDate && x.CreatedAt <= toDate);
  }

  public async Task<int> CountByCategoryAsync(int categoryId)
  {
    return await Task.Run(() => _collection.Count(x => x.CategoryId == categoryId));
  }
}