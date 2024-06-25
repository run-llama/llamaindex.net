using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LlamaIndex.Core.Schema;

/// <summary>
/// The BaseNode class is an abstract base class that represents a node in a graph.
/// </summary>
[JsonConverter(typeof(BaseNodeConverter))]
[JsonDerivedType(typeof(TextNode))]
[JsonDerivedType(typeof(ImageNode))]
[JsonDerivedType(typeof(Document))]
[JsonDerivedType(typeof(ImageDocument))]
public abstract class BaseNode(string id, Dictionary<string, object>? metadata = null)
{
    /// <summary>
    /// Gets the unique identifier of the node.
    /// </summary>
    public string Id { get; } = id;

    /// <summary>
    /// Gets the metadata associated with the node.
    /// </summary>
    public Dictionary<string, object> Metadata { get; } = metadata ?? new Dictionary<string, object>();

    /// <summary>
    /// Gets or sets the source node from which this node originated.
    /// </summary>
    public RelatedNodeInfo? SourceNode { get; set; }

    /// <summary>
    /// Gets or sets the previous node in the sequence.
    /// </summary>
    public RelatedNodeInfo? PreviousNode { get; set; }

    /// <summary>
    /// Gets or sets the next node in the sequence.
    /// </summary>
    public RelatedNodeInfo? NextNode { get; set; }

    /// <summary>
    /// Gets or sets the parent node of this node.
    /// </summary>
    public RelatedNodeInfo? ParentNode { get; set; }

    /// <summary>
    /// Gets or sets the child nodes of this node.
    /// </summary>
    public RelatedNodeInfo[]? ChildNodes { get; set; }
}
