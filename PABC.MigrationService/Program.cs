using System.Text.Json;
using Json.More;
using Json.Schema;
using Json.Schema.Generation;
using PABC.Data;
using PABC.MigrationService;

const int ErrorExitCode = 1;

if (args.Contains("generate"))
{
    var schema = new JsonSchemaBuilder().FromType<DataSet>(new() { PropertyNameResolver = PropertyNameResolvers.CamelCase }).Build();
    var json = schema.ToJsonDocument(new JsonSerializerOptions(JsonSerializerDefaults.Web) { WriteIndented = true });
    await using var file = File.OpenWrite("dataset.schema.json");
    await JsonSerializer.SerializeAsync(file, json, new JsonSerializerOptions(JsonSerializerDefaults.Web) { WriteIndented = true });
    return;
}

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
