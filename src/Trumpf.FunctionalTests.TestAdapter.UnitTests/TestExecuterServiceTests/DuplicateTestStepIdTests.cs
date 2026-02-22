using AwesomeAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq.Expressions;
using Trumpf.FunctionalTests.Tags;
using Trumpf.FunctionalTests.TestAdapter.UnitTests.Helpers;

namespace Trumpf.FunctionalTests.TestAdapter.UnitTests.TestExecuterServiceTests;

[TestClass]
public class DuplicateTestStepIdTests
{
    [TestMethod]
    public void Test_WhenTwoTestStepsHaveSameId_ThrowsDuplicateIdException()
    {
        // Arrange
        var service = new TestExecuterService();
        var testClassObject = new TestClassWithDuplicateStepIds();

        // Act
        Action action = () => service.RunTestHelper<TestClassWithDuplicateStepIds>(testClassObject);

        // Assert
        action.Should().Throw<DupplicatedIdsException>();
    }

    private class TestClassWithDuplicateStepIds
    {
        [TcTestStep(1)]
        public void FirstStep()
            => Expression.Empty();

        [TcTestStep(1)]
        public void SecondStepWithSameId()
            => Expression.Empty();
    }
}
