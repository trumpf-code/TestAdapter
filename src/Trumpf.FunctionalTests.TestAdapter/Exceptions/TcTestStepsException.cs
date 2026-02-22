
using System;

namespace Trumpf.FunctionalTests.TestAdapter;
[Serializable]
public class TcTestStepsException : Exception
{
    public TcTestStepsException()
    {
    }

    public TcTestStepsException(string message) : base(message)
    {
    }

    public TcTestStepsException(string message, Exception innerException) : base(message, innerException)
    {
    }
}