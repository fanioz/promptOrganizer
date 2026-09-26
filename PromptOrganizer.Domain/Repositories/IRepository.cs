using PromptOrganizer.Domain.Common;
using System.Linq.Expressions;

namespace PromptOrganizer.Domain.Repositories;

/// <summary>
/// Generic repository interface for basic CRUD operations.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
public interface IRepository<TEntity> where TEntity : IEntity
{
  /// <summary>
  /// Gets an entity by its ID.
  /// </summary>
  /// <param name="id">The entity ID.</param>
  /// <returns>The entity or null if not found.</returns>
  Task<TEntity?> GetByIdAsync(int id);

  /// <summary>
  /// Gets all entities.
  /// </summary>
  /// <returns>A collection of all entities.</returns>
  Task<IEnumerable<TEntity>> GetAllAsync();

  /// <summary>
  /// Finds entities based on a predicate.
  /// </summary>
  /// <param name="predicate">The search predicate.</param>
  /// <returns>A collection of matching entities.</returns>
  Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);

  /// <summary>
  /// Adds a new entity.
  /// </summary>
  /// <param name="entity">The entity to add.</param>
  /// <returns>The added entity.</returns>
  Task<TEntity> AddAsync(TEntity entity);

  /// <summary>
  /// Updates an existing entity.
  /// </summary>
  /// <param name="entity">The entity to update.</param>
  /// <returns>The updated entity.</returns>
  Task<TEntity> UpdateAsync(TEntity entity);

  /// <summary>
  /// Deletes an entity by its ID.
  /// </summary>
  /// <param name="id">The entity ID.</param>
  /// <returns>True if the entity was deleted; otherwise, false.</returns>
  Task<bool> DeleteAsync(int id);

  /// <summary>
  /// Checks if an entity exists by its ID.
  /// </summary>
  /// <param name="id">The entity ID.</param>
  /// <returns>True if the entity exists; otherwise, false.</returns>
  Task<bool> ExistsAsync(int id);

  /// <summary>
  /// Gets the total count of entities.
  /// </summary>
  /// <returns>The total count of entities.</returns>
  Task<int> CountAsync();
}