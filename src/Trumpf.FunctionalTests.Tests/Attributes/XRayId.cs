using Trumpf.FunctionalTests.Attributes;

namespace Adapter_Tests.Attributes;

public class XRayId : XRayIdBaseAttribute
{
    public XRayId(int id) : base(id)
    {
    }

    protected override string JiraUrl => "https://your-jira-instance.example.com";

    public override string Project => "MYPROJECT";
}
