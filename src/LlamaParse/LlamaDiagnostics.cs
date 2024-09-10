using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace LlamaParse;

internal static class LlamaDiagnostics
{
    private static readonly string Namespace = typeof(LlamaDiagnostics).Namespace!;
    private static readonly ActivitySource s_activitySource = new(Namespace);

    private static string EnableDiagnosticsSwitch => $"{Namespace}.EnableOTelDiagnostics";
    private static string EnableSensitiveEventsSwitch => $"{Namespace}.EnableOTelDiagnosticsSensitive";

    private const string EnableDiagnosticsEnvVar = "LLAMAPARSE_ENABLE_OTEL_DIAGNOSTICS";
    private const string EnableSensitiveEventsEnvVar = "LLAMAPARSE_ENABLE_OTEL_DIAGNOSTICS_SENSITIVE";

    private static bool EnableDiagnostics => AppContextSwitchHelper.GetConfigValue(EnableDiagnosticsSwitch, EnableDiagnosticsEnvVar);

    private static bool EnableSensitiveEvents => AppContextSwitchHelper.GetConfigValue(EnableSensitiveEventsSwitch, EnableSensitiveEventsEnvVar);

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
                new("result_type", resultType)
            ]);


        return activity;
    }

    public static void EndGetResultActivity(Activity? activity, string reason, RawResult? rawResults = null)
    {
        if (!IsDiagnosticsEnabled())
        {
            return;
        }

        if (activity is { })
        {
            activity = activity.EnrichWithTags(
            [
                new("reason", reason),

            ]);
            if (rawResults is { })
            {
                activity.EnrichWithTags(
                [
                    new(Constants.JobIsCacheHitKey, rawResults.IsCacheHit),
                    new(Constants.JobPagesKey, rawResults.JobPages),
                    new(Constants.JobCreditsUsageKey, rawResults.CreditsUsed),

                ]);
            }
        }
    }

    public static Activity? StartGetImageActivity(string jobId, string imageName)
    {
        if (!IsDiagnosticsEnabled())
        {
            return null;
        }

        var activity = s_activitySource.StartActivityWithTags(
            "llamaparse.get_image",
            [
                new("job_id", jobId),

            ]);
        if (EnableSensitiveEvents)
        {
            activity!.EnrichWithTags(
            [
                new("image_name", imageName),

            ]);
        }

        return activity;
    }

    public static void EndGetImageActivity(Activity? activity, string reason)
    {
        if (!IsDiagnosticsEnabled())
        {
            return;
        }

        if (activity is { })
        {
            activity.EnrichWithTags(
             [
                 new("reason", reason),

             ]);

        }
    }

    public static Activity? StartCreateJob(FileInfo fileInfo)
    {
        if (!IsDiagnosticsEnabled())
        {
            return null;
        }

        var activity = s_activitySource.StartActivityWithTags(
            "llamaparse.create_job",
            [
                new("file_extension", fileInfo.Extension),

            ]);
        if (EnableSensitiveEvents)
        {
            activity!.EnrichWithTags(
            [
                new("file_path", fileInfo.Name),

            ]);
        }

        return activity;
    }

    public static Activity? StartCreateJob(string filename)
    {
        if (!IsDiagnosticsEnabled())
        {
            return null;
        }

        var extension = Path.GetExtension(filename);

        var activity = s_activitySource.StartActivityWithTags(
            "llamaparse.create_job",
            [
                new("file_extension", extension),

            ]);
        if (EnableSensitiveEvents)
        {
            activity!.EnrichWithTags(
            [
                new("file_path", filename),

            ]);
        }

        return activity;
    }

    public static void EndCreateJob(Activity? activity, string reason, string jobId)
    {
        if (!IsDiagnosticsEnabled())
        {
            return;
        }

        if (activity is { })
        {
            activity.EnrichWithTags(
            [
                new("reason", reason),
                new("job_id", jobId),

            ]);

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
