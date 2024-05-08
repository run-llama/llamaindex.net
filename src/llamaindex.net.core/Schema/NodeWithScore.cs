namespace llamaindex.net.core.Schema;

public class NodeWithScore(BaseNode node, double score)
{
    public double Score { get;  } = score;
    public BaseNode Node { get;  } = node;
}