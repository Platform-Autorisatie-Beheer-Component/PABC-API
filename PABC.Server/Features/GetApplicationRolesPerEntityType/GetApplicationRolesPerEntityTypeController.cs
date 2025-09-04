using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PABC.Data;
using PABC.Server.Auth;

namespace PABC.Server.Features.GetApplicationRolesPerEntityType
{
    [ApiController]
    [Route("/api/v1/application-roles-per-entity-type")]
    [Authorize(Policy = ApiKeyAuthentication.Policy)]
    public class GetApplicationRolesPerEntityTypeController(PabcDbContext db) : ControllerBase
    {
        [HttpPost(Name = "Get application roles per entity type")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType<GetApplicationRolesResponse>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest,
            MediaTypeNames.Application.ProblemJson)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized,
            MediaTypeNames.Application.ProblemJson)]
        public async Task<ActionResult<GetApplicationRolesResponse>> Post([FromBody] GetApplicationRolesRequest request,
            CancellationToken token = default)
        {
            var allEntityTypesList = await (
                from m in db.Mappings
                    .Include(m => m.ApplicationRole)
                    .Include(m => m.FunctionalRole)
                    .Include(m => m.Domain)
                where request.FunctionalRoleNames.Contains(m.FunctionalRole.Name) && m.IsAllEntityTypes
                from e in db.EntityTypes
                select new { EntityType = e, ApplicationRole = m.ApplicationRole }
            ).ToListAsync(token);

            // Fetch domain-specific mappings — apply only to entity types within the domain
            var domainSpecificList = await (
                from m in db.Mappings
                    .Include(m => m.ApplicationRole)
                    .Include(m => m.FunctionalRole)
                    .Include(m => m.Domain)
                where request.FunctionalRoleNames.Contains(m.FunctionalRole.Name) && !m.IsAllEntityTypes
                from e in m.Domain!.EntityTypes
                select new { EntityType = e, ApplicationRole = m.ApplicationRole }
            ).ToListAsync(token);

            // Combine results
            var rawResults = allEntityTypesList.Concat(domainSpecificList).ToList();

            // Group by EntityType and project
            var groupedResults = rawResults
                .GroupBy(x => new { x.EntityType.Id, x.EntityType.Type, x.EntityType.Name })
                .Select(g => new GetApplicationRolesResponseModel
                {
                    EntityType = new EntityTypeModel { Id = g.Key.Id.ToString(), Type = g.Key.Type, Name = g.Key.Name },
                    ApplicationRoles = g
                        .Select(x => new ApplicationRoleModel
                        {
                            Name = x.ApplicationRole.Name, Application = x.ApplicationRole.Application
                        })
                        .DistinctBy(ar => new { ar.Name, ar.Application })
                        .ToList()
                })
                .ToList();

            return new GetApplicationRolesResponse { Results = groupedResults };
        }
    }
}
