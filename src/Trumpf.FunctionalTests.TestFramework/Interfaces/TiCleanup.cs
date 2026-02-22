using System;

namespace Trumpf.FunctionalTests.Interfaces;

/// <summary>
/// Interface for test cleanup.
/// </summary>
public interface TiCleanup
{
    /// <summary>
    /// Cleans up after the test.
    /// </summary>
    /// <param name="param">The exception from the execute step (if any).</param>
    void Clean(Exception param);
}