using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using llamaindex.core.Schema;

namespace LlamaParse;

public partial class LlamaParse
{

    private class Job(HttpClient client, FileInfo fileInfo, Dictionary<string, object>? metadata)
    {
        public Task CreateAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Document> GetResultAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}