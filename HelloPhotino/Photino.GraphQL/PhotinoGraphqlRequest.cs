using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photino.Access
{
    public class PhotinoGraphqlBody
    {
        public string Query { get; set; }
        public IReadOnlyDictionary<string, object?> Variables { get; set; } = new Dictionary<string,object?>();
    }

    public class PhotinoGraphqlRequest : PhotinoRequest<PhotinoGraphqlBody>
    {
    }
}
