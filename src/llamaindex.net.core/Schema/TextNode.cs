using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace llamaindex.core.Schema;

[JsonConverter(typeof(BaseNodeConverter))]
public class TextNode(
    string id,
    string? text = null, 
    int? startCharIndex = null,
    int? endCharIdx = null,
    Dictionary<string, object>? metadata = null) : 
    BaseNode(id, metadata:metadata)
{
    public string? Text { get; } = text;
    public int? StartCharIndex { get; } = startCharIndex;
    public int? EndCharIdx { get; } = endCharIdx;
}