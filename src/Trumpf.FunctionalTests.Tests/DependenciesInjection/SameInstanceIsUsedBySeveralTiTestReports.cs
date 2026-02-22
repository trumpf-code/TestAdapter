
using AwesomeAssertions;
using Stashbox.Attributes;
using Trumpf.FunctionalTests.Attributes;
using Trumpf.FunctionalTests.Tags;
using Trumpf.FunctionalTests.TestFramework.Interfaces;

namespace Adapter_Tests.DependenciesInjection;
[ComponentTest]
public class SameInstanceIsUsedBySeveralTiTestReports : BaseClass
{
    [Dependency]
    public TiTestContext TestReport1 { get; set; }

    [Dependency]
    public TiTestContext TestReport2 { get; set; }

    [TcTestStep(1)]
    public override void Execute()
    {
        TestReport1.Should().BeSameAs(TestReport2);
    }
}
