using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using LlamaIndex.Core.Schema;


namespace LlamaParse;

public partial class LlamaParse(HttpClient client, string apiKey, string? endpoint = null, Configuration? configuration = null)
{
    private const string LlamaParseJobIdMetadataKey = "job_id";

    private readonly Configuration _configuration = configuration ?? new Configuration();

    private readonly LlamaParseClient _client = new(client, apiKey, string.IsNullOrWhiteSpace(endpoint)
        ? "https://api.cloud.llamaindex.ai"
        : endpoint);

    public IAsyncEnumerable<Document> LoadDataAsync(FileInfo file, Dictionary<string, object>? metadata = null, CancellationToken cancellationToken = default)
    {
        return LoadDataAsync([file], metadata, cancellationToken);
    }

    public async IAsyncEnumerable<Document> LoadDataAsync(IEnumerable<FileInfo> files, Dictionary<string, object>? metadata = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var jobs = new List<Job>();
        foreach (var fileInfo in files)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                yield break;
            }
            var job = await CreateJobAsync(fileInfo, metadata, cancellationToken);
            jobs.Add(job);
        }

        if (cancellationToken.IsCancellationRequested) yield break;

        foreach (var job in jobs)
        {
            var document = await job.GetDocumentAsync(cancellationToken);
            if (_configuration.SplitByPage)
            {
                var chunks = document.Text?.Split("\n---\n") ??
                             [];
                var pageCount = 0;
                foreach (var chunk in chunks)
                {
                    pageCount++;
                    if (cancellationToken.IsCancellationRequested)
                    {
                        yield break;
                    }
                    if (string.IsNullOrWhiteSpace(chunk))
                    {
                        continue;
                    }
                    yield return new Document(document.Id, chunk, new Dictionary<string, object>(document.Metadata)
                    {
                        ["page_number"] = pageCount
                    });
                }
            }
            else
            {
                yield return document;
            }

            if (_configuration.ExtractImages)
            {
                await foreach (var image in LoadImagesAsync(document, cancellationToken))
                {
                    yield return image;
                }
            }
        }
    }

    public IAsyncEnumerable<JsonElement> LoadDataRawAsync(FileInfo file, ResultType? resultType = null, Dictionary<string, object>? metadata = null, CancellationToken cancellationToken = default)
    {
        return LoadDataRawAsync([file], resultType, metadata, cancellationToken);
    }

    public async IAsyncEnumerable<JsonElement> LoadDataRawAsync(IEnumerable<FileInfo> files,
        ResultType? resultType = null,
        Dictionary<string, object>? metadata = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var jobs = new List<Job>();
        foreach (var fileInfo in files)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                yield break;
            }

            var job = await CreateJobAsync(fileInfo, metadata, cancellationToken);
            jobs.Add(job);
        }

        if (cancellationToken.IsCancellationRequested) yield break;

        foreach (var job in jobs)
        {
            var result = await job.GetRawResult(resultType?? _configuration.ResultType, cancellationToken);
            yield return result;
        }
    }

    public async IAsyncEnumerable<ImageDocument> LoadImagesAsync(Document document, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var jobId = document.Metadata[LlamaParseJobIdMetadataKey];

        var job = new Job(_client, document.Metadata, jobId.ToString(), ResultType.Json);

        await foreach (var image in job.GetImagesAsync(cancellationToken))
        {
            yield return image;
        }
    }


    private async Task<Job> CreateJobAsync(FileInfo fileInfo, Dictionary<string, object>? metadata, CancellationToken cancellationToken)
    {
        var fileInfoName = fileInfo.Name;

        if (!FileTypes.IsSupported(fileInfo))
        {
            throw new InvalidOperationException($"Unsupported file type: {fileInfo.Name}");
        }

        if (!fileInfo.Exists)
        {
            throw new FileNotFoundException($"File not found: {fileInfo.FullName}");
        }

        // clone metadata
        var documentMetadata = metadata?.ToDictionary(e => e.Key, e => e.Value) ?? new Dictionary<string, object>();
        documentMetadata["file_path"] = fileInfoName;


        var id = await _client.CreateJob(fileInfo, _configuration, cancellationToken);

        var job = new Job(_client, documentMetadata, id, _configuration.ResultType);

        return job;
    }
}