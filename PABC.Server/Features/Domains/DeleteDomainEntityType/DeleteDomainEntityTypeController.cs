using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PABC.Data;

namespace PABC.Server.Features.Domains.DeleteDomainEntityType
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("/api/v1/domains/{domainId}/entity-types")]
    public class DeleteDomainEntityTypeController(PabcDbContext db) : Controller
    {
        [HttpDelete("{entityTypeId}")]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, MediaTypeNames.Application.ProblemJson)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.ProblemJson)]
        public async Task<IActionResult> DeleteDomainEntityType(Guid domainId, Guid entityTypeId, CancellationToken token = default)
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

                var entityType = domain.EntityTypes.FirstOrDefault(et => et.Id == entityTypeId);

                if (entityType == null)
                {
                    return NotFound(new ProblemDetails
                    {
                        Detail = "Entiteitstype niet gevonden in dit domein",
                        Status = StatusCodes.Status404NotFound
                    });
                }

                domain.EntityTypes.Remove(entityType);

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
