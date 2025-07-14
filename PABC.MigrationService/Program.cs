using PABC.Data;
using PABC.MigrationService;

const int ErrorExitCode = 1;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.AddNpgsqlDbContext<PabcDbContext>(connectionName: "Pabc");
builder.Services.AddHostedService<Worker>();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

var host = builder.Build();

try
{
    host.Run();
}
catch (Exception)
{
    Environment.Exit(ErrorExitCode);
    throw;
}
