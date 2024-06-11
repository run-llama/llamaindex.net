using Xunit.Sdk;
using FluentAssertions;
using LlamaIndex.Core.Schema;
using System.Security.Principal;

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
    public async Task load_files()
    {
        var llamaParseClient = new LlamaParse(new HttpClient(), Environment.GetEnvironmentVariable("LLAMA_CLOUD_API_KEY")??string.Empty);

        var fileInfo = new FileInfo(@"D:/Repos/public/llama_parse/tests/test_files/attention_is_all_you_need.pdf");

    
        var documents = new List<Document>();
        await foreach (var document in llamaParseClient.LoadDataAsync(fileInfo))
        {
            documents.Add(document);
        }

        documents.Should().NotBeEmpty();
    }
}