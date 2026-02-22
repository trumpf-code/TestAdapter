
using Adapter_Tests.Attributes;
using AwesomeAssertions;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Trumpf.FunctionalTests.Interfaces;
using Trumpf.FunctionalTests.Tags;
using Trumpf.FunctionalTests.TestAdapter;

namespace Adapter_Tests.DependenciesInjection;
public class TcSectionEvent : TiSectionEvents
{
    private readonly string patternTestFailed = "Test failed at UTC";

    private static XRayId GetXRayIdFromType(TypeInfo typeInfo)
        => typeInfo.GetCustomAttribute<XRayId>();

    private static string GeneratexRayDescriptionFromId(XRayId xRayId)
        => xRayId == null
            ? string.Empty
            : $" ---{xRayId.Project}-{xRayId.Id}";

    public void AfterTestResolved(TypeInfo typeInfo)
    {
        var xRayId = GetXRayIdFromType(typeInfo);
        if (xRayId != null)
        {
            Trace.WriteLine($"Requirement: {xRayId.Name}");
        }

        var listTags = TcTagsRetriever.GetTags(typeInfo);
        foreach (var tag in listTags)
        {
            Trace.WriteLine($"[Tag] {new string(tag.ToCharArray().Where(c => c.Equals('.') == false).ToArray())}");
        }

        Trace.WriteLine($"CONTEXT: {typeInfo.Name}{GeneratexRayDescriptionFromId(xRayId)} start at UTC {DateTime.UtcNow:o}");
        Trace.WriteLine($"Product: {typeInfo.Namespace?.Split(new[] { '.' }).Last()}");
    }

    public void OnTestFinishing(TypeInfo typeInfo)
    {
        var xRayId = GetXRayIdFromType(typeInfo);
        Trace.WriteLine($"CONTEXT: {typeInfo.Name}{GeneratexRayDescriptionFromId(xRayId)} end at UTC {DateTime.UtcNow:o}");
    }

    public void OnCanExecuteStarting(MethodInfo methodInfo)
    {
        Trace.WriteLine($"STEP: {methodInfo.Name} start at UTC {DateTime.UtcNow:o}");
    }

    public void OnCanExecuteFinishing(MethodInfo methodInfo, Exception exception)
    {
        Trace.WriteLine($"STEP: {methodInfo.Name} end at UTC {DateTime.UtcNow:o}");
        DisplayException(exception);
    }

    public void OnPrepareStarting(MethodInfo methodInfo)
    {
        Trace.WriteLine($"STEP: {methodInfo.Name} start at UTC {DateTime.UtcNow:o}");
    }

    public void OnPrepareFinishing(MethodInfo methodInfo, Exception exception)
    {
        Trace.WriteLine($"STEP: {methodInfo.Name} end at UTC {DateTime.UtcNow:o}");
        DisplayException(exception);
    }

    public void OnTestStepStarting(MethodInfo methodInfo)
    {
        Trace.WriteLine($"STEP: Test step {methodInfo.GetCustomAttribute<TcTestStepAttribute>().Id} - {methodInfo.Name} start at UTC {DateTime.UtcNow:o}");
    }

    public void OnTestStepFinishing(MethodInfo methodInfo, Exception exception)
    {
        Trace.WriteLine($"STEP: Test step {methodInfo.GetCustomAttribute<TcTestStepAttribute>().Id} - {methodInfo.Name} end at UTC {DateTime.UtcNow:o}");
        DisplayException(exception);
    }

    public void OnCleanStarting(MethodInfo methodInfo)
    {
        Trace.WriteLine($"STEP: {methodInfo.Name} start at UTC {DateTime.UtcNow:o}");
    }

    public void OnCleanFinishing(MethodInfo methodInfo, Exception exception)
    {
        Trace.WriteLine($"STEP: {methodInfo.Name} end at UTC {DateTime.UtcNow:o}");
        DisplayException(exception);
    }

    private void DisplayException(Exception exception)
    {
        if (exception != null)
        {
            Trace.Write($"{patternTestFailed} {DateTime.UtcNow:o}");
            Trace.Write(exception);
        }
    }

    public void OnOneTimeInitStarting(TypeInfo typeInfo)
    {
        Trace.WriteLine($"STEP: Init method of type {typeInfo.Name} start at UTC {DateTime.UtcNow:o}");
    }

    public void OnOneTimeInitFinishing(TypeInfo typeInfo, Exception exception)
    {
        Trace.WriteLine($"STEP: Init method of type {typeInfo.Name} end at UTC {DateTime.UtcNow:o}");
        DisplayException(exception);
    }

    public void OnOneTimeDeinitStarting(TypeInfo typeInfo)
    {
        Trace.WriteLine($"STEP: Deinit method of type {typeInfo.Name} start at UTC {DateTime.UtcNow:o}");
    }

    public void OnOneTimeDeinitFinishing(TypeInfo typeInfo, Exception exception)
    {
        Trace.WriteLine($"STEP: Deinit method of type {typeInfo.Name} end at UTC {DateTime.UtcNow:o}");
    }
}
