using System.Data.Common;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PABC.Data;

namespace PABC.Server.Features.Applications.DeleteApplication
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("/api/v1/applications")]
    public class DeleteApplicationController(PabcDbContext db) : Controller
    {
        [HttpDelete("{id}")]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, MediaTypeNames.Application.ProblemJson)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity, MediaTypeNames.Application.ProblemJson)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.ProblemJson)]
        public async Task<IActionResult> DeleteApplication(Guid id, CancellationToken token = default)
        {
            try
            {
                var application = await db.Applications.FindAsync([id], token);

                if (application == null)
                {
                    return NotFound(new ProblemDetails
                    {
                        Detail = "Applicatie niet gevonden",
                        Status = StatusCodes.Status404NotFound
                    });
                }

                await db.Applications.Where(d => d.Id == id).ExecuteDeleteAsync(token);

                return NoContent();
            }
            catch (DbException ex) when (ex.IsForeignKeyException())
            {
                return UnprocessableEntity(new ProblemDetails
                {
                    Detail = "Applicatie kan niet worden verwijderd vanwege bestaande verwijzingen.",
                    Status = StatusCodes.Status422UnprocessableEntity
                });
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
