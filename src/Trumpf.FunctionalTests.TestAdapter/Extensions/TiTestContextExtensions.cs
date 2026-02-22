
using System.IO;

namespace Trumpf.FunctionalTests.TestFramework.Interfaces;
public static class TiTestContextExtensions
{
    public static void AddAttachment(this TiTestContext source, string filePath)
    {
        var contents = File.ReadAllBytes(filePath);
        source.AddAttachment(contents, Path.GetFileName(filePath));
    }
}
