using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PABC.Data;

namespace PABC.Server.Features.Domains.PostDomainEntityType
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("/api/v1/domains/{domainId}/entity-types")]
    public class PostDomainEntityTypeController(PabcDbContext db) : Controller
    {
        [HttpPost("{entityTypeId}")]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, MediaTypeNames.Application.ProblemJson)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict, MediaTypeNames.Application.ProblemJson)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.ProblemJson)]
        public async Task<IActionResult> PostDomainEntityType(Guid domainId, Guid entityTypeId, CancellationToken token = default)
        {
            try
            {
                var domain = await db.Domains
                    .Include(d => d.EntityTypes)
                    .FirstOrDefaultAsync(d => d.Id == domainId, token);

                if (domain == null)
                {
                    return NotFound(new ProblemDetails
                    {
                        Detail = "Domein niet gevonden",
                        Status = StatusCodes.Status404NotFound
                    });
                }

                var entityType = await db.EntityTypes.FindAsync([entityTypeId], token);

                if (entityType == null)
                {
                    return NotFound(new ProblemDetails
                    {
                        Detail = "Entiteitstype niet gevonden",
                        Status = StatusCodes.Status404NotFound
                    });
                }

                if (domain.EntityTypes.Any(et => et.Id == entityTypeId))
                {
                    return Conflict(new ProblemDetails
                    {
                        Detail = "Entiteitstype is al toegevoegd aan dit domein",
                        Status = StatusCodes.Status409Conflict
                    });
                }

                domain.EntityTypes.Add(entityType);

                await db.SaveChangesAsync(token);

                return NoContent();
            }
            catch
            {
                return StatusCode(500, new ProblemDetails
                {
                    Detail = "Internal Server Error",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }
    }
}
