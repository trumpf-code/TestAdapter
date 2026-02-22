using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Trumpf.FunctionalTests.Interfaces;
using Trumpf.FunctionalTests.Tags;
using Trumpf.FunctionalTests.TestAdapter.Adapter;
using Trumpf.FunctionalTests.TestAdapter.Extensions;
using Trumpf.FunctionalTests.TestAdapter.Shared;

namespace Trumpf.FunctionalTests.TestAdapter;
public class TestDiscovererService
{
    private TestCaseFiltering testCaseFilter;

    private readonly List<TestCase> testsInAssembly = new();

    public IEnumerable<TestCase> DiscoverTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext, ILogger logger)
    {
        List<TestCase> testCasesDiscovered = GetClassesUsingTestStepAttribute(sources, logger);

        // Get filter expression and skip discovery in case filter expression has parsing error.
        testCaseFilter = new TestCaseFiltering(discoveryContext, logger);
        var filterExpression = testCaseFilter.GetFilterForTestDiscoverer();
        var filteredTests = testCaseFilter.KeepTestsThatMatchFilter(testCasesDiscovered, filterExpression);
        return filteredTests;
    }

    /// <summary>
    /// Retrieve all types where at least 1 method is using the TcTestStep attribute.
    /// </summary>
    /// <param name="sources">containers provided from the test platform to discover tests </param>
    /// <param name="logger">instance to log any status</param>
    /// <returns> A collection of TestCase objects referring to types where at least 1 method is using the TcTestStep attribute. In case that several methods
    /// are using the same test step id, an exception will be thrown</returns>
    public List<TestCase> GetClassesUsingTestStepAttribute(IEnumerable<string> sources, ILogger logger)
    {
        foreach (string source in sources.Distinct())
        {
            // in case we want that each test case implements an interface instead of using TcTestStepAttribute (like with TiExecute in the past), there is a risk that a class of the test adapter implements this interface.
            if (source.Contains(GetType().Namespace) || source.Contains(typeof(TiWiring).Assembly.GetName().Name))
            {
                continue;
            }

            logger.LogInfo($"The following source will now be analysed: {source}.");

            // check for assemblies already loaded
            Assembly sourceDll = AppDomain.CurrentDomain
                .GetAssemblies()
                .FirstOrDefault(a => Path.GetFullPath(a.GetName().Name) == source) ?? Assembly.LoadFrom(source);
            logger.LogInfo($"Get types from assembly {source}.");

            // the exception ReflectionTypeLoadException is thrown when a type that contains all the needed information can’t be loaded.
            // source: http://haacked.com/archive/2012/07/23/get-all-types-in-an-assembly.aspx/
            IEnumerable<Type> typesFromDll = null;
            try
            {
                typesFromDll = sourceDll.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                typesFromDll = e.Types.Where(t => t != null);
            }

            foreach (var type in typesFromDll)
            {
                // for avoiding to take the +<>c type into account
                if (Attribute.GetCustomAttribute(type, typeof(CompilerGeneratedAttribute)) != null)
                {
                    continue;
                }

                // Test cases need to be instantiated. The Base class will not be taken into account if it is abstract (which is ok, a base class is not a test)
                if (type.IsAbstract)
                {
                    continue;
                }

                if (type.IsNested)
                {
                    continue;
                }

                var enumerableTestStepAttribute = type
                    .GetRuntimeMethods()
                    .WhereMethodsHaveAttribute(typeof(TcTestStepAttribute));

                // Only tests with at least one attribute TcTestStep will be taken into account
                if (enumerableTestStepAttribute.Any())
                {
                    logger.LogInfo($"The test step attribute is used {enumerableTestStepAttribute.Count()} times in {type.FullName}. The test {type} will be taken into account.");

                    string testName = string.Empty;

                    testName = type.Name;
                    AddTestCaseInTestCollection(testName, type, logger);
                }
            }
        }

        return testsInAssembly;
    }

    private void AddTestCaseInTestCollection(string testName, Type type, ILogger logger)
    {
        // For each class with TcTestStep attrubutes, a object of type TestCase will be created. The URI of the test executor will be stamped to the object.
        var testCaseDiscovered = new TestCase(testName, new Uri(TestExecutor.URL_TESTEXECUTOR), type.Assembly.Location);

        var tags = CollectTraitsForType(type);
        var valueTiRequirement = RetrieveValueTiRequirementFromType(type);

        // has same effect than (testCaseDiscovered.Traits.Add("", tag));
#pragma warning disable CS0618 // Type or member is obsolete
        testCaseDiscovered.SetPropertyValue(TestProperty.Register("testClass.TestCategory", "TestCategory", typeof(string[]), TestPropertyAttributes.Hidden | TestPropertyAttributes.Trait, typeof(TestCase)), tags.ToArray());
#pragma warning restore CS0618 // Type or member is obsolete

        testCaseDiscovered.SetPropertyValue(TestProperty.Register("testClass.RequirementName", "RequirementName", typeof(string[]), TestPropertyAttributes.Hidden, typeof(TestCase)), valueTiRequirement.ToArray());

        testCaseDiscovered.FullyQualifiedName = $"{type.Namespace}.{testName}";
        testCaseDiscovered.DisplayName = $"{testName}";
        SetLineNumberAndCodeFilePathForTest(testCaseDiscovered, type, logger);
        testsInAssembly.Add(testCaseDiscovered);
    }

    private static List<string> RetrieveValueTiRequirementFromType(Type type)
        => TcTagsRetriever.GetTiRequirementName(type);

    /// <summary>
    /// Get Tags + last part of namespace
    /// </summary>
    /// <param name="type">type of the test</param>
    /// <returns>a collection of traits for the test case, including the attribute and the last part of namespace</returns>
    private static IEnumerable<string> CollectTraitsForType(Type type)
    {
        // Retrieve all tags
        _ = new TcTagsRetriever();
        var traits = TcTagsRetriever.GetTags(type).Distinct();

        // add function module (last part of testname) as trait
        return traits.Append(type.Namespace.Split('.').Last());
    }

    /// <summary>
    /// Create a navigation session to the source code by double-clicking on the test name in the test explorer
    /// </summary>
    /// <param name="testCaseDiscovered">the TestCase being executed</param>
    /// <param name="type">the type of the test</param>
    /// <param name="logger">instance to log</param>
    private static void SetLineNumberAndCodeFilePathForTest(TestCase testCaseDiscovered, Type type, ILogger logger)
    {
        // Create navigation session
        var diaSession = new DiaSession(testCaseDiscovered.Source);

        if (diaSession == default)
        {
            logger.LogInfo($"The navigation session could not be create for the test {type}");
        }

        string methodNameWithLowestTcTestStepId = type
            .GetMethods()
            .WhereMethodsHaveAttribute(typeof(TcTestStepAttribute))
            .OrderBy(m => m.GetCustomAttribute<TcTestStepAttribute>().Id)
            .Select(m => m.Name)
            .FirstOrDefault();

        if (methodNameWithLowestTcTestStepId == null)
        {
            logger.LogInfo($"No method with a TcTestStep attribute could be retrieved inside the test {type}");
            return;
        }

        var symbolReader = diaSession.GetNavigationData(type.FullName, methodNameWithLowestTcTestStepId);

        if (symbolReader == null)
        {
            logger.LogInfo($"The navigation data could not be retrieved for the test {type}");
            logger.LogInfo($"Input Parameters: type.Name = {type.FullName} / method with lowest TcTestStep ID = {methodNameWithLowestTcTestStepId}");
            return;
        }

        testCaseDiscovered.CodeFilePath = symbolReader.FileName;
        testCaseDiscovered.LineNumber = symbolReader.MinLineNumber;
        return;
    }
}

[FileExtension(".dll")]
[DefaultExecutorUri(TestExecutor.URL_TESTEXECUTOR)]
public class TestDiscoverer : ITestDiscoverer
{
    /// <summary>
    /// This method will be called either by running tests with command line or with the IDE by runnning All tests
    /// </summary>
    /// <param name="sources">containers provided from the test platform to discover tests.</param>
    /// <param name="discoveryContext">to figure out the settings used for the current session.</param>
    /// <param name="logger">instance to log any status.</param>
    /// <param name="discoverySink">to report back on test cases found to the test platform.</param>
    public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext, IMessageLogger messageLogger, ITestCaseDiscoverySink discoverySink)
    {
        if (discoverySink == null)
        {
            messageLogger?.SendMessage(TestMessageLevel.Error, "discoverySink is null. End of tests discovery.");
            return;
        }

        if (sources == null)
        {
            messageLogger?.SendMessage(TestMessageLevel.Error, "Sources are empty. End of tests discovery.");
            return;
        }

        messageLogger.SendMessage(TestMessageLevel.Informational, "Tests Discovery started.");

        var testDiscovererService = new TestDiscovererService();

        var logger = new Logger(messageLogger);
        var filteredTests = testDiscovererService.DiscoverTests(sources, discoveryContext, logger);

        logger.LogInfo($"{filteredTests.Count()} tests have been kept with the given filter.");

        foreach (var testCase in filteredTests)
        {
            discoverySink.SendTestCase(testCase);
        }
    }
}
