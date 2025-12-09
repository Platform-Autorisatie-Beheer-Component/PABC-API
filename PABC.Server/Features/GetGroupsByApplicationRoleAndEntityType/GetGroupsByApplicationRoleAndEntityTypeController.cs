using System.Data;
using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PABC.Data;
using PABC.Server.Auth;
using PABC.Server.Keycloak;

namespace PABC.Server.Features.GetGroupsByApplicationRoleAndEntityType
{
    [ApiController]
    [Route("/api/v1/groups")]
    [Authorize(Policy = ApiKeyAuthentication.Policy)]
    public class GetGroupsByApplicationRoleAndEntityTypeController(IKeycloakAdminClient keycloakClient, PabcDbContext db) : ControllerBase
    {
        [HttpGet(Name = "Get groups by application role and entity type")]
        [ProducesResponseType<GetGroupsByApplicationRoleAndEntityTypeResponse>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest,
            MediaTypeNames.Application.ProblemJson)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized,
            MediaTypeNames.Application.ProblemJson)]
        public async Task<ActionResult<GetGroupsByApplicationRoleAndEntityTypeResponse>> Get([FromQuery] GetGroupsByApplicationRoleAndEntityTypeRequest request, CancellationToken token)
        {
            var functionalRoles = db.Mappings
                .Where(m => m.ApplicationRole.Name == request.ApplicationRoleName
                    && m.ApplicationRole.Application.Name == request.ApplicationName
                    && (m.Domain!.EntityTypes.Any(e => e.EntityTypeId == request.EntityTypeId && e.Type == request.EntityType) 
                        || m.IsAllEntityTypes))
                .Select(m => m.FunctionalRole.Name)
                .Distinct()
                .AsAsyncEnumerable()
                .WithCancellation(token);

            var groups = new SortedDictionary<string, GroupRepresentation>(StringComparer.OrdinalIgnoreCase);

            await foreach (var role in functionalRoles)
            {
                await foreach (var group in keycloakClient.GetGroups(role, token))
                {
                    groups[group.Name] = group;
                }
            }

            return new GetGroupsByApplicationRoleAndEntityTypeResponse { Groups = groups.Values };
        }
    }

    public class GetGroupsByApplicationRoleAndEntityTypeRequest
    {
        /// <summary>The name of the application.</summary>
        /// <example>Zaakafhandelcomponent</example>
        [FromQuery(Name = "application-name")]
        public required string ApplicationName { get; init; }

        /// <summary>The name of the application role.</summary>
        /// <example>behandelaar</example>
        [FromQuery(Name = "application-role-name")]
        public required string ApplicationRoleName { get; init; }

        /// <summary>The ID of the entity type.</summary>
        /// <example>melding-klein-kansspel</example>
        [FromQuery(Name = "entity-type-id")]
        public required string EntityTypeId { get; init; }

        /// <summary>The kind of entity type.</summary>
        /// <example>zaaktype</example>
        [FromQuery(Name = "entity-type")]
        public required string EntityType { get; init; }
    }

    public record GetGroupsByApplicationRoleAndEntityTypeResponse
    {
        public required IReadOnlyCollection<GroupRepresentation> Groups { get; init; }
    }
}
