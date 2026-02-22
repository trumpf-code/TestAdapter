
using System;
using System.Collections.Generic;

namespace Trumpf.FunctionalTests.Tags;
[AttributeUsage(AttributeTargets.Class)]
public class TcCustomTagAttribute : TcTestTagAttribute
{
    private readonly string mTagName;

    public override IEnumerable<TiTag> Tags => new[] { new TcTag { Name = mTagName } };

    public TcCustomTagAttribute(string tagName)
        => mTagName = tagName;
}
