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
                var functionalRole = await db.FunctionalRoles.FindAsync(id, token);

                if (functionalRole == null)
                {
                    return NotFound(new ProblemDetails
                    {
                        Title = "Functional Role Not Found",
                        Status = StatusCodes.Status404NotFound
                    });
                }


                var duplicateFunctionalRole = await db.FunctionalRoles.FirstOrDefaultAsync(d =>
                    d.Id != id && d.Name.ToLower() == model.Name.ToLower(), token);

                if (duplicateFunctionalRole != null)
                {
                    return Conflict(new ProblemDetails
                    {
                        Title = "Duplicate Functional Role Name",
                        Status = StatusCodes.Status409Conflict
                    });
                }

                functionalRole.Name = model.Name;

                db.FunctionalRoles.Update(functionalRole);
                
                await db.SaveChangesAsync(token);

                return Ok(functionalRole);
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
