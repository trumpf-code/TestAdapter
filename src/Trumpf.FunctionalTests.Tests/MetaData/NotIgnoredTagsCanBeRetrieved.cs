
using Adapter_Tests.Attributes;
using AwesomeAssertions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Trumpf.FunctionalTests.Attributes;
using Trumpf.FunctionalTests.Tags;

namespace Adapter_Tests.Metadata;
[TcCustomTag("personalizedAttribute")]
[TcTestCategory("Target.K08")]
[ComponentTest]
[XRayId(1234)]
public class NotIgnoredTagsCanBeRetrieved : BaseClass
{

    [TcTestStep(1)]
    public override void Execute()
    {
        var pathCallingAssembly = Path.GetFullPath(Assembly.GetCallingAssembly().Location);
        var assemblyTestExecutor = Assembly.LoadFile(pathCallingAssembly);
        var typeTagsRetriever = assemblyTestExecutor.GetType("Trumpf.FunctionalTests.TestAdapter.TcTagsRetriever");

        var instanceTestExecutor = Activator.CreateInstance(typeTagsRetriever);
        var methodRetrieveTags = typeTagsRetriever.GetMethod("GetTags");

        object ownClass = typeof(NotIgnoredTagsCanBeRetrieved);

        List<string> listTag = methodRetrieveTags.Invoke(instanceTestExecutor, new object[] { ownClass }) as List<string>;

        listTag.Should().HaveCount(4);
        listTag.Should().Contain("ComponentTest");
        listTag.Should().Contain("Target.K08");
        listTag.Should().Contain("personalizedAttribute");
    }

    [TcTestStep(2)]
    public static void Execute2()
    {
    }

    [TcTestStep(3)]
    public static void Execute3()
    {
    }
}
