using System.Reflection;
using Trumpf.FunctionalTests.Interfaces;

namespace Adapter_Tests.TestSequence;
public class ThrowingPreSectionEvents : TiPreSectionEvents
{
    public void BeforeTestResolved(TypeInfo typeInfo)
    {
        throw new System.Exception($"Exception in {nameof(BeforeTestResolved)}.");
    }
}
