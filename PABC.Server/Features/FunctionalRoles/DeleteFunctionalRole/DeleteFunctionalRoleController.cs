using System.Data.Common;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PABC.Data;

namespace PABC.Server.Features.FunctionalRoles.DeleteFunctionalRole
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("/api/v1/functional-roles")]
    public class DeleteFunctionalRoleController(PabcDbContext db) : Controller
    {
        [HttpDelete("{id}")]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, MediaTypeNames.Application.ProblemJson)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity, MediaTypeNames.Application.ProblemJson)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.ProblemJson)]
        public async Task<IActionResult> DeleteFunctionalRole(Guid id, CancellationToken token = default)
        {
            try
            {
                var functionalRole = await db.FunctionalRoles.FindAsync([id], token);

                if (functionalRole == null)
                {
                    return NotFound(new ProblemDetails
                    {
                        Detail = "Functionele rol niet gevonden",
                        Status = StatusCodes.Status404NotFound
                    });
                }

                await db.FunctionalRoles.Where(d => d.Id == id).ExecuteDeleteAsync(token);

                return NoContent();
            }
            catch (DbException ex) when (ex.IsForeignKeyException())
            {
                return UnprocessableEntity(new ProblemDetails
                {
                    Detail = "Functionele rol kan niet worden verwijderd vanwege bestaande verwijzingen.",
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
