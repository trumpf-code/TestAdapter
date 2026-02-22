
using System;

namespace Trumpf.FunctionalTests.Tags;
[AttributeUsage(AttributeTargets.Method)]
public class TcTestStepAttribute : TcTestTagAttribute
{
    public TcTestStepAttribute(int id)
    {
        Id = id;
    }

    public int Id { get; }
}
