using System.Collections.Generic;

namespace LlamaIndex.Core.Schema;

public class Document(
    string id,
    string? text = null,
    string? mimeType = null,
    Dictionary<string, object>? metadata = null) : TextNode(id, text: text, mimeType:mimeType, metadata: metadata)
{

}