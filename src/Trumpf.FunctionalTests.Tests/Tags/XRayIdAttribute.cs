
using System;
using Trumpf.FunctionalTests.Interfaces;

namespace Trumpf.FunctionalTests.Attributes;
[AttributeUsage(AttributeTargets.Class)]
public abstract class XRayIdBaseAttribute : Attribute, TiRequirement
{
    /// <summary>
    /// JiraUrl like "https://your-jira-instance.example.com"
    /// </summary>
    protected abstract string JiraUrl { get; }

    /// <summary>
    /// Project like "MYPROJECT"
    /// </summary>
    public virtual string Project { get; }

    public int Id { get; }

    public string Name => $"[{Project}-{Id}]({JiraUrl}/browse/{Project}-{Id})";

    public XRayIdBaseAttribute(int id) => Id = id;
}
