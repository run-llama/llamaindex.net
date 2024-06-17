using System.Collections.Generic;
using System.Text.Json;

namespace LlamaParse;

public record RawResult(string JobId, JsonElement Result, Dictionary<string,object>? Metadata)
{
    public string JobId { get; } = JobId;
    public JsonElement Result { get; } = Result;
    public Dictionary<string,object>? Metadata { get; } = Metadata;
}