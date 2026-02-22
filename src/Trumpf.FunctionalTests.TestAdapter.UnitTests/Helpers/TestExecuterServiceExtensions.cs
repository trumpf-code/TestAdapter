using System;
using System.Collections.Generic;
using Trumpf.FunctionalTests.TestAdapter.UnitTests.Mocks;

namespace Trumpf.FunctionalTests.TestAdapter.UnitTests.Helpers;

public static class TestExecuterServiceExtensions
{
    public static (NonFailResult NonFailResult, string Message) RunTestHelper<TTestClass>(
        this TestExecuterService service, 
        ITestClassObject testClassObject, 
        List<Exception> maskedExceptions = null)
    {
        var logger = new LoggerMock();
        var oneTimeOperations = new OneTimeOperationsMock();
        var testCase = new TestCaseMock(typeof(TTestClass));
        var testContext = new TestContextMock();
        var actionToPerform = new ActionsToPerformAtTheInstantTestFails();

        return service.Test(
            logger,
            oneTimeOperations,
            testCase,
            maskedExceptions ?? new List<Exception>(),
            lastTest: false,
            testContext,
            testClassObject,
            actionToPerform);
    }

    public static (NonFailResult NonFailResult, string Message) RunTestHelper<TTestClass>(
        this TestExecuterService service, 
        object testCaseObject)
    {
        return RunTestHelper<TTestClass>(service, new TestClassObjectMock(testCaseObject));
    }

    public static (NonFailResult NonFailResult, string Message) RunTestHelper<TTestClass>(
        this TestExecuterService service, 
        object testCaseObject, 
        List<Exception> maskedExceptions)
    {
        return RunTestHelper<TTestClass>(service, new TestClassObjectMock(testCaseObject), maskedExceptions);
    }
}
