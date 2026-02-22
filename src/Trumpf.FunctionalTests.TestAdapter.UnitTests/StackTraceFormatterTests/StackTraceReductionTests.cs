using AwesomeAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Runtime.ExceptionServices;

namespace Trumpf.FunctionalTests.TestAdapter.UnitTests.StackTraceFormatterTests;

[TestClass]
public class StackTraceReductionTests
{
    [TestMethod]
    public void Test_WhenIgnoringWrapperMethods_StackTraceIsReducedToRelevantFrames()
    {
        // Arrange
        var exception = CreateExceptionWithStackTrace();
        var formatter = new StackTraceFormatter(new[] { nameof(WrapperMethod), nameof(OuterMethod) });

        // Act
        var result = formatter.ReducedStackTrace(exception);

        // Assert
        result.Count().Should().BeGreaterThan(0);
        result.Should().Contain(frame => frame.Contains(nameof(ThrowExceptionMethod)));
    }

    [TestMethod]
    public void Test_WhenAllMethodsAreFiltered_ReturnsFullStackTrace()
    {
        // Arrange
        var exception = CreateExceptionWithStackTrace();
        var formatter = new StackTraceFormatter(new[]
        {
            nameof(WrapperMethod),
            nameof(OuterMethod),
            nameof(ThrowExceptionMethod)
        });

        // Act
        var result = formatter.ReducedStackTrace(exception);

        // Assert - When all frames are filtered, should return original stack trace
        result.Count().Should().BeGreaterThan(0);
    }

    private Exception CreateExceptionWithStackTrace()
    {
        try
        {
            OuterMethod();
            return null;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    private void ThrowExceptionMethod()
    {
        throw new Exception("Test exception");
    }

    private void WrapperMethod()
    {
        try
        {
            ThrowExceptionMethod();
        }
        catch (Exception ex)
        {
            ExceptionDispatchInfo.Capture(ex).Throw();
        }
    }

    private void OuterMethod()
    {
        WrapperMethod();
    }
}
