namespace Trumpf.FunctionalTests.TestAdapter;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

public class StackTraceFormatter
{
    private string[] typesToIgnore;

    public StackTraceFormatter(string[] typesToIgnore)
        => this.typesToIgnore = typesToIgnore;

    public string GetStackTracesContent(IEnumerable<Exception> listExecutionCaught)
        => string.Join(Environment.NewLine, listExecutionCaught
            .Select(e => ReducedStackTrace(e))
            .SelectMany(e => e));

    public string GetMessagesContent(IEnumerable<Exception> listExecutionCaught)
    {
        var MessagesInfo = new StringBuilder();

        foreach (Exception singleException in listExecutionCaught)
        {
            MessagesInfo.Append(AddExceptionMessage(singleException, MessagesInfo));

            var innerException = singleException.InnerException;

            while (innerException != null)
            {
                MessagesInfo.Append("--> InnerException ");
                MessagesInfo.Append($"{innerException.GetType().FullName}: {innerException.Message}");
                MessagesInfo.Append(Environment.NewLine);
                innerException = innerException.InnerException;
            }

            MessagesInfo.Append(Environment.NewLine);
        }

        return MessagesInfo.ToString();
    }

    internal IEnumerable<string> ReducedStackTrace(Exception singleException)
    {
        var stackFrames = SplitStackTrace(singleException);
        var selectedStackFrames = stackFrames.Except(stackFrames.Where(HasIgnoreType));
        var reversed = selectedStackFrames.AsEnumerable().Reverse().ToArray();
        var secondLastLine = reversed.Skip(1).FirstOrDefault()?.Contains("--- End of stack trace from previous location where exception was thrown ---");
        var lastLine = reversed.FirstOrDefault()?.Contains(typeof(System.Runtime.ExceptionServices.ExceptionDispatchInfo).FullName);
        var skip = secondLastLine == true && lastLine == true ? 2 : 0;
        var result = selectedStackFrames.AsEnumerable().Reverse().Skip(skip).Reverse();

        return result.Any() ? result : stackFrames;
    }

    private static string[] SplitStackTrace(Exception singleException)
        => singleException.StackTrace.Split(new string[] { Environment.NewLine }, StringSplitOptions.None)
            .Where(stackFrame => !string.IsNullOrEmpty(stackFrame))
            .ToArray();

    private string AddExceptionMessage(Exception e, StringBuilder messagesInfo)
    {
        StackFrame methodName = new StackTrace(e)
            .GetFrames()
            .Where(f => HasIgnoreType(f.GetMethod().Module.Name) == false)
            .Where(s => string.IsNullOrEmpty(s.ToString()) == false)
            .LastOrDefault();

        StringBuilder messageInfo = new();
        messagesInfo.Append($"Test method threw an exception:");
        messagesInfo.Append(Environment.NewLine);

        messagesInfo.Append($"{e.GetType().FullName}: {e.Message}");
        return messageInfo.ToString();
    }

    private bool HasIgnoreType(string stackFrame)
        => typesToIgnore.Any(type => stackFrame.IndexOf(type, StringComparison.Ordinal) > -1);
}