using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PABC.Data;
using PABC.Data.Entities;

namespace PABC.Server.Features.Domains.GetDomains
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("/api/v1/domains")]
    public class GetDomainsController(PabcDbContext db) : Controller
    {
        [HttpGet]
        [ProducesResponseType<List<Domain>>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
        public async Task<IActionResult> GetDomains([FromQuery] bool includeEntityTypes = false, CancellationToken token = default)
        {
            var query = db.Domains.AsQueryable();

            if (includeEntityTypes)
            {
                query = query.Include(d => d.EntityTypes);
            }

            var domains = await query
                .OrderBy(d => d.Name)
                .Select(d => new DomainEntityTypesResponse
                {
                    Id = d.Id,
                    Name = d.Name,
                    Description = d.Description,
                    EntityTypes = includeEntityTypes
                        ? d.EntityTypes
                            .OrderBy(et => et.Type)
                            .ThenBy(et => et.Name)
                            .Select(et => new EntityTypeResponse
                            {
                                Id = et.Id,
                                Type = et.Type,
                                Name = et.Name
                            })
                            .ToList()
                        : null
                })
                .ToListAsync(token);

            return Ok(domains);
        }
    }

    public class DomainEntityTypesResponse
    {
        public required Guid Id { get; init; }
        public required string Name { get; init; }
        public required string Description { get; init; }
        public List<EntityTypeResponse>? EntityTypes { get; init; }
    }

    public class EntityTypeResponse
    {
        public required Guid Id { get; init; }
        public required string Type { get; init; }
        public required string Name { get; init; }
    }
}
