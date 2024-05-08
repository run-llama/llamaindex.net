using System.Collections.Generic;

namespace llamaindex.net.core.Schema;

public abstract class BaseNode(string id, Dictionary<string, object> metadata)
{
    public string Id { get; } = id;
    public Dictionary<string, object> Metadata { get;} = metadata;
}