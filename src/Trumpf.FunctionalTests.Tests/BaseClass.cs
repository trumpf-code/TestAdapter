
using Adapter_Tests.IDisposableChecks;
using Adapter_Tests.TestSequence;
using Stashbox;
using Stashbox.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Trumpf.FunctionalTests.Interfaces;
using Trumpf.FunctionalTests.Tags;
using Trumpf.FunctionalTests.TestAdapter;
using Trumpf.FunctionalTests.TestFramework.Interfaces;

namespace Adapter_Tests;
[TcTestCategory("OwnCategory")]
public class BaseClass : TiPrepare, TiCleanup, TiCanExecute, TiTestrunEnvironment, TiWiring, TiEvents
{
    [Dependency]
    public TiTestContext TestContext { get; set; }

    [Dependency]
    public ActionsToPerformAtTheInstantTestFails AfterFails { get; set; }

    public DateTime TimeStart { get; set; }

    public static string Description => string.Empty;

    public TiSectionEvents SectionEvents { get; } = new TcSectionsEvents();

    public BaseClass()
    {
        TimeStart = DateTime.Now;
    }

    public virtual bool CanExecute(out string cause)
    {
        cause = string.Empty;
        return true;
    }

    public virtual void Clean(Exception param)
    {
    }

    public virtual void Execute() { }

    public virtual void Prepare()
    {
    }

    public static void LogTestStepsDescription(MethodInfo methodWithTestStepAttribute)
    {
        Trace.WriteLine($"Test step {methodWithTestStepAttribute.GetCustomAttribute<TcTestStepAttribute>().Id} method {methodWithTestStepAttribute.Name}");
    }

    public static void TestListCleanup()
    {
        Trace.WriteLine("TestListCleanup started");
        Trace.WriteLine("TestListCleanup ended");
    }

    public virtual List<TiOneTimeInitialization> GetOperationsToBeTriggeredOnceInAssembly()
    {
        return new List<TiOneTimeInitialization>() { };
    }

    public IDependencyRegistrator RegisterDependencies(IDependencyRegistrator dependencyRegistrator)
    {
        dependencyRegistrator.Register<TiFirstInterfaceIDisposable, FirstClassIDisposable>();
        dependencyRegistrator.Register<TiSecondInterfaceIDisposable, SecondClassIDisposable>();
        dependencyRegistrator.Register<I2, C2>();
        return dependencyRegistrator;
    }
}