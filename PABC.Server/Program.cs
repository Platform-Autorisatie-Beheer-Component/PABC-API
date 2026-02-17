using System.Reflection;
using PABC.Data;
using PABC.Server.Auth;
using PABC.Server.Helper;
using PABC.Server.Keycloak;

var isTooling = Assembly.GetEntryAssembly()?.GetName().Name is "ef" or "GetDocument.Insider";

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddPabcDbContext();

builder.Services.AddRequestTimeouts();
builder.Services.AddOutputCache();

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddApiKeyAuth(builder.Configuration.GetSection("API_KEY")
    .AsEnumerable()
    .Select(x => x.Value)
    .OfType<string>()
    .ToArray());

builder.Services.AddOpenApi(x=>
{
    x.AddDocumentTransformer((a, b, c) =>
    {
        a.Info.Title = "Platform Autorisatie Beheer Component API";
        a.Info.Description = "API for the Platform Autorisatie Beheer Component (PABC)";
        a.Info.Version = "v1";
        return Task.CompletedTask;
    });
});

if (!isTooling)
{
    builder.Services.AddAuth(options =>
    {
        options.Authority = ConfigHelper.GetRequiredConfigValue(builder.Configuration, "Oidc:Authority");
        options.ClientId = ConfigHelper.GetRequiredConfigValue(builder.Configuration, "Oidc:ClientId");
        options.ClientSecret = ConfigHelper.GetRequiredConfigValue(builder.Configuration, "Oidc:ClientSecret");
        options.FunctioneelBeheerderRole = ConfigHelper.GetRequiredConfigValue(builder.Configuration, "Oidc:FunctioneelBeheerderRole");
        options.NameClaimType = builder.Configuration["Oidc:NameClaimType"];
        options.RoleClaimType = builder.Configuration["Oidc:RoleClaimType"];
        options.EmailClaimType = builder.Configuration["Oidc:EmailClaimType"];
        options.RequireHttpsForIdentityProvider = builder.Configuration.GetValue<bool?>("Oidc:RequireHttps");
        options.LogoutFromIdentityProvider = builder.Configuration.GetValue<bool?>("Oidc:LogoutFromIdentityProvider");
    });

    builder.Services.AddKeycloakAdminClient(
        builder.Configuration.GetRequiredConfigValue("KeycloakAdmin:ClientId"),
        builder.Configuration.GetRequiredConfigValue("KeycloakAdmin:ClientSecret"));
}


var app = builder.Build();

app.MapDefaultEndpoints();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
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


if (!isTooling)
{
    app.MapPabcAuthEndpoints();
}

app.MapOpenApi("api/{documentName}/specs.json");

app.MapControllers();

app.UseRequestTimeouts();
app.UseOutputCache();

app.MapFallbackToFile("/index.html").AllowAnonymous();

app.Run();
