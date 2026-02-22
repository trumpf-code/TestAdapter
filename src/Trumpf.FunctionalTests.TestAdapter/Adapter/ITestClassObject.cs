using System;
using Trumpf.FunctionalTests.Interfaces;
using Trumpf.FunctionalTests.TestFramework.Interfaces;

namespace Trumpf.FunctionalTests.TestAdapter;
public interface ITestClassObject
{
    object Object { get; }
    ITestClassObject CreateFrom(Type typeToInstantiate);
    void ResolveWith(TiTestContext testContext, ActionsToPerformAtTheInstantTestFails actionToPerform, ILogger logger);
    void Dispose();
}
