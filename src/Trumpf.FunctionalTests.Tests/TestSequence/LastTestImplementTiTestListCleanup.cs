using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Trumpf.FunctionalTests.Interfaces;
using Trumpf.FunctionalTests.Tags;

namespace Adapter_Tests.TestSequence;
// Expectation: If both tests run, the first shall have the "Init" the last the "Deinit"
public class OneTimeInitialization : TiOneTimeInitialization
{
    public void Init()
        => Trace.WriteLine(nameof(Init));

    public void Deinit()
        => Trace.WriteLine(nameof(Deinit));
}

public class TestWithOneTimeInit : TiTestrunEnvironment
{
    [TcTestStep(1)]
    public static void Execute()
    {
    }

    public List<TiOneTimeInitialization> GetOperationsToBeTriggeredOnceInAssembly()
        => new[] { new OneTimeInitialization() as TiOneTimeInitialization }.ToList();
}

public class TestWithoutOneTimeInit
{
    [TcTestStep(1)]
    public static void Execute()
    {
    }
}