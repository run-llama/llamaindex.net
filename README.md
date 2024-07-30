# LlamaIndex.NET

LlamaIndex.NET contains core types for working with LlamaIndex and client SDKs. 

At this time, the following are supported:

- LlamaParse client SDK for .NET

## What is LlamaIndex?

[LlamaIndex](https://llamaindex.ai/) is a data framework for LLM applications.

[LlamaCloud](https://docs.llamaindex.ai/en/stable/llama_cloud/) is a managed platfor for data parsing and ingestion. It consists of the following components:

- [**LlamaParse**](https://docs.llamaindex.ai/en/stable/llama_cloud/llama_parse/): self-serve document parsing API
- **Ingestion and Retreival API**: Connect to 10+ data sources and sinks. Easily setup a data pipeline that can handle large volumes of data and incremental updates.
- **Evaluations and observability**: Run and track evaluations on your data and model

## Important Links

- Documentation: [https://docs.llamaindex.ai/en/stable/](https://docs.llamaindex.ai/en/stable/)
- Twitter: [https://twitter.com/llama_index](https://twitter.com/llama_index)
- Discord: [https://discord.gg/dGcwcsnxhU](https://discord.gg/dGcwcsnxhU)

## Contributing

Interested in contributing? See our [Contribution Guide](./CONTRIBUTING.md) for more details.

## Example Usage

Install the LlamaParse .NET SDK.

You can find samples in the [samples directory](./samples/README.md).

### Parse documents using the LlamaParse .NET SDK

```csharp
using LlamaParse;

// Initialize LlamaParse client
var parseConfig = new Configuration
{
    ApiKey = "YOUR-API-KEY";
};

var client = new LlamaParseClient(new HttpClient(), parseConfig);

// Get file info
var fileInfo = new FileInfo("attention-is-all-you-need.pdf");

// Parse document and format result as JSON
var documents = new List<RawResult>();
await foreach(var document in client.LoadDataRawAsync(fileInfo, ResultType.Json)
    {
        documents.Add(document);
    }

// Output to console
foreach(var document in documents)
{
    Console.WriteLine(document);
}
```