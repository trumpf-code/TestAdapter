
using System.Collections.Generic;
using Trumpf.FunctionalTests.Interfaces;

namespace Trumpf.FunctionalTests.TestAdapter;
public interface TiOneTimeOperations
{
    IEnumerable<TiOneTimeInitialization> GetTiOneTimeInitializationTypes();
    IEnumerable<TiOneTimeInitialization> GetRemainingInitOperationsToProcess();
    void UpdateTiOneTimeInitializationCollectionWith(IEnumerable<TiOneTimeInitialization> TiOneTimeInitializationTypes);
    void UpdateTypesAlreadyProcessedWith(TiOneTimeInitialization TiOneTimeInitializationType);
}
