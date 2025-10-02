using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using PABC.Data;
using PABC.Data.Entities;

namespace PABC.Server.Features.EntityTypes.GetEntityType
{
    [ApiController]
    [Route("/api/v1/entity-types")]
    public class GetEntityTypeController(PabcDbContext db) : Controller
    {
        [HttpGet("{id}")]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.ProblemJson)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, MediaTypeNames.Application.ProblemJson)]
        [ProducesResponseType<EntityType>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
        public async Task<IActionResult> GetEntityTypeById(Guid id, CancellationToken token = default)
        {
            var entityType = await db.EntityTypes.FindAsync([id], token);

            if (entityType == null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Entity Type Not Found",
                    Status = StatusCodes.Status404NotFound
                });
            }

            return Ok(entityType);
        }
    }
}
