using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PABC.Data;
using PABC.Data.Entities;
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
                select new { EntityType = e, m.ApplicationRole }
            ).ToListAsync(token);

            // Fetch domain-specific mappings — apply only to entity types within the domain
            var domainSpecificList = await (
                from m in db.Mappings
                    .Include(m => m.ApplicationRole)
                    .Include(m => m.FunctionalRole)
                    .Include(m => m.Domain)
                where request.FunctionalRoleNames.Contains(m.FunctionalRole.Name) && !m.IsAllEntityTypes
                from e in m.Domain!.EntityTypes.DefaultIfEmpty()
                select new { EntityType = (EntityType?)e, m.ApplicationRole }
            ).ToListAsync(token);

            // Combine results
            var rawResults = allEntityTypesList.Concat(domainSpecificList).ToList();

            // Group by EntityType and project
            var groupedResults = rawResults
                .GroupBy(x => x.EntityType)
                .Select(g => new GetApplicationRolesResponseModel
                {
                    EntityType = g.Key == null ? null : new EntityTypeModel { Id = g.Key.EntityTypeId, Type = g.Key.Type, Name = g.Key.Name },
                    ApplicationRoles = g
                        .Select(x => new ApplicationRoleModel
                        {
                            Name = x.ApplicationRole.Name, ApplicationId = x.ApplicationRole.ApplicationId
                        })
                        .DistinctBy(ar => new { ar.Name, ar.ApplicationId })
                        .ToList()
                })
                .ToList();

            return new GetApplicationRolesResponse { Results = groupedResults };
        }
    }
}
