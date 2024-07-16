using LlamaParse;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

namespace Microsoft.Extensions.Hosting;

public static class AspireLlamaParseExtensions
{
    /// <summary>
    /// Adds a LlamaParseClient to the service collection
    /// </summary>
    /// <param name="builder">The <see cref="IHostApplicationBuilder" /> to read config from and add services to.</param>
    /// <param name="apiKey">The LlamaCloud API key</param>
    public static void AddLlamaParseClient(this IHostApplicationBuilder builder, string apiKey)
    {
        builder.Services.AddSingleton(p => ConfigureClient(p,apiKey));
    }

    /// <summary>
    /// Adds a keyed LlamaParseClient to the service collection
    /// </summary>
    /// <param name="builder">The <see cref="IHostApplicationBuilder" /> to read config from and add services to.</param>
    /// <param name="apiKey">The LlamaCloud API key</param>
    /// <param name="name">The service keyed name</param>
    public static void AddKeyedLlamaParseClient(this IHostApplicationBuilder builder, string apiKey, string name)
    {
        builder.Services.AddKeyedSingleton<LlamaParseClient>(name, (p,_) => ConfigureClient(p,apiKey));
    }

    private static LlamaParseClient ConfigureClient(IServiceProvider provider, string apiKey)
    {
        var client = provider.GetRequiredService<HttpClient>();
        var llamaParseClient = new LlamaParseClient(client, apiKey: apiKey);
        return llamaParseClient;
    }
}
