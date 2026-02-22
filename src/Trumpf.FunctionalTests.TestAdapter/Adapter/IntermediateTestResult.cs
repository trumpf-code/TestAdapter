
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace Trumpf.FunctionalTests.TestAdapter;
public class IntermediateTestResult
{
    public TestOutcome Outcome { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public string ErrorStackTrace { get; set; } = string.Empty;
}
