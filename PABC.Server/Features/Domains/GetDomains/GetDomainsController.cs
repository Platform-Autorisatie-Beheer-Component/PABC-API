using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PABC.Data;
using PABC.Data.Entities;

namespace PABC.Server.Features.Domains.GetDomains
{
    [ApiController]
    [Route("/api/v1/domains")]
    public class GetDomainsController(PabcDbContext db) : Controller
    {
        [HttpGet]
        [ProducesResponseType<List<Domain>>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
        public async Task<IActionResult> GetDomains(CancellationToken token = default)
        {
            var domains = await db.Domains.ToListAsync(token);

            return Ok(domains);
        }
    }
}
