using System.Text.Json.Serialization;

namespace LlamaIndex.Core.Schema;

public class NodeWithScore(BaseNode node, double score)
{
    [JsonPropertyName("score")]
    public double Score { get; } = score;
    [JsonPropertyName("node")]
    public BaseNode Node { get; } = node;
}
