﻿using PABC.Data;
using PABC.Server.Auth;
using PABC.Server.Helper;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// when this app is ran from aspire, we need to manually include user secrets from this specific assembly
//builder.Configuration.AddUserSecrets<Program>();

builder.AddServiceDefaults();
builder.AddPabcDbContext();

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
        Title = "Platform Autorisatie Beheer Component API",
        Version = "v1",
        Description = "API for the Platform Autorisatie Beheer Component (PABC)"
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

if (Assembly.GetEntryAssembly()?.GetName().Name != "GetDocument.Insider")
{
    builder.Services.AddAuth(options =>
    {
        options.Authority = ConfigHelper.GetRequiredConfigValue(builder.Configuration, "Oidc:Authority");
        options.ClientId = ConfigHelper.GetRequiredConfigValue(builder.Configuration, "Oidc:ClientId");
        options.ClientSecret = ConfigHelper.GetRequiredConfigValue(builder.Configuration, "Oidc:ClientSecret");
        options.FunctioneelBeheerderRole = ConfigHelper.GetRequiredConfigValue(builder.Configuration, "Oidc:FunctioneelBeheerderRole");
    });
}

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
app.UseSwagger(x => x.RouteTemplate = "api/{documentName}/specs.json"); //documetnname = v1

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI(x =>
    {
        x.SwaggerEndpoint("/api/v1/specs.json", "PABC API specs");
        x.RoutePrefix = "swagger";

    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();


if (Assembly.GetEntryAssembly()?.GetName().Name != "GetDocument.Insider")
{
    app.MapPabcAuthEndpoints();
}

app.MapControllers();

app.UseRequestTimeouts();
app.UseOutputCache();

app.MapFallbackToFile("/index.html").AllowAnonymous();

app.Run();
