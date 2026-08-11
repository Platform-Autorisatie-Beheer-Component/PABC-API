using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PABC.Data;
using PABC.Data.Entities;
using PABC.Server.Keycloak;

namespace PABC.Server.Features.FunctionalRoles.ImportKeycloakRoles
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("/api/v1/functional-roles/import-keycloak")]
    public class ImportKeycloakRolesController(PabcDbContext db, IKeycloakAdminClient keycloakClient) : Controller
    {
        [HttpPost]
        [ProducesResponseType<ImportKeycloakRolesResponse>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.ProblemJson)]
        public async Task<IActionResult> ImportKeycloakRoles(CancellationToken token = default)
        {
            try
            {
                var realmRoles = await keycloakClient.GetRealmRoles(token);
                var roleNames = realmRoles.Select(r => r.Name).ToList();

                var existingRoles = await db.FunctionalRoles
                    .Select(r => r.Name)
                    .ToListAsync(token);

                // Case-insensitive: FunctionalRole.Name has a unique index with nl_case_insensitive collation
                var existingSet = new HashSet<string>(existingRoles, StringComparer.OrdinalIgnoreCase);

                var created = new List<string>();
                var skipped = new List<string>();

                foreach (var roleName in roleNames)
                {
                    // Add returns false if already present; also prevents duplicates within the same batch
                    if (!existingSet.Add(roleName))
                    {
                        skipped.Add(roleName);
                        continue;
                    }

                    db.FunctionalRoles.Add(new FunctionalRole
                    {
                        Id = Guid.NewGuid(),
                        Name = roleName
                    });

                    created.Add(roleName);
                }

                await db.SaveChangesAsync(token);

                var keycloakSet = new HashSet<string>(roleNames, StringComparer.OrdinalIgnoreCase);

                var stale = existingSet
                    .Where(name => !keycloakSet.Contains(name))
                    .ToList();

                return Ok(new ImportKeycloakRolesResponse
                {
                    Created = created,
                    Skipped = skipped,
                    Stale = stale
                });
            }
            catch (Exception ex) when (ex is HttpRequestException or InvalidOperationException)
            {
                return StatusCode(500, new ProblemDetails
                {
                    Detail = $"Fout bij het ophalen van rollen uit Keycloak: {ex.Message}",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }
    }

    public class ImportKeycloakRolesResponse
    {
        public required List<string> Created { get; init; }
        public required List<string> Skipped { get; init; }
        public required List<string> Stale { get; init; }
    }
}
