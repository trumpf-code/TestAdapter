
using System;

namespace Adapter_Tests.IDisposableChecks;
public class FirstClassIDisposable : TiFirstInterfaceIDisposable
{
    public bool Disposed { get; private set; }

    public int X
    {
        get;
        set;
    }

    public void Dispose()
    {
        if (Disposed)
        {
#pragma warning disable CA1065 // Do not raise exceptions in unexpected locations
            throw new Exception(nameof(FirstClassIDisposable));
#pragma warning restore CA1065 // Do not raise exceptions in unexpected locations
        }

        Disposed = true;
    }
}