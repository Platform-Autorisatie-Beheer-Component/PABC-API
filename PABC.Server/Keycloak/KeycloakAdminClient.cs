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

    public class KeycloakAdminClient(HttpClient httpClient, AuthOptions authOptions) : IKeycloakAdminClient
    {
        public async IAsyncEnumerable<GroupRepresentation> GetGroups(string role, [EnumeratorCancellation] CancellationToken token)
        {
            var clientGuid = await GetClientGuid(token);
            using var response = await httpClient.GetAsync($"clients/{clientGuid}/roles/{role}/groups?briefRepresentation=false", token);
            if(response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                yield break;
            }
            if (!response.IsSuccessStatusCode)
            {
                var str = await response.Content.ReadAsStringAsync(token);
                Console.WriteLine(str);
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

        private async Task<Guid> GetClientGuid(CancellationToken token)
        {
            using var response = await httpClient.GetAsync($"clients?clientId={authOptions.ClientId}", token);
            response.EnsureSuccessStatusCode();
            var clients = await response.Content.ReadFromJsonAsync<KeycloakClientResponse[]>(cancellationToken: token);
            if (clients is not { Length: > 0 })
            {
                throw new InvalidOperationException($"Client with id '{authOptions.ClientId}' not found in Keycloak.");
            }
            return clients[0].Id;
        }

        private record KeycloakClientResponse(Guid Id);
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
