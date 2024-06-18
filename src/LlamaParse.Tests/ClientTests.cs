using System.Runtime.CompilerServices;
using FluentAssertions;
using LlamaIndex.Core.Schema;
using System.Text;
using SkiaSharp;

namespace LlamaParse.Tests;

public class ClientTests
{
    [Fact]
    public void throws_exception_when_parsing_unsupported_files()
    {
        var llamaParseClient = new LlamaParseClient(new HttpClient(), "app key");

        var fileInfo = new FileInfo("test.ghh");

        var action = async () =>
        {
            await foreach (var document in llamaParseClient.LoadDataAsync(fileInfo))
            {
                // do nothing
            }
        };

        action.Should().ThrowExactlyAsync<InvalidOperationException>();
    }

    [Fact]
    public void throws_exception_when_file_does_not_exist()
    {
        var llamaParseClient = new LlamaParseClient(new HttpClient(), "app key");

        var fileInfo = new FileInfo("test.pdf");

        var action = async () =>
        {
            await foreach (var document in llamaParseClient.LoadDataAsync(fileInfo))
            {
                // do nothing
            }
        };

        action.Should().ThrowExactlyAsync<FileNotFoundException>();
    }

    [SkipOnKeyNotFoundFact]
    public async Task can_load_pdf()
    {
        var llamaParseClient = new LlamaParseClient(new HttpClient(new LoggingHandler(new HttpClientHandler())), Environment.GetEnvironmentVariable("LLAMA_CLOUD_API_KEY")??string.Empty);

        var fileInfo = new FileInfo("./data/attention_is_all_you_need.pdf");

    
        var documents = new List<Document>();
        await foreach (var document in llamaParseClient.LoadDataAsync(fileInfo))
        {
            documents.Add(document);
        }

        documents.Should().NotBeEmpty();
    }

    [SkipOnKeyNotFoundFact]
    public async Task can_load_pdf_as_markdown()
    {
        var llamaParseClient = new LlamaParseClient(new HttpClient(new LoggingHandler(new HttpClientHandler())), Environment.GetEnvironmentVariable("LLAMA_CLOUD_API_KEY") ?? string.Empty);

        var fileInfo = new FileInfo("./data/attention_is_all_you_need.pdf");


        var documents = new List<RawResult>();
        await foreach (var document in llamaParseClient.LoadDataRawAsync(fileInfo,  resultType: ResultType.Markdown))
        {
            documents.Add(document);
        }

        documents.Should().NotBeEmpty();
    }

    [SkipOnKeyNotFoundFact]
    public async Task can_load_pdf_as_text()
    {
        var llamaParseClient = new LlamaParseClient(new HttpClient(new LoggingHandler(new HttpClientHandler())), Environment.GetEnvironmentVariable("LLAMA_CLOUD_API_KEY") ?? string.Empty);

        var fileInfo = new FileInfo("./data/attention_is_all_you_need.pdf");


        var documents = new List<RawResult>();
        await foreach (var document in llamaParseClient.LoadDataRawAsync(fileInfo, resultType: ResultType.Text))
        {
            documents.Add(document);
        }

        documents.Should().NotBeEmpty();
    }

    [SkipOnKeyNotFoundFact]
    public async Task can_load_pdf_as_json()
    {
        var llamaParseClient = new LlamaParseClient(new HttpClient(new LoggingHandler(new HttpClientHandler())), Environment.GetEnvironmentVariable("LLAMA_CLOUD_API_KEY") ?? string.Empty);

        var fileInfo = new FileInfo("./data/attention_is_all_you_need.pdf");


        var documents = new List<RawResult>();
        await foreach (var document in llamaParseClient.LoadDataRawAsync(fileInfo, ResultType.Json))
        {
            documents.Add(document);
        }

        documents.Should().NotBeEmpty();
    }

    [SkipOnKeyNotFoundFact]
    public async Task can_load_pdf_with_images()
    {
        var configuration = new Configuration
            (extractImages: true);
        var llamaParseClient = new LlamaParseClient(new HttpClient(new LoggingHandler(new HttpClientHandler())), Environment.GetEnvironmentVariable("LLAMA_CLOUD_API_KEY") ?? string.Empty, configuration: configuration);

        var fileInfo = new FileInfo("./data/polyglot_tool.pdf");


        var images = new List<SKImage>();
        await foreach (var document in llamaParseClient.LoadDataAsync(fileInfo))
        {
            if (document is ImageDocument imageDocument)
            {
              
                var imageObject = SKImage.FromEncodedData(Convert.FromBase64String( imageDocument.Image!));
                images.Add(imageObject);
            }
        }

        images.Should().NotBeEmpty();
    }
}

internal class LoggingHandler(HttpMessageHandler innerHandler, [CallerMemberName] string name = "")
    : DelegatingHandler(innerHandler)
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var log = new StringBuilder();
        log.AppendLine("Request:");
        log.AppendLine(request.ToString());
        if (request.Content != null)
        {
            log.AppendLine(await request.Content.ReadAsStringAsync(cancellationToken));
        }
        log.AppendLine();
        await File.WriteAllTextAsync(@$"D:\log_request_message_{name}.txt", log.ToString(), cancellationToken);

        log.Clear();
        var response = await base.SendAsync(request, cancellationToken);

        log.AppendLine("Response:");
        log.AppendLine(response.ToString());
        if (response.Content != null)
        {
            log.AppendLine(await response.Content.ReadAsStringAsync(cancellationToken));
        }
        log.AppendLine();

        var message = log.ToString();
        await File.WriteAllTextAsync($@"D:\log_response_message_{name}.txt", message, cancellationToken); 
        return response;
    }
}