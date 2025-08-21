using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PABC.Data;
using PABC.Server.Auth;

namespace PABC.Server.Features.GetApplicationRolesPerEntityType;

[ApiController]
[Route("/api/v1/application-roles-per-entity-type")]
[Authorize(Policy = ApiKeyAuthentication.Policy)]
public class GetApplicationRolesPerEntityTypeController(PabcDbContext db) : ControllerBase
{
    [HttpPost(Name = "Get application roles per entity type")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType<GetApplicationRolesResponse>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.ProblemJson)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status401Unauthorized, MediaTypeNames.Application.ProblemJson)]
    public async Task<ActionResult<GetApplicationRolesResponse>> Post([FromBody] GetApplicationRolesRequest request, CancellationToken token = default)
    {
        var query = db.Mappings
            .Where(x => request.FunctionalRoleNames.Contains(x.FunctionalRole.Name)) // search for matching function roles, unknown function roles get ignored
            .SelectMany(x => x.Domain.EntityTypes, (m, e) => new { ApplicationRoleName = m.ApplicationRole.Name, m.ApplicationRole.Application, e.Id, e.Type, e.EntityTypeId, EntityTypeName = e.Name });

        var result = new Dictionary<Guid, GetApplicationRolesResponseModel>();

        await foreach (var item in query.AsAsyncEnumerable())
        {
            if (!result.TryGetValue(item.Id, out var val))
            {
                val = new()
                {
                    ApplicationRoles = [],
                    EntityType = new()
                    {
                        Id = item.EntityTypeId,
                        Type = item.Type,
                        Name = item.EntityTypeName,
                    }
                };
                result.Add(item.Id, val);
            }

            var isApplicationRoleNotInList = val.ApplicationRoles.Find(x => x.Name == item.ApplicationRoleName && x.Application == item.Application) == null;
            if (isApplicationRoleNotInList)
            {
                val.ApplicationRoles.Add(new()
                {
                    Application = item.Application,
                    Name = item.ApplicationRoleName
                });
            }
        }

        return new GetApplicationRolesResponse() { Results = result.Values };
    }
}
