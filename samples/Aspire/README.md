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
