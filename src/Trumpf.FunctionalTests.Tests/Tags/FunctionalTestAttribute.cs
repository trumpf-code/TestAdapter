
using System;
using Trumpf.FunctionalTests.Tags;

namespace Trumpf.FunctionalTests.Attributes;
[AttributeUsage(AttributeTargets.Class)]
public class FunctionalTestAttribute : TcTestTagAttribute
{
}
