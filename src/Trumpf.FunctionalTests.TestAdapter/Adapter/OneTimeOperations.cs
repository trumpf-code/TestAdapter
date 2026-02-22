
using System.Collections.Generic;
using System.Linq;
using Trumpf.FunctionalTests.Interfaces;
using Trumpf.FunctionalTests.TestAdapter.Extensions;

namespace Trumpf.FunctionalTests.TestAdapter;
public class OneTimeOperations : TiOneTimeOperations
{
    public OneTimeOperations()
    {
        TiOneTimeInitializationTypesCollection = Enumerable.Empty<TiOneTimeInitialization>();
        TiTypesAlreadyProcessed = Enumerable.Empty<TiOneTimeInitialization>();
    }

    private IEnumerable<TiOneTimeInitialization> TiOneTimeInitializationTypesCollection { get; set; }

    private IEnumerable<TiOneTimeInitialization> TiTypesAlreadyProcessed { get; set; }

    public IEnumerable<TiOneTimeInitialization> GetTiOneTimeInitializationTypes()
         => TiOneTimeInitializationTypesCollection;

    public IEnumerable<TiOneTimeInitialization> GetRemainingInitOperationsToProcess()
        => !TiTypesAlreadyProcessed.Any()
         ? TiOneTimeInitializationTypesCollection
        : TiOneTimeInitializationTypesCollection.Where(t => !TiTypesAlreadyProcessed.Any(processed => t.GetType().FullName == processed.GetType().FullName));

    public void UpdateTiOneTimeInitializationCollectionWith(IEnumerable<TiOneTimeInitialization> TiOneTimeInitializationTypesToAdd)
    {
        foreach (var TiOneTimeInitializationType in TiOneTimeInitializationTypesToAdd)
        {
            var TypeAlreadyProcessed = TiOneTimeInitializationTypesCollection
                .Where(t => t.GetType().FullName == TiOneTimeInitializationType.GetType().FullName)
                .FirstOrDefault();

            if (TypeAlreadyProcessed != default)
            {
                var indexToReplace = TiOneTimeInitializationTypesCollection.ToList().IndexOf(TypeAlreadyProcessed);
                TiOneTimeInitializationTypesCollection = TiOneTimeInitializationTypesCollection.Replace(indexToReplace, TiOneTimeInitializationType);
                continue;
            }

            TiOneTimeInitializationTypesCollection = TiOneTimeInitializationTypesCollection.Append(TiOneTimeInitializationType);
        }
    }

    public void UpdateTypesAlreadyProcessedWith(TiOneTimeInitialization TiOneTimeInitializationType)
    {
        TiTypesAlreadyProcessed = TiTypesAlreadyProcessed
            .Where(t => t.GetType().FullName != TiOneTimeInitializationType.GetType().FullName)
            .Append(TiOneTimeInitializationType);
    }
}
