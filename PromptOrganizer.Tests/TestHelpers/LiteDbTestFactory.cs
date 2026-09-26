using LiteDB;

namespace PromptOrganizer.Tests.TestHelpers;

/// <summary>
/// Factory for creating in-memory LiteDB databases for testing.
/// </summary>
public static class LiteDbTestFactory
{
  /// <summary>
  /// Creates a new in-memory LiteDB database for testing.
  /// </summary>
  /// <returns>A disposable LiteDB database instance.</returns>
  public static ILiteDatabase CreateInMemoryDatabase()
  {
    // Use memory mode for true isolation
    var connectionString = new ConnectionString
    {
      Filename = ":memory:",
      Connection = ConnectionType.Direct
    };

    return new LiteDatabase(connectionString);
  }
}