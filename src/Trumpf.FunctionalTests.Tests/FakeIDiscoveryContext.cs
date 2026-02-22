
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

namespace Adapter_Tests;
public class FakeIDiscoveryContext : IDiscoveryContext
{
    public IRunSettings RunSettings => null;
}
