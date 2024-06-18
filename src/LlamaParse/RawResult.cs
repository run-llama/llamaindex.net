using System.Collections.Generic;
using System.Text.Json;

namespace LlamaParse;

public record RawResult(string JobId, JsonElement Result, Dictionary<string, object>? Metadata, double CreditsUsed, double CreditsMax, double JobCreditsUsage, double JobPages, bool IsCacheHit)
{
    public string JobId { get; } = JobId;
    public JsonElement Result { get; } = Result;
    public Dictionary<string, object>? Metadata { get; } = Metadata;
    public double CreditsUsed { get; } = CreditsUsed;
    public double CreditsMax { get; } = CreditsMax;
    public double JobCreditsUsage { get; } = JobCreditsUsage;
    public double JobPages { get; } = JobPages;
    public bool IsCacheHit { get; } = IsCacheHit;
}