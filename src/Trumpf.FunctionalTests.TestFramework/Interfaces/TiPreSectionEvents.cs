using System.Reflection;

namespace Trumpf.FunctionalTests.Interfaces;

public interface TiPreSectionEvents
{
    void BeforeTestResolved(TypeInfo typeInfo);
}
