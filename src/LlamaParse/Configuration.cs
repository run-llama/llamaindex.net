using System;

namespace LlamaParse;

[Flags]
public enum ItemType
{
    None = 0,
    Image = 1,
    Table = 2,

}
public class Configuration(
    Languages language = Languages.English,
    string? parsingInstructions = null,
    bool skipDiagonalText = false,
    bool invalidateCache = false,
    bool doNotCache = false,
    bool fastMode = false,
    bool doNotUnrollColumns = false,
    string? pageSeparator = null,
    bool gpt4oMode = false,
    string? gpt4oApiKey = null,
    ItemType itemsToInclude = ItemType.None,
    ResultType resultType = default)
{
    public Languages Language { get; } = language;
    public string? ParsingInstructions { get; } = parsingInstructions;
    public bool SkipDiagonalText { get; } = skipDiagonalText;
    public bool InvalidateCache { get; } = invalidateCache;
    public bool DoNotCache { get; } = doNotCache;
    public bool FastMode { get; } = fastMode;
    public bool DoNotUnrollColumns { get; } = doNotUnrollColumns;
    public string? PageSeparator { get; } = pageSeparator;
    public bool Gpt4oMode { get; } = gpt4oMode;
    public string? Gpt4oApiKey { get; } = gpt4oApiKey;
    public ItemType ItemsToInclude { get; } = itemsToInclude;
    public ResultType ResultType { get; } = resultType;
}