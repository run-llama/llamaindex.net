using System.Collections.Generic;

namespace LlamaIndex.Core.Schema;

public class ImageDocument(
    string id,
    string? text = null,
    string? image = null,
    string? imagePath = null,
    string? imageUrl = null,
    string? imageMimetype = null,
    Dictionary<string, object>? metadata = null) : Document(id, text: text, metadata: metadata)
{
    public string? Image { get; } = image;
    public string? ImagePath { get; } = imagePath;
    public string? ImageUrl { get; } = imageUrl;
    public string? ImageMimetype { get; } = imageMimetype;
}