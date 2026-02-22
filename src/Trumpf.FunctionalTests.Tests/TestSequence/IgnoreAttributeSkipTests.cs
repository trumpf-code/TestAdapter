
using AwesomeAssertions;
using Trumpf.FunctionalTests.Tags;

namespace Adapter_Tests.TestSequence;
[TcIgnoreTest]
public class IgnoreAttributeSkipTests
{
    private readonly bool functionToBeExecuted = false;

    [TcTestStep(1)]
    public void Step1()
    {
        functionToBeExecuted.Should().BeTrue();
    }
}