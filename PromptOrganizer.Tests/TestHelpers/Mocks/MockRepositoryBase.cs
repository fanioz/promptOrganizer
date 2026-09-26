using Moq;
using System.Linq.Expressions;

namespace PromptOrganizer.Tests.TestHelpers.Mocks;

/// <summary>
/// Base class for mock repositories providing common functionality.
/// </summary>
/// <typeparam name="TEntity">The type of entity the repository works with.</typeparam>
/// <typeparam name="TKey">The type of the entity's key.</typeparam>
public abstract class MockRepositoryBase<TEntity, TKey> where TEntity : class
{
  protected Mock<MockRepositoryBase<TEntity, TKey>> Mock { get; }

  protected MockRepositoryBase()
  {
    Mock = new Mock<MockRepositoryBase<TEntity, TKey>>();
  }

  /// <summary>
  /// Sets up the mock to return a specific entity when GetById is called.
  /// </summary>
  public virtual MockRepositoryBase<TEntity, TKey> SetupGetById(TKey id, TEntity? entity)
  {
    Mock.Setup(x => x.GetById(It.Is<TKey>(k => k.Equals(id))))
        .Returns(entity);
    return this;
  }

  /// <summary>
  /// Sets up the mock to return a specific list of entities when GetAll is called.
  /// </summary>
  public virtual MockRepositoryBase<TEntity, TKey> SetupGetAll(List<TEntity> entities)
  {
    Mock.Setup(x => x.GetAll())
        .Returns(entities);
    return this;
  }

  /// <summary>
  /// Sets up the mock to return a specific list of entities when Find is called with a predicate.
  /// </summary>
  public virtual MockRepositoryBase<TEntity, TKey> SetupFind(Expression<Func<TEntity, bool>> predicate, List<TEntity> entities)
  {
    Mock.Setup(x => x.Find(It.Is<Expression<Func<TEntity, bool>>>(p => p == predicate)))
        .Returns(entities);
    return this;
  }

  /// <summary>
  /// Sets up the mock to verify that Add was called with a specific entity.
  /// </summary>
  public virtual MockRepositoryBase<TEntity, TKey> VerifyAdd(TEntity entity, Times times)
  {
    Mock.Verify(x => x.Add(It.Is<TEntity>(e => e == entity)), times);
    return this;
  }

  /// <summary>
  /// Sets up the mock to verify that Update was called with a specific entity.
  /// </summary>
  public virtual MockRepositoryBase<TEntity, TKey> VerifyUpdate(TEntity entity, Times times)
  {
    Mock.Verify(x => x.Update(It.Is<TEntity>(e => e == entity)), times);
    return this;
  }

  /// <summary>
  /// Sets up the mock to verify that Delete was called with a specific id.
  /// </summary>
  public virtual MockRepositoryBase<TEntity, TKey> VerifyDelete(TKey id, Times times)
  {
    Mock.Verify(x => x.Delete(It.Is<TKey>(k => k.Equals(id))), times);
    return this;
  }

  /// <summary>
  /// Builds the mock object.
  /// </summary>
  public virtual Mock<MockRepositoryBase<TEntity, TKey>> Build()
  {
    return Mock;
  }

  // Mock interface methods that can be overridden in derived classes
  public virtual TEntity? GetById(TKey id) => Mock.Object.GetById(id);
  public virtual List<TEntity> GetAll() => Mock.Object.GetAll();
  public virtual List<TEntity> Find(Expression<Func<TEntity, bool>> predicate) => Mock.Object.Find(predicate);
  public virtual void Add(TEntity entity) => Mock.Object.Add(entity);
  public virtual void Update(TEntity entity) => Mock.Object.Update(entity);
  public virtual void Delete(TKey id) => Mock.Object.Delete(id);
}