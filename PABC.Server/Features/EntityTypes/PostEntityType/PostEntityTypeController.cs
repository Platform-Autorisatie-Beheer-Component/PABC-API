using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PABC.Data;
using PABC.Data.Entities;

namespace PABC.Server.Features.EntityTypes.PostEntityType
{
    [ApiController]
    [Route("/api/v1/entity-types")]
    public class PostEntityTypeController(PabcDbContext db) : Controller
    {
        [HttpPost]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.ProblemJson)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict, MediaTypeNames.Application.ProblemJson)]
        [ProducesResponseType<EntityType>(StatusCodes.Status201Created, MediaTypeNames.Application.Json)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.ProblemJson)]
        public async Task<IActionResult> PostEntityType([FromBody] EntityTypeUpsertModel model, CancellationToken token = default)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            try
            {
                var entityType = new EntityType
                {
                    EntityTypeId = model.EntityTypeId,
                    Type = model.Type,
                    Name = model.Name,
                    Uri = model.Uri
                };
                
                db.EntityTypes.Add(entityType);
                
                await db.SaveChangesAsync(token);

                return StatusCode(201, entityType);
            }
            catch (DbUpdateException ex) when (ex.IsDuplicateException())
            {
                return Conflict(new ProblemDetails
                {
                    Detail = "Combinatie Type / EntityTypeId bestaat al",
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
