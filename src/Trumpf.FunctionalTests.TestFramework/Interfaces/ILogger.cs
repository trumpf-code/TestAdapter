namespace Trumpf.FunctionalTests.Interfaces;

public interface ILogger
{
    void LogError(string message);
    void LogInfo(string message);
    void LogWarning(string message);
}
