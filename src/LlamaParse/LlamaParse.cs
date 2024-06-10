using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using LlamaIndex.CoreSchema;

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
        if (!cancellationToken.IsCancellationRequested)
        {
            foreach (var job in jobs)
            {
                yield return await job.GetResultAsync(cancellationToken);
            }
        }
    }

    private Task<Job> CreateJobAsync(FileInfo fileInfo, Dictionary<string, object>? metadata, CancellationToken cancellationToken)
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

        var requestData = new
        {
            language = languageCode,
            parsing_instruction = _configuration.ParsingInstructions,
            skip_diagonal_text = _configuration.SkipDiagonalText,
            do_not_cache = _configuration.DoNotCache,
            fast_mode = _configuration.FastMode,
            do_not_unroll_columns = _configuration.DoNotUnrollColumns,
            page_separator = _configuration.PageSeparator,
            gpt4o_mode = _configuration.Gpt4oMode,
            gpt4o_api_key = _configuration.Gpt4oApiKey,
        };

        throw new NotImplementedException();
    }
}