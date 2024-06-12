using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using LlamaIndex.Core.Schema;

namespace LlamaParse;

public partial class LlamaParse(HttpClient client, string apiKey, string? endpoint = null, Configuration? configuration = null)
{
    private const string LlamaparseJobId = "llamaparse_job_id";

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
            var document = await job.GetResultAsync(cancellationToken);
            if (_configuration.SplitByPage)
            {
                var chunks = document.Text?.Split("\n---\n", StringSplitOptions.RemoveEmptyEntries) ??
                             [];

               foreach (var chunk in chunks)
                {
                    yield return new Document(document.Id, chunk, new Dictionary<string, object>( document.Metadata));
                }
            }
            else
            {
                yield return document;
            }
        }

    }

    public async IAsyncEnumerable<ImageDocument> LoadImagesAsync(Document document, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var jobId = document.Metadata[LlamaparseJobId];

        var jobResult = await GetJobResults(jobId, cancellationToken);


        throw new NotImplementedException();
        yield return null;
    }

    private Task<object> GetJobResults(object jobId, CancellationToken cancellationToken)
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
        var languageCode = _configuration.Language.ToLanguageCode();

        var uploadUri = new Uri($"{_endpoint.TrimEnd('/')}/api/parsing/upload");

        var mimeType = FileTypes.GetMimeType(fileInfo);
        var fileContent = new StreamContent(File.OpenRead(fileInfo.FullName));

        fileContent.Headers.ContentType = new MediaTypeHeaderValue(mimeType);

        var formData = new MultipartFormDataContent
        {
            { fileContent, "file", fileInfoName },
            { new StringContent(languageCode), "language" },
            { new StringContent(_configuration.SkipDiagonalText.ToString()), "skip_diagonal_text" },
            { new StringContent(_configuration.DoNotCache.ToString()), "do_not_cache" },
            { new StringContent(_configuration.FastMode.ToString()), "fast_mode" },
            { new StringContent(_configuration.DoNotUnrollColumns.ToString()), "do_not_unroll_columns" },
            { new StringContent(_configuration.ParsingInstructions ?? string.Empty), "parsing_instruction" },
            { new StringContent(_configuration.PageSeparator ?? string.Empty), "page_separator"},
            { new StringContent(_configuration.Gpt4oMode.ToString()), "gpt4o_mode"}
        };

        if (_configuration.Gpt4oMode)
        {
            formData.Add(new StringContent(_configuration.Gpt4oApiKey ?? string.Empty), "gpt4o_api_key");
        }

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var response = await client.PostAsync(uploadUri, content: formData, cancellationToken);


        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"Failed to upload file: {fileInfo.FullName}");
        }

        var jobId = await response.Content.ReadAsStringAsync();

        throw new NotImplementedException();
    }
}