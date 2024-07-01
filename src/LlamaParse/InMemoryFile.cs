using System;

namespace LlamaParse;

/// <summary>
/// Represents an in-memory file.
/// </summary>
public class InMemoryFile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InMemoryFile"/> class.
    /// </summary>
    /// <param name="fileData">The file data.</param>
    /// <param name="fileName">The file name.</param>
    /// <param name="mimeType">The MIME type of the file.</param>
    public InMemoryFile(ReadOnlyMemory<byte> fileData, string fileName, string? mimeType = null)
    {
        FileData = fileData;
        FileName = fileName;
        MimeType = string.IsNullOrWhiteSpace(mimeType) ? FileTypes.GetMimeType(fileName) : mimeType!;
    }

    /// <summary>
    /// Gets the file data.
    /// </summary>
    public ReadOnlyMemory<byte> FileData { get; }

    /// <summary>
    /// Gets the file name.
    /// </summary>
    public string FileName { get; }

    /// <summary>
    /// Gets the MIME type of the file.
    /// </summary>
    public string MimeType { get; }
}
