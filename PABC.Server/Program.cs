using PABC.Data;
using PABC.Server.Auth;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

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

const string AuthorityKey = "OIDC_AUTHORITY";

var authority = builder.Configuration[AuthorityKey];

if (string.IsNullOrWhiteSpace(authority))
{
    //Logger.Fatal("Environment variable {variableKey} is missing", AuthorityKey);
}

builder.Services.AddPabcAuth(options =>
{
    options.Authority = authority;
    options.ClientId = builder.Configuration["OIDC_CLIENT_ID"];
    options.ClientSecret = builder.Configuration["OIDC_CLIENT_SECRET"];
    options.KlantcontactmedewerkerRole = builder.Configuration["OIDC_KLANTCONTACTMEDEWERKER_ROLE"];
    options.RedacteurRole = builder.Configuration["OIDC_REDACTEUR_ROLE"];
    options.MedewerkerIdentificatieClaimType = builder.Configuration["OIDC_MEDEWERKER_IDENTIFICATIE_CLAIM"];
    if (int.TryParse(builder.Configuration["OIDC_MEDEWERKER_IDENTIFICATIE_TRUNCATE"], out var truncate))
    {
        options.TruncateMedewerkerIdentificatie = truncate;
    }
    options.JwtTokenAuthenticationSecret = builder.Configuration["MANAGEMENTINFORMATIE_API_KEY"];
});

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseDefaultFiles();
app.UseStaticFiles();

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

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthentication();

app.UsePabcAuthMiddlewares();

app.MapControllers();

app.UseRequestTimeouts();
app.UseOutputCache();

app.MapFallbackToFile("/index.html");

app.Run();
