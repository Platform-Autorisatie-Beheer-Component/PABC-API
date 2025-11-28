using System.Data;
using System.Net.Mime;
using System.Runtime.CompilerServices;
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
    public class GetGroupsByApplicationRoleAndEntityTypeController(KeycloakClient keycloakClient, PabcDbContext db) : ControllerBase
    {
        [HttpGet(Name = "Get groups by application role and entity type")]
        [ProducesResponseType<GetGroupsByApplicationRoleAndEntityTypeResponse>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest,
            MediaTypeNames.Application.ProblemJson)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized,
            MediaTypeNames.Application.ProblemJson)]
        public async Task<ActionResult<GetGroupsByApplicationRoleAndEntityTypeResponse>> Get([FromQuery] GetGroupsByApplicationRoleAndEntityTypeRequest request, CancellationToken token)
        {
            return new GetGroupsByApplicationRoleAndEntityTypeResponse(
                GetGroups(request, token)
            );
        }

        private async IAsyncEnumerable<GroupBase> GetGroups(GetGroupsByApplicationRoleAndEntityTypeRequest request, [EnumeratorCancellation] CancellationToken token)
        {
            var functionalRoles = db.Mappings
                .Where(m => m.ApplicationRole.Name == request.ApplicationRoleName
                    && m.ApplicationRole.Application.Name == request.ApplicationName
                    && m.Domain!.EntityTypes.Any(e => e.EntityTypeId == request.EntityTypeId))
                .Select(m => m.FunctionalRole.Name)
                .Distinct()
                .AsAsyncEnumerable();

            var groupIds = new HashSet<string>();
            var roles = new HashSet<string>();

            await foreach (var role in functionalRoles)
            {
                roles.Add(role);
            }

            await foreach (var group in keycloakClient.GetGroups(roles, token))
            {
                if (groupIds.Add(group.Id))
                {
                    yield return group;
                }
            }
        }
    }

    public class GetGroupsByApplicationRoleAndEntityTypeRequest
    {
        /// <summary>
        /// 
        /// </summary>
        /// <example>ZAC</example>
        [FromQuery(Name = "application-name")]
        public required string ApplicationName { get; init; }

        /// <example>Raadpleger</example>
        [FromQuery(Name = "application-role-name")]
        public required string ApplicationRoleName { get; init; }

        /// <example>melding-klein-kansspel</example>
        [FromQuery(Name = "entity-type-id")]
        public required string EntityTypeId { get; init; }
    }

    public record GetGroupsByApplicationRoleAndEntityTypeResponse
    (
        IAsyncEnumerable<GroupBase> Groups
    );
}
