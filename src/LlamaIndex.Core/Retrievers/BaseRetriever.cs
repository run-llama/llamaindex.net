using System.Threading;
using System.Threading.Tasks;
using LlamaIndex.Core.Schema;

namespace LlamaIndex.Core.Retrievers
{
    /// <summary>
    /// Provides an abstraction for retreivers that retrieve nodes from a data source.
    /// </summary>
    public abstract class BaseRetriever
    {
        /// <summary>
        /// Given a query, retrieves nodes from the data source.
        /// </summary>
        /// <param name="query">An input query used to retreive similar nodes.</param>
        /// <param name="cancellationToken">Propagates notification for operations to be cancelled.<see cref="CancellationToken"/> </param>
        /// <returns>A collection of nodes. See <see cref="NodeWithScore"/></returns>
        public Task<NodeWithScore[]> RetrieveAsync(string query, CancellationToken cancellationToken = default)
        {
            return RetrieveNodesAsync(query, cancellationToken);
        }

        protected abstract Task<NodeWithScore[]> RetrieveNodesAsync(string query, CancellationToken cancellationToken);
    }
}
