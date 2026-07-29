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
            var records = await db.Mappings
                .Where(m => request.FunctionalRoleNames.Contains(m.FunctionalRole.Name))
                // left join with Domain, then left join with EntityType
                .SelectMany(x => x.Domain!.EntityTypes.DefaultIfEmpty(), (x, e) => new
                {
                    ApplicationName = x.ApplicationRole.Application.Name,
                    ApplicationRole = x.ApplicationRole.Name,
                    EntityType = e,
                    x.IsAllEntityTypes
                })
                .ToListAsync(token);

            // performance: don't get all entity types from db if not necessary
            var allEntityTypes = records.Any(x => x.IsAllEntityTypes)
                ? await db.EntityTypes.ToArrayAsync(token)
                : [];

            // For records with IsAllEntityTypes = true, create a record for each entity type
            var allEntityTypeRecords = records.Where(x => x.IsAllEntityTypes)
                .SelectMany(x => allEntityTypes, (x, e) => new
                {
                    x.ApplicationName,
                    x.ApplicationRole,
                    EntityType = (EntityType?)e,
                    x.IsAllEntityTypes
                });

            var regularRecords = records.Where(x => !x.IsAllEntityTypes);

            var groupedResults = regularRecords
                .Concat(allEntityTypeRecords)
                .GroupBy(x => x.EntityType)
                .Select(g => new GetApplicationRolesResponseModel
                {
                    EntityType = g.Key == null
                        ? null
                        : new EntityTypeModel { Id = g.Key.EntityTypeId, Type = g.Key.Type, Name = g.Key.Name },
                    ApplicationRoles = g
                        .Select(x => new ApplicationRoleModel
                        {
                            Name = x.ApplicationRole,
                            Application = x.ApplicationName,
                        })
                        .DistinctBy(ar => new { ar.Name, ar.Application })
                        .ToList()
                })
                .ToList();

            return new GetApplicationRolesResponse { Results = groupedResults };
        }
    }
}
