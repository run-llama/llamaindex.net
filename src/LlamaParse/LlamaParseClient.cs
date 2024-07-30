using LlamaIndex.Core.Schema;

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LlamaParse;

/// <summary>
/// The LlamaParseClient class provides methods for parsing data from files using the LlamaParse service.
/// </summary>
public partial class LlamaParseClient
{ 
    internal Configuration Configuration { get; }

    private readonly LlamaParseApiClient _client;

    /// <summary>
    /// The LlamaParseClient constructor.
    /// </summary>
    /// <param name="client">The <see cref="HttpClient"/> used to make requests to the LlamaParse service.</param>
    /// <param name="configuration">The LlamaParse <see cref="Configuration"/></param>
    /// <exception cref="ArgumentException"></exception>
    public LlamaParseClient(HttpClient client, Configuration configuration)
    {
        if (string.IsNullOrWhiteSpace(configuration.ApiKey))
        {
            throw new ArgumentException("API key is required", nameof(configuration.ApiKey));
        }

        Configuration = JsonSerializer.Deserialize<Configuration>(
            JsonSerializer.Serialize( configuration ))!;
        _client = new(client, configuration.ApiKey, string.IsNullOrWhiteSpace(configuration.Endpoint)
            ? "https://api.cloud.llamaindex.ai"
            : configuration.Endpoint!);
    }


    /// <summary>
    /// Loads data from a file asynchronously and returns the raw results.
    /// </summary>
    /// <param name="fileInfo">The file to load.</param>
    /// <param name="resultType">The type of result to retrieve. (Optional) <see cref="ResultType"/></param>
    /// <param name="metadata">Additional metadata for the document. (Optional)</param>
    /// <param name="language">Language (Optional)</param>
    /// <param name="cancellationToken">The cancellation token. (Optional)</param>
    /// <returns>An asynchronous enumerable of RawResult objects representing the loaded data.</returns>
    public IAsyncEnumerable<RawResult> LoadDataRawAsync(FileInfo fileInfo, ResultType? resultType = null, Dictionary<string, object>? metadata = null, Languages? language = null, CancellationToken cancellationToken = default)
    {
        if (!fileInfo.Exists)
        {
            throw new FileNotFoundException($"File {fileInfo.FullName} not found");
        }
        var inMemoryFile =
            new InMemoryFile(File.ReadAllBytes(fileInfo.FullName), fileInfo.Name, FileTypes.GetMimeType(fileInfo.Name));
        return LoadDataRawAsync(inMemoryFile, resultType, metadata, language, cancellationToken);
    }

    /// <summary>
    /// Loads data from a file asynchronously and returns the raw results.
    /// </summary>
    /// <param name="inMemoryFile">The in-memory file to load.</param>
    /// <param name="resultType">The type of result to retrieve. (Optional) <see cref="ResultType"/></param>
    /// <param name="metadata">Additional metadata for the document. (Optional)</param>
    /// <param name="language">Language (Optional)</param>
    /// <param name="cancellationToken">The cancellation token. (Optional)</param>
    /// <returns>An asynchronous enumerable of RawResult objects representing the loaded data.</returns>
    public IAsyncEnumerable<RawResult> LoadDataRawAsync(InMemoryFile inMemoryFile, ResultType? resultType = null, Dictionary<string, object>? metadata = null, Languages? language = null, CancellationToken cancellationToken = default)
    {
        return LoadDataRawAsync([inMemoryFile], resultType, metadata, language, cancellationToken);
    }


    /// <summary>
    /// Loads data from a file asynchronously and returns the raw results.
    /// </summary>
    /// <param name="files">The collection of files to load.</param>
    /// <param name="resultType">The type of result to retrieve. (Optional) <see cref="ResultType"/></param>
    /// <param name="metadata">Additional metadata for the document. (Optional)</param>
    /// <param name="language">Language (Optional)</param>
    /// <param name="cancellationToken">The cancellation token. (Optional)</param>
    /// <returns>An asynchronous enumerable of RawResult objects representing the loaded data.</returns>
    public async IAsyncEnumerable<RawResult> LoadDataRawAsync(
        IEnumerable<InMemoryFile> files,
        ResultType? resultType = null,
        Dictionary<string, object>? metadata = null,
        Languages? language = null, 
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var jobs = new List<Job>();
        foreach (var file in files)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                yield break;
            }

            var documentMetadata = metadata ?? new Dictionary<string, object>();

            var job = await CreateJobAsync(file, documentMetadata, language, cancellationToken);
            jobs.Add(job);
        }

        if (cancellationToken.IsCancellationRequested) yield break;

        foreach (var job in jobs)
        {
            var result = await job.GetRawResult(resultType ?? Configuration.ResultType, cancellationToken);
            yield return result;
        }
    }
    /// <summary>
    /// Loads data from a file asynchronously and returns the raw results.
    /// </summary>
    /// <param name="files">The collection of files to load.</param>
    /// <param name="resultType">The type of result to retrieve. (Optional) <see cref="ResultType"/></param>
    /// <param name="metadata">Additional metadata for the document. (Optional)</param>
    /// <param name="language">Language (Optional)</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> (Optional)</param>
    /// <returns>An asynchronous enumerable of RawResult objects representing the loaded data.</returns>
    public async IAsyncEnumerable<RawResult> LoadDataRawAsync(
        IEnumerable<FileInfo> files,
        ResultType? resultType = null,
        Dictionary<string, object>? metadata = null,
        Languages? language = null, 
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var jobs = new List<Job>();
        foreach (var fileInfo in files)
        {
            if (!fileInfo.Exists)
            {
                throw new FileNotFoundException($"File {fileInfo.FullName} not found");
            }

            if (cancellationToken.IsCancellationRequested)
            {
                yield break;
            }

            var documentMetadata = metadata ?? new Dictionary<string, object>();

            var inMemoryFile =
                new InMemoryFile(File.ReadAllBytes(fileInfo.FullName), fileInfo.Name, FileTypes.GetMimeType(fileInfo.Name));

            var job = await CreateJobAsync(inMemoryFile, documentMetadata, language, cancellationToken);
            jobs.Add(job);
        }

        if (cancellationToken.IsCancellationRequested) yield break;

        foreach (var job in jobs)
        {
            var result = await job.GetRawResult(resultType ?? Configuration.ResultType, cancellationToken);
            yield return result;
        }
    }

    /// <summary>
    /// Loads images from a document asynchronously.
    /// </summary>
    /// <param name="document">The document containing the image metadata. <see cref="Document"/></param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
    /// <returns>An asynchronous enumerable of ImageDocument objects representing the loaded images.</returns>
    public async IAsyncEnumerable<ImageDocument> LoadImagesAsync(Document document, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var jobId = document.Metadata[Constants.JobIdKey];

        await foreach (var image in LoadImagesAsync(jobId.ToString(), document.Metadata, cancellationToken))
        {
            image.SourceNode = new RelatedNodeInfo(document.Id, NodeType.Document);
            yield return image;
        }
    }

    /// <summary>
    /// Loads images from a document asynchronously.
    /// </summary>
    /// <param name="rawResult">The <see cref="RawResult"/> from a parsing job.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
    /// <returns>An asynchronous enumerable of ImageDocument objects representing the loaded images.</returns>
    public IAsyncEnumerable<ImageDocument> LoadImagesAsync(RawResult rawResult, CancellationToken cancellationToken = default)
    {
        var jobId = rawResult.JobId;

        return LoadImagesAsync(jobId, rawResult.Metadata, cancellationToken);
    }

    /// <summary>
    /// Loads images from a document asynchronously.
    /// </summary>
    /// <param name="jobId">The parse job ID</param>
    /// <param name="documentMetadata">Additional document metadata.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/></param>
    /// <returns></returns>
    public async IAsyncEnumerable<ImageDocument> LoadImagesAsync(string jobId, Dictionary<string, object>? documentMetadata = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var metadata = documentMetadata ?? new Dictionary<string, object>();

        var job = CreateJob(jobId, metadata, ResultType.Json);

        await foreach (var image in job.GetImagesAsync(cancellationToken))
        {
            yield return image;
        }
    }

    private Job CreateJob(string jobId, Dictionary<string, object> metadata, ResultType? resultType = null)
    {
        var documentMetadata = metadata;
        var job = new Job(_client, documentMetadata, jobId, resultType ?? Configuration.ResultType);
        return job;
    }

    private async Task<Job> CreateJobAsync(InMemoryFile file, Dictionary<string, object> metadata, Languages? language, CancellationToken cancellationToken)
    {
        if (!FileTypes.IsSupported(file.FileName))
        {
            throw new InvalidOperationException($"Unsupported file type: {file.FileName}");
        }

        // clone metadata
        var documentMetadata = metadata;
        documentMetadata["file_path"] = file.FileName;

        using var activity = LlamaDiagnostics.StartCreateJob(file.FileName);

        var id = await _client.CreateJobAsync(file.FileData, file.FileName, file.MimeType, Configuration, language, cancellationToken);
        LlamaDiagnostics.EndCreateJob(activity, "succeeded", id);
        return CreateJob(id, metadata, Configuration.ResultType);
    }

    /// <summary>
    /// Loads tables from a raw result asynchronously.
    /// </summary>
    /// <param name="rawResult">The raw result containing the tables. <see cref="RawResult"/></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An asynchronous enumerable of Document objects representing the tables.</returns>
    public async IAsyncEnumerable<Document> LoadTablesAsync(RawResult rawResult, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await Task.Yield();
        var metadata = rawResult.Metadata ?? new Dictionary<string, object>();

        foreach (var page in rawResult.Result.GetProperty("pages").EnumerateArray())
        {
            var tableIndex = 1;
            foreach (var item in page.GetProperty("items").EnumerateArray())
            {
                var type = item.GetProperty("type").GetString();
                if (type! == "table")
                {
                    var rows = item.GetProperty("rows");
                    var tableMetadata = new Dictionary<string, object>(metadata)
                    {
                        ["page_number"] = page.GetProperty("page").GetInt32(),
                        ["table_index"] = tableIndex++,
                        ["table_rows"] = rows.GetArrayLength(),
                        ["table_format"] = "list of arrays"
                    };
                    var tableDocument = new Document(
                        id: Guid.NewGuid().ToString(),
                        text: rows.GetRawText(),
                        mimeType: "application/json",
                        metadata: tableMetadata
                    );

                    yield return tableDocument;
                }
            }
        }
    }
}
