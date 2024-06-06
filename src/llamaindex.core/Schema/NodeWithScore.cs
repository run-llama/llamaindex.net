using System.Text.Json.Serialization;

namespace LlamaIndex.CoreSchema;

public class NodeWithScore(BaseNode node, double score)
{
    [JsonPropertyName("score")]
    public double Score { get;  } = score;
    [JsonPropertyName("node")]
    public BaseNode Node { get;  } = node;
}