using FluentAssertions;
using Xunit;

namespace PromptOrganizer.Tests.TestHelpers;

/// <summary>
/// Base test class that provides common functionality for all tests.
/// This class ensures consistent test setup and teardown patterns.
/// </summary>
public abstract class BaseTest : IDisposable
{
  private bool _disposed;

  protected BaseTest()
  {
    Setup();
  }

  /// <summary>
  /// Setup method that runs before each test.
  /// Override this method in derived classes to provide specific setup logic.
  /// </summary>
  protected virtual void Setup()
  {
    // Base setup logic can be added here
  }

  /// <summary>
  /// Cleanup method that runs after each test.
  /// Override this method in derived classes to provide specific cleanup logic.
  /// </summary>
  protected virtual void Cleanup()
  {
    // Base cleanup logic can be added here
  }

  /// <summary>
  /// Asserts that the specified action throws an exception of type T.
  /// </summary>
  protected static void AssertThrows<T>(Action action, string? because = null, params object[] becauseArgs)
      where T : Exception
  {
    action.Should().Throw<T>(because, becauseArgs);
  }

  /// <summary>
  /// Asserts that the specified action throws an exception of type T with the specified message.
  /// </summary>
  protected static void AssertThrowsWithMessage<T>(Action action, string message, string? because = null, params object[] becauseArgs)
      where T : Exception
  {
    action.Should().Throw<T>(because, becauseArgs)
        .WithMessage(message);
  }

  /// <summary>
  /// Asserts that the specified condition is true.
  /// </summary>
  protected static void AssertTrue(bool condition, string? because = null, params object[] becauseArgs)
  {
    condition.Should().BeTrue(because, becauseArgs);
  }

  /// <summary>
  /// Asserts that the specified condition is false.
  /// </summary>
  protected static void AssertFalse(bool condition, string? because = null, params object[] becauseArgs)
  {
    condition.Should().BeFalse(because, becauseArgs);
  }

  /// <summary>
  /// Asserts that the specified objects are equal.
  /// </summary>
  protected static void AssertEqual<T>(T expected, T actual, string? because = null, params object[] becauseArgs)
  {
    actual.Should().Be(expected, because, becauseArgs);
  }

  /// <summary>
  /// Asserts that the specified objects are not equal.
  /// </summary>
  protected static void AssertNotEqual<T>(T expected, T actual, string? because = null, params object[] becauseArgs)
  {
    actual.Should().NotBe(expected, because, becauseArgs);
  }

  /// <summary>
  /// Asserts that the specified object is not null.
  /// </summary>
  protected static void AssertNotNull(object? obj, string? because = null, params object[] becauseArgs)
  {
    obj.Should().NotBeNull(because, becauseArgs);
  }

  /// <summary>
  /// Asserts that the specified object is null.
  /// </summary>
  protected static void AssertNull(object? obj, string? because = null, params object[] becauseArgs)
  {
    obj.Should().BeNull(because, becauseArgs);
  }

  public void Dispose()
  {
    Dispose(true);
    GC.SuppressFinalize(this);
  }

  protected virtual void Dispose(bool disposing)
  {
    if (!_disposed)
    {
      if (disposing)
      {
        Cleanup();
      }
      _disposed = true;
    }
  }
}