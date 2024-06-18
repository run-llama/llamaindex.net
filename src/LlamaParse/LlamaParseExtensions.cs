using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using LlamaIndex.Core.Schema;

namespace LlamaParse;

public static class LlamaParseExtensions
{
    public static IAsyncEnumerable<Document> LoadDataAsync(this LlamaParseClient llamaParseClient, FileInfo file, bool splitByPage = false, Dictionary<string, object>? metadata = null, CancellationToken cancellationToken = default)
    {
        return llamaParseClient.LoadDataAsync([file], splitByPage, metadata, cancellationToken);
    }

    public static  async IAsyncEnumerable<Document> LoadDataAsync(this LlamaParseClient llamaParseClient,IEnumerable<FileInfo> files, bool splitByPage = false, Dictionary<string, object>? metadata = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var documentMetadata = metadata ?? new Dictionary<string, object>();

        await foreach (var rawResult in llamaParseClient.LoadDataRawAsync(files, ResultType.Json, documentMetadata, cancellationToken))
        {
            var jobId = rawResult.JobId;

            var result = rawResult.Result;

            if (splitByPage)
            {
                foreach (var page in result.GetProperty("pages").EnumerateArray())
                {
                    switch (llamaParseClient.Configuration.ResultType)
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
                            yield return new Document(Guid.NewGuid().ToString(), page.GetRawText(),
                                documentMetadata);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            else
            {
                if (llamaParseClient.Configuration.ResultType == ResultType.Json)
                {
                    yield return new Document(jobId, result.GetRawText(), documentMetadata);

                }

                var content = new StringBuilder();
                foreach (var page in result.GetProperty("pages").EnumerateArray())
                {
                    switch (llamaParseClient.Configuration.ResultType)
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
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                yield return new Document(jobId, content.ToString(), documentMetadata);
            }

            if (llamaParseClient.Configuration.ExtractImages)
            {
                await foreach (var image in llamaParseClient.LoadImagesAsync(jobId, documentMetadata, cancellationToken))
                {
                    yield return image;
                }
            }
        }

    }

}