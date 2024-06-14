using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using LlamaIndex.Core.Schema;

namespace LlamaParse;

public partial class LlamaParse(HttpClient client, string apiKey, string? endpoint = null, Configuration? configuration = null)
{
    private const string LlamaParseJobIdMetadataKey = "job_id";

    private readonly string _endpoint = string.IsNullOrWhiteSpace(endpoint)
            ? "https://api.cloud.llamaindex.ai"
            : endpoint;
    private readonly Configuration _configuration = configuration ?? new Configuration();

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

    public async IAsyncEnumerable<ImageDocument> LoadImagesAsync(Document document, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var jobId = document.Metadata[LlamaParseJobIdMetadataKey];

        var job = new Job(client, document.Metadata, jobId.ToString(),  _endpoint, ResultType.Json, apiKey);

        await foreach (var image in job.GetImagesAsync(cancellationToken))
        {
            yield return image;
        }
    }

    private Task<object> GetJobResults(object jobId, ResultType resulType, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
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


        // upload file and create a job
        var uploadUri = new Uri($"{_endpoint.TrimEnd('/')}/api/parsing/upload");

        var mimeType = FileTypes.GetMimeType(fileInfo);
        var form = new MultipartFormDataContent();
        var fileStream = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read);

        //  Set up the file content
        var fileContent = new StreamContent(fileStream);
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(mimeType);

        fileContent.Headers.ContentDisposition = ContentDispositionHeaderValue.Parse(
            $"form-data; name=\"file\"; filename=\"{fileInfo.Name}\"");

        form.Add(fileContent);

        // Add additional configuration to form data
        form.Add(new StringContent(_configuration.Language.ToLanguageCode()), "language");

        if (!string.IsNullOrWhiteSpace(_configuration.ParsingInstructions))
        {
            form.Add(new StringContent(_configuration.ParsingInstructions), "parsing_instruction");
        }

        form.Add(new StringContent(_configuration.InvalidateCache.ToString()), "invalidate_cache");
        form.Add(new StringContent(_configuration.SkipDiagonalText.ToString()), "skip_diagonal_text");
        form.Add(new StringContent(_configuration.DoNotCache.ToString()), "do_not_cache");
        form.Add(new StringContent(_configuration.FastMode.ToString()), "fast_mode");
        form.Add(new StringContent(_configuration.DoNotUnrollColumns.ToString()), "do_not_unroll_columns");

        if (!string.IsNullOrWhiteSpace(_configuration.ParsingInstructions))
        {
            form.Add(new StringContent(_configuration.PageSeparator), "page_separator");
        }

        form.Add(new StringContent(_configuration.Gpt4oMode.ToString()), "gpt4o_mode");

        if (_configuration.Gpt4oMode)
        {
            form.Add(new StringContent(_configuration.Gpt4oApiKey), "gpt4o_api_key");
        }

        var request = new HttpRequestMessage(HttpMethod.Post, uploadUri);
        request.Content = form;
        
        request.Headers.Add("Authorization", $"Bearer {apiKey}");

        var response = await client.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            if (response.Content != null)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Failed to upload file: {fileInfo.FullName}. Error: {error}");
            }

            throw new InvalidOperationException($"Failed to upload file: {fileInfo.FullName}");
        }

        var responseBody =  await response.Content.ReadAsStringAsync();

        var jobCreationResult = JsonDocument.Parse(responseBody).RootElement;

        var id = jobCreationResult.GetProperty("id").GetString();

        var job = new Job(client, documentMetadata, id!, _endpoint, _configuration.ResultType, apiKey);

        return job;
    }
}