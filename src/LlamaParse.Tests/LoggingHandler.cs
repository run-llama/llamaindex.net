using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace LlamaParse.Tests;

internal class LoggingHandler(HttpMessageHandler innerHandler, string folder = "", [CallerMemberName] string name = "")
    : DelegatingHandler(innerHandler)
{
    readonly string _folder = string.IsNullOrWhiteSpace( folder) ? Path.Combine( Path.GetTempPath(), "llamaparse_tests") : folder;

    private int _sequenceNumber = 0;
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (!Directory.Exists(_folder))
        {
            Directory.CreateDirectory(_folder);
        }
        var pattern = "Authorization: Bearer llx-[A-Za-z0-9-_]+";
        var replacement = "Authorization: Bearer 00";
    
        var id = Interlocked.Increment(ref _sequenceNumber);
        var now  = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");

        var log = new StringBuilder();
        log.AppendLine("Request:");
        log.AppendLine(request.ToString());
        if (request.Content != null)
        {
            log.AppendLine(await request.Content.ReadAsStringAsync(cancellationToken));
        }
        log.AppendLine();

        await File.WriteAllTextAsync(Path.Combine(_folder,$"{now}_{name}_request_message_{id}.txt"), Regex.Replace(log.ToString(), pattern, replacement), cancellationToken);

        log.Clear();
        var response = await base.SendAsync(request, cancellationToken);

        log.AppendLine("Response:");
        log.AppendLine(response.ToString());
        if (response.Content is not null)
        {
            log.AppendLine(await response.Content.ReadAsStringAsync(cancellationToken));
        }
        log.AppendLine();

        await File.WriteAllTextAsync(Path.Combine(_folder, $"{now}_{name}_response_message_{id}.txt"), Regex.Replace(log.ToString(), pattern, replacement), cancellationToken); 
        return response;
    }
}