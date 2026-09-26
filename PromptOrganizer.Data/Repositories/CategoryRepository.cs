using LiteDB;
using PromptOrganizer.Domain.Entities;
using PromptOrganizer.Domain.Repositories;
using System.Linq.Expressions;

namespace PromptOrganizer.Data.Repositories;

/// <summary>
/// LiteDB implementation of ICategoryRepository.
/// </summary>
public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
{
  public CategoryRepository(ILiteDatabase database) : base(database, "categories")
  {
    // Ensure index for better query performance
    _collection.EnsureIndex(x => x.Name);
    _collection.EnsureIndex(x => x.IsActive);
  }

  public async Task<Category?> GetByNameAsync(string name)
  {
    if (string.IsNullOrWhiteSpace(name))
      return null;

    return await Task.Run(() =>
        _collection.Find(x => x.Name.ToLower() == name.ToLower()).FirstOrDefault());
  }

  public async Task<IEnumerable<Category>> GetActiveCategoriesAsync()
  {
    return await FindAsync(x => x.IsActive);
  }

  public async Task<IEnumerable<Category>> SearchAsync(string searchTerm)
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

  public async Task<IEnumerable<(Category Category, int PromptCount)>> GetCategoriesWithPromptCountsAsync()
  {
    return await Task.Run(() =>
    {
      var categories = _collection.FindAll().ToList();
      var promptCollection = _database.GetCollection<Prompt>("prompts");

      var result = categories.Select(category =>
          {
          var promptCount = promptCollection.Count(x => x.CategoryId == category.Id);
          return (category, promptCount);
        }).ToList();

      return result;
    });
  }
}