using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using System;
using System.Collections.Generic;
using Trumpf.FunctionalTests.TestAdapter.Inject;
using Trumpf.FunctionalTests.TestAdapter.Shared;

namespace Trumpf.FunctionalTests.TestAdapter;
public class ActionsToPerformAtTheInstantTestFails
{
    private List<Action> ListActionsToPerform { get; set; }

    public void Add(Action actionToPerform)
    {
        ListActionsToPerform ??= new List<Action>();

        ListActionsToPerform.Add(actionToPerform);
    }
}

[ExtensionUri(URL_TESTEXECUTOR)]
public class TestExecutor : ITestExecutor
{
    public const string URL_TESTEXECUTOR = "executor://testexecutoradapter/v1";

    public TestExecuterService TestExecuterService { get; private set; }

    public void Cancel()
    {
        TestExecuterService.Cancel();
    }

    public void RunTests(IEnumerable<string> sources, IRunContext runContext, IFrameworkHandle frameworkHandle)
    {
        // gets an IEnumerable<TestCase> instead of an IEnumerable<string>
        var testDiscovererService = new TestDiscovererService();
        var logger = new Logger(frameworkHandle);
        IEnumerable<TestCase> tests = testDiscovererService.GetClassesUsingTestStepAttribute(sources, logger);
        RunTests(tests, runContext, frameworkHandle);
    }

    /// <summary>
    /// Run all tests
    /// </summary>
    /// <param name="tests">list of classes marked as test case</param>
    /// <param name="runContext">Context to use when executing the tests</param>
    /// <param name="frameworkHandle">Handle to the framework to record results and to do framework operations</param>
    public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
    {
        TestExecuterService = new TestExecuterService();
        var testClassResolver = new TestClassObject();
        var logger = new Logger(frameworkHandle);
        var recorder = frameworkHandle as ITestExecutionRecorder;
        TestExecuterService.RunTests(tests, runContext, recorder, testClassResolver, logger);
    }
}