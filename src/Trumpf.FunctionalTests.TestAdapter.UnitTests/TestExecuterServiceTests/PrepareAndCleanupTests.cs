using AwesomeAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Trumpf.FunctionalTests.Interfaces;
using Trumpf.FunctionalTests.Tags;
using Trumpf.FunctionalTests.TestAdapter.Inject;
using Trumpf.FunctionalTests.TestAdapter.UnitTests.Helpers;

namespace Trumpf.FunctionalTests.TestAdapter.UnitTests.TestExecuterServiceTests;

[TestClass]
public class PrepareAndCleanupTests
{
    [TestMethod]
    public void Test_WhenPrepareThrows_PrepareExceptionIsThrown()
    {
        // Arrange
        var service = new TestExecuterService();
        var testClassObject = new TestClassObject();
        var maskedExceptions = new List<Exception>();

        // Act
        Action action = () => service.RunTestHelper<TestClassThrowingInPrepareAndCleanup>(testClassObject, maskedExceptions);

        // Assert - Prepare exception is thrown
        action.Should()
            .Throw<InvalidCastException>()
            .WithMessage("prepare exception")
            .Where(e => e.StackTrace.Contains($"{nameof(TestClassThrowingInPrepareAndCleanup)}.{nameof(TestClassThrowingInPrepareAndCleanup.Prepare)}()"));

        // Assert - Cleanup exception is masked
        maskedExceptions.Should().HaveCount(1);
        maskedExceptions.First().Should().BeOfType<NotImplementedException>();
        maskedExceptions.First().Message.Should().Be("cleanup exception");
        maskedExceptions.First().StackTrace.Should().Contain($"{nameof(TestClassThrowingInPrepareAndCleanup)}.{nameof(TestClassThrowingInPrepareAndCleanup.Clean)}");
    }

    private class TestClassThrowingInPrepareAndCleanup : TiCleanup, TiPrepare
    {
        public void Clean(Exception param)
        {
            throw new NotImplementedException("cleanup exception");
        }

        public void Prepare()
        {
            throw new InvalidCastException("prepare exception");
        }

        [TcTestStep(1)]
        public void ExecuteStep()
            => Expression.Empty();
    }
}
