using System.Data.Common;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PABC.Data;

namespace PABC.Server.Features.Domains.DeleteDomain
{
    [ApiController]
    [Route("/api/v1/domains")]
    public class DeleteDomainController(PabcDbContext db) : Controller
    {
        [HttpDelete("{id}")]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, MediaTypeNames.Application.ProblemJson)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity, MediaTypeNames.Application.ProblemJson)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.ProblemJson)]
        public async Task<IActionResult> DeleteDomain(Guid id, CancellationToken token = default)
        {
            try
            {
                var domain = await db.Domains.FindAsync([id], token);

            if (domain == null)
            {
                return NotFound(new ProblemDetails
                {
                    Detail = "Domein niet gevonden",
                    Status = StatusCodes.Status404NotFound
                });
            }

                await db.Domains.Where(d => d.Id == id).ExecuteDeleteAsync(token);

                return NoContent();
            }
            catch (DbException ex) when (ex.IsForeignKeyException())
            {
                return UnprocessableEntity(new ProblemDetails
                {
                    Title = "Cannot delete referenced Domain",
                    Status = StatusCodes.Status422UnprocessableEntity
                });
            }
            catch
            {
                return StatusCode(500, new ProblemDetails
                {
                    Title = "Internal Server Error",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }
    }
}
