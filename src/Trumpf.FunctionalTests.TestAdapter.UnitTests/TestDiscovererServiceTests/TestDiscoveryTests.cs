using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Trumpf.FunctionalTests.TestAdapter.UnitTests.Mocks;

namespace Trumpf.FunctionalTests.TestAdapter.UnitTests.TestDiscovererServiceTests;

[TestClass]
public class TestDiscoveryTests
{
    [TestMethod]
    public void Test_WhenDiscoveringTestsInCurrentAssembly_NoExceptionIsThrown()
    {
        // Arrange
        var service = new TestDiscovererService();
        var logger = new LoggerMock();
        var assemblyLocations = new List<string> { GetType().Assembly.Location };

        // Act & Assert - Should not throw
        service.GetClassesUsingTestStepAttribute(assemblyLocations, logger);
    }
}
