
using System;

namespace Trumpf.FunctionalTests.TestAdapter.Exceptions;
public class TcInvalidTestBaseClassException : Exception
{
    public TcInvalidTestBaseClassException(Type sourceTestClassType, Type frameworkClassType)
        : base($"The test type {sourceTestClassType.Name} must not inherit from {frameworkClassType.Name}")
    {
    }

    public TcInvalidTestBaseClassException()
    {
    }

    public TcInvalidTestBaseClassException(string message) : base(message)
    {
    }

    public TcInvalidTestBaseClassException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
