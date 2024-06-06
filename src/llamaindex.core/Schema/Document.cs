using System.Collections.Generic;

namespace LlamaIndex.CoreSchema;

public class Document(string id, string? text = null, Dictionary<string, object>? metadata = null) : TextNode(id, text:text, metadata: metadata)
{

}