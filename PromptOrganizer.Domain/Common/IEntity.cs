namespace PromptOrganizer.Domain.Common;

/// <summary>
/// Base interface for all entities in the domain.
/// </summary>
public interface IEntity
{
  /// <summary>
  /// Gets or sets the unique identifier of the entity.
  /// </summary>
  int Id { get; set; }
}