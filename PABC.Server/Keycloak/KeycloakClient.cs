using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using PABC.Server.Auth;

namespace PABC.Server.Keycloak
{
    public class KeycloakClient(HttpClient httpClient)
    {
        public async IAsyncEnumerable<GroupBase> GetGroups(IReadOnlySet<string> roles, [EnumeratorCancellation] CancellationToken token)
        {
            using var response = await httpClient.GetAsync($"groups?briefRepresentation=false", token);
            if (!response.IsSuccessStatusCode)
            {
                var str = await response.Content.ReadAsStringAsync(token);
                Console.WriteLine(str);
            }
            response.EnsureSuccessStatusCode();
            await foreach (var item in response.Content.ReadFromJsonAsAsyncEnumerable<GroupRepresentation>(cancellationToken: token))
            {
                if (item is not null && item.ClientRoles.Values.Any(x=> x.Any(r => roles.Contains(r, StringComparer.OrdinalIgnoreCase))))
                {
                    yield return item;
                }
            }
        }

        private record KeycloakClientResponse(Guid Id);

        private record GroupRepresentation(string Id, string Name, string Description, Dictionary<string, string[]> Attributes, Dictionary<string, string[]> ClientRoles) : GroupBase(Id, Name, Description, Attributes);
    }


    public record GroupBase(string Id, string Name, string Description, Dictionary<string, string[]> Attributes);
    
}
