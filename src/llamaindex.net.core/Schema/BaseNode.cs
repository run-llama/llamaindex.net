using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace llamaindex.core.Schema;

[JsonConverter(typeof(BaseNodeConverter))]
[JsonDerivedType(typeof(TextNode))]
public abstract class BaseNode(string id, Dictionary<string, object>? metadata = null)
{
    public string Id { get; } = id;
    public Dictionary<string, object> Metadata { get;} = metadata ?? new Dictionary<string, object>();

    public RelatedNodeInfo SourceNode { get; set; }
    public RelatedNodeInfo PreviousNode { get; set; }
    public RelatedNodeInfo NextNode { get; set; }
    public RelatedNodeInfo ParentNode { get; set; }

    public RelatedNodeInfo[] ChildNodes { get; set; }
}