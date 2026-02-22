
using System;
using System.Collections.Generic;
using System.Linq;
using Trumpf.FunctionalTests.TestFramework.Interfaces;

namespace Trumpf.FunctionalTests.TestAdapter;
public class TcTestContext : TiTestContext
{
    private readonly Dictionary<string, TcAttachment> attachments = new();

    public TcTestContext(bool isLastTest, DateTime startTime)
    {
        this.isLastTest = isLastTest;
        StartTime = startTime;
    }

    private bool isLastTest { get; set; }

    public DateTime StartTime { get; set; }

    public void ClearAttachments()
        => attachments.Clear();

    public void AddAttachment(string content, string filename)
    {
        var attachment = new TcAttachment { Filename = filename, StringContent = content };
        if (attachments.ContainsKey(filename))
        {
            attachments[filename] = attachment;
        }
        else
        {
            attachments.Add(filename, attachment);
        }
    }

    public void AddAttachment(byte[] content, string filename)
    {
        var attachment = new TcAttachment { Filename = filename, ByteContents = content };
        if (attachments.ContainsKey(filename))
        {
            attachments[filename] = attachment;
        }
        else
        {
            attachments.Add(filename, attachment);
        }
    }

    internal IList<TcAttachment> GetAttachments()
        => attachments.Select(e => e.Value).ToList();

    public bool IsTestLastTestCase()
        => isLastTest;
}
