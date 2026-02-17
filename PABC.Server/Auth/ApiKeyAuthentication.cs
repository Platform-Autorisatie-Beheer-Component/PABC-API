using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;

namespace PABC.Server.Auth
{
    public static class ApiKeyAuthentication
    {
        internal const string API_KEY_HEADER_NAME = "X-API-KEY";
        public const string Scheme = "ApiKey";
        public const string Policy = "ApiKeyPolicy";
        public const string ClaimType = "ApiKeyClaim";

        public static void AddApiKeyAuth(this IServiceCollection services, IReadOnlyList<string> apiKeys)
        {
            services.AddAuthentication().AddScheme<ApiKeyOptions, Handler>(Scheme, null);

            services.AddAuthorizationBuilder()
                .AddPolicy(Policy, policyBuilder => policyBuilder
                    .AddAuthenticationSchemes(Scheme)
                    .AddRequirements(new ApiKeyRequirement(apiKeys))
                    );
            services.AddSingleton<IAuthorizationHandler, ApiKeyRequirement.Handler>();
            services.ConfigureAll<OpenApiOptions>(options =>
            {
                options.AddOperationTransformer<ApiKeyOperationTransformer>();
                options.AddDocumentTransformer<ApiKeyDocumentTransformer>();
            });
        }

        internal class ApiKeyOptions : AuthenticationSchemeOptions;

        // this handler is only responsible for authentication: it checks if there is an api key and adds it to the claimsprincipal
        internal class Handler(IOptionsMonitor<ApiKeyOptions> options, ILoggerFactory logger, UrlEncoder encoder) : AuthenticationHandler<ApiKeyOptions>(options, logger, encoder)
        {
            protected override Task<AuthenticateResult> HandleAuthenticateAsync() => Task.FromResult(SucceedRequirementIfApiKeyPresentAndValid());

            private AuthenticateResult SucceedRequirementIfApiKeyPresentAndValid()
            {
                var apiKey = Request.Headers[API_KEY_HEADER_NAME].FirstOrDefault();
                if (apiKey != null)
                {
                    var identity = new ClaimsIdentity([new(ClaimType, apiKey)], ApiKeyAuthentication.Scheme);
                    var claimsPrincipal = new ClaimsPrincipal(identity);
                    var ticket = new AuthenticationTicket(claimsPrincipal, ApiKeyAuthentication.Scheme);
                    return AuthenticateResult.Success(ticket);
                }
                return AuthenticateResult.NoResult();
            }
        }

        private class ApiKeyOperationTransformer : IOpenApiOperationTransformer
        {
            private static bool HasApiKeyPolicy(OpenApiOperationTransformerContext context) =>
                context.Description.ActionDescriptor.EndpointMetadata
                    .OfType<AuthorizeAttribute>()
                    .Any(x => x.Policy == Policy);

            public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
            {
                if (HasApiKeyPolicy(context))
                {
                    operation.Security ??= [];
                    operation.Security.Add(new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecuritySchemeReference(Scheme, context.Document)] = []
                    });
                }
                return Task.CompletedTask;
            }
        }

        private class ApiKeyDocumentTransformer : IOpenApiDocumentTransformer
        {
            public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
            {
                document.Components ??= new();
                document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
                document.Components.SecuritySchemes[Scheme] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Name = API_KEY_HEADER_NAME,
                };
                return Task.CompletedTask;
            }
        }
    }

    public record ApiKeyRequirement(IReadOnlyList<string> ApiKeys) : IAuthorizationRequirement
    {
        // this handler is responsible for authorization: for specific routes, it checks if the claimsprincipal has a valid api key
        public class Handler(ILogger<Handler> logger) : AuthorizationHandler<ApiKeyRequirement>
        {
            protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ApiKeyRequirement requirement)
            {
                var apiKeys = GetApiKeyClaimValues(context);
                if (requirement.ApiKeys.Any(apiKeys.Contains))
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
                var invalidKeys = apiKeys.Where(x => !apiKeys.Contains(x)).ToList();
                if (invalidKeys.Count == 0)
                {
                    logger.LogWarning("Authentication attempt with missing API key");
                }
                else
                {
                    foreach (var apiKey in invalidKeys)
                    {
                        logger.LogWarning("Authentication attempt with invalid API key: {apiKey}", apiKey[..(apiKey.Length < 5 ? apiKey.Length : 4)]);
                    }
                }
                return Task.CompletedTask;
            }

            private static IReadOnlyList<string> GetApiKeyClaimValues(AuthorizationHandlerContext context) => context.User
                .FindAll(ApiKeyAuthentication.ClaimType)
                .Select(x => x.Value)
                .ToList();
        }
    }
}
