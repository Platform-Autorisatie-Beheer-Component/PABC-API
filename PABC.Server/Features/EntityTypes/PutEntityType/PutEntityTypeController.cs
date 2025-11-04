using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PABC.Data;
using PABC.Data.Entities;

namespace PABC.Server.Features.EntityTypes.PutEntityType
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("/api/v1/entity-types")]
    public class PutEntityTypeController(PabcDbContext db) : Controller
    {
        [HttpPut("{id}")]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.ProblemJson)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, MediaTypeNames.Application.ProblemJson)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict, MediaTypeNames.Application.ProblemJson)]
        [ProducesResponseType<EntityType>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.ProblemJson)]
        public async Task<IActionResult> PutEntityType(Guid id, [FromBody] EntityType model, CancellationToken token = default)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            try
            {
                var entityType = await db.EntityTypes.FindAsync([id], token);

                if (entityType == null)
                {
                    return NotFound(new ProblemDetails
                    {
                        Detail = "Entiteitstype niet gevonden",
                        Status = StatusCodes.Status404NotFound
                    });
                }

                entityType.EntityTypeId = model.EntityTypeId;
                entityType.Type = model.Type;
                entityType.Name = model.Name;
                entityType.Uri = model.Uri;

                db.EntityTypes.Update(entityType);
                
                await db.SaveChangesAsync(token);

                return Ok(entityType);
            }
            catch (DbUpdateException ex) when (ex.IsDuplicateException())
            {
                return Conflict(new ProblemDetails
                {
                    Detail = "Combinatie Entiteitstype en Entiteitstype ID bestaat al",
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
