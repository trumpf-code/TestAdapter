
using System;
using Trumpf.FunctionalTests.Tags;

namespace Adapter_Tests.Metadata;
[AttributeUsage(AttributeTargets.Class)]
public class SecondDerivedIgnoredAttribute : TcIgnoreTestAttribute
{
    public SecondDerivedIgnoredAttribute(string message) : base(message)
    {
    }

    public SecondDerivedIgnoredAttribute()
    {
    }
}
