using LiteDB;
using PromptOrganizer.Domain.Common;
using PromptOrganizer.Domain.Repositories;
using System.Linq.Expressions;

namespace PromptOrganizer.Data.Repositories;

/// <summary>
/// Base repository implementation using LiteDB.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
public abstract class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : IEntity
{
  protected readonly ILiteDatabase _database;
  protected readonly ILiteCollection<TEntity> _collection;
  private readonly string _collectionName;

  protected BaseRepository(ILiteDatabase database, string collectionName)
  {
    _database = database ?? throw new ArgumentNullException(nameof(database));
    _collectionName = collectionName ?? throw new ArgumentNullException(nameof(collectionName));
    _collection = _database.GetCollection<TEntity>(collectionName);

    // Ensure index on Id for better performance
    _collection.EnsureIndex(x => x.Id);
  }

  public virtual async Task<TEntity?> GetByIdAsync(int id)
  {
    return await Task.Run(() => _collection.FindById(id));
  }

  public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
  {
    return await Task.Run(() => _collection.FindAll().ToList());
  }

  public virtual async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
  {
    return await Task.Run(() => _collection.Find(predicate).ToList());
  }

  public virtual async Task<TEntity> AddAsync(TEntity entity)
  {
    if (entity == null)
      throw new ArgumentNullException(nameof(entity));

    return await Task.Run(() =>
    {
      // Auto-generate ID if it's 0 (default)
      if (entity.Id == 0)
      {
        // Try to get the max ID, default to 0 if collection is empty
        var maxId = 0;
        try
        {
          maxId = _collection.Max(x => x.Id);
        }
        catch
        {
          // Collection is empty, start with ID 1
        }

        var newId = maxId == 0 ? 1 : maxId + 1;
        entity.Id = newId;
      }

      _collection.Insert(entity);
      return entity;
    });
  }

  public virtual async Task<TEntity> UpdateAsync(TEntity entity)
  {
    if (entity == null)
      throw new ArgumentNullException(nameof(entity));

    return await Task.Run(() =>
    {
      var updated = _collection.Update(entity);
      if (!updated)
        throw new InvalidOperationException($"Entity with ID {entity.Id} not found for update.");

      return entity;
    });
  }

  public virtual async Task<bool> DeleteAsync(int id)
  {
    return await Task.Run(() => _collection.Delete(id));
  }

  public virtual async Task<bool> ExistsAsync(int id)
  {
    return await Task.Run(() => _collection.Exists(x => x.Id == id));
  }

  public virtual async Task<int> CountAsync()
  {
    return await Task.Run(() => (int)_collection.Count());
  }
}