
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Trumpf.FunctionalTests.TestAdapter.Extensions;
public static class LinqExtensions
{
    public static IEnumerable<MethodInfo> WhereMethodsHaveAttribute(this IEnumerable<MethodInfo> enumerable, Type attributeType)
        => enumerable
        .Where(item => item.GetCustomAttribute(attributeType) != null);

    public static IEnumerable<T> Replace<T>(this IEnumerable<T> enumerable, int index, T value)
    {
        int current = 0;
        foreach (var item in enumerable)
        {
            yield return current == index ? value : item;
            current++;
        }
    }
}