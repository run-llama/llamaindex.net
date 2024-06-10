﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using LlamaIndex.Core.Schema;

namespace LlamaParse;

public partial class LlamaParse(HttpClient client, string apiKey, string? endpoint = null, Configuration? configuration = null)
{
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
            yield return await job.GetResultAsync(cancellationToken);
        }

    }

    public async IAsyncEnumerable<ImageDocument> LoadImagesAsync(Document document, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var jobId = document.Metadata["llamaparse_job_id"];

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
        var mimeType = FileTypes.GetMimeType(fileInfo);
        var uploadUri = new Uri($"{_endpoint.TrimEnd('/')}/api/parsing/upload");
        var request = new HttpRequestMessage(HttpMethod.Post, uploadUri);
        request.Headers.Add("Authorization", $"Bearer {apiKey}");
        var bytes = await File.ReadAllBytesAsync(fileInfo.FullName, cancellationToken);

        if (bytes.LongLength == 0)
        {
            throw new InvalidOperationException($"Failed to read file: {fileInfo.FullName}, file is empty");
        }

        var fileContent = new ByteArrayContent(bytes)
        {
            Headers = { { "Content-Type", mimeType } }
        };

        var requestContent  = new MultipartFormDataContent
        {
            { fileContent, "file", fileInfoName },
            { new StringContent(languageCode), "language" },
            { new StringContent(_configuration.SkipDiagonalText.ToString()), "skip_diagonal_text" },
            { new StringContent(_configuration.DoNotCache.ToString()), "do_not_cache" },
            { new StringContent(_configuration.FastMode.ToString()), "fast_mode" },
            { new StringContent(_configuration.DoNotUnrollColumns.ToString()), "do_not_unroll_columns" },
        };

        if (_configuration.Gpt4oMode)
        {
            requestContent.Add(new StringContent(_configuration.Gpt4oMode.ToString()), "gpt4o_mode");
            requestContent.Add(new StringContent(_configuration.Gpt4oApiKey ?? string.Empty), "gpt4o_api_key");
        }

        if (!string.IsNullOrWhiteSpace(_configuration.ParsingInstructions))
        {
            requestContent.Add(new StringContent(_configuration.ParsingInstructions ?? string.Empty), "parsing_instruction");
        }

        if (!string.IsNullOrWhiteSpace(_configuration.PageSeparator))
        {
            requestContent.Add(new StringContent(_configuration.PageSeparator ?? string.Empty), "page_separator");
        }

        request.Content = requestContent;
        var response = await client.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"Failed to upload file: {fileInfo.FullName}");
        }

        var jobId = await response.Content.ReadAsStringAsync();

        throw new NotImplementedException();
    }
}