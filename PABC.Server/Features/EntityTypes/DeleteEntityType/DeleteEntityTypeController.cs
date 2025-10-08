using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PABC.Data;

namespace PABC.Server.Features.EntityTypes.DeleteEntityType
{
    [ApiController]
    [Route("/api/v1/entity-types")]
    public class DeleteEntityTypeController(PabcDbContext db) : Controller
    {
        [HttpDelete("{id}")]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, MediaTypeNames.Application.ProblemJson)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteEntityType(Guid id, CancellationToken token = default)
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

            await db.EntityTypes.Where(d => d.Id == id).ExecuteDeleteAsync(token);

            return NoContent();
        }
    }
}
