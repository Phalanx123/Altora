using System.Net;
using Altora.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Altora.Tests;

public class AltoraClientConfigurationTests
{
    private static readonly AltoraOptions Options = new()
    {
        ClientId = "pfwork",
        ApiKey = "key-1234",
        ApiSecret = "secret-5678"
    };

    [Fact]
    public async Task Clients_from_the_factory_call_the_tenant_with_each_header_once()
    {
        var handler = new RecordingHandler();
        using var provider = BuildProvider(handler, new RecordingLoggerProvider());

        // Two resolutions, as two scoped services would make.
        foreach (var client in new[] { provider.GetRequiredService<AltoraClient>(), provider.GetRequiredService<AltoraClient>() })
            await client.GetCoursesAsync();

        Assert.Equal(2, handler.Requests.Count);
        Assert.All(handler.Requests, request =>
        {
            Assert.Equal("https://api.userlogin.com.au/v1/pfwork/courses", request.RequestUri!.ToString());
            Assert.Equal(["key-1234"], request.Headers.GetValues("X-Api-Key"));
            Assert.Equal(["secret-5678"], request.Headers.GetValues("X-Api-Secret"));
            Assert.Single(request.Headers.Accept);
        });
    }

    [Fact]
    public void Creating_a_client_logs_nothing_above_debug()
    {
        var loggerProvider = new RecordingLoggerProvider();
        using var provider = BuildProvider(new RecordingHandler(), loggerProvider);

        for (var i = 0; i < 5; i++)
            provider.GetRequiredService<AltoraClient>();

        Assert.DoesNotContain(loggerProvider.Entries, entry => entry.Level > LogLevel.Debug);
    }

    [Fact]
    public void Configure_twice_leaves_one_of_each_header()
    {
        using var httpClient = new HttpClient();

        AltoraClient.Configure(httpClient, Options);
        AltoraClient.Configure(httpClient, Options);

        Assert.Equal(["key-1234"], httpClient.DefaultRequestHeaders.GetValues("X-Api-Key"));
        Assert.Equal(["secret-5678"], httpClient.DefaultRequestHeaders.GetValues("X-Api-Secret"));
        Assert.Single(httpClient.DefaultRequestHeaders.Accept);
    }

    [Fact]
    public void A_client_handed_an_unconfigured_HttpClient_configures_it()
    {
        using var httpClient = new HttpClient();

        _ = new AltoraClient(httpClient, Microsoft.Extensions.Options.Options.Create(Options), NullLogger<AltoraClient>.Instance);

        Assert.Equal(new Uri("https://api.userlogin.com.au/v1/pfwork/"), httpClient.BaseAddress);
        Assert.Equal(["key-1234"], httpClient.DefaultRequestHeaders.GetValues("X-Api-Key"));
    }

    [Fact]
    public void A_blank_secret_sends_no_header()
    {
        using var httpClient = new HttpClient();

        AltoraClient.Configure(httpClient, new AltoraOptions { ClientId = "pfwork", ApiKey = "key-1234", ApiSecret = "" });

        Assert.False(httpClient.DefaultRequestHeaders.Contains("X-Api-Secret"));
    }

    private static ServiceProvider BuildProvider(RecordingHandler handler, RecordingLoggerProvider loggerProvider)
    {
        var services = new ServiceCollection();
        services.AddLogging(logging => logging.SetMinimumLevel(LogLevel.Trace).AddProvider(loggerProvider));
        services.AddSingleton<IOptions<AltoraOptions>>(Microsoft.Extensions.Options.Options.Create(Options));
        services.AddAltoraClient().ConfigurePrimaryHttpMessageHandler(() => handler);
        return services.BuildServiceProvider();
    }

    private sealed class RecordingHandler : HttpMessageHandler
    {
        public List<HttpRequestMessage> Requests { get; } = [];

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Requests.Add(request);
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("[]") });
        }
    }

    private sealed class RecordingLoggerProvider : ILoggerProvider
    {
        public List<(string Category, LogLevel Level)> Entries { get; } = [];

        public ILogger CreateLogger(string categoryName) => new RecordingLogger(categoryName, Entries);

        public void Dispose()
        {
        }

        private sealed class RecordingLogger(string category, List<(string Category, LogLevel Level)> entries) : ILogger
        {
            public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            {
                if (category.StartsWith("Altora"))
                    entries.Add((category, logLevel));
            }
        }
    }
}
