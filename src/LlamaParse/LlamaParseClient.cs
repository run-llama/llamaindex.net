using LlamaIndex.Core.Schema;

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace LlamaParse;

public partial class LlamaParseClient(HttpClient client, string apiKey, string? endpoint = null, Configuration? configuration = null)
{
    public Configuration Configuration { get; } = configuration ?? new Configuration();

    private readonly LlamaParseApiClient _client = new(client, apiKey, string.IsNullOrWhiteSpace(endpoint)
        ? "https://api.cloud.llamaindex.ai"
        : endpoint!);


    public IAsyncEnumerable<RawResult> LoadDataRawAsync(FileInfo fileInfo, ResultType? resultType = null, Dictionary<string, object>? metadata = null, CancellationToken cancellationToken = default)
    {
        if (!fileInfo.Exists)
        {
            throw new FileNotFoundException($"File {fileInfo.FullName} not found");
        }
        var inMemoryFile =
            new InMemoryFile(File.ReadAllBytes(fileInfo.FullName), fileInfo.Name, FileTypes.GetMimeType(fileInfo.Name));
        return LoadDataRawAsync(inMemoryFile, resultType, metadata, cancellationToken);
    }

    public IAsyncEnumerable<RawResult> LoadDataRawAsync(InMemoryFile inMemoryFile, ResultType? resultType = null, Dictionary<string, object>? metadata = null, CancellationToken cancellationToken = default)
    {

        return LoadDataRawAsync([inMemoryFile], resultType, metadata, cancellationToken);
    }

    public async IAsyncEnumerable<RawResult> LoadDataRawAsync(
        IEnumerable<InMemoryFile> files,
        ResultType? resultType = null,
        Dictionary<string, object>? metadata = null,
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

            var job = await CreateJobAsync(file, documentMetadata, cancellationToken);
            jobs.Add(job);
        }

        if (cancellationToken.IsCancellationRequested) yield break;

        foreach (var job in jobs)
        {
            var result = await job.GetRawResult(resultType ?? Configuration.ResultType, cancellationToken);
            yield return result;
        }

    }
    public async IAsyncEnumerable<RawResult> LoadDataRawAsync(
        IEnumerable<FileInfo> files,
        ResultType? resultType = null,
        Dictionary<string, object>? metadata = null,
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

            var job = await CreateJobAsync(inMemoryFile, documentMetadata, cancellationToken);
            jobs.Add(job);
        }

        if (cancellationToken.IsCancellationRequested) yield break;

        foreach (var job in jobs)
        {
            var result = await job.GetRawResult(resultType ?? Configuration.ResultType, cancellationToken);
            yield return result;
        }
    }

    public IAsyncEnumerable<ImageDocument> LoadImagesAsync(Document document, CancellationToken cancellationToken = default)
    {
        var jobId = document.Metadata[Constants.JobIdKey];

        return LoadImagesAsync(jobId.ToString(), document.Metadata, cancellationToken);
    }

    public IAsyncEnumerable<ImageDocument> LoadImagesAsync(RawResult rawResult, CancellationToken cancellationToken = default)
    {
        var jobId = rawResult.JobId;

        return LoadImagesAsync(jobId, rawResult.Metadata, cancellationToken);
    }

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

    private Task<Job> CreateJobAsync(FileInfo fileInfo, Dictionary<string, object> metadata, CancellationToken cancellationToken)
    {
        if (!FileTypes.IsSupported(fileInfo))
        {
            throw new InvalidOperationException($"Unsupported file type: {fileInfo.Name}");
        }

        if (!fileInfo.Exists)
        {
            throw new FileNotFoundException($"File not found: {fileInfo.FullName}");
        }

        var data = File.ReadAllBytes(fileInfo.FullName);
        var mimeType = FileTypes.GetMimeType(fileInfo.Name);
        var inMemoryFile = new InMemoryFile(data, fileInfo.Name, mimeType);
        return CreateJobAsync(inMemoryFile, metadata, cancellationToken);
    }

    private async Task<Job> CreateJobAsync(InMemoryFile file, Dictionary<string, object> metadata, CancellationToken cancellationToken)
    {
        if (!FileTypes.IsSupported(file.FileName))
        {
            throw new InvalidOperationException($"Unsupported file type: {file.FileName}");
        }

        // clone metadata
        var documentMetadata = metadata;
        documentMetadata["file_path"] = file.FileName;

        using var activity = LlamaDiagnostics.StartCreateJob(file.FileName);

        var id = await _client.CreateJob(file.FileData, file.FileName, file.MimeType, Configuration, cancellationToken);
        LlamaDiagnostics.EndCreateJob(activity, "succeeded", id);
        return CreateJob(id, metadata, Configuration.ResultType);
    }
}