namespace Adapter_Tests.IDisposableChecks;

public class SecondClassIDisposable : TiSecondInterfaceIDisposable
{
    public TiFirstInterfaceIDisposable Test1 { get; private set; }

    public SecondClassIDisposable(TiFirstInterfaceIDisposable test1)
    {
        Test1 = test1;
    }
}