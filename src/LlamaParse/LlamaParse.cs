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
        using var streamReader = new StreamReader(file.OpenRead());
        return LoadDataAsync(streamReader, file.Name, cancellationToken);
    }

    public Task<IEnumerable<Document>> LoadDataAsync(StreamReader source, string fileName, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}