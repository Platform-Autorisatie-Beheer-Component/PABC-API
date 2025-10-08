using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PABC.Data;
using PABC.Data.Entities;

namespace PABC.Server.Features.FunctionalRoles.PutFunctionalRole
{
    [ApiController]
    [Route("/api/v1/functional-roles")]
    public class PutFunctionalRoleController(PabcDbContext db) : Controller
    {
        [HttpPut("{id}")]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.ProblemJson)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, MediaTypeNames.Application.ProblemJson)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict, MediaTypeNames.Application.ProblemJson)]
        [ProducesResponseType<FunctionalRole>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.ProblemJson)]
        public async Task<IActionResult> PutFunctionalRole(Guid id, [FromBody] FunctionalRoleUpsertModel model, CancellationToken token = default)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

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

                functionalRole.Name = model.Name;

                db.FunctionalRoles.Update(functionalRole);
                
                await db.SaveChangesAsync(token);

                return Ok(functionalRole);
            }
            catch (DbUpdateException ex) when (ex.IsDuplicateException())
            {
                return Conflict(new ProblemDetails
                {
                    Detail = "Functionele rolnaam bestaat al",
                    Status = StatusCodes.Status409Conflict
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
