using System;
using System.Collections.Generic;
using Trumpf.FunctionalTests.Interfaces;

namespace Trumpf.FunctionalTests.TestAdapter.UnitTests.Mocks;

public class OneTimeOperationsMock : TiOneTimeOperations
{
    public IEnumerable<TiOneTimeInitialization> GetRemainingInitOperationsToProcess()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<TiOneTimeInitialization> GetTiOneTimeInitializationTypes()
    {
        throw new NotImplementedException();
    }

    public void UpdateTiOneTimeInitializationCollectionWith(IEnumerable<TiOneTimeInitialization> tiOneTimeInitializationTypes)
    {
        throw new NotImplementedException();
    }

    public void UpdateTypesAlreadyProcessedWith(TiOneTimeInitialization tiOneTimeInitializationType)
    {
        throw new NotImplementedException();
    }
}
