using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PABC.Data;

namespace PABC.Server.Features.FunctionalRoles.DeleteFunctionalRole
{
    [ApiController]
    [Route("/api/v1/functional-roles")]
    public class DeleteFunctionalRoleController(PabcDbContext db) : Controller
    {
        [HttpDelete("{id}")]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, MediaTypeNames.Application.ProblemJson)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteFunctionalRole(Guid id, CancellationToken token = default)
        {
            var functionalRole = await db.FunctionalRoles.FindAsync(id, token);

            if (functionalRole == null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Functional Role Not Found",
                    Status = StatusCodes.Status404NotFound
                });
            }

            await db.FunctionalRoles.Where(d => d.Id == id).ExecuteDeleteAsync(token);

            return NoContent();
        }
    }
}
