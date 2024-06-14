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
        var llamaParseClient = new LlamaParse(new HttpClient(), "app key");

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
        var llamaParseClient = new LlamaParse(new HttpClient(), "app key");

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
        var llamaParseClient = new LlamaParse(new HttpClient(new LoggingHandler(new HttpClientHandler())), Environment.GetEnvironmentVariable("LLAMA_CLOUD_API_KEY")??string.Empty);

        var fileInfo = new FileInfo(@"D:\llama-rag\pdfs\1-29-24_An-actuarys-guide-to-Julia.pdf");

    
        var documents = new List<Document>();
        await foreach (var document in llamaParseClient.LoadDataAsync(fileInfo))
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
        var llamaParseClient = new LlamaParse(new HttpClient(new LoggingHandler(new HttpClientHandler())), Environment.GetEnvironmentVariable("LLAMA_CLOUD_API_KEY") ?? string.Empty, configuration: configuration);

        var fileInfo = new FileInfo(@"D:\llama-rag\pdfs\1-29-24_An-actuarys-guide-to-Julia.pdf");


        var images = new List<SKImage>();
        await foreach (var document in llamaParseClient.LoadDataAsync(fileInfo))
        {
            if (document is ImageDocument imageDocument)
            {
              
                var imageObject = SKImage.FromEncodedData(Convert.FromBase64String( imageDocument.Image));
                images.Add(imageObject);
            }
        }

        images.Should().NotBeEmpty();
    }
}

public class LoggingHandler : DelegatingHandler
{
    private readonly string _name;

    public LoggingHandler(HttpMessageHandler innerHandler,[CallerMemberName] string name = "")
        : base(innerHandler)
    {
        _name = name;
    }

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
        await File.WriteAllTextAsync(@$"D:\log_request_message_{_name}.txt", log.ToString(), cancellationToken);

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
        await File.WriteAllTextAsync($@"D:\log_response_message_{_name}.txt", message, cancellationToken); 
        return response;
    }
}