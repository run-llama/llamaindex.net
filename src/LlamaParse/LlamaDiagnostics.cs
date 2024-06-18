using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LlamaParse;

internal static class LlamaDiagnostics
{
    private static readonly string Namespace = typeof(LlamaDiagnostics).Namespace!;
    private static readonly ActivitySource s_activitySource = new(Namespace);

    private const string EnableDiagnosticsSwitch = "LlamaParse.EnableOTelDiagnostics";
    private const string EnableSensitiveEventsSwitch = "LlamaParse.EnableOTelDiagnosticsSensitive";
    private const string EnableDiagnosticsEnvVar = "LLAMAPARSE_ENABLE_OTEL_DIAGNOSTICS";
    private const string EnableSensitiveEventsEnvVar = "LLAMAPARSE_ENABLE_OTEL_DIAGNOSTICS_SENSITIVE";

    private static  bool EnableDiagnostics => AppContextSwitchHelper.GetConfigValue(EnableDiagnosticsSwitch, EnableDiagnosticsEnvVar);

    private static  bool EnableSensitiveEvents => AppContextSwitchHelper.GetConfigValue(EnableSensitiveEventsSwitch, EnableSensitiveEventsEnvVar);

    /// <summary>
    /// Check if diagnostics is enabled
    /// Diagnostics is enabled if either EnableDiagnostics or EnableSensitiveEvents is set to true and there are listeners.
    /// </summary>
    public static bool IsDiagnosticsEnabled()
    {
        return (EnableDiagnostics || EnableSensitiveEvents) && s_activitySource.HasListeners();
    }

    /// <summary>
    /// Check if sensitive events are enabled.
    /// Sensitive events are enabled if EnableSensitiveEvents is set to true and there are listeners.
    /// </summary>
    public static bool IsSensitiveEventsEnabled() => EnableSensitiveEvents && s_activitySource.HasListeners();

    public static Activity? StartGetResultActivity(string jobId, ResultType resultType)
    {
        if (!IsDiagnosticsEnabled())
        {
            return null;
        }

        var activity = s_activitySource.StartActivityWithTags(
            "llamaparse.get_result",
            [
                new("job_id", jobId),
                new ("result_type", resultType),
                new ("start_time", DateTime.UtcNow)
            ]);


        return activity;
    }

    public static void EndGetResultActivity(Activity? activity, string reason, RawResult? rawResults = null)
    {
        if (!IsDiagnosticsEnabled())
        {
            return ;
        }

        if (activity is { })
        {
            activity = activity.EnrichWithTags(
            [
                new("reason", reason),
                new ("end_time", DateTime.UtcNow)

            ]);
            if (rawResults is { })
            {
                activity= activity.EnrichWithTags(
                [
                    new (Constants.JobIsCacheHitKey, rawResults.IsCacheHit),
                    new (Constants.JobPagesKey, rawResults.JobPages),
                    new (Constants.JobCreditsUsageKey, rawResults.CreditsUsed),

                ]);
            }
        }
    }
}

internal static class ActivityExtensions
{
    public static Activity? StartActivityWithTags(this ActivitySource source, string name, List<KeyValuePair<string, object?>> tags)
    {
        return source.StartActivity(
            name,
            ActivityKind.Internal,
            Activity.Current?.Context ?? new ActivityContext(),
            tags);
    }

    public static Activity EnrichWithTags(this Activity activity, List<KeyValuePair<string, object?>> tags)
    {
        tags.ForEach(tag =>
        {
            if (tag.Value is not null)
            {
                activity.SetTag(tag.Key, tag.Value);
            }
        });

        return activity;
    }

    public static Activity AttachSensitiveDataAsEvent(this Activity activity, string name, List<KeyValuePair<string, object?>> tags)
    {
        activity.AddEvent(new ActivityEvent(
            name,
            tags: new ActivityTagsCollection(tags)
        ));

        return activity;
    }
}