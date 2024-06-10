using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using LlamaIndex.Core.Schema;
using LlamaIndex.CoreSchema;

namespace LlamaParse;

public partial class LlamaParse
{

    private class Job(HttpClient client, FileInfo fileInfo, Dictionary<string, object> metadata)
    {
        private readonly Dictionary<string, object> _metadata = metadata;

        public Task CreateAsync(CancellationToken cancellationToken)
        {
            var jobId = "asdasdasdas";
            metadata["llamaparse_job_id"] = jobId;
            throw new NotImplementedException();
        }

        public Task<Document> GetResultAsync(CancellationToken cancellationToken)
        {
            var document = new Document(id:"", text:"asdasdasd", metadata = metadata);
            throw new NotImplementedException();

            
        }
    }
}