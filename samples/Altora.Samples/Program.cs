using Altora;
using Altora.Configuration;
using Altora.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

// Build host
var builder = Host.CreateApplicationBuilder(args);

// Add configuration sources
builder.Configuration
    .AddUserSecrets<Program>()        // load from user secrets
    .AddJsonFile("appsettings.json", optional: true) // optional local override
    .AddEnvironmentVariables();       // allow env vars override

// Bind Altora options (from user secrets or overrides)
builder.Services.Configure<AltoraOptions>(
    builder.Configuration.GetSection("Altora"));

// Register AltoraClient with DI
builder.Services.AddTransient<AltoraClient>();

// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(o =>
{
    o.SingleLine = true;
    o.TimestampFormat = "hh:mm:ss ";
});

// Build the app
using var host = builder.Build();

// Resolve the client
var client = host.Services.GetRequiredService<AltoraClient>();
var logger = host.Services.GetRequiredService<ILogger<Program>>();

try
{
    // Example: fetch companies
    var companies = await client.GetCompaniesAsync();
    Console.WriteLine("Companies:");
    foreach (var company in companies)
    {
        Console.WriteLine($"- {company.Id}: {company.Name}");
    }

    // Example: fetch workers
    var workers = await client.GetWorkersAsync(new AltoraWorkerSearchParameters
    {
        FirstName = "John"
    });

    Console.WriteLine("\nWorkers:");
    foreach (var worker in workers)
    {
        Console.WriteLine($"- {worker.Id}: {worker.Firstname} {worker.Lastname}, Company={worker.AltoraCompany?.Name}");
    }
}
catch (Exception ex)
{
    logger.LogError(ex, "Error running Altora samples");
}