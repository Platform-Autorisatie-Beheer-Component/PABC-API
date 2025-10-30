using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PABC.Data;
using PABC.Data.Entities;

namespace PABC.Server.Features.FunctionalRoles.GetFunctionalRoles
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("/api/v1/functional-roles")]
    public class GetFunctionalRolesController(PabcDbContext db) : Controller
    {
        [HttpGet]
        [ProducesResponseType<List<FunctionalRole>>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
        public async Task<IActionResult> GetFunctionalRoles(CancellationToken token = default)
        {
            var functionalRoles = await db.FunctionalRoles
                .OrderBy(d => d.Name)
                .Select(fr => new FunctionalRoleMappingsResponse
                {
                    Id = fr.Id,
                    Name = fr.Name,
                    Mappings = db.Mappings
                        .Include(m => m.ApplicationRole)
                        .Include(m => m.Domain)
                        .Where(m => m.FunctionalRoleId == fr.Id)
                        .OrderBy(m => m.ApplicationRole.Application)
                        .ThenBy(m => m.ApplicationRole.Name)
                        .ThenBy(m => m.Domain!.Name)
                        .Select(m => new MappingResponse
                        {
                            Id = m.Id,
                            Name = $"{m.ApplicationRole.Name} ({m.ApplicationRole.Application})",
                            Domain = m.Domain != null ? m.Domain.Name : null,
                            IsAllEntityTypes = m.IsAllEntityTypes
                        })
                        .ToList()
                })
                .ToListAsync(token);

            return Ok(functionalRoles);
        }
    }

    public class FunctionalRoleMappingsResponse
    {
        public required Guid Id { get; init; }
        public required string Name { get; init; }
        public required List<MappingResponse> Mappings { get; init; }
    }

    public class MappingResponse
    {
        public required Guid Id { get; init; }
        public required string Name { get; init; }
        public string? Domain { get; init; }
        public required bool IsAllEntityTypes { get; init; }
    }
}
