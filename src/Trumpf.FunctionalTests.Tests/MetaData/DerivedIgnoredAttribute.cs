
using System;
using Trumpf.FunctionalTests.Tags;

namespace Adapter_Tests.Metadata;
[AttributeUsage(AttributeTargets.Class)]
public class DerivedIgnoredAttribute : TcIgnoreTestAttribute
{
    public DerivedIgnoredAttribute()
    {
    }

    public DerivedIgnoredAttribute(string message) : base(message)
    {
    }
}
