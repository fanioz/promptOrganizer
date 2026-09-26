namespace PromptOrganizer.Tests.TestHelpers.Builders;

/// <summary>
/// Base class for test data builders providing common functionality.
/// </summary>
/// <typeparam name="TEntity">The type of entity being built.</typeparam>
/// <typeparam name="TBuilder">The type of the builder itself for fluent interface.</typeparam>
public abstract class BuilderBase<TEntity, TBuilder> where TBuilder : BuilderBase<TEntity, TBuilder>
{
  protected TEntity _entity;

  protected BuilderBase()
  {
    _entity = CreateDefault();
  }

  /// <summary>
  /// Creates a default instance of the entity.
  /// </summary>
  protected abstract TEntity CreateDefault();

  /// <summary>
  /// Builds the entity with the current configuration.
  /// </summary>
  public virtual TEntity Build()
  {
    return _entity;
  }

  /// <summary>
  /// Builds a list of entities with the current configuration.
  /// </summary>
  public virtual List<TEntity> BuildList(int count)
  {
    var list = new List<TEntity>();
    for (int i = 0; i < count; i++)
    {
      list.Add(Build());
    }
    return list;
  }

  /// <summary>
  /// Resets the builder to its default state.
  /// </summary>
  public virtual TBuilder Reset()
  {
    _entity = CreateDefault();
    return (TBuilder)this;
  }

  /// <summary>
  /// Creates a new builder instance with default values.
  /// </summary>
  public static TBuilder New() => Activator.CreateInstance<TBuilder>();

  /// <summary>
  /// Creates a new builder instance with default values.
  /// </summary>
  public static TBuilder Create() => New();
}