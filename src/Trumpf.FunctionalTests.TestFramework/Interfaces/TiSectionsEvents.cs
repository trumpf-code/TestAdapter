using System;
using System.Reflection;

namespace Trumpf.FunctionalTests.Interfaces;

/// <summary>
/// Interface for test section events. Implementations must not throw exceptions.
/// </summary>
public interface TiSectionEvents
{
    /// <summary>
    /// Called after the test has been resolved.
    /// Must not throw any exception.
    /// </summary>
    /// <param name="typeInfo">The type information of the test class.</param>
    void AfterTestResolved(TypeInfo typeInfo);

    /// <summary>
    /// Called when the test is finishing.
    /// Must not throw any exception.
    /// </summary>
    /// <param name="typeInfo">The type information of the test class.</param>
    void OnTestFinishing(TypeInfo typeInfo);

    /// <summary>
    /// Called when a one-time initialization is starting.
    /// Must not throw any exception.
    /// </summary>
    /// <param name="typeInfo">The type information of the initialization class.</param>
    void OnOneTimeInitStarting(TypeInfo typeInfo);

    /// <summary>
    /// Called when a one-time initialization is finishing.
    /// Must not throw any exception.
    /// </summary>
    /// <param name="typeInfo">The type information of the initialization class.</param>
    /// <param name="exception">The exception that occurred during initialization, if any.</param>
    void OnOneTimeInitFinishing(TypeInfo typeInfo, Exception exception);

    /// <summary>
    /// Called when a one-time de-initialization is starting.
    /// Must not throw any exception.
    /// </summary>
    /// <param name="typeInfo">The type information of the de-initialization class.</param>
    void OnOneTimeDeinitStarting(TypeInfo typeInfo);

    /// <summary>
    /// Called when a one-time de-initialization is finishing.
    /// Must not throw any exception.
    /// </summary>
    /// <param name="typeInfo">The type information of the de-initialization class.</param>
    /// <param name="exception">The exception that occurred during de-initialization, if any.</param>
    void OnOneTimeDeinitFinishing(TypeInfo typeInfo, Exception exception);

    /// <summary>
    /// Called when CanExecute check is starting.
    /// Must not throw any exception.
    /// </summary>
    /// <param name="methodInfo">The method information of the CanExecute method.</param>
    void OnCanExecuteStarting(MethodInfo methodInfo);

    /// <summary>
    /// Called when CanExecute check is finishing.
    /// Must not throw any exception.
    /// </summary>
    /// <param name="methodInfo">The method information of the CanExecute method.</param>
    /// <param name="exception">The exception that occurred during CanExecute, if any.</param>
    void OnCanExecuteFinishing(MethodInfo methodInfo, Exception exception);

    /// <summary>
    /// Called when the Prepare method is starting.
    /// Must not throw any exception.
    /// </summary>
    /// <param name="methodInfo">The method information of the Prepare method.</param>
    void OnPrepareStarting(MethodInfo methodInfo);

    /// <summary>
    /// Called when the Prepare method is finishing.
    /// Must not throw any exception.
    /// </summary>
    /// <param name="methodInfo">The method information of the Prepare method.</param>
    /// <param name="exception">The exception that occurred during Prepare, if any.</param>
    void OnPrepareFinishing(MethodInfo methodInfo, Exception exception);

    /// <summary>
    /// Called when a test step is starting.
    /// Must not throw any exception.
    /// </summary>
    /// <param name="methodInfo">The method information of the test step.</param>
    void OnTestStepStarting(MethodInfo methodInfo);

    /// <summary>
    /// Called when a test step is finishing.
    /// Must not throw any exception.
    /// </summary>
    /// <param name="methodInfo">The method information of the test step.</param>
    /// <param name="exception">The exception that occurred during the test step, if any.</param>
    void OnTestStepFinishing(MethodInfo methodInfo, Exception exception);

    /// <summary>
    /// Called when the Cleanup method is starting.
    /// Must not throw any exception.
    /// </summary>
    /// <param name="methodInfo">The method information of the Cleanup method.</param>
    void OnCleanStarting(MethodInfo methodInfo);

    /// <summary>
    /// Called when the Cleanup method is finishing.
    /// Must not throw any exception.
    /// </summary>
    /// <param name="methodInfo">The method information of the Cleanup method.</param>
    /// <param name="exception">The exception that occurred during Cleanup, if any.</param>
    void OnCleanFinishing(MethodInfo methodInfo, Exception exception);
}
