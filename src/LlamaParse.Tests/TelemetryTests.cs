using System.Diagnostics;
using FluentAssertions;
using LlamaIndex.Core.Schema;

namespace LlamaParse.Tests;

public class TelemetryTests
{
    [SkipOnKeyNotFoundFact]
    public async Task produces_telemetry_on_loading()
    {
        AppContext.SetSwitch("LlamaParse.EnableOTelDiagnostics", true);

        var activities = new Dictionary<string, Activity>();

        using var listener = new ActivityListener
        {
            ShouldListenTo = _ => true,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData,
            ActivityStarted = activity =>
            {
                if (activity.Id is not null && activity.DisplayName.Contains("llamaparse."))
                {
                    activities[activity.Id] = activity;
                }

            },
            ActivityStopped = activity =>
            {
                if (activity.Id is not null && activity.DisplayName.Contains("llamaparse."))
                {
                    activities[activity.Id] = activity;
                }
            }
        };


        ActivitySource.AddActivityListener(listener);

        var config = new Configuration { ApiKey = Environment.GetEnvironmentVariable("LLAMA_CLOUD_API_KEY") ?? string.Empty, ItemsToExtract = ItemType.Image };
        var llamaParseClient = new LlamaParseClient(new HttpClient(new LoggingHandler(new HttpClientHandler())),
            config);

        var fileInfo = new FileInfo("./data/polyglot_tool.pdf");

        var documents = new List<Document>();
        await foreach (var document in llamaParseClient.LoadDataAsync(fileInfo))
        {
            documents.Add(document);
        }

        activities.Should().NotBeEmpty();
    }
}
