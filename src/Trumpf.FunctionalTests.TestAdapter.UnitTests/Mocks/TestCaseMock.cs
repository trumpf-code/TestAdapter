using System;

namespace Trumpf.FunctionalTests.TestAdapter.UnitTests.Mocks;

public class TestCaseMock : ITestCase
{
    private readonly Type _type;

    public TestCaseMock(Type testType)
    {
        _type = testType;
    }

    public string Source => _type.Assembly.Location;
    public string FullyQualifiedName => _type.FullName;
    public string DisplayName => _type.Name;
    public Type Type => _type;
}
