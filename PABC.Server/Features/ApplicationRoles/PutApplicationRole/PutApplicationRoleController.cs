using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PABC.Data;
using PABC.Data.Entities;

namespace PABC.Server.Features.ApplicationRoles.PutApplicationRole
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("/api/v1/application-roles")]
    public class PutApplicationRoleController(PabcDbContext db) : Controller
    {
        [HttpPut("{id}")]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.ProblemJson)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, MediaTypeNames.Application.ProblemJson)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict, MediaTypeNames.Application.ProblemJson)]
        [ProducesResponseType<ApplicationRole>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.ProblemJson)]
        public async Task<IActionResult> PutApplicationRole(Guid id, [FromBody] ApplicationRole model, CancellationToken token = default)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            try
            {
                var applicationRole = await db.ApplicationRoles.FindAsync([id], token);

                if (applicationRole == null)
                {
                    return NotFound(new ProblemDetails
                    {
                        Detail = "Applicatierol niet gevonden",
                        Status = StatusCodes.Status404NotFound
                    });
                }

                applicationRole.Name = model.Name;
                applicationRole.ApplicationId = model.ApplicationId;

                db.ApplicationRoles.Update(applicationRole);
                
                await db.SaveChangesAsync(token);

                return Ok(applicationRole);
            }
            catch (DbUpdateException ex) when (ex.IsDuplicateException())
            {
                return Conflict(new ProblemDetails
                {
                    Detail = "Combinatie Naam en Applicatie bestaat al",
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
