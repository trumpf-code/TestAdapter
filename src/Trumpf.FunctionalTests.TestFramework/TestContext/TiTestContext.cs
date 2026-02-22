
using System;

namespace Trumpf.FunctionalTests.TestFramework.Interfaces;
public interface TiTestContext
{
    DateTime StartTime { get; }

    void AddAttachment(string content, string filename);

    void AddAttachment(byte[] content, string filename);
}
