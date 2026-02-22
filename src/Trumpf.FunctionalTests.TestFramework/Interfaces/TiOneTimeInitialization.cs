namespace Trumpf.FunctionalTests.Interfaces;

/// <summary>
/// Identifies methods to executed before all tests in the assembly (Init method) and after all tests in the assembly have run (Deinit method)
/// </summary>
public interface TiOneTimeInitialization
{
    void Init();

    void Deinit();
}