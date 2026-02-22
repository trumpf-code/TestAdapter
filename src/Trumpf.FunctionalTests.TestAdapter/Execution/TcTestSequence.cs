
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using Trumpf.FunctionalTests.Interfaces;
using Trumpf.FunctionalTests.Tags;

namespace Trumpf.FunctionalTests.TestAdapter.Execution;
public class TcTestSequence
{
    public static void ExecuteTest(object testClass, IList<Exception> maskedExceptions, TiOneTimeOperations oneTimeOperations, ITestClassObject testClassResolver, ActionsToPerformAtTheInstantTestFails actionToPerform, ILogger logger)
    {
        logger.LogInfo($"ExecuteTest started for: {testClass.GetType().Name}");

        Exception executeException = null;
        Exception cleanupException = null;

        try
        {
            if (testClass is TiPrepare prepare)
            {
                logger.LogInfo("Executing Prepare method");
                var methodInfo = GetIndirectMethodInfoUtil.GetIndirectMethodInfo((TiPrepare i) => i.Prepare());
                (testClass as TiEvents)?.SectionEvents.OnPrepareStarting(methodInfo);
                try
                {
                    prepare.Prepare();
                }
                catch (Exception e)
                {
                    logger.LogInfo($"Prepare method failed: {e.Message}");
                    executeException = e;
                    throw;
                }
                finally
                {
                    logger.LogInfo("Prepare method finishing");
                    (testClass as TiEvents)?.SectionEvents.OnPrepareFinishing(methodInfo, executeException);
                }
            }

            logger.LogInfo("Executing test steps");
            var testStepsMethods = testClass.GetType()
                    .GetRuntimeMethods()
                    .Where(item => item.GetCustomAttribute<TcTestStepAttribute>() != null)
                    .OrderBy(item => item.GetCustomAttribute<TcTestStepAttribute>().Id);

            foreach (var testStepMethod in testStepsMethods)
            {
                try
                {
                    var stepAttribute = testStepMethod.GetCustomAttribute<TcTestStepAttribute>();
                    logger.LogInfo($"Executing test step {stepAttribute.Id}: {testStepMethod.Name}");
                    (testClass as TiEvents)?.SectionEvents.OnTestStepStarting(testStepMethod);
                    testStepMethod.Invoke(testClass, Array.Empty<object>());
                }
                catch (TargetInvocationException e)
                {
                    // unpack Invoke exception
                    // TODO: is this a warning?
                    logger.LogInfo($"Test step {testStepMethod.Name} failed with TargetInvocationException: {e.InnerException?.Message}");
                    executeException = e.InnerException;
                    ExceptionDispatchInfo.Capture(e.InnerException).Throw();
                }
                catch (Exception e)
                {
                    // TODO: is this a warning?
                    logger.LogInfo($"Test step {testStepMethod.Name} failed with Exception: {e.Message}");
                    executeException = e;
                    throw;
                }
                finally
                {
                    logger.LogInfo($"Test step {testStepMethod.Name} finishing");
                    (testClass as TiEvents)?.SectionEvents.OnTestStepFinishing(testStepMethod, executeException);
                }
            }
        }
        catch (Exception e)
        {
            // exception will not be masked, why we don't add it to the exception list
            logger.LogInfo($"ExecuteTest caught exception: {e.Message}");
            executeException = e;

            RunCustomActionsWhenTestFails(actionToPerform, logger);

            throw;
        }
        finally
        {
            var methodInfo = GetIndirectMethodInfoUtil.GetIndirectMethodInfo((TiCleanup i) => i.Clean(null));
            try
            {
                if (testClass is TiCleanup cleanup)
                {
                    logger.LogInfo("Executing Cleanup method");
                    (testClass as TiEvents)?.SectionEvents.OnCleanStarting(methodInfo);
                    cleanup.Clean(executeException);
                    logger.LogInfo("Cleanup method completed successfully");
                }
            }
            // we have two exceptions, one from the Prepare or test steps and one from the Cleanup
            catch (Exception e) when (executeException != null)
            {
                cleanupException = e;

                maskedExceptions.Add(cleanupException);

                logger.LogWarning("Cleanup threw exception, yet Execute exception dominates.");
                logger.LogWarning("Cleanup exception:");
                logger.LogWarning(cleanupException.ToString());
            }
            catch (Exception e) when (executeException == null)
            {
                logger.LogInfo($"Cleanup threw exception: {e.Message}");
                cleanupException = e;
                RunCustomActionsWhenTestFails(actionToPerform, logger);
                logger.LogInfo("Rethrowing cleanup exception from finally block");
#pragma warning disable CA2219 // Do not raise exceptions in finally clauses
                throw;
#pragma warning restore CA2219 // Do not raise exceptions in finally clauses
            }
            finally
            {
                if (testClass is TiCleanup)
                {
                    logger.LogInfo("Cleanup method finishing");
                    (testClass as TiEvents)?.SectionEvents.OnCleanFinishing(methodInfo, cleanupException);
                }
                Dispose(testClass, oneTimeOperations, testClassResolver, logger);
                logger.LogInfo("ExecuteTest completed");
            }
        }
    }

    public static bool CanExecute(IList<Exception> maskedExceptions, object testClass, TiOneTimeOperations oneTimeOperations, bool lastTest, out string whyNot, ITestClassObject testClassResolver, ActionsToPerformAtTheInstantTestFails actionToPerform, ILogger logger)
    {
        logger.LogInfo($"CanExecute check for: {testClass.GetType().Name}");

        if (testClass is TiCanExecute execute)
        {
            Exception exception = null;
            string cause;
            var methodInfo = GetIndirectMethodInfoUtil.GetIndirectMethodInfo((TiCanExecute i) => i.CanExecute(out cause));
            try
            {
                (testClass as TiEvents)?.SectionEvents.OnCanExecuteStarting(methodInfo);
                bool canExecuteResult = execute.CanExecute(out whyNot);

                if (!canExecuteResult)
                {
                    logger.LogInfo($"CanExecute returned false. Reason: {whyNot}");
                    Dispose(testClass, oneTimeOperations, testClassResolver, logger);
                    return false;
                }
                else
                {
                    logger.LogInfo($"CanExecute returned true.");
                    return true;
                }
            }
            catch (Exception e)
            {
                logger.LogInfo($"CanExecute failed with exception: {e.Message}");
                exception = e;
                maskedExceptions.Add(e);
                RunCustomActionsWhenTestFails(actionToPerform, logger);
                Dispose(testClass, oneTimeOperations, testClassResolver, logger);
                throw;
            }
            finally
            {
                try
                {
                    (testClass as TiEvents)?.SectionEvents.OnCanExecuteFinishing(methodInfo, exception);
                }
                catch
                {
                    logger.LogWarning("Exception occurred in OnCanExecuteFinishing event");
                    if (exception != null)
                        maskedExceptions.Add(exception);

                    logger.LogWarning("Rethrowing finally exception from CanExecute");
#pragma warning disable CA2219 // Do not raise exceptions in finally clauses
                    throw; // rethrow finally exception
#pragma warning restore CA2219 // Do not raise exceptions in finally clauses
                }
            }
        }
        else
        {
            whyNot = null;
            return true;
        }
    }

    public static void Init(object resolvedTestClass, TiOneTimeOperations oneTimeOperations, ILogger logger)
    {
        logger.LogInfo($"Init started for: {resolvedTestClass.GetType().Name}");

        if (resolvedTestClass is TiTestrunEnvironment environment)
        {
            var InitDeinitOfCurrentTest = environment.GetOperationsToBeTriggeredOnceInAssembly();
            oneTimeOperations.UpdateTiOneTimeInitializationCollectionWith(InitDeinitOfCurrentTest);
            var initsNeeded = oneTimeOperations.GetRemainingInitOperationsToProcess();
            Exception exception = null;

            if (initsNeeded.Any())
            {
                logger.LogInfo($"Executing {initsNeeded.Count()} one-time init operations");
                foreach (var initAction in initsNeeded)
                {
                    try
                    {
                        logger.LogInfo($"Executing one-time init for: {initAction.GetType().Name}");
                        (resolvedTestClass as TiEvents)?.SectionEvents.OnOneTimeInitStarting(initAction.GetType().GetTypeInfo());
                        initAction.Init();
                        oneTimeOperations.UpdateTypesAlreadyProcessedWith(initAction);
                    }
                    catch (Exception e)
                    {
                        exception = e;
                        logger.LogInfo($"Method Execute() in TcTestSequence {e}");
                        throw;
                    }
                    finally
                    {
                        (resolvedTestClass as TiEvents)?.SectionEvents.OnOneTimeInitFinishing(initAction.GetType().GetTypeInfo(), exception);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Calls the dispose methods of all registered objects in the container
    /// </summary>
    public static void Dispose(object testClass, TiOneTimeOperations oneTimeOperations, ITestClassObject testClassResolver, ILogger logger)
    {
        logger.LogInfo($"{Environment.NewLine}The services resolved from the stashbox container will be disposed.");
        testClassResolver.Dispose();
        logger.LogInfo("The services have been disposed.");
    }

    /// <summary>
    /// Call the Deinit method if the test case if the last one
    /// </summary>
    public static void CallAllCleanupActionsIfTestIsLastTestCase(object testClass, TiOneTimeOperations oneTimeOperations, bool lastTest, ILogger logger)
    {
        if (lastTest)
        {
            logger.LogInfo("Executing cleanup actions for last test");
            Exception exception = null;
            foreach (var deinitAction in oneTimeOperations.GetTiOneTimeInitializationTypes())
            {
                try
                {
                    logger.LogInfo($"Executing one-time deinit for: {deinitAction.GetType().Name}");
                    (testClass as TiEvents)?.SectionEvents.OnOneTimeDeinitStarting(deinitAction.GetType().GetTypeInfo());
                    deinitAction.Deinit();
                    oneTimeOperations.UpdateTypesAlreadyProcessedWith(deinitAction);
                }
                catch (Exception e)
                {
                    exception = e;
                    logger.LogInfo($"{DateTime.Now}: An exception has been thrown in the Deinit method of the class {deinitAction}.");
                    logger.LogInfo(exception.ToString());
                }
                finally
                {
                    (testClass as TiEvents)?.SectionEvents.OnOneTimeDeinitFinishing(deinitAction.GetType().GetTypeInfo(), exception);
                }
            }
        }
    }

    private static void RunCustomActionsWhenTestFails(ActionsToPerformAtTheInstantTestFails eventsFails, ILogger logger)
    {
        if (eventsFails.GetType().GetProperty("ListActionsToPerform", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(eventsFails) is IEnumerable<Action> listActions)
        {
            logger.LogInfo($"Execute the delegates assigned to the object of type ActionsToPerformAtTheInstantTestFails");
            foreach (var action in listActions)
            {
                action.Invoke();
            }
        }
    }
}