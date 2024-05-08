using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace llamaindex.net.core.Schema;

[JsonConverter(typeof(BaseNodeConverter))]
public class TextNode(
    string id,
    string? text = null, 
    int? startCharIndex = null,
    int? endCharIdx = null,
    Dictionary<string, object>? metadata = null, 
    string? documentId = null, 
    string? docId = null, 
    string? refDocId = null) : BaseNode(id, metadata:metadata, documentId:documentId,docId: docId, refDocId: refDocId)
{
    public string? Text { get; } = text;
    public int? StartCharIndex { get; } = startCharIndex;
    public int? EndCharIdx { get; } = endCharIdx;
}