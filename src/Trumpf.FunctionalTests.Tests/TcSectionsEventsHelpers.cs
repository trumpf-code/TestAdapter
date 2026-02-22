using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Trumpf.FunctionalTests.Tags;
using Trumpf.FunctionalTests.TestAdapter;

namespace Adapter_Tests;

internal class TcSectionsEventsHelpers
{
    public const string patternTestFailed = "Test failed at UTC";

    public static void LogIpAddresses()
    {
        Trace.WriteLine($"IP Address: ...");
    }
    public static void TraceContext(TypeInfo typeInfo, bool op)
       => Trace.WriteLine($"CONTEXT: Test {typeInfo.Name} {(op ? "start" : "end")} at UTC {DateTime.UtcNow:o}");

    public static void TraceTestStep(MethodInfo methodInfo, bool op)
    {
        Trace.WriteLine($"STEP: Test step {methodInfo.GetCustomAttribute<TcTestStepAttribute>().Id} --- {methodInfo.Name} {(op ? "start" : "end")} at UTC {DateTime.UtcNow:o}");
    }

    public static void TraceStep(MethodInfo methodInfo, bool op)
    {
        Trace.WriteLine($"STEP: {methodInfo.Name} {(op ? "start" : "end")} at UTC {DateTime.UtcNow:o}");
    }

    public static void TraceInit(TypeInfo typeInfo, bool op)
    {
        Trace.WriteLine($"STEP: Init method of type {typeInfo.Name} {(op ? "start" : "end")} at UTC {DateTime.UtcNow:o}");
    }

    public static void TraceOneTimeDeInit(TypeInfo typeInfo, bool op)
    {
        Trace.WriteLine($"STEP: Deinit method of type {typeInfo.Name} {(op ? "start" : "end")} at UTC {DateTime.UtcNow:o}");
    }

    public static void TraceProduct(TypeInfo typeInfo)
       => Trace.WriteLine($"Product: {typeInfo.Namespace?.Split(new[] { '.' }).Last()}");

    public static void TraceTags(Type type)
    {
        var listTags = TcTagsRetriever.GetTags(type);
        foreach (var tag in listTags)
        {
            Trace.WriteLine($"[Tag] {new string(tag.ToCharArray().Where(c => c.Equals('.') == false).ToArray())}");
        }
    }

    public static void DisplayException(Exception exception)
    {
        if (exception != null)
        {
            Trace.WriteLine($"{patternTestFailed} {DateTime.UtcNow:o}");
            Trace.WriteLine(exception);
        }
    }
}