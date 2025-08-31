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
            var allEntityTypesQuery = from m in db.Mappings
                    .Include(m => m.ApplicationRole)
                    .Include(m => m.FunctionalRole)
                    .Include(m => m.Domain)
                    .Where(x => request.FunctionalRoleNames.Contains(x.FunctionalRole.Name) && x.IsAllEntityTypes)
                from e in db.EntityTypes
                select new { m, e };

            var domainSpecificQuery = from m in db.Mappings
                    .Include(m => m.ApplicationRole)
                    .Include(m => m.FunctionalRole)
                    .Include(m => m.Domain)
                    .Where(x => request.FunctionalRoleNames.Contains(x.FunctionalRole.Name) && !x.IsAllEntityTypes)
                from e in m.Domain!.EntityTypes
                select new { m, e };

            var combinedQuery = allEntityTypesQuery.Union(domainSpecificQuery);

            var rawResults = await combinedQuery.ToListAsync(token);

            var results = rawResults
                .GroupBy(x => new { x.m.Id, x.m.ApplicationRole.Name, x.m.ApplicationRole.Application })
                .Select(g => new GetApplicationRolesResponseModel
                {
                    EntityTypes = g.Select(x => new EntityTypeModel
                        {
                            Id = x.e.Id.ToString(), Type = x.e.Type, Name = x.e.Name
                        })
                        .DistinctBy(et => et.Id)
                        .ToList(),
                    ApplicationRole = new ApplicationRoleModel { Name = g.Key.Name, Application = g.Key.Application }
                })
                .ToList();

            return new GetApplicationRolesResponse { Results = results };
        }
    }
}
