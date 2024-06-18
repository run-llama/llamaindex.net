using System;

namespace LlamaParse;

public class InMemoryFile(Memory<byte> stream, string fileName, string? mimeType = null)
{
    public Memory<byte> Stream { get; } = stream;
    public string FileName { get; } = fileName;
    public string MimeType { get; } = string.IsNullOrWhiteSpace(mimeType) ? FileTypes.GetMimeType(fileName) : mimeType!;
}