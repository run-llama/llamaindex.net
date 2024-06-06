using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using llamaindex.core.Schema;

namespace LlamaParse;

public partial class LlamaParse(HttpClient client)
{
    public IAsyncEnumerable<Document> LoadDataAsync(FileInfo file, Dictionary<string,object>? metadata = null, CancellationToken cancellationToken = default)
    {
        return LoadDataAsync([file], metadata, cancellationToken);
    }
    public async IAsyncEnumerable<Document> LoadDataAsync(IEnumerable<FileInfo> files, Dictionary<string, object>? metadata = null, CancellationToken cancellationToken = default)
    {
        var jobs = new List<Job>();
        foreach (var fileInfo in files)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                yield break;
            }
            var job = await CreateJobAsync(fileInfo, metadata, cancellationToken);
            jobs.Add(job);
        }
        if (!cancellationToken.IsCancellationRequested)
        {
            foreach (var job in jobs)
            {
                yield return await job.GetResultAsync(cancellationToken);
            }
        }
    }

    private Task<Job> CreateJobAsync(FileInfo fileInfo, Dictionary<string, object>? metadata, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

