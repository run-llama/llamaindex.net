using System.Collections.Generic;

namespace LlamaIndex.Core.Schema;

/// <summary>
/// Represents an image node.
/// </summary>
/// <param name="id">The node ID</param>
/// <param name="text">A text description of the image. For example, alt-text.</param>
/// <param name="image">A string representation of the image.</param>
/// <param name="imagePath">A file path where the image is located.</param>
/// <param name="imageUrl">A URL where the image is located.</param>
/// <param name="imageMimetype">The mime type for the image.</param>
/// <param name="mimeType">The mime type for the node.</param>
/// <param name="metadata">Additional node metadata.</param>
public class ImageDocument(
    string id,
    string? text = null,
    string? image = null,
    string? imagePath = null,
    string? imageUrl = null,
    string? imageMimetype = null,
    string? mimeType = null,
    Dictionary<string, object>? metadata = null) : Document(id, text: text, mimeType: mimeType, metadata: metadata)
{
    public string? Image { get; } = image;
    public string? ImagePath { get; } = imagePath;
    public string? ImageUrl { get; } = imageUrl;
    public string? ImageMimetype { get; } = imageMimetype;
}
