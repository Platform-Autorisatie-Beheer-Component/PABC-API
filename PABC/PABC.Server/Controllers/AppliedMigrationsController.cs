using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PABC.Server.Data;

namespace PABC.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AppliedMigrationsController(PabcDbContext db) : ControllerBase
    {
        [HttpGet(Name = "GetAppliedMigrations")]
        public async Task<ActionResult<IEnumerable<string>>> Get() => Ok(await db.Database.GetAppliedMigrationsAsync());
    }
}
