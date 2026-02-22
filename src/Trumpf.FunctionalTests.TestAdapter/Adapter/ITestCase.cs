using System;

namespace Trumpf.FunctionalTests.TestAdapter;
public interface ITestCase
{
    string Source { get; }
    string FullyQualifiedName { get; }
    string DisplayName { get; }
    Type Type { get; }
}
