using System;
using System.IO;
using System.IO.Pipes;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LlamaParse;

internal class LlamaParseApiClient(HttpClient client, string apiKey, string endpoint)
{
  
    public async Task<byte[]> GetImage(string jobId, string imageName, CancellationToken cancellationToken)
    {
        var getImageUri = new Uri($"{endpoint.TrimEnd('/')}/api/parsing/job/{jobId}/result/image/{imageName}");
        var request = new HttpRequestMessage(HttpMethod.Get, getImageUri);
        request.Headers.Add("Authorization", $"Bearer {apiKey}");
        var response = await client.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsByteArrayAsync();
        return content!;
    }
    public async Task<JobStatus> GetJobStatusAsync(string jobId, CancellationToken cancellationToken)
    {
        var getStatusUri = new Uri($"{endpoint.TrimEnd('/')}/api/parsing/job/{jobId}");
        var request = new HttpRequestMessage(HttpMethod.Get, getStatusUri);
        request.Headers.Add("Authorization", $"Bearer {apiKey}");
        var response = await client.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        var statusString = JsonDocument.Parse(responseContent).RootElement.GetProperty("status").GetString();
        return (JobStatus)Enum.Parse(typeof(JobStatus), statusString!, true);

    }
    public async Task<RawResult> GetJobResultAsync(string jobId, ResultType resultType, CancellationToken cancellationToken)
    {
        var getResultUri = new Uri($"{endpoint.TrimEnd('/')}/api/parsing/job/{jobId}/result/{resultType.ToString().ToLowerInvariant()}");
        var request = new HttpRequestMessage(HttpMethod.Get, getResultUri);
        request.Headers.Add("Authorization", $"Bearer {apiKey}");
        var response = await client.SendAsync(request, cancellationToken);

        response.EnsureSuccessStatusCode();
        var resultString = await response.Content.ReadAsStringAsync();

        var jsonElement = JsonDocument.Parse(resultString).RootElement;
        var jobMetaData = jsonElement.GetProperty(Constants.JobMetadataKey);
        return new RawResult(
            jobId,
            jsonElement,
            null,
            jobMetaData.GetProperty(Constants.CreditsUsedKey).GetDouble(),
            jobMetaData.GetProperty(Constants.CreditsMaxKey).GetDouble(),
            jobMetaData.GetProperty(Constants.JobCreditsUsageKey).GetDouble(),
            jobMetaData.GetProperty(Constants.JobPagesKey).GetDouble(),
            jobMetaData.GetProperty(Constants.JobIsCacheHitKey).GetBoolean());

    }

    public async Task<string> CreateJob(Stream fileStream, string fileName , string mimeType, Configuration configuration, CancellationToken cancellationToken)
    {
        // upload file and create a job
        var uploadUri = new Uri($"{endpoint.TrimEnd('/')}/api/parsing/upload");

        var form = new MultipartFormDataContent();

        //  Set up the file content
        var fileContent = new StreamContent(fileStream);
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(mimeType);

        fileContent.Headers.ContentDisposition = ContentDispositionHeaderValue.Parse(
            $"form-data; name=\"file\"; filename=\"{fileName}\"");

        form.Add(fileContent);

        // Add additional configuration to form data
        form.Add(new StringContent(configuration.Language.ToLanguageCode()), "language");

        if (!string.IsNullOrWhiteSpace(configuration.ParsingInstructions))
        {
            form.Add(new StringContent(configuration.ParsingInstructions), "parsing_instruction");
        }

        form.Add(new StringContent(configuration.InvalidateCache.ToString()), "invalidate_cache");
        form.Add(new StringContent(configuration.SkipDiagonalText.ToString()), "skip_diagonal_text");
        form.Add(new StringContent(configuration.DoNotCache.ToString()), "do_not_cache");
        form.Add(new StringContent(configuration.FastMode.ToString()), "fast_mode");
        form.Add(new StringContent(configuration.DoNotUnrollColumns.ToString()), "do_not_unroll_columns");

        if (!string.IsNullOrWhiteSpace(configuration.ParsingInstructions))
        {
            form.Add(new StringContent(configuration.PageSeparator), "page_separator");
        }

        form.Add(new StringContent(configuration.Gpt4oMode.ToString()), "gpt4o_mode");

        if (configuration.Gpt4oMode)
        {
            form.Add(new StringContent(configuration.Gpt4oApiKey), "gpt4o_api_key");
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
                throw new InvalidOperationException($"Failed to upload file: {fileName}. Error: {error}");
            }

            throw new InvalidOperationException($"Failed to upload file: {fileName}");
        }

        var responseBody = await response.Content.ReadAsStringAsync();

        var jobCreationResult = JsonDocument.Parse(responseBody).RootElement;

        var id = jobCreationResult.GetProperty("id").GetString();
        return id!;
    }

    public Task<string> CreateJob(FileInfo fileInfo, Configuration configuration, CancellationToken cancellationToken)
    {
        var mimeType = FileTypes.GetMimeType(fileInfo);
    
        var fileStream = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read);

        return CreateJob(fileStream, fileInfo.Name, mimeType, configuration, cancellationToken);
    }
}