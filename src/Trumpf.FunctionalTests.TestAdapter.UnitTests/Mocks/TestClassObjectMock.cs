using System;
using Trumpf.FunctionalTests.Interfaces;
using Trumpf.FunctionalTests.TestFramework.Interfaces;

namespace Trumpf.FunctionalTests.TestAdapter.UnitTests.Mocks;

public class TestClassObjectMock : ITestClassObject
{
    private readonly object _testObject;

    public TestClassObjectMock(object testObject)
    {
        _testObject = testObject;
    }

    public object Object => _testObject;
    public bool Disposed { get; private set; }
    public bool Resolved { get; private set; }

    public ITestClassObject CreateFrom(Type typeToInstantiate)
    {
        if (typeToInstantiate != _testObject.GetType())
            throw new InvalidOperationException("Type mismatch");

        return this;
    }

    public void Dispose() => Disposed = true;

    public void ResolveWith(TiTestContext testContext, ActionsToPerformAtTheInstantTestFails actionToPerform, ILogger logger)
        => Resolved = true;
}
