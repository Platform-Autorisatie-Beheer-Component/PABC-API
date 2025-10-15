using System.Security.Claims;
using System.Text;
using System.Text.Json.Nodes;
using Duende.AccessTokenManagement.OpenIdConnect;
using Duende.IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Tokens;
using PABC.Server.Auth;

namespace PABC.Server.Auth
{
    public static class Policies
    {
        public const string RedactiePolicy = "RedactiePolicy";
        public const string ExternSysteemPolicy = "ExternSysteemPolicy";
        public const string KcmOrRedactiePolicy = "KcmOrRedactiePolicy";
    }

    public static class KissClaimTypes
    {
        public const string KissUserNameClaimType = "KISS:UserName";
    }

    public static class UserExtensions
    {
        private const string ObjectIdentitifier = "http://schemas.microsoft.com/identity/claims/objectidentifier";

        public static string? GetId(this ClaimsPrincipal? user)
        {
            return user?.FindFirstValue(ObjectIdentitifier) ?? user?.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public static string? GetEmail(this ClaimsPrincipal? user)
        {
            return user?.FindFirstValue(JwtClaimTypes.Email) ?? user?.FindFirstValue(JwtClaimTypes.PreferredUserName);
        }

        public static string? GetLastName(this ClaimsPrincipal? user)
        {
            return user?.FindFirstValue(JwtClaimTypes.FamilyName) ??
                   user?.FindFirstValue(JwtClaimTypes.Name) ?? user?.Identity?.Name;
        }

        public static string? GetFirstName(this ClaimsPrincipal? user)
        {
            return user?.FindFirstValue(JwtClaimTypes.GivenName);
        }

        public static string? GetUserName(this ClaimsPrincipal? user)
        {
            return user?.FindFirstValue(KissClaimTypes.KissUserNameClaimType);
        }

        public static JsonObject GetMedewerkerIdentificatie(this ClaimsPrincipal? user, int? truncate)
        {
            var userName = user?.GetUserName();
            var lastName = user?.GetLastName();
            var firstName = user?.GetFirstName();

            if (truncate.HasValue)
            {
                userName = Truncate(userName, truncate.Value);
            }

            return new JsonObject
            {
                ["achternaam"] = Truncate(lastName, 200) ?? "",
                ["identificatie"] = userName ?? "",
                ["voorletters"] = Truncate(firstName, 20) ?? "",
                ["voorvoegselAchternaam"] = ""
            };
        }

        private static string? Truncate(string? value, int maxLength)
        {
            return value != null && value.Length > maxLength
                ? value[..maxLength]
                : value;
        }
    }
}

namespace Microsoft.Extensions.DependencyInjection
{
    public delegate bool IsRedacteur(ClaimsPrincipal? user);

    public delegate bool IsKcm(ClaimsPrincipal? user);

    public delegate JsonObject? GetMedewerkerIdentificatie();

    public class PabcAuthOptions
    {
        public string Authority { get; set; } = "";
        public string ClientId { get; set; } = "";
        public string ClientSecret { get; set; } = "";
        public string? KlantcontactmedewerkerRole { get; set; }
        public string? RedacteurRole { get; set; }
        public string? MedewerkerIdentificatieClaimType { get; set; }
        public int? TruncateMedewerkerIdentificatie { get; set; }

        public string? JwtTokenAuthenticationSecret { get; set; }
    }

    public static class AuthenticationSetupExtensions
    {
        private const string SignOutCallback = "/signout-callback-oidc";
        private const string CookieSchemeName = "cookieScheme";
        private const string ChallengeSchemeName = "challengeScheme";


        private static string GetSafeRedirectPath(HttpContext httpContext)
        {
            var redirectPath = "/";

            if (httpContext.Request.Query.TryGetValue("returnUrl", out var returnUrlValues))
            {
                var requestedUrl = returnUrlValues.FirstOrDefault();

                if (!string.IsNullOrEmpty(requestedUrl))
                {
                    if (Uri.TryCreate(requestedUrl, UriKind.Relative, out Uri uri))
                    {
                        if (requestedUrl.StartsWith("/") && !requestedUrl.StartsWith("//") && !requestedUrl.Contains(".."))
                        {
                            redirectPath = requestedUrl;
                        }
                    }
                }
            }
            return redirectPath;
        }
        public static IServiceCollection AddPabcAuth(this IServiceCollection services,
            Action<PabcAuthOptions> setOptions)
        {
            var authOptions = new PabcAuthOptions();
            setOptions(authOptions);

            var klantcontactmedewerkerRole = string.IsNullOrWhiteSpace(authOptions.KlantcontactmedewerkerRole)
                ? "Klantcontactmedewerker"
                : authOptions.KlantcontactmedewerkerRole;
            var redacteurRole = string.IsNullOrWhiteSpace(authOptions.RedacteurRole)
                ? "Redacteur"
                : authOptions.RedacteurRole;
            var userNameClaimType = string.IsNullOrWhiteSpace(authOptions.MedewerkerIdentificatieClaimType)
                ? "email"
                : authOptions.MedewerkerIdentificatieClaimType;


            services.AddSingleton<IsRedacteur>(user => user?.IsInRole(redacteurRole) ?? false);
            services.AddSingleton<IsKcm>(user => user?.IsInRole(klantcontactmedewerkerRole) ?? false);
            services.AddSingleton<GetMedewerkerIdentificatie>(s =>
            {
                var accessor = s.GetRequiredService<IHttpContextAccessor>();
                return () =>
                    accessor.HttpContext?.User?.GetMedewerkerIdentificatie(authOptions.TruncateMedewerkerIdentificatie);
            });

            var authBuilder = services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieSchemeName;
                if (!string.IsNullOrWhiteSpace(authOptions.Authority))
                {
                    options.DefaultChallengeScheme = ChallengeSchemeName;
                }
            }).AddCookie(CookieSchemeName, options =>
            {
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.IsEssential = true;
                options.Cookie.HttpOnly = true;
                // TODO: make configurable?
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.SlidingExpiration = true;
                //options.Events.OnSigningOut = (e) => e.HttpContext.RevokeRefreshTokenAsync();
                options.Events.OnRedirectToAccessDenied = ctx =>
                {
                    ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return Task.CompletedTask;
                };
                options.Events.OnRedirectToLogin = HandleLoggedOut;
            });

            if (!string.IsNullOrWhiteSpace(authOptions.Authority))
            {
                authBuilder.AddOpenIdConnect(ChallengeSchemeName, options =>
                {
                    options.NonceCookie.HttpOnly = true;
                    options.NonceCookie.IsEssential = true;
                    options.NonceCookie.SameSite = SameSiteMode.None;
                    options.NonceCookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.CorrelationCookie.HttpOnly = true;
                    options.CorrelationCookie.IsEssential = true;
                    options.CorrelationCookie.SameSite = SameSiteMode.None;
                    options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;

                    options.Authority = authOptions.Authority;
                    options.ClientId = authOptions.ClientId;
                    options.ClientSecret = authOptions.ClientSecret;
                    options.SignedOutRedirectUri = SignOutCallback;
                    options.ResponseType = OidcConstants.ResponseTypes.Code;
                    options.UsePkce = true;
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.ClaimActions.MapJsonKey(KissClaimTypes.KissUserNameClaimType, userNameClaimType);
                    options.Scope.Clear();
                    options.Scope.Add(OidcConstants.StandardScopes.OpenId);
                    options.Scope.Add(OidcConstants.StandardScopes.Profile);
                    //options.Scope.Add(OidcConstants.StandardScopes.OfflineAccess);
                    //options.SaveTokens = true;

                    options.Events.OnRemoteFailure = RedirectToRoot;
                    options.Events.OnSignedOutCallbackRedirect = RedirectToRoot;
                    options.Events.OnRedirectToIdentityProvider = ctx =>
                    {
                        if (ctx.Request.Headers.ContainsKey("is-api"))
                        {
                            ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            ctx.Response.Headers.Location = ctx.ProtocolMessage.CreateAuthenticationRequestUrl();
                            ctx.HandleResponse();
                        }

                        return Task.CompletedTask;
                    };
                });
            }


            if (authOptions.JwtTokenAuthenticationSecret != null)
            {
                authBuilder.AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(authOptions.JwtTokenAuthenticationSecret))
                    };
                });
            }


            services.AddDistributedMemoryCache();
            services.AddOpenIdConnectAccessTokenManagement();

            services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireRole(klantcontactmedewerkerRole)
                    .Build();

                options.AddPolicy(Policies.KcmOrRedactiePolicy, new AuthorizationPolicyBuilder()
                    .RequireRole(klantcontactmedewerkerRole, redacteurRole)
                    .Build());

                options.AddPolicy(Policies.RedactiePolicy,
                    new AuthorizationPolicyBuilder()
                        .RequireRole(redacteurRole)
                        .Build());

                //endpoints beschermd met een authorize attribuut, met de policy extrensysteem,
                //worden geautoriseerd adhv het jwt token scheme
                options.AddPolicy(Policies.ExternSysteemPolicy, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                });
            });

            return services;
        }

        public static IApplicationBuilder UsePabcAuthMiddlewares(this IApplicationBuilder app)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.All });

            app.Use((context, next) =>
            {
                if (context.Request.Headers["x-forwarded-proto"] == "https")
                {
                    context.Request.Scheme = "https";
                }

                return next();
            });

            app.Use(StrictSameSiteExternalAuthenticationMiddleware);

            return app;
        }

        public static IEndpointRouteBuilder MapKissAuthEndpoints(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("api/logoff", LogoffAsync).AllowAnonymous();
            endpoints.MapGet("api/me", GetMe).AllowAnonymous();
            endpoints.MapGet("api/challenge", ChallengeAsync).AllowAnonymous();

            return endpoints;
        }

        private static Task RedirectToRoot<TOptions>(HandleRequestContext<TOptions> context)
            where TOptions : AuthenticationSchemeOptions
        {
            context.Response.Redirect("/");
            context.HandleResponse();

            return Task.CompletedTask;
        }

        public static Task HandleLoggedOut<TOptions>(RedirectContext<TOptions> ctx)
            where TOptions : AuthenticationSchemeOptions
        {
            if (ctx.Request.Headers.ContainsKey("is-api"))
            {
                ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                ctx.Response.Headers.Location = ctx.RedirectUri;
            }

            return Task.CompletedTask;
        }

        private static async Task LogoffAsync(HttpContext httpContext)
        {
            await httpContext.SignOutAsync(CookieSchemeName);
            await httpContext.SignOutAsync(ChallengeSchemeName);
        }

        private static PabcUser GetMe(HttpContext httpContext)
        {
            var isLoggedIn = httpContext.User.Identity?.IsAuthenticated ?? false;
            var email = httpContext.User.GetEmail();
            var isKcm = httpContext.RequestServices.GetService<IsKcm>()?.Invoke(httpContext.User) ?? false;
            var isRedacteur = httpContext.RequestServices.GetService<IsRedacteur>()?.Invoke(httpContext.User) ?? false;

            var organisatieIds = httpContext.RequestServices
                                     .GetService<IConfiguration>()
                                     ?["ORGANISATIE_IDS"]
                                     ?.Split('/')
                                 ?? Array.Empty<string>();

            return new PabcUser(email, isLoggedIn, isKcm, isRedacteur, organisatieIds);
        }


        private static Task ChallengeAsync(HttpContext httpContext)
        {

            var safeRedirectPath = GetSafeRedirectPath(httpContext);

            if (httpContext.User.Identity?.IsAuthenticated ?? false)
            {
                httpContext.Response.Redirect(safeRedirectPath);
                return Task.CompletedTask;
            }

            return httpContext.ChallengeAsync(new AuthenticationProperties { RedirectUri = safeRedirectPath });
        }

        private static async Task StrictSameSiteExternalAuthenticationMiddleware(HttpContext ctx, RequestDelegate next)
        {
            var schemes = ctx.RequestServices.GetRequiredService<IAuthenticationSchemeProvider>();
            var handlers = ctx.RequestServices.GetRequiredService<IAuthenticationHandlerProvider>();

            foreach (var scheme in await schemes.GetRequestHandlerSchemesAsync())
            {
                if (await handlers.GetHandlerAsync(ctx, scheme.Name) is IAuthenticationRequestHandler handler &&
                    await handler.HandleRequestAsync())
                {
                    // start same-site cookie special handling
                    string? location = null;
                    if (ctx.Response.StatusCode == 302)
                    {
                        location = ctx.Response.Headers["location"];
                    }
                    else if (ctx.Request.Method == "GET" && !ctx.Request.Query["skip"].Any())
                    {
                        location = ctx.Request.Path + ctx.Request.QueryString + "&skip=1";
                    }

                    if (location != null)
                    {
                        ctx.Response.ContentType = "text/html";
                        ctx.Response.StatusCode = 200;
                        var html = $@"
                        <html><head>
                            <meta http-equiv='refresh' content='0;url={location}' />
                        </head></html>";
                        await ctx.Response.WriteAsync(html);
                    }
                    // end same-site cookie special handling

                    return;
                }
            }

            await next(ctx);
        }

        private readonly record struct PabcUser(
            string? Email,
            bool IsLoggedIn,
            bool IsKcm,
            bool IsRedacteur,
            IReadOnlyList<string> OrganisatieIds);
    }
}
