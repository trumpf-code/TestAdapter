# Trumpf Functional Tests - TestAdapter

A custom **VSTest adapter** for discovering and executing step-based functional tests within the Visual Studio Test Platform ecosystem. This adapter replaces the traditional `[TestMethod]` paradigm with a structured, **one-class-per-test-case** approach where test logic is defined as ordered steps using `[TcTestStep]` attributes.

---

## Table of Contents

- [Overview](#overview)
- [Solution Structure](#solution-structure)
- [Architecture](#architecture)
  - [Test Discovery](#test-discovery)
  - [Test Execution Lifecycle](#test-execution-lifecycle)
  - [Dependency Injection](#dependency-injection)
  - [Tag & Categorization System](#tag--categorization-system)
  - [One-Time Operations](#one-time-operations)
  - [Event Hooks](#event-hooks)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Build](#build)
  - [Run Tests](#run-tests)
- [Writing Tests](#writing-tests)
  - [Minimal Test Case](#minimal-test-case)
  - [Full-Featured Test Case](#full-featured-test-case)
  - [Custom Tags & Categories](#custom-tags--categories)
  - [Requirement Traceability](#requirement-traceability)
- [Configuration & Build](#configuration--build)
  - [Versioning](#versioning)
  - [NuGet Packaging](#nuget-packaging)
  - [Central Package Management](#central-package-management)
- [Key Components Reference](#key-components-reference)
- [Contributing](#contributing)
- [License](#license)

---

## Overview

The **TestAdapter** integrates into the standard VSTest pipeline (`dotnet test`, Visual Studio Test Explorer, Azure DevOps) and provides:

- **Step-based test execution** - each test class contains ordered methods annotated with `[TcTestStep(id)]`, executed sequentially by ascending ID.
- **Lifecycle hooks** - optional `Prepare`, `Cleanup`, `CanExecute` phases via framework interfaces.
- **Built-in dependency injection** - powered by [Stashbox](https://github.com/z4kn4fein/stashbox) IoC container, supporting both adapter-level and test-level service registrations.
- **Tag and category system** - extensible attribute-based tagging for filtering, categorization, and requirement traceability.
- **One-time initialization** - assembly-scoped setup/teardown operations that run once across the entire test run.
- **Section event hooks** - fine-grained lifecycle events for cross-cutting concerns such as logging, diagnostics, or reporting.

---

## Solution Structure

```
TestAdapter.sln
│
├── Build/
│   ├── Common.targets            # Shared MSBuild properties (TFM, packaging, symbols)
│   ├── Version.targets           # Git-based version derivation via GitInfo
│   └── clean_bin_obj.bat         # Utility to clean bin/obj directories
│
├── src/
│   ├── Trumpf.FunctionalTests.TestAdapter/          # The VSTest adapter (packable)
│   │   ├── Adapter/              # Core: discovery, execution, test case model
│   │   ├── Exceptions/           # Domain-specific exception types
│   │   ├── Execution/            # Test sequence orchestration
│   │   ├── Extensions/           # LINQ and TestContext extension methods
│   │   ├── Inject/               # Stashbox DI container setup and wirings
│   │   ├── MsAdapters/           # VSTest ITestExecutor implementation
│   │   ├── Reflection/           # Reflection utilities for indirect method resolution
│   │   ├── Shared/               # Logging infrastructure
│   │   ├── Tags/                 # Tag retrieval logic
│   │   └── TraceListenerHelpers/ # Trace output redirection to file
│   │
│   ├── Trumpf.FunctionalTests.TestFramework/        # Framework contracts (non-packable)
│   │   ├── Interfaces/           # Core interfaces (TiPrepare, TiCleanup, TiWiring, etc.)
│   │   ├── Tags/                 # Tag model, attributes (TcTestStep, TcTestCategory, etc.)
│   │   └── TestContext/          # TiTestContext interface
│   │
│   ├── Trumpf.FunctionalTests.Tests/                # Integration / functional test suite
│   │   ├── Attributes/           # Custom tag attributes (e.g., XRayId)
│   │   ├── DependenciesInjection/# DI registration examples
│   │   ├── MetaData/             # Tag metadata tests
│   │   ├── Tags/                 # Tag-related test scenarios
│   │   ├── TestSequence/         # Lifecycle and sequence test scenarios
│   │   └── TestResults/          # Result verification tests
│   │
│   └── Trumpf.FunctionalTests.TestAdapter.UnitTests/ # Unit tests for adapter internals
│       ├── Helpers/
│       ├── Mocks/
│       ├── StackTraceFormatterTests/
│       ├── TestDiscovererServiceTests/
│       ├── TestExecuterServiceTests/
│       └── TraceListenerTests/
│
└── Directory.Packages.props      # Central NuGet package version management
```

---

## Architecture

### Test Discovery

The adapter registers itself with the VSTest platform under the executor URI `executor://testexecutoradapter/v1`.

Discovery scans all provided `.dll` sources and identifies test classes by checking for methods annotated with `[TcTestStep]`. For each qualifying class:

1. A `TestCase` is created with `FullyQualifiedName` set to the class's full namespace-qualified name.
2. Tags from `TiHasTags` attributes are collected and mapped to `TestCategory` traits.
3. Requirement identifiers from `TiRequirement` attributes are stored as a `RequirementName` trait.
4. Source file and line number are resolved via `DiaSession` for Test Explorer navigation.

Test case filtering supports `TestCategory`, `FullyQualifiedName`, and `Name` properties, compatible with `dotnet test --filter`.

### Test Execution Lifecycle

Each test class follows this execution sequence:

```
BeforeTestResolved()        ← TiPreEvents (before DI resolution)
        │
   DI Resolution            ← Stashbox container setup
        │
AfterTestResolved()         ← TiEvents
        │
Init()                      ← TiOneTimeInitialization (only unprocessed operations)
        │
CanExecute()                ← TiCanExecute (skip if returns false)
        │
Prepare()                   ← TiPrepare
        │
TcTestStep(1).Invoke()      ← Ordered by ascending Id
TcTestStep(2).Invoke()
TcTestStep(N).Invoke()
        │
Clean(exception)            ← TiCleanup (always runs; receives any exception from execute)
        │
Dispose()                   ← Stashbox container disposal
        │
Deinit()                    ← TiOneTimeInitialization (only if last test in run)
```

Pre-execution validation ensures all `[TcTestStep]` IDs are unique and non-negative.

### Dependency Injection

The adapter uses [Stashbox](https://github.com/z4kn4fein/stashbox) as its IoC container with disposable transient tracking enabled.

**Adapter-level wirings** (registered automatically):
- `TiTestContext` - test context for attachments and metadata
- `TiOneTimeOperations` - one-time init/deinit tracking
- `ActionsToPerformAtTheInstantTestFails` - failure hook collection

**Test-level wirings** are registered by implementing `TiWiring.RegisterDependencies()` on the test class or its base class. Dependencies are injected via the `[Dependency]` attribute on properties.

When multiple classes in the inheritance chain implement `TiWiring`, registrations are applied in order - derived class registrations can override base class registrations.

### Tag & Categorization System

Tags are attribute-based and extend `TcTestTagAttribute` (which implements `TiHasTags`):

| Attribute | Purpose |
|---|---|
| `[TcTestStep(id)]` | Marks a method as a test step; `id` controls execution order |
| `[TcTestCategory("name")]` | Categorizes a test (analogous to MSTest's `[TestCategory]`) |
| `[TcCustomTag("name")]` | Arbitrary free-text tagging |
| `[TcIgnoreTest("reason")]` | Skips test execution with an optional reason |

Custom tag attributes can be created by extending `TcTestTagAttribute`. The tag name defaults to the class name minus the `Attribute` suffix.

### One-Time Operations

Tests implementing `TiTestrunEnvironment` declare `TiOneTimeInitialization` objects whose `Init()` runs once per unique type across the entire test run, and whose `Deinit()` runs when the last test completes.

### Event Hooks

Two event interfaces enable lifecycle instrumentation without modifying test logic:

- **`TiSectionEvents`** - hooks for every phase: `OnPrepareStarting/Finishing`, `OnTestStepStarting/Finishing`, `OnCleanStarting/Finishing`, `OnCanExecuteStarting/Finishing`, `OnOneTimeInitStarting/Finishing`, `OnOneTimeDeinitStarting/Finishing`, `AfterTestResolved`, `OnTestFinishing`
- **`TiPreSectionEvents`** - `BeforeTestResolved(TypeInfo)`, fired before DI resolution

---

## Getting Started

### Prerequisites

| Requirement | Version |
|---|---|
| .NET SDK | 8.0 or later |
| Visual Studio | 2022 (17.5+) recommended |
| C# Language | 12.0 |

### Build

```bash
dotnet build TestAdapter.sln
```

To clean all build artifacts:

```bash
Build\clean_bin_obj.bat
```

### Run Tests

```bash
# Run all tests
dotnet test TestAdapter.sln

# Run with category filter
dotnet test --filter "TestCategory=ComponentTest"

# Run a specific test by name
dotnet test --filter "FullyQualifiedName~MyTestClassName"
```

---

## Writing Tests

### Minimal Test Case

```csharp
using Trumpf.FunctionalTests.Tags;

public class MyFirstTest
{
    [TcTestStep(1)]
    public void Step_OpenApplication()
    {
        // First step
    }

    [TcTestStep(2)]
    public void Step_PerformAction()
    {
        // Second step
    }

    [TcTestStep(3)]
    public void Step_VerifyResult()
    {
        // Third step
    }
}
```

### Full-Featured Test Case

```csharp
using Stashbox.Attributes;
using Trumpf.FunctionalTests.Interfaces;
using Trumpf.FunctionalTests.Tags;
using Trumpf.FunctionalTests.TestFramework.Interfaces;

[TcTestCategory("Smoke")]
public class FullFeaturedTest : TiPrepare, TiCleanup, TiCanExecute, TiWiring
{
    [Dependency]
    public TiTestContext TestContext { get; set; }

    public IDependencyRegistrator RegisterDependencies(IDependencyRegistrator registrator)
    {
        return registrator.Register<IMyService, MyServiceImpl>();
    }

    public bool CanExecute(out string cause)
    {
        cause = string.Empty;
        return true; // Return false to skip
    }

    public void Prepare()
    {
        // Runs before test steps
    }

    [TcTestStep(1)]
    public void Step_Action()
    {
        // Test logic
    }

    [TcTestStep(2)]
    public void Step_Verify()
    {
        TestContext.AddAttachment("screenshot.png", "Evidence");
    }

    public void Clean(Exception exception)
    {
        // Runs after test steps; exception is null on success
    }
}
```

### Custom Tags & Categories

Create project-specific tags by extending `TcTestTagAttribute`:

```csharp
public class FunctionalTestAttribute : TcTestTagAttribute { }

// Usage
[FunctionalTest]
public class MyTest
{
    [TcTestStep(1)]
    public void Step() { }
}
```

The tag name is automatically derived from the class name (`FunctionalTest`).

### Requirement Traceability

Implement `TiRequirement` to link tests to external tracking systems:

```csharp
public class XRayIdAttribute : Attribute, TiRequirement
{
    public string Name => $"[{Project}-{Id}]({JiraUrl}/browse/{Project}-{Id})";
    // ...
}

[XRayId(12345)]
public class TrackedTest
{
    [TcTestStep(1)]
    public void Step() { }
}
```

Requirement names are stored as `RequirementName` traits on the test case.

---

## Configuration & Build

### Versioning

Versioning is derived from **Git tags** using [GitInfo](https://github.com/devlooped/GitInfo):

- `PackageVersion` = the latest Git tag
- `AssemblyVersion` = `Major.Minor.Patch` extracted from the Git tag

Defined in [Build/Version.targets](Build/Version.targets).

### NuGet Packaging

The `Trumpf.FunctionalTests.TestAdapter` project is the only packable project. It produces a self-contained NuGet package that bundles:

- `Trumpf.FunctionalTests.TestAdapter.dll`
- `Trumpf.FunctionalTests.TestFramework.dll` (embedded via `PrivateAssets="All"`)
- `Stashbox.dll` (explicitly included in the package)

Consumers only need to reference the single adapter package.

### Central Package Management

All NuGet package versions are managed centrally in [Directory.Packages.props](Directory.Packages.props):

| Package | Version | Purpose |
|---|---|---|
| Microsoft.TestPlatform.ObjectModel | 17.5.0 | VSTest platform integration |
| MSTest.TestAdapter | 4.0.2 | MSTest adapter (unit tests only) |
| MSTest.TestFramework | 4.0.2 | MSTest framework (unit tests only) |
| Microsoft.NET.Test.Sdk | 17.11.1 | Test SDK (unit tests only) |
| Stashbox | 2.8.9 | IoC container |
| AwesomeAssertions | 9.3.0 | Assertion library (tests only) |
| GitInfo | 3.6.0 | Git-based versioning |

---

## Key Components Reference

### Framework Interfaces

| Interface | Method(s) | Purpose |
|---|---|---|
| `TiPrepare` | `Prepare()` | Pre-step setup |
| `TiCleanup` | `Clean(Exception)` | Post-step teardown (always runs) |
| `TiCanExecute` | `CanExecute(out string)` | Runtime skip condition |
| `TiWiring` | `RegisterDependencies(IDependencyRegistrator)` | DI service registration |
| `TiTestrunEnvironment` | `GetOperationsToBeTriggeredOnceInAssembly()` | Assembly-scoped init/deinit |
| `TiOneTimeInitialization` | `Init()`, `Deinit()` | One-time setup/teardown |
| `TiEvents` | `SectionEvents` | Lifecycle event hooks |
| `TiPreEvents` | `PreSectionEvents` | Pre-resolution event hook |
| `TiHasTags` | `Tags` | Tag provider |
| `TiRequirement` | `Name` | Requirement traceability |
| `TiTestContext` | `StartTime`, `AddAttachment(...)` | Test context and attachments |
| `ILogger` | `LogError`, `LogInfo`, `LogWarning` | Framework-agnostic logging |

### Custom Exceptions

| Exception | Thrown When |
|---|---|
| `DupplicatedIdsException` | Two `[TcTestStep]` attributes share the same ID |
| `NegativeIdsException` | A `[TcTestStep]` has a negative ID |
| `TcIgnoreTestException` | Test is marked with `[TcIgnoreTest]` |
| `TcCanExecuteFalseException` | `CanExecute()` returns `false` |
| `TcInvalidTestBaseClassException` | Test inherits from a disallowed base type |
| `TcNoTestStepDefinedException` | No `[TcTestStep]` methods found on the class |
| `TcTestContextNullException` | `TiTestContext` is null when expected |
| `TcTestStepsNegativeIdException` | Negative step ID detected during execution |

---

## Contributing

1. Create a feature branch from `main`.
2. Implement your changes and ensure all existing tests pass.
3. Add or update unit tests for any new or modified behavior.
4. Submit a pull request for review.

Please follow the existing code conventions and naming patterns (`Tc` prefix for concrete types, `Ti` prefix for interfaces).

---

## License

Proprietary - All rights reserved.
This software is the intellectual property of TRUMPF and is intended for internal use only. Unauthorized distribution, modification, or use outside of TRUMPF is strictly prohibited.
