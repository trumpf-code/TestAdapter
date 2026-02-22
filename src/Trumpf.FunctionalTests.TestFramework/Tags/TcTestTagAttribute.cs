
using System;
using System.Collections.Generic;
using Trumpf.FunctionalTests.Interfaces;

namespace Trumpf.FunctionalTests.Tags;
public abstract class TcTestTagAttribute : Attribute, TiHasTags
{
    public virtual IEnumerable<TiTag> Tags
        => new[] { new TcTag { Name = GetType().Name.Replace("Attribute", string.Empty) } }; // todo: replace only suffix "Attribute"
}