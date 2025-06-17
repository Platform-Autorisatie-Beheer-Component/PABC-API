using Microsoft.OpenApi;
using Microsoft.OpenApi.Extensions;
using PABC.Data;
using PABC.Server.Auth;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddNpgsqlDbContext<PabcDbContext>(connectionName: "Pabc");

builder.Services.AddRequestTimeouts();
builder.Services.AddOutputCache();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "PodiumD Autorisatie Beheer Component API",
        Version = "v1",
        Description = "API for the PodiumD Autorisatie Beheer Component (PABC)"
    });
    options.SupportNonNullableReferenceTypes();
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddApiKeyAuth(builder.Configuration.GetSection("API_KEY")
    .AsEnumerable()
    .Select(x => x.Value)
    .OfType<string>()
    .ToArray());

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
app.UseSwagger( x=> x.RouteTemplate = "api/{documentName}/specs.json"); //documetnname = v1

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI(x =>
    {
        x.SwaggerEndpoint("/api/v1/specs.json", "PABC API specs");
        x.RoutePrefix = "swagger";

    });
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