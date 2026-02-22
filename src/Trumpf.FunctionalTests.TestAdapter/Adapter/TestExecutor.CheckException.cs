
using System;

namespace Trumpf.FunctionalTests.TestAdapter;
public class CheckException : Exception
{
    public CheckException()
    {
    }

    public CheckException(string message) : base(message)
    {
    }
}