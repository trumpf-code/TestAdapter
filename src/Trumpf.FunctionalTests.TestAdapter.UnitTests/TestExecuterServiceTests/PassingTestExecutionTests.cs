using AwesomeAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;
using Trumpf.FunctionalTests.Tags;
using Trumpf.FunctionalTests.TestAdapter.UnitTests.Helpers;

namespace Trumpf.FunctionalTests.TestAdapter.UnitTests.TestExecuterServiceTests;

[TestClass]
public class PassingTestExecutionTests
{
    [TestMethod]
    public void Test_WhenTestPasses_ReturnsPassedResult()
    {
        // Arrange
        var service = new TestExecuterService();
        var testClassObject = new PassingTestClass();

        // Act
        var result = service.RunTestHelper<PassingTestClass>(testClassObject);

        // Assert
        result.NonFailResult.Should().Be(NonFailResult.Passed);
    }

    private class PassingTestClass
    {
        [TcTestStep(1)]
        public void ExecuteSuccessfully()
            => Expression.Empty();
    }
}
