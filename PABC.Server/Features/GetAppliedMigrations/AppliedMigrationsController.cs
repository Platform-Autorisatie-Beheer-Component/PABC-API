using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PABC.Data;
using System.Net.Mime;

namespace PABC.Server.Features.GetAppliedMigrations
{
    [ApiController]
    public class AppliedMigrationsController(PabcDbContext db)
    {
        [HttpGet("api/applied-migrations", Name = "GetAppliedMigrations")]
        [ProducesResponseType<IEnumerable<string>>(200, MediaTypeNames.Application.Json)]
        public async Task<ActionResult<IEnumerable<string>>> Get() => new OkObjectResult(await db.Database.GetAppliedMigrationsAsync());
    }
}
