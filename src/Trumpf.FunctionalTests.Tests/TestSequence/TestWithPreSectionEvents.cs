using Trumpf.FunctionalTests.Interfaces;

namespace Adapter_Tests.TestSequence;
// Expectation: Test should be reported as failed
public class TestWithPreSectionEvents : TiPreEvents
{
    public TiPreSectionEvents PreSectionEvents => new ThrowingPreSectionEvents();

    //[TcTestStep(1)] // negative test: cannot run in the CI
    public void Step()
    {
        throw new System.Exception($"Exception in {nameof(Step)}.");
    }
}
