using System.Diagnostics;

namespace Trumpf.FunctionalTests.TestAdapter;
public class TraceListener : TiTraceListener
{
    private DefaultTraceListener DefaultTraceListener { get; set; }

    public DefaultTraceListener GetDefaultTraceListener()
        => DefaultTraceListener;

    public TraceListener(string file)
    {
        // Remove all existing trace listeners
        Trace.Listeners.Clear();

        // Create and add a new default trace listener
        DefaultTraceListener = new DefaultTraceListener
        {
            LogFileName = file
        };

        Trace.Listeners.Add(DefaultTraceListener);
    }
}