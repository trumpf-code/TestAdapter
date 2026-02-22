
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using System.Collections.Generic;

namespace Adapter_Tests;
public class FakeITestCaseDiscoverySink : ITestCaseDiscoverySink
{
    private readonly List<TestCase> testCasesDiscoverd = new();

    public void SendTestCase(TestCase discoveredTest)
    {
        testCasesDiscoverd.Add(discoveredTest);
    }
}
