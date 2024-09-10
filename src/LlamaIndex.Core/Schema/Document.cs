using System.Collections.Generic;

namespace LlamaIndex.Core.Schema;

/// <summary>
/// Represents a document node.
/// </summary>
/// <param name="id">The node ID</param>
/// <param name="text">The text contents of a node</param>
/// <param name="mimeType">The data type represented in the node</param>
/// <param name="metadata">Additional metadata for the node</param>
public class Document(
string id,
string? text = null,
string? mimeType = null,
Dictionary<string, object>? metadata = null)
: TextNode(id, text: text, mimeType: mimeType, metadata: metadata);
