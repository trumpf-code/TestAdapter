
using AwesomeAssertions;
using Stashbox;
using Stashbox.Attributes;
using Trumpf.FunctionalTests.Attributes;
using Trumpf.FunctionalTests.Interfaces;
using Trumpf.FunctionalTests.Tags;

namespace Adapter_Tests.TestSequence;
[ComponentTest]
public class SeveralClassesCanImplementTiWiring : TiWiring
{
    [Dependency]
    public I2 C2inst { get; set; }

    [TcTestStep(1)]
    public void Execute()
    {
        C2inst.Should().NotBeNull();
        C2inst.Should().BeOfType(typeof(C2_bis));
    }

    public IDependencyRegistrator RegisterDependencies(IDependencyRegistrator dependencyRegistrator)
    {
        dependencyRegistrator.Register<I2, C2_bis>();
        return dependencyRegistrator;
    }
}

public interface I2
{
    void F2();
}

public class C2 : I2
{
    public void F2() { }
}

public class C2_bis : I2
{
    public void F2() { }
}
