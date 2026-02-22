
using System;

namespace Adapter_Tests.IDisposableChecks;
public interface TiFirstInterfaceIDisposable : IDisposable
{
    bool Disposed { get; }

    int X { get; set; }
}