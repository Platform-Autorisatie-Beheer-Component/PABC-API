using System.Data.Common;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PABC.Data;

namespace PABC.Server.Features.EntityTypes.DeleteEntityType
{
    [ApiController]
    [Route("/api/v1/entity-types")]
    public class DeleteEntityTypeController(PabcDbContext db) : Controller
    {
        [HttpDelete("{id}")]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, MediaTypeNames.Application.ProblemJson)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity, MediaTypeNames.Application.ProblemJson)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.ProblemJson)]
        public async Task<IActionResult> DeleteEntityType(Guid id, CancellationToken token = default)
        {
            try
            {
                var entityType = await db.EntityTypes.FindAsync([id], token);

            if (entityType == null)
            {
                return NotFound(new ProblemDetails
                {
                    Detail = "Entiteitstype niet gevonden",
                    Status = StatusCodes.Status404NotFound
                });
            }

                await db.EntityTypes.Where(d => d.Id == id).ExecuteDeleteAsync(token);

                return NoContent();
            }
            catch (DbException ex) when (ex.IsForeignKeyException())
            {
                return UnprocessableEntity(new ProblemDetails
                {
                    Detail = "Entiteitstype kan niet worden verwijderd vanwege bestaande verwijzingen.",
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
