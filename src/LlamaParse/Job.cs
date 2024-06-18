using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using LlamaIndex.Core.Schema;


namespace LlamaParse;

public partial class LlamaParse
{
   private class Job(
        LlamaParseClient client,
        Dictionary<string, object> metadata,
        string id,
        ResultType resultType)
    {
        private readonly Dictionary<string, object> _metadata = new(metadata)
        {
            [Constants.JobIdKey] = id
        };

        public async Task<RawResult> GetRawResult(ResultType type, CancellationToken cancellationToken)
        {
            await WaitForJobToCompleteAsync(cancellationToken);
            var rawResults =  await client.GetJobResultAsync(id, type, cancellationToken);
            var documentMetadata = new Dictionary<string, object>(_metadata);
            PopulateMetadataFromJobResult(rawResults.Result, documentMetadata);
            return new RawResult(rawResults.JobId, rawResults.Result, documentMetadata, rawResults.CreditsUsed, rawResults.CreditsMax, rawResults.JobCreditsUsage, rawResults.JobPages, rawResults.IsCacheHit);
        }

        private static void PopulateMetadataFromJobResult(JsonElement results, IDictionary<string, object> documentMetadata)
        {
            var jobMetadata = results.GetProperty(Constants.JobMetadataKey).Deserialize<Dictionary<string, JsonElement>>();

            if (jobMetadata is not null)
            {
                foreach (var o in jobMetadata)
                {
                    documentMetadata[o.Key] = o.Value.ValueKind switch
                    {
                        JsonValueKind.String => o.Value.GetString()!,
                        JsonValueKind.Number => o.Value.GetDouble(),
                        JsonValueKind.True => true,
                        JsonValueKind.False => false,
                        _ => throw new ArgumentOutOfRangeException()
                    };
                }
            }
        }

        private async Task WaitForJobToCompleteAsync(CancellationToken cancellationToken)
        {
            var status = await client.GetJobStatusAsync(id, cancellationToken);
            while (status == JobStatus.Pending)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    throw new TaskCanceledException();
                }
                
                await Task.Delay(500, cancellationToken);
                status = await client.GetJobStatusAsync(id, cancellationToken);
            }

            switch (status)
            {
                case JobStatus.Cancelled:
                    throw new InvalidOperationException($"Job {id} was cancelled.");
                case JobStatus.Error:
                    throw new InvalidOperationException($"Job {id} failed.");
            }
        }

        public async IAsyncEnumerable<ImageDocument> GetImagesAsync( RawResult rawResult,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            foreach (var pageElement in rawResult.Result.GetProperty("pages").EnumerateArray())
            {
                var pageNumber = pageElement.GetProperty("page").GetInt32();
                foreach (var imageElement in pageElement.GetProperty("images").EnumerateArray())
                {
                    var name = imageElement.GetProperty("name").GetString();
                    var width = imageElement.GetProperty("width").GetInt32();
                    var height = imageElement.GetProperty("height").GetInt32();
                    var content = await client.GetImage(id, name!, cancellationToken);

                    var pageMetadata = new Dictionary<string, object>(_metadata)
                    {
                        ["page_number"] = pageNumber,
                        ["image_name"] = name!,
                        ["image_height"] = height,
                        ["image_width"] = width
                    };

                    var jobMetadata = rawResult.Result.GetProperty("job_metadata").Deserialize<Dictionary<string, JsonElement>>();

                    if (jobMetadata is not null)
                    {
                        foreach (var o in jobMetadata)
                        {
                            pageMetadata[o.Key] = o.Value.ValueKind switch
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
                        id: Guid.NewGuid().ToString(),
                        image: encodedImage,
                        metadata: pageMetadata
                    );

                    yield return imageDocument;
                }
            }
        }
        public async IAsyncEnumerable<ImageDocument> GetImagesAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await WaitForJobToCompleteAsync(cancellationToken);

            if (resultType != ResultType.Json)
            {
                throw new InvalidOperationException("Images can only be extracted from JSON results.");
            }
            var rawResult = await client.GetJobResultAsync(id, resultType, cancellationToken);
           
           await foreach (var imageDocument in GetImagesAsync(rawResult, cancellationToken))
           {
               yield return imageDocument;
           }
        }
    }
}