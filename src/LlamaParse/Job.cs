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
            [LlamaParseJobIdMetadataKey] = id
        };

        public async Task<JsonElement> GetRawResult(ResultType type, CancellationToken cancellationToken)
        {
            await WaitForJobToCompleteAsync(cancellationToken);
            return await client.GetJobResultAsync(id, type, cancellationToken);
        }

        public  Task<JsonElement> GetRawResult( CancellationToken cancellationToken)
        {
            return GetRawResult(resultType, cancellationToken);
        }

        public async Task<Document> GetDocumentAsync(CancellationToken cancellationToken)
        {
            await WaitForJobToCompleteAsync(cancellationToken);

            var results = await client.GetJobResultAsync(id, resultType, cancellationToken);
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
            var documentMetadata = new Dictionary<string, object>(_metadata);
            PopulateMetadataFromJobResult(results, documentMetadata);
            throw new NotImplementedException();
        }

        private Document CreateDocumentFromTextResults(JsonElement results)
        {
            var documentMetadata = new Dictionary<string, object>(_metadata);
            PopulateMetadataFromJobResult(results, documentMetadata);
            throw new NotImplementedException();
        }

        private Document CreateDocumentFromMarkdownResults(JsonElement results)
        {
            var resultKey = resultType.ToString().ToLowerInvariant();
            var jobResult = results.GetProperty(resultKey).GetString();

            var documentMetadata = new Dictionary<string, object>(_metadata);
            PopulateMetadataFromJobResult(results, documentMetadata);

            var document = new Document(id: id, text: jobResult, metadata = documentMetadata);
            return document;
        }

        private void PopulateMetadataFromJobResult(JsonElement results, IDictionary<string, object> documentMetadata)
        {
            var jobMetadata = results.GetProperty("job_metadata").Deserialize<Dictionary<string, JsonElement>>();

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
                
                await Task.Delay(1000, cancellationToken);
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

        public async IAsyncEnumerable<ImageDocument> GetImagesAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await WaitForJobToCompleteAsync(cancellationToken);

            if (resultType != ResultType.Json)
            {
                throw new InvalidOperationException("Images can only be extracted from JSON results.");
            }
            var results = await client.GetJobResultAsync(id, resultType, cancellationToken);
            foreach (var pageElement in results.GetProperty("pages").EnumerateArray())
            {
                var pageNumber = pageElement.GetProperty("page").GetInt32();
                foreach (var imageElement in pageElement.GetProperty("images").EnumerateArray())
                {
                    var name = imageElement.GetProperty("name").GetString();
                    var width = imageElement.GetProperty("width").GetInt32();
                    var height = imageElement.GetProperty("height").GetInt32();

                    var content = await client.GetImage(id, name!, cancellationToken);


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