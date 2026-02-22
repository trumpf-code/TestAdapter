
using System.Linq.Expressions;
using Trumpf.FunctionalTests.Tags;

namespace Adapter_Tests.DependenciesInjection;
public class TcIgnoreNegativeTest : TcIgnoreTestAttribute
{
    public TcIgnoreNegativeTest() : base("negative test, do not run in the CI")
        => Expression.Empty();
}
