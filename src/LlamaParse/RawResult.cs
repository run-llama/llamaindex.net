using System.Collections.Generic;
using System.Text.Json;


/// <summary>
/// Represents the raw result of a job.
/// </summary>
public record RawResult(
    string JobId,
    JsonElement Result,
    Dictionary<string, object>? Metadata,
    double CreditsUsed,
    double CreditsMax,
    double JobCreditsUsage,
    double JobPages,
    bool IsCacheHit)
{
    /// <summary>
    /// Gets the unique identifier of the job.
    /// </summary>
    public string JobId { get; } = JobId;

    /// <summary>
    /// Gets the result of the job as a JSON element.
    /// </summary>
    public JsonElement Result { get; } = Result;

    /// <summary>
    /// Gets the metadata associated with the job, if any.
    /// </summary>
    public Dictionary<string, object>? Metadata { get; } = Metadata;

    /// <summary>
    /// Gets the number of credits used by the job.
    /// </summary>
    public double CreditsUsed { get; } = CreditsUsed;

    /// <summary>
    /// Gets the maximum number of credits allowed for the job.
    /// </summary>
    public double CreditsMax { get; } = CreditsMax;

    /// <summary>
    /// Gets the credits usage of the job.
    /// </summary>
    public double JobCreditsUsage { get; } = JobCreditsUsage;

    /// <summary>
    /// Gets the number of pages processed by the job.
    /// </summary>
    public double JobPages { get; } = JobPages;

    /// <summary>
    /// Gets a value indicating whether the result is a cache hit.
    /// </summary>
    public bool IsCacheHit { get; } = IsCacheHit;
}
