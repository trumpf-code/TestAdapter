using AwesomeAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stashbox.Attributes;
using System;
using Trumpf.FunctionalTests.Tags;
using Trumpf.FunctionalTests.TestAdapter.Inject;
using Trumpf.FunctionalTests.TestAdapter.UnitTests.Helpers;

namespace Trumpf.FunctionalTests.TestAdapter.UnitTests.TestExecuterServiceTests;

[TestClass]
public class ActionsToPerformTests
{
    [TestMethod]
    public void Test_WhenTestFails_ActionsToPerformAreCalled()
    {
        // Arrange
        var service = new TestExecuterService();
        var testClassObject = new TestClassObject();

        // Act
        Action action = () => service.RunTestHelper<TestClassWithActionToPerform>(testClassObject);

        // Assert
        action.Should().Throw<InvalidCastException>();
        testClassObject.Object.As<TestClassWithActionToPerform>().WasActionCalled.Should().BeTrue();
    }

    private class TestClassWithActionToPerform
    {
        public bool WasActionCalled { get; set; }

        [Dependency]
        public ActionsToPerformAtTheInstantTestFails ActionToPerform { get; set; }

        [TcTestStep(1)]
        public void ExecuteStep()
        {
            ActionToPerform.Add(() => { WasActionCalled = true; });
            throw new InvalidCastException();
        }
    }
}
