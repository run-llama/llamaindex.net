using System;
using System.Collections.Generic;
using System.IO;
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
        FileInfo fileInfo,
        Dictionary<string, object> metadata,
        string id,
        string status,
        string endpoint,
        ResultType resultType,
        string apiKey)
    {
        public string Id { get; } = id;
        private JobStatus Status { get; set; } = Enum.Parse<JobStatus>(status,true);

        private readonly Dictionary<string, object> _metadata = new( metadata)
        {
            [LlamaParseJobIdMetadataKey] = id
        };

        private async Task<JsonElement> GetJobResultAsync(CancellationToken cancellationToken)
        {
            var getResultUri = new Uri($"{endpoint.TrimEnd('/')}/api/parsing/job/{Id}/result/{resultType.ToString().ToLowerInvariant()}");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            var response = await client.GetAsync(getResultUri, cancellationToken);
            response.EnsureSuccessStatusCode();
            var resultString = await response.Content.ReadAsStringAsync();
            return JsonDocument.Parse(resultString).RootElement;
        }

        private async Task UpdateJobStatusAsync(CancellationToken cancellationToken)
        {
            var getStatusUri = new Uri($"{endpoint.TrimEnd('/')}/api/parsing/job/{Id}");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            var response = await client.GetAsync(getStatusUri, cancellationToken);
            response.EnsureSuccessStatusCode();
            var statusString = await response.Content.ReadAsStringAsync();
            var status = JsonDocument.Parse(statusString).RootElement.GetProperty("status").GetString();
            Status = Enum.Parse<JobStatus>(status, true);

        }

        public async Task<Document> GetResultAsync(CancellationToken cancellationToken)
        {
            while (Status == JobStatus.Pending)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    throw new TaskCanceledException();
                }
                await UpdateJobStatusAsync(cancellationToken);
                await Task.Delay(1000, cancellationToken);
            }

            switch (Status)
            {
                case JobStatus.Success:
                    break;
                case JobStatus.Cancelled:
                    throw new InvalidOperationException($"Job {id} was cancelled.");
                case JobStatus.Error:
                    throw new InvalidOperationException($"Job {id} failed.");
            }

            var results = await GetJobResultAsync(cancellationToken);
            var resultKey = resultType.ToString().ToLowerInvariant();
            var jobResult = results.GetProperty(resultKey).GetString();
            var jobMetadata = results.GetProperty("job_metadata").Deserialize<Dictionary<string, JsonElement>>();

            if (jobMetadata is {})
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

            var document = new Document(id:id, text:jobResult, metadata = _metadata);
            return document;
        }
    }
}