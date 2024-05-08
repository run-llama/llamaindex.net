﻿using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using llamaindex.net.core.Schema;

namespace llamaindex.net.core.Retrievers;

public class RetrieverClient(Uri host, string vectorDbCollectionName) : BaseRetriever
{
    protected override async Task<NodeWithScore[]> RetrieveNodesAsync(string query, CancellationToken cancellationToken)
    {
        var client = new HttpClient();
        var request = new RetrieveRequest(query, vectorDbCollectionName);
        var content = new StringContent(JsonSerializer.Serialize( request), Encoding.UTF8, "application/json");
        var requestUri = new Uri(host, "retrieve");
        var response = client.PostAsync(requestUri, content, cancellationToken);

        var responseContent = await response.Result.Content.ReadAsStringAsync();
        var nodesWithScores = JsonSerializer.Deserialize<NodeWithScore[]>(responseContent);
        return nodesWithScores ?? Array.Empty<NodeWithScore>();
        
    }

    internal record RetrieveRequest(string Query, string VectorDbCollectionName)
    {
        [JsonPropertyName("query")]
        public string Query { get; } = Query;

        [JsonPropertyName("vdb_session")]
        public string VectorDbCollectionName { get; } = VectorDbCollectionName;
    }
}