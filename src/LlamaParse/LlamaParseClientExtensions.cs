using LlamaIndex.Core.Schema;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace LlamaParse;

/// <summary>
/// The LlamaParseClientExtensions class provides extension methods for the LlamaParseClient class.
/// </summary>
public static class LlamaParseClientExtensions
{
    /// <summary>
    /// Loads data asynchronously from a file.
    /// </summary>
    /// <param name="llamaParseClient">The LlamaParseClient instance.</param>
    /// <param name="file">The file to load data from.</param>
    /// <param name="splitByPage">Indicates whether to split the data by page.</param>
    /// <param name="metadata">Optional metadata for the document.</param>
    /// <param name="itemsToExtract">Optional item to extract <see cref="ItemType"/></param>
    /// <param name="language">Optional Language</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>An asynchronous enumerable of Document objects.</returns>
    public static IAsyncEnumerable<Document> LoadDataAsync(this LlamaParseClient llamaParseClient, FileInfo file, bool splitByPage = false, Dictionary<string, object>? metadata = null, Languages? language = null, ItemType? itemsToExtract = null, CancellationToken cancellationToken = default)
    {
        return llamaParseClient.LoadDataAsync(
            files: [file], 
            splitByPage: splitByPage,
            metadata: metadata, 
            itemsToExtract: itemsToExtract, 
            language: language, 
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Loads data asynchronously from a file.
    /// </summary>
    /// <param name="llamaParseClient">The LlamaParseClient instance.</param>
    /// <param name="inMemoryFile">The file to load data from. <see cref="InMemoryFile"/></param>
    /// <param name="splitByPage">Indicates whether to split the data by page.</param>
    /// <param name="metadata">Optional metadata for the document.</param>
    /// <param name="itemsToExtract">Optional item to extract <see cref="ItemType"/></param>
    /// <param name="language">Optional Language</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>An asynchronous enumerable of Document objects.</returns>
    public static IAsyncEnumerable<Document> LoadDataAsync(this LlamaParseClient llamaParseClient, InMemoryFile inMemoryFile, bool splitByPage = false, Dictionary<string, object>? metadata = null, Languages? language = null, ItemType? itemsToExtract = null, CancellationToken cancellationToken = default)
    {
        return llamaParseClient.LoadDataAsync(
            inMemoryFiles: [inMemoryFile], 
            splitByPage: splitByPage, 
            metadata: metadata, 
            itemsToExtract: itemsToExtract, 
            language: language, 
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Loads data asynchronously from in-memory files.
    /// </summary>
    /// <param name="llamaParseClient">The LlamaParseClient instance.</param>
    /// <param name="inMemoryFiles">The in-memory files to load data from. <see cref="inMemoryFiles"/> </param>
    /// <param name="splitByPage">Indicates whether to split the data by page.</param>
    /// <param name="metadata">Metadata for the document. (Optional)</param>
    /// <param name="itemsToExtract">Item to extract.(Optional)<see cref="ItemType"/></param>
    /// <param name="language">Language (Optional)</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>An asynchronous enumerable of Document objects.</returns>
    public static async IAsyncEnumerable<Document> LoadDataAsync(
        this LlamaParseClient llamaParseClient,
        IEnumerable<InMemoryFile> inMemoryFiles,
        bool splitByPage = false,
        Languages? language = null, 
        ItemType? itemsToExtract = null,
        Dictionary<string, object>? metadata = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var documentMetadata = metadata ?? new Dictionary<string, object>();

        await foreach (var rawResult in llamaParseClient.LoadDataRawAsync(
                           files:inMemoryFiles, 
                           resultType: ResultType.Json, 
                           metadata: documentMetadata, 
                           language: language, cancellationToken))
        {
            await foreach (var document in CreateDocumentsFromRawResult(
                               llamaParseClient: llamaParseClient, 
                               rawResult: rawResult, 
                               splitByPage: splitByPage, 
                               documentMetadata: documentMetadata, 
                               itemsToExtract: itemsToExtract, 
                               cancellationToken: cancellationToken))
            {
                yield return document;
            }
        }

    }

    /// <summary>
    /// Loads data asynchronously from files.
    /// </summary>
    /// <param name="llamaParseClient">The LlamaParseClient instance.</param>
    /// <param name="files">The files to load data from.</param>
    /// <param name="splitByPage">Indicates whether to split the data by page.</param>
    /// <param name="metadata">Optional metadata for the document.</param>
    /// <param name="itemsToExtract">Optional item to extract <see cref="ItemType"/></param>
    /// <param name="language">Language (Optional)</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>An asynchronous enumerable of Document objects.</returns>
    public static async IAsyncEnumerable<Document> LoadDataAsync(this LlamaParseClient llamaParseClient, IEnumerable<FileInfo> files, bool splitByPage = false, Dictionary<string, object>? metadata = null, Languages? language = null, ItemType? itemsToExtract = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var documentMetadata = metadata ?? new Dictionary<string, object>();

        await foreach (var rawResult in llamaParseClient.LoadDataRawAsync(
                           files: files, 
                           resultType: ResultType.Json, 
                           metadata: documentMetadata, 
                           language: language, 
                           cancellationToken: cancellationToken))
        {
            await foreach (var document in CreateDocumentsFromRawResult(
                               llamaParseClient: llamaParseClient,
                               rawResult: rawResult,
                               splitByPage: splitByPage,
                               documentMetadata: documentMetadata,
                               itemsToExtract: itemsToExtract,
                               cancellationToken: cancellationToken))
            {
                yield return document;
            }
        }
    }

    /// <summary>
    /// Adds a LlamaParseClient to the service collection
    /// </summary>
    /// <param name="builder">The <see cref="IHostApplicationBuilder" /> to read config from and add services to.</param>
    /// <param name="configuration">The configuration <see cref="Configuration"/></param>
    public static void AddLlamaParseClient(this IHostApplicationBuilder builder, Configuration configuration)
    {
        builder.Services.AddSingleton(p => ConfigureClient(p, configuration));
    }

    /// <summary>
    /// Adds a keyed LlamaParseClient to the service collection
    /// </summary>
    /// <param name="builder">The <see cref="IHostApplicationBuilder" /> to read config from and add services to.</param>
    /// <param name="configuration">The configuration <see cref="Configuration"/></param>
    /// <param name="name">The service keyed name</param>
    public static void AddKeyedLlamaParseClient(this IHostApplicationBuilder builder, string name, Configuration configuration)
    {
        builder.Services.AddKeyedSingleton<LlamaParseClient>(name, (p, _) => ConfigureClient(p, configuration));
    }

    private static async IAsyncEnumerable<Document> CreateDocumentsFromRawResult(
        LlamaParseClient llamaParseClient,
        RawResult rawResult, 
        bool splitByPage,
        ItemType? itemsToExtract,
        Dictionary<string, object> documentMetadata,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var jobId = rawResult.JobId;

        var result = rawResult.Result;

        var documentByPage = new Dictionary<int, RelatedNodeInfo>();

        if (splitByPage)
        {
            foreach (var page in result.GetProperty("pages").EnumerateArray())
            {
                var pageNumber = page.GetProperty("page").GetInt32();
                var pageMetadata = new Dictionary<string, object>(documentMetadata)
                {
                    ["page_number"] = pageNumber
                };

                Document? document = null;
                switch (llamaParseClient.Configuration.ResultType)
                {
                    case ResultType.Markdown:
                        if (page.TryGetProperty("md", out var markdown))
                        {
                            document = new Document(
                                id: Guid.NewGuid().ToString(),
                                text: markdown.GetString(),
                                mimeType: "text/markdown",
                                metadata: pageMetadata);
                        }

                        break;
                    case ResultType.Text:
                        if (page.TryGetProperty("text", out var text))
                        {
                            document = new Document(
                                id: Guid.NewGuid().ToString(),
                                text: text.GetString(),
                                mimeType: "text/plain",
                                metadata: pageMetadata);
                        }

                        break;
                    case ResultType.Json:
                        document = new Document(
                            id: Guid.NewGuid().ToString(),
                            text: page.GetRawText(),
                            mimeType: "application/json",
                            metadata: pageMetadata);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (document is not null)
                {
                    documentByPage[pageNumber] = new RelatedNodeInfo(document.Id, NodeType.Document, pageMetadata);
                    yield return document;
                }
            }
        }
        else
        {
            documentByPage[-1] = new RelatedNodeInfo(jobId, NodeType.Document, documentMetadata);

            if (llamaParseClient.Configuration.ResultType == ResultType.Json)
            {

                yield return new Document(
                    id: jobId,
                    text: result.GetRawText(),
                    mimeType: "application/json",
                    metadata: documentMetadata);
            }

            var mimeType = llamaParseClient.Configuration.ResultType switch
            {
                ResultType.Markdown => "text/markdown",
                ResultType.Text => "text/plain",
                _ => throw new ArgumentOutOfRangeException()
            };

            var content = new StringBuilder();
            foreach (var page in result.GetProperty("pages").EnumerateArray())
            {

                switch (llamaParseClient.Configuration.ResultType)
                {
                    case ResultType.Markdown:
                        if (page.TryGetProperty("md", out var markdown))
                        {
                            content.AppendLine(markdown.GetString());
                        }

                        break;
                    case ResultType.Text:
                        if (page.TryGetProperty("text", out var text))
                        {
                            content.AppendLine(text.GetString());
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            yield return new Document(
                id: jobId,
                text: content.ToString(),
                mimeType: mimeType,
                metadata: documentMetadata);
        }

        var itemTypesToExtract = itemsToExtract ?? llamaParseClient.Configuration.ItemsToExtract;

        var extractImages = (itemTypesToExtract) == ItemType.Image;

        if (extractImages)
        {
            await foreach (var image in llamaParseClient.LoadImagesAsync(rawResult, cancellationToken))
            {
                if (documentByPage.Count > 0)
                {
                    image.ParentNode =
                        documentByPage.TryGetValue((int)image.Metadata["page_number"], out var nodeReference)
                            ? nodeReference
                            : documentByPage[-1];
                }

                yield return image;
            }
        }

        var extractTables = (itemTypesToExtract & ItemType.Table) == ItemType.Table;

        if (extractTables)
        {
            await foreach (var table in llamaParseClient.LoadTablesAsync(rawResult, cancellationToken))
            {
                if (documentByPage.Count > 0)
                {
                    table.ParentNode =
                        documentByPage.TryGetValue((int)table.Metadata["page_number"], out var nodeReference)
                            ? nodeReference
                            : documentByPage[-1];
                }

                yield return table;
            }
        }
    }

    private static LlamaParseClient ConfigureClient(IServiceProvider provider,  Configuration configuration)
    {
        var client = provider.GetRequiredService<HttpClient>();
        var llamaParseClient = new LlamaParseClient(client, configuration);
        return llamaParseClient;
    }
}
