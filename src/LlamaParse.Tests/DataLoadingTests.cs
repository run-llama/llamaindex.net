using FluentAssertions;
using LlamaIndex.Core.Schema;

namespace LlamaParse.Tests;

public class DataLoadingTests
{
    [SkipOnKeyNotFoundFact]
    public async Task can_load_pdf_file()
    {
        var llamaParseClient = new LlamaParseClient(new HttpClient(new LoggingHandler(new HttpClientHandler())), Environment.GetEnvironmentVariable("LLAMA_CLOUD_API_KEY") ?? string.Empty);

        var fileInfo = new FileInfo("./data/attention_is_all_you_need.pdf");

        var documents = new List<Document>();
        await foreach (var document in llamaParseClient.LoadDataAsync(fileInfo))
        {
            documents.Add(document);
        }

        documents.Should().NotBeEmpty();
    }

    [SkipOnKeyNotFoundFact]
    public async Task can_load_pdf_file_by_page()
    {
        
        var llamaParseClient = new LlamaParseClient(new HttpClient(new LoggingHandler(new HttpClientHandler())), Environment.GetEnvironmentVariable("LLAMA_CLOUD_API_KEY") ?? string.Empty);

        var fileInfo = new FileInfo("./data/attention_is_all_you_need.pdf");

        var documents = new List<Document>();
        await foreach (var document in llamaParseClient.LoadDataAsync(fileInfo, splitByPage:true))
        {
            documents.Add(document);
        }

        documents.Should().NotBeEmpty();
    }

    [SkipOnKeyNotFoundFact]
    public async Task can_load_pdf_file_by_page_and_include_images()
    {
        var configuration = new Configuration(itemsToInclude: ItemType.Image);
        var llamaParseClient = new LlamaParseClient(new HttpClient(new LoggingHandler(new HttpClientHandler())), Environment.GetEnvironmentVariable("LLAMA_CLOUD_API_KEY") ?? string.Empty, configuration:configuration);

        var fileInfo = new FileInfo("./data/polyglot_tool.pdf");

        var documents = new List<Document>();
        await foreach (var document in llamaParseClient.LoadDataAsync(fileInfo, splitByPage: true))
        {
            documents.Add(document);
        }

        documents.Should().NotBeEmpty();

        documents.OfType<ImageDocument>().Should().NotBeEmpty();
    }

    [SkipOnKeyNotFoundFact]
    public async Task can_load_pdf_powerpoint_file()
    {
        var llamaParseClient = new LlamaParseClient(new HttpClient(new LoggingHandler(new HttpClientHandler())), Environment.GetEnvironmentVariable("LLAMA_CLOUD_API_KEY") ?? string.Empty);

        var fileInfo = new FileInfo("./data/working_memory_update.pptx");

        var documents = new List<Document>();
        await foreach (var document in llamaParseClient.LoadDataAsync(fileInfo))
        {
            documents.Add(document);
        }

        documents.Should().NotBeEmpty();
    }

    [SkipOnKeyNotFoundFact]
    public async Task can_load_excel_file()
    {
        var llamaParseClient = new LlamaParseClient(new HttpClient(new LoggingHandler(new HttpClientHandler())), Environment.GetEnvironmentVariable("LLAMA_CLOUD_API_KEY") ?? string.Empty);

        var fileInfo = new FileInfo("./data/customers_100.xlsx");

        var documents = new List<Document>();
        await foreach (var document in llamaParseClient.LoadDataAsync(fileInfo))
        {
            documents.Add(document);
        }

        documents.Should().NotBeEmpty();
    }
}