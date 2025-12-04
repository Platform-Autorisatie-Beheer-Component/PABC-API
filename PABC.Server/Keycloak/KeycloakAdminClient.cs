using System.Runtime.CompilerServices;
using Duende.AccessTokenManagement;
using Microsoft.Extensions.Options;
using PABC.Server.Auth;

namespace PABC.Server.Keycloak
{
    public interface IKeycloakAdminClient
    {
        IAsyncEnumerable<GroupRepresentation> GetGroups(string role, CancellationToken token);
    }

    public class KeycloakAdminClient(HttpClient httpClient, ILogger<KeycloakAdminClient> logger) : IKeycloakAdminClient
    {
        public async IAsyncEnumerable<GroupRepresentation> GetGroups(string role, [EnumeratorCancellation] CancellationToken token)
        {
            using var response = await httpClient.GetAsync($"roles/{role}/groups?briefRepresentation=false", HttpCompletionOption.ResponseHeadersRead, token);
            
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                yield break;
            }
            
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(token);
                logger.LogError("unexpected result from keycloak while fetching groups for role {Role}: {StatusCode}, {Body}", role, response.StatusCode, body);
            }

            response.EnsureSuccessStatusCode();
            
            await foreach (var item in response.Content.ReadFromJsonAsAsyncEnumerable<GroupRepresentation>(cancellationToken: token))
            {
                if (item is not null)
                {
                    yield return item;
                }
            }
        }
    }


    public record GroupRepresentation(string Name, string Description, Dictionary<string, string[]> Attributes);

    public static class KeycloakClientExtensions
    {
        private static readonly ClientCredentialsClientName s_keycloakAdminClientName = ClientCredentialsClientName.Parse("KeycloakAdmin");

        public static void AddKeycloakAdminClient(this IServiceCollection services, string clientId, string clientSecret)
        {
            services.AddClientCredentialsTokenManagement()
                .AddClient(s_keycloakAdminClientName, client =>
                {
                    client.ClientId = ClientId.Parse(clientId);
                    client.ClientSecret = ClientSecret.Parse(clientSecret);
                });

            services.AddHttpClient<IKeycloakAdminClient, KeycloakAdminClient>()
                .AddClientCredentialsTokenHandler(s_keycloakAdminClientName)
                .ConfigureHttpClient((services, client) =>
                {
                    var authOptions = services.GetRequiredService<AuthOptions>();
                    var uriBuilder = new UriBuilder(authOptions.Authority);
                    uriBuilder.Path = "/admin/" + uriBuilder.Path.TrimStart('/');
                    client.BaseAddress = uriBuilder.Uri;
                });

            services.AddSingleton<IConfigureOptions<ClientCredentialsClient>, ClientConfigurer>();
        }

        private class ClientConfigurer(AuthOptions authOptions) : IConfigureNamedOptions<ClientCredentialsClient>
        {
            public void Configure(string? name, ClientCredentialsClient options) => Configure(options);

            public void Configure(ClientCredentialsClient options)
            {
                options.TokenEndpoint = new Uri($"{authOptions.Authority}protocol/openid-connect/token");
            }
        }
    }

}
