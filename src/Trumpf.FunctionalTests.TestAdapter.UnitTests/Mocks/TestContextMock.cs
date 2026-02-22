using System;
using Trumpf.FunctionalTests.TestFramework.Interfaces;

namespace Trumpf.FunctionalTests.TestAdapter.UnitTests.Mocks;

public class TestContextMock : TiTestContext
{
    public DateTime StartTime => throw new NotImplementedException();

    public void AddAttachment(string content, string filename)
    {
        throw new NotImplementedException();
    }

    public void AddAttachment(byte[] content, string filename)
    {
        throw new NotImplementedException();
    }
}
