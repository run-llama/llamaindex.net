﻿namespace LlamaParse;


/// <summary>
/// Represents the configuration settings for LlamaParse.
/// </summary>
public class Configuration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Configuration"/> class.
    /// </summary>
    /// <param name="language">The language to use for parsing.</param>
    /// <param name="endpoint">The Llama cloud endpoint</param>
    /// <param name="parsingInstructions">The parsing instructions.</param>
    /// <param name="skipDiagonalText">Whether to skip diagonal text.</param>
    /// <param name="invalidateCache">Whether to invalidate the cache.</param>
    /// <param name="doNotCache">Whether to disable caching.</param>
    /// <param name="fastMode">Whether to enable fast mode.</param>
    /// <param name="doNotUnrollColumns">Whether to disable unrolling columns.</param>
    /// <param name="pageSeparator">The page separator.</param>
    /// <param name="gpt4oMode">Whether to enable GPT-4o mode.</param>
    /// <param name="gpt4oApiKey">The GPT-4o API key.</param>
    /// <param name="itemsToExtract">The items to extract while processing.</param>
    /// <param name="resultType">The result type.</param>
    /// <param name="apiKey">The LlamaCloud key.</param>
    public Configuration(
        string apiKey,
        Languages language = Languages.English,
        string? endpoint = null,
        string? parsingInstructions = null,
        bool skipDiagonalText = false,
        bool invalidateCache = false,
        bool doNotCache = false,
        bool fastMode = false,
        bool doNotUnrollColumns = false,
        string? pageSeparator = null,
        bool gpt4oMode = false,
        string? gpt4oApiKey = null,
        ItemType itemsToExtract = ItemType.None,
        ResultType resultType = default)
    {
        ApiKey = apiKey;
        Language = language;
        Endpoint = endpoint;
        ParsingInstructions = parsingInstructions;
        SkipDiagonalText = skipDiagonalText;
        InvalidateCache = invalidateCache;
        DoNotCache = doNotCache;
        FastMode = fastMode;
        DoNotUnrollColumns = doNotUnrollColumns;
        PageSeparator = pageSeparator;
        Gpt4oMode = gpt4oMode;
        Gpt4oApiKey = gpt4oApiKey;
        ItemsToExtract = itemsToExtract;
        ResultType = resultType;
    }

    /// <summary>
    /// Gets the LlamaCloud key.
    /// </summary>
    public string ApiKey { get; }

    /// <summary>
    /// Gets the language to use for parsing.
    /// </summary>
    public Languages Language { get; }

    /// <summary>
    ///  Gets the Llama cloud endpoint.
    /// </summary>
    public string? Endpoint { get; }

    /// <summary>
    /// Gets the parsing instructions.
    /// </summary>
    public string? ParsingInstructions { get; }

    /// <summary>
    /// Gets whether to skip diagonal text.
    /// </summary>
    public bool SkipDiagonalText { get; }

    /// <summary>
    /// Gets whether to invalidate the cache.
    /// </summary>
    public bool InvalidateCache { get; }

    /// <summary>
    /// Gets whether to disable caching.
    /// </summary>
    public bool DoNotCache { get; }

    /// <summary>
    /// Gets whether to enable fast mode.
    /// </summary>
    public bool FastMode { get; }

    /// <summary>
    /// Gets whether to disable unrolling columns.
    /// </summary>
    public bool DoNotUnrollColumns { get; }

    /// <summary>
    /// Gets the page separator.
    /// </summary>
    public string? PageSeparator { get; }

    /// <summary>
    /// Gets whether to enable GPT-4o mode.
    /// </summary>
    public bool Gpt4oMode { get; }

    /// <summary>
    /// Gets the GPT-4o API key.
    /// </summary>
    public string? Gpt4oApiKey { get; }

    /// <summary>
    /// Gets the items to include.
    /// </summary>
    public ItemType ItemsToExtract { get; }

    /// <summary>
    /// Gets the result type.
    /// </summary>
    public ResultType ResultType { get; }
}