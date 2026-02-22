
using System;

namespace Trumpf.FunctionalTests.TestAdapter;
[Serializable]
public class TcTestStepsNegativeIdException : TcTestStepsException
{
    public TcTestStepsNegativeIdException()
    {
    }

    public TcTestStepsNegativeIdException(string message) : base(message)
    {
    }

    public TcTestStepsNegativeIdException(string message, Exception innerException) : base(message, innerException)
    {
    }
}