using Trumpf.FunctionalTests.Tags;

namespace Adapter_Tests.DependenciesInjection;

[TcIgnoreNegativeTest]
public class TestThrows
{
    [TcTestStep(1)]
#pragma warning disable CA1822 // Mark members as static
    public void Step1()
#pragma warning restore CA1822 // Mark members as static
    {
        //System.Diagnostics.Debugger.Launch();
        throw new System.Exception("asdasd");
    }
}