
using System.Collections.Generic;
using Trumpf.FunctionalTests.Tags;

namespace Trumpf.FunctionalTests.Interfaces;
public interface TiHasTags
{
    IEnumerable<TiTag> Tags { get; }
}