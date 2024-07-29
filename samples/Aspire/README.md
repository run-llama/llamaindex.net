# LlamaParse + Aspire

This sample shows how to add a [LlamaParse](https://docs.llamaindex.ai/en/stable/llama_cloud/llama_parse/) client for file parsing to a .NET Web API using Aspire.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) or greater
- [Aspire workload](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [LlamaIndex API key](https://docs.cloud.llamaindex.ai/llamaparse/getting_started/get_an_api_key)
- [Visual Studio](https://visualstudio.microsoft.com/downloads/) or [Visual Studio Code](https://code.visualstudio.com/Download)

## Configuration

1. Add your LlamaParse API key to the *appsettings.Development.json* configuration in the `LlamaParseAspire` project.

```json
"LlamaParse": {
    "ApiKey": "ADD-YOUR-KEY-HERE"
}
```

## Guide

1. In your Web API project `LlamaParseAspire`, add the following code to register the `LlamaParseClient`.

```csharp
builder.AddLlamaParseClient(builder.Configuration.GetSection("LlamaParse").Get<Configuration>()!);
```

1. Use the `LlamaParseClient` just like you would in any other application. In this case, the `/parse` endpoint handler takes a file as input, uses LlamaParse to extract the data, and returns the parsed results back to the user for further downstream processing.

```csharp
var fileUploadHandler = async (LlamaParseClient client, IFormFile file) =>
{
    var fileName = file.FileName;

    // Read the file into a byte array
    using var ms = new MemoryStream();
    file.CopyTo(ms);

    var inMemoryFile = new InMemoryFile(ms.ToArray(), fileName);

    var sb = new StringBuilder();
    await foreach (var doc in client.LoadDataAsync(inMemoryFile))
    {
        if(doc is ImageDocument)
        {
            continue;
        }
        else
        {
            sb.AppendLine(doc.Text);
        }
    }
    return Results.Ok(sb.ToString());
};
```

## Enable telemetry

The LlamaParse .NET client SDK contains OpenTelemetry instrumentation to log traces and metrics related to LlamaParse jobs.

To enable it:

1. Add the following code to the `ConfigureOpenTelemetry` method in the `*.ServiceDefaults` project.

```csharp
builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics =>
    {
        //other metrics code...

        // Add a meter for the LlamaParse namespace
        metrics.AddMeter("LlamaParse");
    })
    .WithTracing(tracing =>
    {
        //other tracing code...

        // Add a source for the LlamaParse namespace
        tracing.AddSource("LlamaParse");
    });
```

Now that this is configured, traces and metrics will begin to display in the Aspire dasboard. For more details, on [Aspire telemetry](https://learn.microsoft.com/dotnet/aspire/fundamentals/telemetry) and the [dashboard](https://learn.microsoft.com/dotnet/aspire/fundamentals/dashboard/overview?tabs=bash), see the documentation.