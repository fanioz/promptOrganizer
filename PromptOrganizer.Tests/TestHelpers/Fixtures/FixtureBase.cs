namespace PromptOrganizer.Tests.TestHelpers.Fixtures;

/// <summary>
/// Base class for test fixtures providing common functionality.
/// </summary>
public abstract class FixtureBase
{
  protected FixtureBase()
  {
    Initialize();
  }

  /// <summary>
  /// Initializes the fixture with test data.
  /// </summary>
  protected abstract void Initialize();

  /// <summary>
  /// Resets the fixture to its initial state.
  /// </summary>
  public virtual void Reset()
  {
    Initialize();
  }
}