using System.Collections.Generic;

namespace Trumpf.FunctionalTests.TestAdapter;
public class NegativeIdsException : CheckException
{
    public NegativeIdsException(IEnumerable<int> ids) : base($"No negative Ids is allowed by the TcTestStep attributes. Please update the following Ids: {string.Join(",", ids)}")
    {
    }
}