using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Trumpf.FunctionalTests.TestAdapter.UnitTests.TraceListenerTests;

[TestClass]
public class TraceListenerThreadSafetyTests
{
    [TestInitialize]
    public void Setup()
    {
        // Clean up trace listeners before each test
        Trace.Listeners.Clear();
    }

    [TestCleanup]
    public void Cleanup()
    {
        // Restore default state after each test
        Trace.Listeners.Clear();
    }

    [TestMethod]
    [Description("Tests that creating TraceListener when Trace.Listeners is empty does not throw IndexOutOfRangeException")]
    public void TraceListener_WhenListenersCollectionIsEmpty_DoesNotThrowException()
    {
        // Arrange
        Trace.Listeners.Clear(); // Ensure collection is empty
        var testFile = Path.Combine(Path.GetTempPath(), $"trace_{Guid.NewGuid()}.log");

        // Act & Assert - Should not throw IndexOutOfRangeException
        var listener = new TestAdapter.TraceListener(testFile);
        
        Assert.IsNotNull(listener);
        Assert.IsNotNull(listener.GetDefaultTraceListener());
        Assert.AreEqual(testFile, listener.GetDefaultTraceListener().LogFileName);
        Assert.HasCount(1, Trace.Listeners);
    }

    [TestMethod]
    [Description("Tests that multiple threads creating TraceListener concurrently do not cause race conditions or exceptions")]
    public void TraceListener_WhenCreatedConcurrently_DoesNotThrowException()
    {
        // Arrange
        const int threadCount = 10;
        var exceptions = new List<Exception>();
        var threads = new List<Thread>();
        var barrier = new Barrier(threadCount); // Ensures all threads start at the same time

        // Act
        for (int i = 0; i < threadCount; i++)
        {
            var threadIndex = i;
            var thread = new Thread(() =>
            {
                try
                {
                    // Wait for all threads to be ready
                    barrier.SignalAndWait();

                    // This is where the original race condition occurred
                    var testFile = Path.Combine(Path.GetTempPath(), $"trace_{threadIndex}_{Guid.NewGuid()}.log");
                    var listener = new TestAdapter.TraceListener(testFile);
                    
                    Assert.IsNotNull(listener);
                }
                catch (Exception ex)
                {
                    lock (exceptions)
                    {
                        exceptions.Add(ex);
                    }
                }
            });

            threads.Add(thread);
            thread.Start();
        }

        // Wait for all threads to complete
        foreach (var thread in threads)
        {
            thread.Join();
        }

        // Assert - No exceptions should have occurred
        if (exceptions.Count > 0)
        {
            Assert.Fail($"Expected no exceptions, but got {exceptions.Count}. First exception: {exceptions[0].GetType().Name}: {exceptions[0].Message}");
        }
    }

    [TestMethod]
    [Description("Tests that TraceListener correctly removes existing listeners before adding new one")]
    public void TraceListener_RemovesExistingListeners_BeforeAddingNewOne()
    {
        // Arrange
        var existingListener = new DefaultTraceListener();
        Trace.Listeners.Add(existingListener);
        Assert.HasCount(1, Trace.Listeners);

        var testFile = Path.Combine(Path.GetTempPath(), $"trace_{Guid.NewGuid()}.log");

        // Act
        var listener = new TestAdapter.TraceListener(testFile);

        // Assert
        Assert.HasCount(1, Trace.Listeners, "Should have exactly one listener after creating TraceListener");
        Assert.AreSame(listener.GetDefaultTraceListener(), Trace.Listeners[0], 
            "The listener in Trace.Listeners should be the newly created one");
    }

    [TestMethod]
    [Description("Demonstrates the original bug: trying to create TraceListener with empty collection should now work")]
    public void OriginalBugScenario_EmptyListenersCollection_DoesNotThrowException()
    {
        // This test demonstrates the fix for the original bug
        // OLD code would throw: IndexOutOfRangeException when doing RemoveAt(0) on empty collection
        // NEW code should work fine because it uses Clear() instead
        
        // Arrange - simulate the exact condition that caused the bug
        Trace.Listeners.Clear(); // Empty collection - this was the problematic state
        Assert.IsEmpty(Trace.Listeners, "Precondition: Collection should be empty");
        
        var testFile = Path.Combine(Path.GetTempPath(), $"trace_{Guid.NewGuid()}.log");

        // Act & Assert - Should NOT throw any exception
        var listener = new TestAdapter.TraceListener(testFile);
        
        Assert.IsNotNull(listener, "TraceListener should be created successfully even when Trace.Listeners is empty");
        Assert.HasCount(1, Trace.Listeners, "Should have exactly one listener after creation");
    }
}
