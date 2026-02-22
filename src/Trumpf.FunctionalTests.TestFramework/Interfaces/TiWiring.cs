
using Stashbox;

namespace Trumpf.FunctionalTests.Interfaces;
/// <summary>
/// Contains functions for binding interfaces to their implementations
/// </summary>
public interface TiWiring
{
    /// <summary>
    /// Binds interfaces to their implementations
    /// </summary>
    /// <param name="dependencyRegistrator"> used to register dependencies</param>
    IDependencyRegistrator RegisterDependencies(IDependencyRegistrator dependencyRegistrator);
}