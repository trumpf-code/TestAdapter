
using System;

namespace Trumpf.FunctionalTests.TestAdapter.Exceptions;
public class TcTestContextNullException : Exception
{
    public TcTestContextNullException()
    {
    }

    public TcTestContextNullException(string message) : base(message)
    {
    }

    public TcTestContextNullException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
