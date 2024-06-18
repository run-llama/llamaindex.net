using System;

namespace LlamaParse;

public class InMemoryFile(ReadOnlyMemory<byte> fileData, string fileName, string? mimeType = null)
{
    public ReadOnlyMemory<byte> FileData { get; } = fileData;
    public string FileName { get; } = fileName;
    public string MimeType { get; } = string.IsNullOrWhiteSpace(mimeType) ? FileTypes.GetMimeType(fileName) : mimeType!;
}