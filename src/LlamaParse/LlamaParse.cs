using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using llamaindex.core.Schema;

namespace LlamaParse;

public class LlamaParse(HttpClient client)
{
    public Task<IEnumerable<Document>> LoadDataAsync(FileInfo file, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}