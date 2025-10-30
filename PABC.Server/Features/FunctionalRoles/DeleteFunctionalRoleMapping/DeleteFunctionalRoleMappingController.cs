using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PABC.Data;

namespace PABC.Server.Features.FunctionalRoles.DeleteFunctionalRoleMapping
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("/api/v1/functional-roles/{functionalRoleId}/mappings")]
    public class DeleteFunctionalRoleMappingController(PabcDbContext db) : Controller
    {
        [HttpDelete("{mappingId}")]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, MediaTypeNames.Application.ProblemJson)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.ProblemJson)]
        public async Task<IActionResult> DeleteFunctionalRoleMapping(Guid functionalRoleId, Guid mappingId, CancellationToken token = default)
        {
            try
            {
                var functionalRole = await db.FunctionalRoles.FindAsync([functionalRoleId], token);

                if (functionalRole == null)
                {
                    return NotFound(new ProblemDetails
                    {
                        Detail = "Functionele rol niet gevonden",
                        Status = StatusCodes.Status404NotFound
                    });
                }

                var mapping = await db.Mappings
                    .FirstOrDefaultAsync(m => m.Id == mappingId && m.FunctionalRoleId == functionalRoleId, token);

                if (mapping == null)
                {
                    return NotFound(new ProblemDetails
                    {
                        Detail = "Mapping niet gevonden voor deze functionele rol",
                        Status = StatusCodes.Status404NotFound
                    });
                }

                db.Mappings.Remove(mapping);

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
