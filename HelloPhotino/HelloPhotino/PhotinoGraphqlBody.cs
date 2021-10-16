using System.Collections.Generic;

namespace HelloPhotinoApp
{
    public class PhotinoGraphqlBody
    {
        public string Query { get; set; }
        public IReadOnlyDictionary<string, object?> Variables { get; set; } = new Dictionary<string, object?>();
    }
}
