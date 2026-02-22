using System.Collections.Generic;
using Trumpf.FunctionalTests.Interfaces;

namespace Trumpf.FunctionalTests.TestAdapter.UnitTests.Mocks;

public class LoggerMock : ILogger
{
    public List<string> Errors { get; } = new();
    public List<string> Warnings { get; } = new();
    public List<string> Info { get; } = new();
    public List<string> Messages { get; } = new();

    public void LogError(string message) => Errors.Add(message);

    public void LogInfo(string message) => Info.Add(message);

    public void LogWarning(string message) => Warnings.Add(message);
}
