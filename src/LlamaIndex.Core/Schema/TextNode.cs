using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LlamaIndex.Core.Schema;

/// <summary>
/// Represents a text node.
/// </summary>
/// <param name="id">The node ID.</param>
/// <param name="text">The text contents of the node.</param>
/// <param name="startCharIndex">The index where the node starts.</param>
/// <param name="endCharIdx">The index where the node ends.</param>
/// <param name="mimeType">The node mime type.</param>
/// <param name="metadata">Additional node metadata</param>
[JsonConverter(typeof(BaseNodeConverter))]
public class TextNode(
    string id,
    string? text = null,
    int? startCharIndex = null,
    int? endCharIdx = null,
    string? mimeType = null,
    Dictionary<string, object>? metadata = null) :
    BaseNode(id, metadata: metadata)
{
    public string? Text { get; } = text;
    public int? StartCharIndex { get; } = startCharIndex;
    public int? EndCharIdx { get; } = endCharIdx;
    public string? MimeType { get; } = mimeType;
}

/// <summary>
/// Represents an image node.
/// </summary>
/// <param name="id">The node ID</param>
/// <param name="text">The text description of the image.</param>
/// <param name="image">The representation of the image.</param>
/// <param name="imagePath">The file path image location.</param>
/// <param name="imageUrl">The URL image location.</param>
/// <param name="imageMimetype">The mime type of the image.</param>
/// <param name="mimeType">The mime type of the node.</param>
/// <param name="metadata">Additional node metadata.</param>
[JsonConverter(typeof(BaseNodeConverter))]
public class ImageNode(
    string id,
    string? text = null,
    string? image = null,
    string? imagePath = null,
    string? imageUrl = null,
    string? imageMimetype = null,
    string? mimeType = null,
    Dictionary<string, object>? metadata = null)
    : BaseNode(id, metadata)
{
    public string? Text { get; } = text;
    public string? Image { get; } = image;
    public string? ImagePath { get; } = imagePath;
    public string? ImageUrl { get; } = imageUrl;
    public string? ImageMimetype { get; } = imageMimetype;
    public string? MimeType { get; } = mimeType;
}
