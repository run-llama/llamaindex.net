# ParseDocuments Sample

This samples shows off the basics you need to get started parsing documents in .NET using the LlamaParse .NET client SDK inside of a console application. 

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [LlamaCloud API Key](https://docs.cloud.llamaindex.ai/llamacloud/getting_started/api_key)
- [Visual Studio](https://visualstudio.microsoft.com/downloads/) or [Visual Studio Code](https://code.visualstudio.com/Download)

## Guide

1. Configure your client

    ```csharp
    var apiKey = Environment.GetEnvironmentVariable("LLAMACLOUD_API_KEY");

    var parseConfig = new Configuration()
    {
        ApiKey = apiKey?? string.Empty
    };

    var llamaParseClient = new LlamaParseClient(new HttpClient(), parseConfig);
    ```

1. Use the client to parse your documents. In this case, we're using an `InMemoryFile`, which contains the document data `byte[]` from the paper [Attention is all you need](https://arxiv.org/pdf/1706.03762). For simplicity and further processing, we've opted to get the results in JSON format.

    ```csharp
    var document = new InMemoryFile(documentData, "attention-is-all-you-need.pdf");
    var parsedDocs = llamaParseClient.LoadDataRawAsync(document, ResultType.Json);
    ```

1. Extract parsed results and post-process. In this case, the code just takes the paginated results and prints them out to the console. 

    ```csharp
    await foreach (var parsedDoc in parsedDocs)
    {
        var serializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var result = JsonSerializer.Deserialize<ParseResult>(parsedDoc.Result, serializerOptions);
        
        foreach(var page in result.Pages)
        {
            Console.WriteLine($"Page {page.Page}");
            Console.WriteLine("-------------------");
            Console.WriteLine(page.Text);
            Console.WriteLine("-------------------");
        }
    }

    public record ParseResult(PageContent[] Pages);
    public record PageContent(int Page, string Text);
    ```