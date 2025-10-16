using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PABC.Data;
using PABC.Data.Entities;

namespace PABC.Server.Features.EntityTypes.GetEntityTypes
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("/api/v1/entity-types")]
    public class GetEntityTypesController(PabcDbContext db) : Controller
    {
        [HttpGet]
        [ProducesResponseType<List<EntityType>>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
        public async Task<IActionResult> GetEntityTypes(CancellationToken token = default)
        {
            var entityTypes = await db.EntityTypes
                .OrderBy(d => d.Type)
                .ThenBy(d => d.Name)
                .ToListAsync(token);

            return Ok(entityTypes);
        }
    }
}
