using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using LlamaIndex.Core.Schema;


namespace LlamaParse;

public partial class LlamaParse
{
    private enum JobStatus
    {
        Pending = 0,
        Success,
        Cancelled,
        Error
    }

    private class Job(
        HttpClient client,
        Dictionary<string, object> metadata,
        string id,
        string endpoint,
        ResultType resultType,
        string apiKey)
    {

        private readonly Dictionary<string, object> _metadata = new(metadata)
        {
            [LlamaParseJobIdMetadataKey] = id
        };

        private async Task<JsonElement> GetJobResultAsync(CancellationToken cancellationToken)
        {
            var getResultUri = new Uri($"{endpoint.TrimEnd('/')}/api/parsing/job/{id}/result/{resultType.ToString().ToLowerInvariant()}");
            var request = new HttpRequestMessage(HttpMethod.Get, getResultUri);
            request.Headers.Add("Authorization", $"Bearer {apiKey}");
            var response = await client.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();
            var resultString = await response.Content.ReadAsStringAsync();
            return JsonDocument.Parse(resultString).RootElement;
        }

        private async Task<JobStatus> GetJobStatusAsync(CancellationToken cancellationToken)
        {
            var getStatusUri = new Uri($"{endpoint.TrimEnd('/')}/api/parsing/job/{id}");
            var request = new HttpRequestMessage(HttpMethod.Get, getStatusUri);
            request.Headers.Add("Authorization", $"Bearer {apiKey}");
            var response = await client.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();
            var statusString = await response.Content.ReadAsStringAsync();
            var status1 = JsonDocument.Parse(statusString).RootElement.GetProperty("status").GetString();
            return Enum.Parse<JobStatus>(status1!, true);

        }

        public async Task<Document> GetDocumentAsync(CancellationToken cancellationToken)
        {
            await WaitForJobToCompleteAsync(cancellationToken);

            var results = await GetJobResultAsync(cancellationToken);
            switch (resultType)
            {
                case ResultType.Markdown:
                    return CreateDocumentFromMarkdownResults(results);
                case ResultType.Text:
                    return CreateDocumentFromTextResults(results);
                case ResultType.Json:
                    return CreateDocumentFromJsonResults(results);
                default:
                    throw new ArgumentOutOfRangeException(nameof(resultType), resultType, null);
            }

        }

        private Document CreateDocumentFromJsonResults(JsonElement results)
        {
            throw new NotImplementedException();
        }

        private Document CreateDocumentFromTextResults(JsonElement results)
        {
            throw new NotImplementedException();
        }

        private Document CreateDocumentFromMarkdownResults(JsonElement results)
        {
            var resultKey = resultType.ToString().ToLowerInvariant();
            var jobResult = results.GetProperty(resultKey).GetString();

            var jobMetadata = results.GetProperty("job_metadata").Deserialize<Dictionary<string, JsonElement>>();

            if (jobMetadata is not null)
            {
                foreach (var o in jobMetadata)
                {
                    _metadata[o.Key] = o.Value.ValueKind switch
                    {
                        JsonValueKind.String => o.Value.GetString()!,
                        JsonValueKind.Number => o.Value.GetDouble(),
                        JsonValueKind.True => true,
                        JsonValueKind.False => false,
                        _ => throw new ArgumentOutOfRangeException()
                    };
                }
            }

            var document = new Document(id: id, text: jobResult, metadata = _metadata);
            return document;
        }

        private async Task WaitForJobToCompleteAsync(CancellationToken cancellationToken)
        {
            var status = await GetJobStatusAsync(cancellationToken);
            while (status == JobStatus.Pending)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    throw new TaskCanceledException();
                }
                
                await Task.Delay(1000, cancellationToken);
                status = await GetJobStatusAsync(cancellationToken);
            }

            switch (status)
            {
                case JobStatus.Cancelled:
                    throw new InvalidOperationException($"Job {id} was cancelled.");
                case JobStatus.Error:
                    throw new InvalidOperationException($"Job {id} failed.");
            }
        }

        public async IAsyncEnumerable<ImageDocument> GetImagesAsync(CancellationToken cancellationToken)
        {
            await WaitForJobToCompleteAsync(cancellationToken);

            if (resultType != ResultType.Json)
            {
                throw new InvalidOperationException("Images can only be extracted from JSON results.");
            }
            var results = await GetJobResultAsync(cancellationToken);
            foreach (var pageElement in results.GetProperty("pages").EnumerateArray())
            {
                var pageNumber = pageElement.GetProperty("page").GetInt32();
                foreach (var imageElement in pageElement.GetProperty("images").EnumerateArray())
                {
                    var name = imageElement.GetProperty("name").GetString();
                    var width = imageElement.GetProperty("width").GetInt32();
                    var height = imageElement.GetProperty("height").GetInt32();
                    var getImageUri = new Uri($"{endpoint.TrimEnd('/')}/api/parsing/job/{id}/result/image/{name!}");
                    var request = new HttpRequestMessage(HttpMethod.Get, getImageUri);
                    request.Headers.Add("Authorization", $"Bearer {apiKey}");
                    var response = await client.SendAsync(request, cancellationToken);
                    response.EnsureSuccessStatusCode();

                    var content  = await response.Content.ReadAsByteArrayAsync();


                    var pageMetadata = new Dictionary<string, object>(metadata)
                    {
                        ["page_number"] = pageNumber,
                        ["image_name"] = name!,
                        ["image_height"] = height,
                        ["image_width"] = width
                    };

                    var jobMetadata = results.GetProperty("job_metadata").Deserialize<Dictionary<string, JsonElement>>();

                    if (jobMetadata is not null)
                    {
                        foreach (var o in jobMetadata)
                        {
                             metadata[o.Key] = o.Value.ValueKind switch
                            {
                                JsonValueKind.String => o.Value.GetString()!,
                                JsonValueKind.Number => o.Value.GetDouble(),
                                JsonValueKind.True => true,
                                JsonValueKind.False => false,
                                _ => throw new ArgumentOutOfRangeException()
                            };
                        }
                    }

                    var encodedImage = Convert.ToBase64String(content);

                    var imageDocument = new ImageDocument(
                        id: id,
                        image: encodedImage,
                        metadata: pageMetadata
                    );

                    yield return imageDocument;
                }
            }
           
        }
    }
}