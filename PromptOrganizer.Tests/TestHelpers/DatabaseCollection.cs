using Xunit;

namespace PromptOrganizer.Tests.TestHelpers;

/// <summary>
/// Collection fixture to ensure database tests run sequentially for proper isolation.
/// </summary>
[CollectionDefinition("DatabaseCollection")]
public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
{
  // This class has no code, and is never created. Its purpose is simply
  // to be the place to apply [CollectionDefinition] and all the
  // ICollectionFixture<> interfaces.
}

/// <summary>
/// Database fixture to manage shared database resources.
/// </summary>
public class DatabaseFixture : IDisposable
{
  public void Dispose()
  {
    // Clean up any resources if needed
  }
}