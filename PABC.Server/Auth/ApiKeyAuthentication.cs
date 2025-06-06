using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace PABC.Server.Auth
{
    public static class ApiKeyAuthentication
    {
        private const string API_KEY_HEADER_NAME = "X-API-KEY";
        private const string Scheme = "ApiKey";
        public const string Policy = "ApiKeyPolicy";

        public static void AddApiKeyAuth(this IServiceCollection services, IReadOnlyList<string> apiKeys)
        {
            services.AddAuthentication().AddScheme<ApiKeyOptions, Handler>(Scheme, opt =>
            {
                opt.ApiKeys = apiKeys;
            });

            services.AddAuthorizationBuilder()
                .AddPolicy(Policy, policyBuilder => policyBuilder
                    .AddAuthenticationSchemes(Scheme)
                    .RequireAuthenticatedUser()
                    );

            services.ConfigureSwaggerGen(options =>
            {
                options.AddSecurityDefinition(Scheme, new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Name = API_KEY_HEADER_NAME,
                });
                options.OperationFilter<ApiKeySwaggerOperationFilter>();
            });
        }


        private class ApiKeyOptions : AuthenticationSchemeOptions
        {
            public IReadOnlyList<string> ApiKeys { get; set; } = [];
        }

        private class Handler(IOptionsMonitor<ApiKeyOptions> options, ILoggerFactory logger, UrlEncoder encoder) : AuthenticationHandler<ApiKeyOptions>(options, logger, encoder)
        {
            protected override Task<AuthenticateResult> HandleAuthenticateAsync() => Task.FromResult(SucceedRequirementIfApiKeyPresentAndValid());

            private AuthenticateResult SucceedRequirementIfApiKeyPresentAndValid()
            {
                var apiKey = Request.Headers[API_KEY_HEADER_NAME].FirstOrDefault();
                if (apiKey != null && Options.ApiKeys.Any(requiredApiKey => apiKey == requiredApiKey))
                {
                    var identity = new ClaimsIdentity([], ApiKeyAuthentication.Scheme);
                    var claimsPrincipal = new ClaimsPrincipal(identity);
                    var ticket = new AuthenticationTicket(claimsPrincipal, ApiKeyAuthentication.Scheme);
                    return AuthenticateResult.Success(ticket);
                }
                return AuthenticateResult.NoResult();
            }
        }

        private class ApiKeySwaggerOperationFilter : IOperationFilter
        {
            private static readonly OpenApiSecurityRequirement s_securityRequirement = new()
            {
                [new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = Scheme } }] = []
            };

            public void Apply(OpenApiOperation operation, OperationFilterContext context)
            {
                if (HasApiKeyPolicy(context))
                {
                    operation.Security.Add(s_securityRequirement);
                }
            }

            private static bool HasApiKeyPolicy(OperationFilterContext context) =>
                context.ApiDescription.ActionDescriptor.EndpointMetadata
                    .OfType<AuthorizeAttribute>()
                    .Any(x => x.Policy == Policy);
        }
    }
}
