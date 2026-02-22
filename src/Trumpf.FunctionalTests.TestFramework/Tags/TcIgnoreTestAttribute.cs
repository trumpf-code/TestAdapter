
using System;
using System.Collections.Generic;

namespace Trumpf.FunctionalTests.Tags;
[AttributeUsage(AttributeTargets.Class)]
public class TcIgnoreTestAttribute : TcTestTagAttribute
{
    private readonly string mReason;

    public override IEnumerable<TiTag> Tags => new[] { new TcTag { Name = GetType().Name, Description = mReason } };

    public TcIgnoreTestAttribute(string reason)
        => mReason = reason;

    public TcIgnoreTestAttribute() { }
}