using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using LlamaIndex.Core.Schema;

namespace LlamaIndex.Core.Retrievers;

/// <summary>
/// A client for retrieving nodes from a data source.
/// </summary>
/// <param name="host">The URI host for the data source</param>
/// <param name="vectorDbCollectionName">The name of the collection where nodes are stored.</param>
public class RetrieverClient(Uri host, string vectorDbCollectionName) : BaseRetriever
{
    /// <summary>
    /// Retrieves nodes from the data source.
    /// </summary>
    /// <param name="query">An input query used to retreive similar nodes.</param>
    /// <param name="cancellationToken">Propagates notification for operations to be cancelled.<see cref="CancellationToken"/> </param>
    /// <returns>A collection of nodes. See <see cref="NodeWithScore"/></returns>
    protected override async Task<NodeWithScore[]> RetrieveNodesAsync(string query, CancellationToken cancellationToken)
    {
        var client = new HttpClient();
        var request = new RetrieveRequest(query, vectorDbCollectionName);
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var requestUri = new Uri(host, "retrieve");
        var response = client.PostAsync(requestUri, content, cancellationToken);

        var responseContent = await response.Result.Content.ReadAsStringAsync();
        var nodesWithScores = JsonSerializer.Deserialize<NodeWithScore[]>(responseContent);
        return nodesWithScores ?? [];

    }

    internal record RetrieveRequest(string Query, string VectorDbCollectionName)
    {
        [JsonPropertyName("query")]
        public string Query { get; } = Query;

        [JsonPropertyName("vdb_session")]
        public string VectorDbCollectionName { get; } = VectorDbCollectionName;
    }
}
