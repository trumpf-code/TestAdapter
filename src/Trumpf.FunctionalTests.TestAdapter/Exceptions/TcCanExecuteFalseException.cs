
using System;

namespace Trumpf.FunctionalTests.TestAdapter.Exceptions;
public class TcCanExecuteFalseException : Exception
{
    public TcCanExecuteFalseException()
    {
    }

    public TcCanExecuteFalseException(string message) : base(message)
    {
    }

    public TcCanExecuteFalseException(string message, Exception innerException) : base(message, innerException)
    {
    }
}