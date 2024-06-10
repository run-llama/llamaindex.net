using System.Collections.Generic;
using LlamaIndex.CoreSchema;

namespace LlamaIndex.Core.Schema;

public class Document(
    string id, 
    string? text = null, 
    Dictionary<string, object>? metadata = null) : TextNode(id, text:text, metadata: metadata)
{

}