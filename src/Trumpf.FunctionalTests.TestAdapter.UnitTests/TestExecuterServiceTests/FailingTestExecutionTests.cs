using AwesomeAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Runtime.InteropServices;
using Trumpf.FunctionalTests.Tags;
using Trumpf.FunctionalTests.TestAdapter.UnitTests.Helpers;

namespace Trumpf.FunctionalTests.TestAdapter.UnitTests.TestExecuterServiceTests;

[TestClass]
public class FailingTestExecutionTests
{
    [TestMethod]
    public void Test_WhenTestThrowsException_ExceptionIsPropagated()
    {
        // Arrange
        var service = new TestExecuterService();
        var testClassObject = new FailingTestClass();

        // Act
        Action action = () => service.RunTestHelper<FailingTestClass>(testClassObject);

        // Assert
        action.Should()
            .Throw<InvalidComObjectException>()
            .WithMessage("test exception")
            .Where(e => e.StackTrace.Contains($"{nameof(FailingTestClass)}.{nameof(FailingTestClass.ExecuteAndFail)}()"));
    }

    private class FailingTestClass
    {
        [TcTestStep(1)]
        public void ExecuteAndFail()
            => throw new InvalidComObjectException("test exception");
    }
}
