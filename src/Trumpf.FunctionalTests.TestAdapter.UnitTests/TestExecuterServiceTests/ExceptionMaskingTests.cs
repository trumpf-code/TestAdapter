using AwesomeAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Trumpf.FunctionalTests.Interfaces;
using Trumpf.FunctionalTests.Tags;
using Trumpf.FunctionalTests.TestAdapter.Inject;
using Trumpf.FunctionalTests.TestAdapter.UnitTests.Helpers;

namespace Trumpf.FunctionalTests.TestAdapter.UnitTests.TestExecuterServiceTests;

[TestClass]
public class ExceptionMaskingTests
{
    [TestMethod]
    public void Test_WhenCleanupThrowsException_CleanupExceptionIsMasked()
    {
        // Arrange
        var service = new TestExecuterService();
        var testClassObject = new TestClassObject();
        var maskedExceptions = new List<Exception>();

        // Act
        Action action = () => service.RunTestHelper<TestClassThrowingInStepAndCleanup>(testClassObject, maskedExceptions);

        // Assert - Step exception is thrown
        action.Should()
            .Throw<InvalidCastException>()
            .WithMessage("step exception")
            .Where(e => e.StackTrace.Contains($"{nameof(TestClassThrowingInStepAndCleanup)}.{nameof(TestClassThrowingInStepAndCleanup.ExecuteStep)}()"));

        // Assert - Cleanup exception is masked
        maskedExceptions.Should().HaveCount(1);
        maskedExceptions.First().Should().BeOfType<NotImplementedException>();
        maskedExceptions.First().Message.Should().Be("cleanup exception");
        maskedExceptions.First().StackTrace.Should().Contain($"{nameof(TestClassThrowingInStepAndCleanup)}.{nameof(TestClassThrowingInStepAndCleanup.Clean)}");
    }

    private class TestClassThrowingInStepAndCleanup : TiCleanup
    {
        public void Clean(Exception param)
        {
            throw new NotImplementedException("cleanup exception");
        }

        [TcTestStep(1)]
        public void ExecuteStep()
            => throw new InvalidCastException("step exception");
    }
}
