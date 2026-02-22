
using System;

namespace Trumpf.FunctionalTests.TestAdapter.Exceptions;
public class TcIgnoreTestException : Exception
{
    public TcIgnoreTestException()
    {
    }

    public TcIgnoreTestException(string message) : base(message)
    {
    }

    public TcIgnoreTestException(string message, Exception innerException) : base(message, innerException)
    {
    }
}