using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LlamaIndex.Core.Schema;

[JsonConverter(typeof(BaseNodeConverter))]
public class TextNode(
    string id,
    string? text = null,
    int? startCharIndex = null,
    int? endCharIdx = null,
    Dictionary<string, object>? metadata = null) :
    BaseNode(id, metadata: metadata)
{
    public string? Text { get; } = text;
    public int? StartCharIndex { get; } = startCharIndex;
    public int? EndCharIdx { get; } = endCharIdx;
}

[JsonConverter(typeof(BaseNodeConverter))]
public class ImageNode(
    string id,
    string? text = null,
    string? image = null,
    string? imagePath = null,
    string? imageUrl = null,
    string? imageMimetype = null,
    Dictionary<string, object>? metadata = null)
    : BaseNode(id, metadata)
{
    public string? Text { get; } = text;
    public string? Image { get; } = image;
    public string? ImagePath { get; } = imagePath;
    public string? ImageUrl { get; } = imageUrl;
    public string? ImageMimetype { get; } = imageMimetype;
}