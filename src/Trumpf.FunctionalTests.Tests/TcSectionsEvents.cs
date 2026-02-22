using System;
using System.Reflection;
using Trumpf.FunctionalTests.Interfaces;

namespace Adapter_Tests;

internal class TcSectionsEvents : TiSectionEvents
{
    public void AfterTestResolved(TypeInfo typeInfo)
    {
        TcSectionsEventsHelpers.TraceContext(typeInfo, true);
        TcSectionsEventsHelpers.TraceTags(typeInfo);
        TcSectionsEventsHelpers.TraceProduct(typeInfo);
        TcSectionsEventsHelpers.LogIpAddresses();
    }

    public void OnTestFinishing(TypeInfo typeInfo)
    {
        TcSectionsEventsHelpers.TraceContext(typeInfo, false);
    }

    public void OnCanExecuteStarting(MethodInfo methodInfo)
    {
        TcSectionsEventsHelpers.TraceStep(methodInfo, true);
    }

    public void OnCanExecuteFinishing(MethodInfo methodInfo, Exception exception)
    {
        TcSectionsEventsHelpers.TraceStep(methodInfo, false);
        TcSectionsEventsHelpers.DisplayException(exception);
    }

    public void OnPrepareStarting(MethodInfo methodInfo)
    {
        TcSectionsEventsHelpers.TraceStep(methodInfo, true);
    }

    public void OnPrepareFinishing(MethodInfo methodInfo, Exception exception)
    {
        TcSectionsEventsHelpers.TraceStep(methodInfo, false);
        TcSectionsEventsHelpers.DisplayException(exception);
    }

    public void OnTestStepStarting(MethodInfo methodInfo)
    {
        TcSectionsEventsHelpers.TraceTestStep(methodInfo, true);
    }

    public void OnTestStepFinishing(MethodInfo methodInfo, Exception exception)
    {
        TcSectionsEventsHelpers.TraceTestStep(methodInfo, false);
        TcSectionsEventsHelpers.DisplayException(exception);
    }

    public void OnCleanStarting(MethodInfo methodInfo)
    {
        TcSectionsEventsHelpers.TraceStep(methodInfo, true);
    }

    public void OnCleanFinishing(MethodInfo methodInfo, Exception exception)
    {
        TcSectionsEventsHelpers.TraceStep(methodInfo, false);
        TcSectionsEventsHelpers.DisplayException(exception);
    }

    public void OnOneTimeInitStarting(TypeInfo typeInfo)
    {
        TcSectionsEventsHelpers.TraceInit(typeInfo, true);
    }

    public void OnOneTimeInitFinishing(TypeInfo typeInfo, Exception exception)
    {
        TcSectionsEventsHelpers.TraceInit(typeInfo, false);
        TcSectionsEventsHelpers.DisplayException(exception);
    }

    public void OnOneTimeDeinitStarting(TypeInfo typeInfo)
    {
        TcSectionsEventsHelpers.TraceOneTimeDeInit(typeInfo, true);
    }

    public void OnOneTimeDeinitFinishing(TypeInfo typeInfo, Exception exception)
    {
        TcSectionsEventsHelpers.TraceOneTimeDeInit(typeInfo, false);
        TcSectionsEventsHelpers.DisplayException(exception);
    }
}