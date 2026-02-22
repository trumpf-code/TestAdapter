using System.Collections.Generic;
using Trumpf.FunctionalTests.Tags;

namespace Trumpf.FunctionalTests.TestAdapter;
public class DupplicatedIdsException : CheckException
{
    public DupplicatedIdsException(IEnumerable<int> ids) : base($"Each Id by the {nameof(TcTestStepAttribute)} attributes shall be used only once. The following Ids are used several times: {string.Join(",", ids)}")
    {
    }
}