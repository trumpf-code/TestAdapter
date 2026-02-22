
using AwesomeAssertions;
using Trumpf.FunctionalTests.Attributes;
using Trumpf.FunctionalTests.Tags;

namespace Adapter_Tests.DependenciesInjection;
//[TcIgnoreTest("cannot make a negative test; should throw an exception")]
//public class NonUniqueTestStepParametersAreNotPermitted
//{
//    [TcTestStep(1)]
//    public void Step1()
//    {
//    }

//    [TcTestStep(1)]
//    public void Step2()
//    {
//    }
//}

[ComponentTest]
public class TestContextIsInstantiated : BaseClass
{
    [TcTestStep(1)]
    public override void Execute()
    {
        TestContext.Should().NotBeNull();
    }
}
