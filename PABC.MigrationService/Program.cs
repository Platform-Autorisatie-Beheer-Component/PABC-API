using Microsoft.EntityFrameworkCore;
using PABC.Data;
using PABC.MigrationService;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();

builder.Services.AddDbContext<PabcDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("PabcConnection")));
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

var host = builder.Build();
host.Run();
