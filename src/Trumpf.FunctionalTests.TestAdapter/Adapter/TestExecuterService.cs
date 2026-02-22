using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Trumpf.FunctionalTests.Interfaces;
using Trumpf.FunctionalTests.Tags;
using Trumpf.FunctionalTests.TestAdapter.Adapter;
using Trumpf.FunctionalTests.TestAdapter.Execution;
using Trumpf.FunctionalTests.TestFramework.Interfaces;

namespace Trumpf.FunctionalTests.TestAdapter;
public class TestExecuterService
{
    private const string TRACE_LISTENER_FILENAME = "traceListener.log";

    private bool mCancelled = false;

    public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext, ITestExecutionRecorder testExecutionRecorder, ITestClassObject testClassResolver, ILogger logger)
    {
        logger.LogInfo($"TestAdpater version {Assembly.GetExecutingAssembly().GetName().Version}");
        logger.LogInfo($"{nameof(RunTests)} method started.");

        CreateDirectoryIfNew(runContext.TestRunDirectory);

        mCancelled = false;

        var testCaseFiltering = new TestCaseFiltering(runContext, logger);
        var filterExpression = testCaseFiltering.GetFilterForTestExecutor();
        var filteredTests = testCaseFiltering.KeepTestsThatMatchFilter(tests, filterExpression);

        var oneTimeOperations = new OneTimeOperations();

        foreach (var test in filteredTests)
        {
            var eventsTestFails = new ActionsToPerformAtTheInstantTestFails();
            var lastTest = test == filteredTests.Last();
            RunTest(runContext.TestRunDirectory, testExecutionRecorder, logger, lastTest, oneTimeOperations, test, testClassResolver);
        }
    }

    public void RunTest(
        string testRunDirectory,
        ITestExecutionRecorder testExecutionRecorder,
        ILogger logger,
        bool lastTest,
        TiOneTimeOperations oneTimeOperations,
        TestCase test,
        ITestClassObject testClassResolver
        )
    {
        var traceListener = new TraceListener(Path.Combine(Directory.GetParent(test.Source).FullName, TRACE_LISTENER_FILENAME));
        string testResultsFolderForThisTest = Path.Combine(testRunDirectory, $"Deployment_{Environment.UserName}_{DateTime.Now:ddMMyyyy_HH_mm_ss_fff}");

        // The command will not work by using @"\\?\" instead of "\\\\?\\" when fullPath has more than 260 characters !
        Directory.CreateDirectory(testResultsFolderForThisTest);
        logger.LogInfo($"TestResults Directory {testResultsFolderForThisTest} created.");

        var traceListenerLogFileName = Path.GetFullPath(traceListener.GetDefaultTraceListener().LogFileName);
        if (File.Exists(traceListenerLogFileName))
        {
            File.Delete(traceListenerLogFileName);
        }
        logger.LogInfo($"traceListenerLogFileName: {traceListenerLogFileName}");

        try
        {
            // start single test
            logger.LogInfo($"Start execution of the test {test.DisplayName} in AppDomain {AppDomain.CurrentDomain.FriendlyName}.");
            logger.LogInfo($"Test runs from entry assembly '{Assembly.GetEntryAssembly().Location}'");
            DateTimeOffset startTime = DateTime.Now;
            testExecutionRecorder.RecordStart(test);
            var testCaseAdapter = new TestCaseAdaper(test, AppDomain.CurrentDomain);

            Run(
                logger,
                oneTimeOperations,
                testCaseAdapter,
                startTime,
                lastTest,
                testClassResolver,
                out var executeResult,
                out var attachments
            );

            logger.LogInfo($"Execution of the test {test.DisplayName} finished with result {executeResult.Outcome}.");

            // create test result
            var endTime = DateTime.Now;
            var testResult = new TestResult(test)
            {
                Outcome = executeResult.Outcome,
                ErrorMessage = executeResult.ErrorMessage,
                ErrorStackTrace = executeResult.ErrorStackTrace,
                StartTime = startTime,
                EndTime = endTime,
                Duration = endTime - startTime,
            };

            // CRITICAL: Ensure flush before reading
            var defaultListener = traceListener.GetDefaultTraceListener();
            defaultListener.Flush();
            System.Diagnostics.Trace.Flush();

            // add tracing message
            if (TryGetTraceListenerContents(traceListener, out var contents))
            {
                testResult.Messages.Add(new TestResultMessage(TestResultMessage.StandardOutCategory, contents));
                logger.LogInfo($"Trace listener contents added to test result ({contents.Length} characters).");
            }
            else
            {
                logger.LogWarning($"Trace listener file not found at {traceListenerLogFileName}.");
            }

            // add attachments
            var attachmentSets = SaveAttachments(testResultsFolderForThisTest, attachments, logger);
            foreach (var attachmentSet in attachmentSets)
                testResult.Attachments.Add(attachmentSet);

            // report result
            logger.LogInfo($"Result: {testResult.Outcome}");
            testExecutionRecorder.RecordEnd(test, testResult.Outcome);
            testExecutionRecorder.RecordResult(testResult);
        }
        finally
        {
            // CRITICAL: Clean up the trace listener
            var defaultListener = traceListener.GetDefaultTraceListener();
            if (defaultListener != null)
            {
                try
                {
                    defaultListener.Flush();
                    System.Diagnostics.Trace.Listeners.Remove(defaultListener);
                    defaultListener.Dispose();
                    logger.LogInfo("Trace listener properly disposed.");
                }
                catch (Exception ex)
                {
                    logger.LogWarning($"Error disposing trace listener: {ex.Message}");
                }
            }
        }
    }

    private void Run(
        ILogger logger,
        TiOneTimeOperations oneTimeOperations,
        ITestCase test,
        DateTimeOffset startTime,
        bool lastTest,
        ITestClassObject testClassResolver,
        out IntermediateTestResult executeResult,
        out IList<TcAttachment> attachments
        )
    {
        logger.LogInfo($"Run method started for test: {test.Type.Name}");

        var testContext = new TcTestContext(lastTest, startTime.LocalDateTime);
        var actionToPerform = new ActionsToPerformAtTheInstantTestFails();

        var maskedExceptions = new List<Exception>();
        try
        {
            var nonFailTestResult = Test(logger, oneTimeOperations, test, maskedExceptions, lastTest, testContext, testClassResolver, actionToPerform);
            var outcome = nonFailTestResult.NonFailResult switch
            {
                NonFailResult.Passed => TestOutcome.Passed,
                NonFailResult.Skipped => TestOutcome.Skipped,
                NonFailResult.Ignored => TestOutcome.Skipped,
                _ => throw new NotImplementedException()
            };

            logger.LogInfo($"Test outcome determined: {outcome}");
            executeResult = new IntermediateTestResult { Outcome = outcome, ErrorMessage = nonFailTestResult.Message };
        }
        catch (CheckException e)
        {
            logger.LogInfo($"CheckException caught in Run method:{Environment.NewLine}{e}");
            executeResult = new IntermediateTestResult
            {
                Outcome = TestOutcome.Failed,
                ErrorMessage = e.Message,
                ErrorStackTrace = e.StackTrace
            };
        }
        catch (Exception e)
        {
            logger.LogInfo($"Exception caught in Run method:{Environment.NewLine}{e}");
            maskedExceptions.Add(e);
            var distinctExceptions = maskedExceptions.Distinct().ToArray();
            var stackTraceFormatter = new StackTraceFormatter(new[] { typeof(TestExecutor).Namespace });
            executeResult = new IntermediateTestResult
            {
                Outcome = TestOutcome.Failed,
                ErrorMessage = stackTraceFormatter.GetMessagesContent(distinctExceptions),
                ErrorStackTrace = stackTraceFormatter.GetStackTracesContent(distinctExceptions)
            };
        }

        if (mCancelled)
        {
            logger.LogInfo("Test execution was cancelled");
            executeResult = new IntermediateTestResult { Outcome = TestOutcome.Skipped, ErrorMessage = "The test execution was canceled." };
        }

        attachments = testContext.GetAttachments();
        logger.LogInfo($"Run method completed. Attachments count: {attachments.Count}");
    }

    public (NonFailResult NonFailResult, string Message) Test(ILogger logger, TiOneTimeOperations oneTimeOperations, ITestCase testCase, IList<Exception> maskedExceptions, bool lastTest, TiTestContext testContext, ITestClassObject testClassResolver, ActionsToPerformAtTheInstantTestFails actionToPerform)
    {
        logger.LogInfo($"Starting test execution for: {testCase.Type.Name}");

        Check(testCase);

        var instance = testClassResolver.CreateFrom(testCase.Type);
        logger.LogInfo($"Test instance created for type: {testCase.Type.FullName}");
        PreEvents(instance.Object);
        if (Ignore(testCase.Type, out var ignoreMessage))
        {
            logger.LogInfo($"Test {testCase.Type.Name} is being skipped: {ignoreMessage}");
            TcTestSequence.CallAllCleanupActionsIfTestIsLastTestCase(instance.Object, oneTimeOperations, lastTest, logger);
            return (NonFailResult.Skipped, ignoreMessage);
        }
        else
        {
            instance.ResolveWith(testContext, actionToPerform, logger);
            logger.LogInfo($"Dependencies resolved for test: {testCase.Type.Name}");

            (instance.Object as TiEvents)?.SectionEvents.AfterTestResolved(instance.Object.GetType().GetTypeInfo());
            TcTestSequence.Init(instance.Object, oneTimeOperations, logger);

            Exception exception = null;
            if (TcTestSequence.CanExecute(maskedExceptions, instance.Object, oneTimeOperations, lastTest, out var cause, testClassResolver, actionToPerform, logger))
            {
                logger.LogInfo($"Executing test steps for: {testCase.Type.Name}");
                try
                {
                    TcTestSequence.ExecuteTest(instance.Object, maskedExceptions, oneTimeOperations, testClassResolver, actionToPerform, logger);
                    logger.LogInfo($"Test passed: {testCase.Type.Name}");
                    return (NonFailResult.Passed, string.Empty);
                }
                catch (Exception ex)
                {
                    logger.LogInfo($"Test failed: {testCase.Type.Name}{Environment.NewLine}{ex}");
                    exception = ex;
                    throw;
                }
                finally
                {
                    try
                    {
                        (instance.Object as TiEvents)?.SectionEvents.OnTestFinishing(instance.Object.GetType().GetTypeInfo());
                    }
                    catch (Exception)
                    {
                        if (exception != null)
                            maskedExceptions.Add(exception);

#pragma warning disable CA2219 // Do not raise exceptions in finally clauses
                        throw; // rethrow finally exception
#pragma warning restore CA2219 // Do not raise exceptions in finally clauses
                    }
                    finally
                    {
                        TcTestSequence.CallAllCleanupActionsIfTestIsLastTestCase(instance.Object, oneTimeOperations, lastTest, logger);
                    }
                }
            }
            else
            {
                logger.LogInfo($"Test skipped: {testCase.Type.Name} - CanExecute returned false. Reason: {cause}");
                return (NonFailResult.Skipped, $"{nameof(TiCanExecute.CanExecute)} returned false - Reason: {cause}");
            }
        }
    }

    private static void PreEvents(object instantiatedTestClass)
    {
        (instantiatedTestClass as TiPreEvents)?.PreSectionEvents.BeforeTestResolved(instantiatedTestClass.GetType().GetTypeInfo());
    }

    private static void CreateDirectoryIfNew(string testRunDirectory)
    {
        if (Directory.Exists(testRunDirectory) == false)
        {
            Directory.CreateDirectory(testRunDirectory);
        }
    }

    private static bool TryGetTraceListenerContents(TraceListener traceListener, out string contents)
    {
        traceListener.GetDefaultTraceListener().Flush();
        string path = Path.GetFullPath(traceListener.GetDefaultTraceListener().LogFileName);
        if (File.Exists(path))
        {
            contents = File.ReadAllText(path);
            return true;
        }
        else
        {
            contents = null;
            return false;
        }
    }

    private static void Check(ITestCase testCase)
    {
        var collectionTestStepsIds = testCase.TestStepMethods().Select(e => e.Id);
        var dupplicatedIds = collectionTestStepsIds
            .GroupBy(id => id)
            .Where(id => id.Count() > 1);
        var negativeIds = collectionTestStepsIds
            .Where(id => id < 0);

        if (dupplicatedIds.Any())
        {
            throw new DupplicatedIdsException(dupplicatedIds.SelectMany(e => e));
        }
        else if (negativeIds.Any())
        {
            throw new NegativeIdsException(negativeIds);
        }
    }

    private static bool Ignore(Type testClassType, out string ignoreMessage)
    {
        var ignoreAttributes = testClassType
            .GetCustomAttributes()
            .Where(attribute => attribute is TcIgnoreTestAttribute)
            .Cast<TcIgnoreTestAttribute>();

        if (ignoreAttributes.Any())
        {
            ignoreMessage = GetReasonsIgnoreAttributes(ignoreAttributes);
            return true;
        }
        else
        {
            ignoreMessage = null;
            return false;
        }
    }

    private static string GetReasonsIgnoreAttributes(IEnumerable<TcIgnoreTestAttribute> typeAttributes)
    {
        string descriptionIgnoreAttributes = string.Empty;

        foreach (TcIgnoreTestAttribute attr in typeAttributes)
        {
            // check if a message is given to the contructor of the attribute
            string reasonIgnore = string.IsNullOrEmpty(((TiHasTags)attr).Tags.First().Description) == false
                ? $" Reason = {attr.Tags.First().Description}."
                : string.Empty;

            descriptionIgnoreAttributes += $"{attr.GetType().Name} is used. {reasonIgnore}{Environment.NewLine}";
        }

        return descriptionIgnoreAttributes;
    }

    private static AttachmentSet[] SaveAttachments(string testResultsFolderForThisTest, IList<TcAttachment> attachments, ILogger logger)
        => attachments.Select(attachment =>
        {
            // write file the test result folder
            var targetFilePath = Path.Combine(testResultsFolderForThisTest, attachment.Filename);
            if (attachment.ByteContents != null)
            {
                File.WriteAllBytes(targetFilePath, attachment.ByteContents);
            }
            else if (attachment.StringContent != null)
            {
                File.WriteAllText(targetFilePath, attachment.StringContent);
            }

            logger.LogInfo($"The file {attachment.Filename} has been copied to the test results folder.");

            // attach file
            Uri testfile = new(targetFilePath, UriKind.Absolute);
            UriDataAttachment uriDataAttachment = new(testfile, attachment.Filename);
            AttachmentSet attachmentSet = new(new Uri("attachment://dummy"), "attachment");
            attachmentSet.Attachments.Add(uriDataAttachment);
            logger.LogInfo($"The file {attachment.Filename} is now part of the attachments.");
            return attachmentSet;
        })
            .ToArray();

    public void Cancel()
    {
        mCancelled = true;
    }
}