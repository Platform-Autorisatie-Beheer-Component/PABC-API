using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PABC.Data;
using PABC.Data.Entities;

namespace PABC.Server.Features.EntityTypes.GetEntityTypes
{
    [ApiController]
    [Route("/api/v1/entity-types")]
    public class GetEntityTypesController(PabcDbContext db) : Controller
    {
        [HttpGet]
        [ProducesResponseType<List<EntityType>>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
        public async Task<IActionResult> GetEntityTypes(CancellationToken token = default)
        {
            var entityTypes = await db.EntityTypes.ToListAsync(token);

            return Ok(entityTypes);
        }
    }
}
