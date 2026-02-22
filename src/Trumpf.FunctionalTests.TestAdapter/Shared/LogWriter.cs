using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System.Diagnostics;
using Trumpf.FunctionalTests.Interfaces;

namespace Trumpf.FunctionalTests.TestAdapter.Shared;

public class Logger : ILogger
{
    private readonly IMessageLogger messageLogger;

    public Logger(IMessageLogger messageLogger)
        => this.messageLogger = messageLogger;

    public void LogError(string message)
    {
        Log(TestMessageLevel.Error, message);
    }

    public void LogInfo(string message)
    {
        Log(TestMessageLevel.Informational, message);
    }

    public void LogWarning(string message)
    {
        Log(TestMessageLevel.Warning, message);
    }

    private void Log(TestMessageLevel level, string message)
    {
        messageLogger.SendMessage(level, message);
        Trace.WriteLine(message);
    }
}
