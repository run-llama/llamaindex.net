using LlamaParse;
using System.Diagnostics.Contracts;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

// Configure LlamaParse client
var apiKey = Environment.GetEnvironmentVariable("LLAMACLOUD_API_KEY");

var parseConfig = new Configuration()
{
    ApiKey = apiKey?? string.Empty
};

var llamaParseClient = new LlamaParseClient(new HttpClient(), parseConfig);

// Get document
var client = new HttpClient();
var documentData = await client.GetByteArrayAsync("https://arxiv.org/pdf/1706.03762");

// Parse documents
var document = new InMemoryFile(documentData, "attention-is-all-you-need.pdf");
var parsedDocs = llamaParseClient.LoadDataRawAsync(document, ResultType.Json);

//  Output parse results
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