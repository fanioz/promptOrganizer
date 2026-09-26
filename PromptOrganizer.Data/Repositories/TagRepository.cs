using LiteDB;
using PromptOrganizer.Domain.Entities;
using PromptOrganizer.Domain.Repositories;
using System.Linq.Expressions;

namespace PromptOrganizer.Data.Repositories;

/// <summary>
/// LiteDB implementation of ITagRepository.
/// </summary>
public class TagRepository : BaseRepository<Tag>, ITagRepository
{
  public TagRepository(ILiteDatabase database) : base(database, "tags")
  {
    // Ensure indexes for better query performance
    _collection.EnsureIndex(x => x.Name);
    _collection.EnsureIndex(x => x.Color);
    _collection.EnsureIndex(x => x.IsActive);
  }

  public async Task<Tag?> GetByNameAsync(string name)
  {
    if (string.IsNullOrWhiteSpace(name))
      return null;

    return await Task.Run(() =>
        _collection.Find(x => x.Name.ToLower() == name.ToLower()).FirstOrDefault());
  }

  public async Task<IEnumerable<Tag>> GetActiveTagsAsync()
  {
    return await FindAsync(x => x.IsActive);
  }

  public async Task<IEnumerable<Tag>> GetByColorAsync(string color)
  {
    if (string.IsNullOrWhiteSpace(color))
      return Enumerable.Empty<Tag>();

    return await Task.Run(() =>
        _collection.Find(x => x.Color.ToLower() == color.ToLower()).ToList());
  }

  public async Task<IEnumerable<Tag>> SearchAsync(string searchTerm)
  {
    if (string.IsNullOrWhiteSpace(searchTerm))
      return await GetAllAsync();

    var lowerSearchTerm = searchTerm.ToLower();
    return await FindAsync(x =>
        x.Name.ToLower().Contains(lowerSearchTerm) ||
        x.Description.ToLower().Contains(lowerSearchTerm));
  }

  public async Task<bool> ExistsByNameAsync(string name)
  {
    if (string.IsNullOrWhiteSpace(name))
      return false;

    return await Task.Run(() =>
        _collection.Exists(x => x.Name.ToLower() == name.ToLower()));
  }

  public async Task<IEnumerable<Tag>> GetByNamesAsync(IEnumerable<string> names)
  {
    var nameList = names.ToList();
    if (!nameList.Any())
      return Enumerable.Empty<Tag>();

    return await Task.Run(() =>
    {
      var lowerNames = nameList.Select(n => n.ToLower()).ToList();
      return _collection.Find(x => lowerNames.Contains(x.Name.ToLower())).ToList();
    });
  }
}