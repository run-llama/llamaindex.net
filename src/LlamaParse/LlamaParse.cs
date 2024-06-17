using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using LlamaIndex.Core.Schema;


namespace LlamaParse;

public partial class LlamaParse(HttpClient client, string apiKey, string? endpoint = null, Configuration? configuration = null)
{
    private const string LlamaParseJobIdKey = "job_id";
    private const string LlamaParseJobMetadataKey = "job_metadata";

    private readonly Configuration _configuration = configuration ?? new Configuration();

    private readonly LlamaParseClient _client = new(client, apiKey, string.IsNullOrWhiteSpace(endpoint)
        ? "https://api.cloud.llamaindex.ai"
        : endpoint!);

    public IAsyncEnumerable<Document> LoadDataAsync(FileInfo file, bool splitByPage = false, Dictionary<string, object>? metadata = null, CancellationToken cancellationToken = default)
    {
        return LoadDataAsync([file], splitByPage, metadata, cancellationToken);
    }

    public async IAsyncEnumerable<Document> LoadDataAsync(IEnumerable<FileInfo> files, bool splitByPage = false, Dictionary<string, object>? metadata = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var documentMetadata = metadata?? new Dictionary<string, object>();
  
        await foreach (var rawResult in LoadDataRawAsync(files, ResultType.Json, documentMetadata, cancellationToken))
        {
            var jobId = rawResult.JobId;

            var result = rawResult.Result;

            if (splitByPage)
            {
                foreach (var page in result.GetProperty("pages").EnumerateArray())
                {
                    switch (_configuration.ResultType)
                    {
                        case ResultType.Markdown:
                            if (page.TryGetProperty("md", out var markdown))
                            {
                                yield return new Document(Guid.NewGuid().ToString(), markdown.GetString(),
                                    documentMetadata);
                            }

                            break;
                        case ResultType.Text:
                            if (page.TryGetProperty("text", out var text))
                            {
                                yield return new Document(Guid.NewGuid().ToString(), text.GetString(),
                                    documentMetadata);
                            }

                            break;
                        case ResultType.Json:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            else
            {
                var content = new StringBuilder();
                foreach(var page in result.GetProperty("pages").EnumerateArray())
                {
                    switch (_configuration.ResultType)
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
                        case ResultType.Json:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                yield return new Document(jobId, content.ToString(), documentMetadata);

            }

            if (_configuration.ExtractImages)
            {
                await foreach (var image in LoadImagesAsync(jobId, documentMetadata,  cancellationToken))
                {
                    yield return image;
                }
            }
        }
        
    }

    public IAsyncEnumerable<RawResult> LoadDataRawAsync(FileInfo file, ResultType? resultType = null, Dictionary<string, object>? metadata = null, CancellationToken cancellationToken = default)
    {
        return LoadDataRawAsync([file], resultType, metadata, cancellationToken);
    }

    public async IAsyncEnumerable<RawResult> LoadDataRawAsync(IEnumerable<FileInfo> files,
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

            var documentMetadata = metadata ?? new Dictionary<string, object>();

            var job = await CreateJobAsync(fileInfo, documentMetadata, cancellationToken);
            jobs.Add(job);
        }

        if (cancellationToken.IsCancellationRequested) yield break;

        foreach (var job in jobs)
        {
            var result = await job.GetRawResult(resultType?? _configuration.ResultType, cancellationToken);
            yield return result;
        }
    }

    public IAsyncEnumerable<ImageDocument> LoadImagesAsync(Document document, CancellationToken cancellationToken = default)
    {
        var jobId = document.Metadata[LlamaParseJobIdKey];
        
        return LoadImagesAsync(jobId.ToString(), document.Metadata, cancellationToken);
    }

    public async IAsyncEnumerable<ImageDocument> LoadImagesAsync(string jobId, Dictionary<string, object>? documentMetadata = null ,[EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var metadata = documentMetadata?.ToDictionary(e => e.Key, e => e.Value) ?? new Dictionary<string, object>();

        var job = new Job(_client, metadata, jobId, ResultType.Json);

        await foreach (var image in job.GetImagesAsync(cancellationToken))
        {
            yield return image;
        }
    }


    private async Task<Job> CreateJobAsync(FileInfo fileInfo, Dictionary<string, object> metadata, CancellationToken cancellationToken)
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
        var documentMetadata = metadata;
        documentMetadata["file_path"] = fileInfoName;


        var id = await _client.CreateJob(fileInfo, _configuration, cancellationToken);

        var job = new Job(_client, documentMetadata, id, _configuration.ResultType);

        return job;
    }
}