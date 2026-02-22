using Trumpf.FunctionalTests.Tags;

namespace Adapter_Tests.DependenciesInjection;
[TcIgnoreNegativeTest]
public class TestThrowsInFirstCall
{
    [TcTestStep(1)]
    public void Step1()
        => h();

    private void h()
        => throw new System.Exception("asdasd");
}
