
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Trumpf.FunctionalTests.TestAdapter.Execution;
// Copied from https://stackoverflow.com/a/56476316
public static class GetIndirectMethodInfoUtil
{
    public static MethodInfo GetIndirectMethodInfo<T>(Expression<Action<T>> expression)
        => GetIndirectMethodInfo((LambdaExpression)expression);

    // Used by the above
    private static MethodInfo GetIndirectMethodInfo(LambdaExpression expression)
    {
        if (!(expression.Body is MethodCallExpression methodCall))
        {
            throw new ArgumentException(
                $"Invalid Expression ({expression.Body}). Expression should consist of a method call only.");
        }
        return methodCall.Method;
    }
}
