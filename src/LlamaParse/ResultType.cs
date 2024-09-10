namespace LlamaParse;

/// <summary>
/// Represents the type of result returned by a parsing operation.
/// </summary>
public enum ResultType
{
    /// <summary>
    /// The result is in Markdown format.
    /// </summary>
    Markdown,

    /// <summary>
    /// The result is in plain text format.
    /// </summary>
    Text,

    /// <summary>
    /// The result is in JSON format.
    /// </summary>
    Json,
}
