using Microsoft.OpenApi;
using Microsoft.OpenApi.Extensions;
using PABC.Data;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddNpgsqlDbContext<PabcDbContext>(connectionName: "PabcConnection");

builder.Services.AddRequestTimeouts();
builder.Services.AddOutputCache();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Export Swagger YAML at startup
app.Lifetime.ApplicationStarted.Register(() =>
{
    var provider = app.Services.GetRequiredService<Microsoft.AspNetCore.Mvc.ApiExplorer.IApiDescriptionGroupCollectionProvider>();
    var generator = app.Services.GetRequiredService<Swashbuckle.AspNetCore.Swagger.ISwaggerProvider>();

    var swaggerDoc = generator.GetSwagger("v1");

    var yaml = swaggerDoc.SerializeAsYaml(OpenApiSpecVersion.OpenApi3_0);

    var outputPath = Path.Combine(app.Environment.ContentRootPath, "Docs/swagger.yaml");

    Directory.CreateDirectory(Path.GetDirectoryName(outputPath )?? string.Empty);

    File.WriteAllText(outputPath, yaml);
});
app.UseAuthorization();

app.MapControllers();

app.UseRequestTimeouts();
app.UseOutputCache();

app.Run();
