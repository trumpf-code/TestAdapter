using System;
using System.Reflection;

namespace Trumpf.FunctionalTests.TestAdapter.UnitTests.Helpers;

public static class MethodInfoHelper
{
    public static MethodInfo GetMethodInfo(Action action) => action.Method;
    public static MethodInfo GetMethodInfo<T>(Action<T> action) => action.Method;
    public static MethodInfo GetMethodInfo<T, U>(Action<T, U> action) => action.Method;
    public static MethodInfo GetMethodInfo<TResult>(Func<TResult> func) => func.Method;
    public static MethodInfo GetMethodInfo<T, TResult>(Func<T, TResult> func) => func.Method;
    public static MethodInfo GetMethodInfo<T, U, TResult>(Func<T, U, TResult> func) => func.Method;
    public static MethodInfo GetMethodInfo(Delegate del) => del.Method;
}
