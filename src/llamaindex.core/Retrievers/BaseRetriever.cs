using System.Threading;
using System.Threading.Tasks;
using LlamaIndex.Core.Schema;

namespace LlamaIndex.Core.Retrievers
{
    public abstract class BaseRetriever  
    {

        public Task<NodeWithScore[]> RetrieveAsync(string query, CancellationToken cancellationToken = default)
        {
           return RetrieveNodesAsync(query, cancellationToken);
        }

        protected abstract Task<NodeWithScore[]> RetrieveNodesAsync(string query,  CancellationToken cancellationToken);
    }
}
