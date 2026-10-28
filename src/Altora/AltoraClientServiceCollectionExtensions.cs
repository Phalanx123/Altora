using Altora.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Altora;

public static class AltoraClientServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="AltoraClient"/> as a typed client of IHttpClientFactory, its base address and
    /// credential headers set by the factory from <see cref="AltoraOptions"/>. The client is named
    /// "AltoraClient", so resilience handlers or timeouts can be added through the returned builder.
    /// </summary>
    public static IHttpClientBuilder AddAltoraClient(this IServiceCollection services) =>
        services.AddHttpClient<AltoraClient>((provider, httpClient) =>
            AltoraClient.Configure(httpClient, provider.GetRequiredService<IOptions<AltoraOptions>>().Value));
}
