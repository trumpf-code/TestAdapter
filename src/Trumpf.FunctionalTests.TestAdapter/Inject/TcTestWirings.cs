
using Stashbox;
using Trumpf.FunctionalTests.Interfaces;
using Trumpf.FunctionalTests.TestFramework.Interfaces;

namespace Trumpf.FunctionalTests.TestAdapter;
public class TcTestWirings : TiWiring
{
    private readonly TiTestContext testContext;
    private readonly ActionsToPerformAtTheInstantTestFails actionToPerform;

    public TcTestWirings(TiTestContext testContext, ActionsToPerformAtTheInstantTestFails actionToPerform)
    {
        this.testContext = testContext;
        this.actionToPerform = actionToPerform;
    }

    public IDependencyRegistrator RegisterDependencies(IDependencyRegistrator dependencyRegistrator)
    {
        return dependencyRegistrator
            .WireUpAs(testContext)
            .WireUpAs<TiOneTimeOperations>(new OneTimeOperations())
            .WireUpAs(actionToPerform);
        ;
    }
}