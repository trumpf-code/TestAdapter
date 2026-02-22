
using System.Collections.Generic;

namespace Trumpf.FunctionalTests.Interfaces;
public interface TiTestrunEnvironment
{
    List<TiOneTimeInitialization> GetOperationsToBeTriggeredOnceInAssembly();
}