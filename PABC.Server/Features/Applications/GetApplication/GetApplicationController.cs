using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using PABC.Data;
using PABC.Data.Entities;

namespace PABC.Server.Features.Applications.GetApplication
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("/api/v1/applications")]
    public class GetApplicationController(PabcDbContext db) : Controller
    {
        [HttpGet("{id}")]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.ProblemJson)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, MediaTypeNames.Application.ProblemJson)]
        [ProducesResponseType<Application>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
        public async Task<IActionResult> GetApplicationById(Guid id, CancellationToken token = default)
        {
            var application = await db.Applications.FindAsync([id], token);

            if (application == null)
            {
                return NotFound(new ProblemDetails
                {
                    Detail = "Applicatie niet gevonden",
                    Status = StatusCodes.Status404NotFound
                });
            }

            return Ok(application);
        }
    }
}
