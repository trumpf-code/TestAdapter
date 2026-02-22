using Stashbox;
using Stashbox.Registration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using Trumpf.FunctionalTests.Interfaces;
using Trumpf.FunctionalTests.TestFramework.Interfaces;

namespace Trumpf.FunctionalTests.TestAdapter.Inject;

public class TestClassObject : ITestClassObject
{
    private IStashboxContainer Container { get; set; }

    public object Object { get; private set; }

    public ITestClassObject CreateFrom(Type typeToInstantiate)
    {
        Container = new StashboxContainer(config => config.WithDisposableTransientTracking());
        Container.Register(typeToInstantiate);
        Object = Container.Resolve(typeToInstantiate, nullResultAllowed: true);
        return this;
    }

    public void ResolveWith(TiTestContext testContext, ActionsToPerformAtTheInstantTestFails actionToPerform, ILogger logger)
    {
        var testWiring = new TcTestWirings(testContext, actionToPerform);

        logger.LogInfo($"{Environment.NewLine}The RegisterDependencies method of the class {testWiring.GetType().Name} (part of the TestAdapter) will be executed.");
        RegisterDependenciesFromTiWiringClass(testWiring, logger);

        if (Object is TiWiring wiring)
        {
            logger.LogInfo($"The RegisterDependencies method of the classes implementing TiWiring will be called.");
            RegisterDependenciesFromTiWiringClass(wiring, logger);
        }

        logger.LogInfo($"Try to resolve the stashbox container.");
        Object = Container.ResolveAll(Object.GetType()).FirstOrDefault();
        logger.LogInfo($"The stashbox container could be resolved.");
    }

    public void Dispose()
    {
        Container.Dispose();
    }

    private IStashboxContainer RegisterDependenciesFromTiWiringClass(TiWiring wiringClass, ILogger logger)
    {
        var oldRegisteredDependencies = Container.GetRegistrationMappings();

        try
        {
            wiringClass.RegisterDependencies(Container);
            logger.LogInfo("The following registrations have been done:");
            LogNewRegisteredDependencies(Container, oldRegisteredDependencies, logger);
        }
        catch (Exception registrationException)
        {
            logger.LogInfo($"The execution of the RegisterDependencies method in the class {wiringClass.GetType().Name} has failed.");
            logger.LogInfo("The following registrations have been done before the error was generated:");
            LogNewRegisteredDependencies(Container, oldRegisteredDependencies, logger);
            ExceptionDispatchInfo.Capture(registrationException).Throw();
        }

        return Container;
    }

    private static void LogNewRegisteredDependencies(IStashboxContainer container, IEnumerable<KeyValuePair<Type, IServiceRegistration>> oldRegistrations, ILogger logger)
    {
        var newRegisteredTypes = container.GetRegistrationMappings().Except(oldRegistrations);

        foreach (var registeredType in newRegisteredTypes)
        {
            if (registeredType.Value.ImplementationType.FullName == registeredType.Key.FullName)
            {
                logger.LogInfo($"- {registeredType.Key.FullName}");
                continue;
            }

            logger.LogInfo($"- '{registeredType.Key.FullName}' --> '{registeredType.Value.ImplementationType.FullName}'");
        }
    }
}