using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using PABC.Data;
using PABC.Data.Entities;

namespace PABC.Server.Features.Domains
{
    [ApiController]
    [Route("/api/v1/domains")]
    public class GetDomainController(PabcDbContext db) : Controller
    {
        [HttpGet("{id}")]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.ProblemJson)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, MediaTypeNames.Application.ProblemJson)]
        [ProducesResponseType<Domain>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
        public async Task<IActionResult> GetDomainById(string id, CancellationToken token = default)
        {
            if (!Guid.TryParse(id, out var guid))
            {
                return BadRequest(new ValidationProblemDetails
                {
                    Title = "Invalid Domain Id",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            var domain = await db.Domains.FindAsync(guid, token);

            if (domain == null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Domain Not Found",
                    Status = StatusCodes.Status404NotFound
                });
            }

            return Ok(domain);
        }
    }
}
