using AwesomeAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Trumpf.FunctionalTests.Interfaces;
using Trumpf.FunctionalTests.Tags;
using Trumpf.FunctionalTests.TestAdapter.UnitTests.Helpers;

namespace Trumpf.FunctionalTests.TestAdapter.UnitTests.TestExecuterServiceTests;

[TestClass]
public class EventsTests
{
    [TestMethod]
    public void Test_WhenTestExecutes_AllEventsAreCalled()
    {
        // Arrange
        var service = new TestExecuterService();
        var preEventsHandler = new PreEventsHandler();
        var eventsHandler = new EventsHandler();
        var testClassObject = new TestClassWithEvents
        {
            PreSectionEvents = preEventsHandler,
            SectionEvents = eventsHandler
        };

        // Act
        var result = service.RunTestHelper<TestClassWithEvents>(testClassObject);

        // Assert
        preEventsHandler.WasBeforeTestResolvedCalled.Should().BeTrue();
        preEventsHandler.ResolvedType.Should().Be(typeof(TestClassWithEvents));

        eventsHandler.CalledMethods.Should().HaveCount(4);
        eventsHandler.CalledMethods.Select(e => e.Name).Should().BeEquivalentTo(new[]
        {
            nameof(TiSectionEvents.AfterTestResolved),
            nameof(TiSectionEvents.OnTestStepStarting),
            nameof(TiSectionEvents.OnTestStepFinishing),
            nameof(TiSectionEvents.OnTestFinishing)
        });
    }

    private class PreEventsHandler : TiPreSectionEvents
    {
        public bool WasBeforeTestResolvedCalled { get; private set; }
        public Type ResolvedType { get; private set; }

        public void BeforeTestResolved(TypeInfo typeInfo)
        {
            WasBeforeTestResolvedCalled = true;
            ResolvedType = typeInfo;
        }
    }

    private class EventsHandler : TiSectionEvents
    {
        public List<MethodInfo> CalledMethods { get; } = new();

        public void AfterTestResolved(TypeInfo typeInfo)
            => CalledMethods.Add(MethodInfoHelper.GetMethodInfo(AfterTestResolved));

        public void OnCanExecuteFinishing(MethodInfo methodInfo, Exception exception)
            => CalledMethods.Add(MethodInfoHelper.GetMethodInfo(OnCanExecuteFinishing));

        public void OnCanExecuteStarting(MethodInfo methodInfo)
            => CalledMethods.Add(MethodInfoHelper.GetMethodInfo(OnCanExecuteStarting));

        public void OnCleanFinishing(MethodInfo methodInfo, Exception exception)
            => CalledMethods.Add(MethodInfoHelper.GetMethodInfo(OnCleanFinishing));

        public void OnCleanStarting(MethodInfo methodInfo)
            => CalledMethods.Add(MethodInfoHelper.GetMethodInfo(OnCleanStarting));

        public void OnOneTimeDeinitFinishing(TypeInfo typeInfo, Exception exception)
            => CalledMethods.Add(MethodInfoHelper.GetMethodInfo(OnOneTimeDeinitFinishing));

        public void OnOneTimeDeinitStarting(TypeInfo typeInfo)
            => CalledMethods.Add(MethodInfoHelper.GetMethodInfo(OnOneTimeDeinitStarting));

        public void OnOneTimeInitFinishing(TypeInfo typeInfo, Exception exception)
            => CalledMethods.Add(MethodInfoHelper.GetMethodInfo(OnOneTimeInitFinishing));

        public void OnOneTimeInitStarting(TypeInfo typeInfo)
            => CalledMethods.Add(MethodInfoHelper.GetMethodInfo(OnOneTimeInitStarting));

        public void OnPrepareFinishing(MethodInfo methodInfo, Exception exception)
            => CalledMethods.Add(MethodInfoHelper.GetMethodInfo(OnPrepareFinishing));

        public void OnPrepareStarting(MethodInfo methodInfo)
            => CalledMethods.Add(MethodInfoHelper.GetMethodInfo(OnPrepareStarting));

        public void OnTestFinishing(TypeInfo typeInfo)
            => CalledMethods.Add(MethodInfoHelper.GetMethodInfo(OnTestFinishing));

        public void OnTestStepFinishing(MethodInfo methodInfo, Exception exception)
            => CalledMethods.Add(MethodInfoHelper.GetMethodInfo(OnTestStepFinishing));

        public void OnTestStepStarting(MethodInfo methodInfo)
            => CalledMethods.Add(MethodInfoHelper.GetMethodInfo(OnTestStepStarting));
    }

    private class TestClassWithEvents : TiPreEvents, TiEvents
    {
        public TiPreSectionEvents PreSectionEvents { get; set; }
        public TiSectionEvents SectionEvents { get; set; }

        [TcTestStep(1)]
        public void ExecuteStep()
            => Expression.Empty();
    }
}
