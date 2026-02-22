using System.Collections.Generic;
using Trumpf.FunctionalTests.Interfaces;

namespace Trumpf.FunctionalTests.TestAdapter;

public interface IBinder
{
    void Bind(ILogger logger, IEnumerable<string> testSources);
}
