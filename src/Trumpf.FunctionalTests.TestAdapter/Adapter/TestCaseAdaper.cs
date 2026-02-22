
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Trumpf.FunctionalTests.TestAdapter;
public class TestCaseAdaper : ITestCase
{
    private readonly TestCase test;
    private readonly AppDomain appDomain;

    public TestCaseAdaper(TestCase test, AppDomain appDomain)
    {
        this.test = test;
        this.appDomain = appDomain;
    }

    public string Source => test.Source;

    public string FullyQualifiedName => test.FullyQualifiedName;

    public string DisplayName => test.DisplayName;

    public Type Type
        => (appDomain
            .GetAssemblies()
            .FirstOrDefault(a => a.GetName().Name == Path.GetFileNameWithoutExtension(test.Source)) ?? Assembly.LoadFrom(test.Source))
            .GetTypes().Where(t => t.FullName == test.FullyQualifiedName).FirstOrDefault();
}
