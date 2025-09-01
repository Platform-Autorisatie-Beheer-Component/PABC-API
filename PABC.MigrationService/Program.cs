using PABC.Data;
using PABC.MigrationService;
using PABC.MigrationService.Features.DatabaseInitialization;
using Microsoft.EntityFrameworkCore; 

// use `dotnet run generate` to generate the json schema. we do this in a github action

if (args.Contains("generate"))
{
    await DatasetParser.WriteSchemaToFile(CancellationToken.None);
    return;
}

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.AddNpgsqlDbContext<PabcDbContext>(
    connectionName: "Pabc",
    configureDbContextOptions: options =>
    {
        options.UseNpgsql(npgsqlOptions =>
        { 
            npgsqlOptions.MigrationsHistoryTable("__ef_migrations_history", "public");
        })
        .UseSnakeCaseNamingConvention();
    });
builder.Services.AddScoped<IDatabaseInitializer, DatabaseInitializer>();
builder.Services.AddSingleton<IDatasetParser, DatasetParser>();
builder.Services.AddHostedService<Worker>();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

var host = builder.Build();
host.Run();
