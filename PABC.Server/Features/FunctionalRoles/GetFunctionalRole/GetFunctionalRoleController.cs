using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using PABC.Data;
using PABC.Data.Entities;

namespace PABC.Server.Features.FunctionalRoles.GetFunctionalRole
{
    [ApiController]
    [Route("/api/v1/functional-roles")]
    public class GetFunctionalRoleController(PabcDbContext db) : Controller
    {
        [HttpGet("{id}")]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.ProblemJson)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, MediaTypeNames.Application.ProblemJson)]
        [ProducesResponseType<FunctionalRole>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
        public async Task<IActionResult> GetFunctionalRoleById(Guid id, CancellationToken token = default)
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

            return Ok(functionalRole);
        }
    }
}
