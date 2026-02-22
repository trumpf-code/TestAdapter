
using System;
using System.Collections.Generic;

namespace Trumpf.FunctionalTests.Tags;
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class TcTestCategoryAttribute : TcTestTagAttribute
{
    private readonly string mTestCategory;

    public override IEnumerable<TiTag> Tags => new[] { new TcTag { Name = mTestCategory } };

    public TcTestCategoryAttribute(string testCategory)
    {
        mTestCategory = testCategory;
    }
}