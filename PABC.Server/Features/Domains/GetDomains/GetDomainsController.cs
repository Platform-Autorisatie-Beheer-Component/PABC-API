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
        [ProducesResponseType<List<DomainEntityTypesResponse>>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
        public async Task<IActionResult> GetDomains(CancellationToken token = default)
        {
            var domains = await db.Domains
                .Include(d => d.EntityTypes)
                .OrderBy(d => d.Name)
                .Select(d => new DomainEntityTypesResponse
                {
                    Id = d.Id,
                    Name = d.Name,
                    Description = d.Description,
                    EntityTypes = d.EntityTypes
                        .OrderBy(et => et.Type)
                        .ThenBy(et => et.Name)
                        .Select(et => et.Id)
                        .ToList()
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
        public required List<Guid> EntityTypes { get; init; }
    }
}
