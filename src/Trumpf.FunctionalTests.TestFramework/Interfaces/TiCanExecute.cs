namespace Trumpf.FunctionalTests.Interfaces;

/// <summary>
/// for defining the context needed for the tests to be executed.
/// </summary>
public interface TiCanExecute
{
    /// <summary>
    /// Determines whether this test can be executed in the current environment.
    /// </summary>
    /// <param name="cause">Out variable for remarks when CanExecute returns <c>false</c>.</param>
    /// <returns><c>true</c> when this test can be executed in the current environment; otherwise, <c>false</c>.</returns>
    bool CanExecute(out string cause);
}