
using System;

namespace Trumpf.FunctionalTests.TestAdapter.Exceptions;
public class TcNoTestStepDefinedException : Exception
{
    public TcNoTestStepDefinedException()
    {
    }

    public TcNoTestStepDefinedException(string message) : base(message)
    {
    }

    public TcNoTestStepDefinedException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
