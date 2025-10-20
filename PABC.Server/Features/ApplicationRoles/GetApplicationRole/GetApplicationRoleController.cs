using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using PABC.Data;
using PABC.Data.Entities;

namespace PABC.Server.Features.ApplicationRoles.GetApplicationRole
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("/api/v1/application-roles")]
    public class GetApplicationRoleController(PabcDbContext db) : Controller
    {
        [HttpGet("{id}")]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.ProblemJson)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, MediaTypeNames.Application.ProblemJson)]
        [ProducesResponseType<ApplicationRole>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
        public async Task<IActionResult> GetApplicationRoleById(Guid id, CancellationToken token = default)
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

            return Ok(applicationRole);
        }
    }
}
