using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PABC.Data;
using PABC.Data.Entities;

namespace PABC.Server.Features.FunctionalRoles.PostFunctionalRole
{
    [ApiController]
    [Route("/api/v1/functional-roles")]
    public class PostFunctionalRoleController(PabcDbContext db) : Controller
    {
        [HttpPost]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.ProblemJson)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict, MediaTypeNames.Application.ProblemJson)]
        [ProducesResponseType<FunctionalRole>(StatusCodes.Status201Created, MediaTypeNames.Application.Json)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.ProblemJson)]
        public async Task<IActionResult> PostFunctionalRole([FromBody] FunctionalRoleUpsertModel model, CancellationToken token = default)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            try
            {
                var functionalRole = new FunctionalRole { Name = model.Name };
                
                db.FunctionalRoles.Add(functionalRole);
                
                await db.SaveChangesAsync(token);

                return StatusCode(201, functionalRole);
            }
            catch (DbUpdateException ex) when (ex.IsDuplicateException())
            {
                return Conflict(new ProblemDetails
                {
                    Title = "Duplicate Functional Role Name",
                    Status = StatusCodes.Status409Conflict
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
