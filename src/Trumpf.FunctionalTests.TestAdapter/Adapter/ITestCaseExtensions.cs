using System.Linq;
using System.Reflection;
using Trumpf.FunctionalTests.Tags;

namespace Trumpf.FunctionalTests.TestAdapter;
public static class ITestCaseExtensions
{
    public static TcTestStepAttribute[] TestStepMethods(this ITestCase source)
        => source.Type
            .GetMethods()
            .Where(m => m.GetCustomAttribute<TcTestStepAttribute>() != null)
            .Select(m => m.GetCustomAttribute<TcTestStepAttribute>())
            .ToArray();
}
