using System.Diagnostics;

namespace Trumpf.FunctionalTests.TestAdapter;

public interface TiTraceListener
{
    DefaultTraceListener GetDefaultTraceListener();
}