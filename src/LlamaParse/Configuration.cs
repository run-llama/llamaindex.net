using System;

namespace LlamaParse;


/// <summary>
/// Represents the configuration settings for LlamaParse.
/// </summary>
public class Configuration
{
    /// <summary>
    /// Gets the LlamaCloud key.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets the language to use for parsing.
    /// </summary>
    public Languages Language { get; set; } = Languages.English;

    /// <summary>
    ///  Gets the Llama cloud endpoint.
    /// </summary>
    public string? Endpoint { get; set; }

    /// <summary>
    /// Gets the parsing instructions.
    /// </summary>
    public string? ParsingInstructions { get; set; }

    /// <summary>
    /// Gets whether to skip diagonal text.
    /// </summary>
    public bool SkipDiagonalText { get; set; }

    /// <summary>
    /// Gets whether to invalidate the cache.
    /// </summary>
    public bool InvalidateCache { get; set; }

    /// <summary>
    /// Gets whether to disable caching.
    /// </summary>
    public bool DoNotCache { get; set; }

    /// <summary>
    /// Gets whether to enable fast mode.
    /// </summary>
    public bool FastMode { get; set; }

    /// <summary>
    /// Gets whether to disable unrolling columns.
    /// </summary>
    public bool DoNotUnrollColumns { get; set; }

    /// <summary>
    /// Gets the page separator.
    /// </summary>
    public string? PageSeparator { get; set; }

    /// <summary>
    /// Gets whether to enable GPT-4o mode.
    /// </summary>
    public bool Gpt4oMode { get; set; }

    /// <summary>
    /// Gets the GPT-4o API key.
    /// </summary>
    public string? Gpt4oApiKey { get; set; }

    /// <summary>
    /// Gets the items to include.
    /// </summary>
    public ItemType ItemsToExtract { get; set; }

    /// <summary>
    /// Gets the result type.
    /// </summary>
    public ResultType ResultType { get; set; }
}
