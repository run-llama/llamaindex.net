using System.Collections.Generic;

namespace LlamaIndex.Core.Schema;

/// <summary>
/// Represents a related node.
/// </summary>
/// <param name="nodeId">The node ID.</param>
/// <param name="nodeType">The node type. <see cref="NodeType"/></param>
/// <param name="metadata">Additional node metadata.</param>
public class RelatedNodeInfo(string nodeId, NodeType nodeType, Dictionary<string, object>? metadata = null)
{
    public string NodeId { get; } = nodeId;
    public NodeType NodeType { get; } = nodeType;
    public Dictionary<string, object>? Metadata { get; } = metadata;
}
