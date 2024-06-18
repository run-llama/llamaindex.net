using FluentAssertions;
using LlamaIndex.Core.Schema;

using System.Diagnostics;

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
            ActivityStopped = activity => {
                if (activity.Id is not null && activity.DisplayName.Contains("llamaparse."))
                {
                    activities[activity.Id] = activity;
                }
            }
        };


        ActivitySource.AddActivityListener(listener);

        var llamaParseClient = new LlamaParse(new HttpClient(new LoggingHandler(new HttpClientHandler())), Environment.GetEnvironmentVariable("LLAMA_CLOUD_API_KEY") ?? string.Empty);

        var fileInfo = new FileInfo("./data/attention_is_all_you_need.pdf");

        var documents = new List<Document>();
        await foreach (var document in llamaParseClient.LoadDataAsync(fileInfo))
        {
            documents.Add(document);
        }

        activities.Should().NotBeEmpty();
    }
}