using Trumpf.FunctionalTests.Tags;

namespace Adapter_Tests.DependenciesInjection;
[TcIgnoreNegativeTest]
public class TestThrowsInDeepMethod
{
    [TcTestStep(1)]
    public void Step1()
        => f();

    private void f()
        => g();

    private void g()
        => h();

    private void h()
        => throw new System.Exception("asdasd");
}