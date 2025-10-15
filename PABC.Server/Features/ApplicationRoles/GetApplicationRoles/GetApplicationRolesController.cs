using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PABC.Data;
using PABC.Data.Entities;

namespace PABC.Server.Features.ApplicationRoles.GetApplicationRoles
{
    [ApiController]
    [Route("/api/v1/application-roles")]
    public class GetApplicationRolesController(PabcDbContext db) : Controller
    {
        [HttpGet]
        [ProducesResponseType<List<ApplicationRole>>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
        public async Task<IActionResult> GetApplicationRoles(CancellationToken token = default)
        {
            var applicationRoles = await db.ApplicationRoles
                .OrderBy(d => d.Application)
                .ThenBy(d => d.Name)
                .ToListAsync(token);

            return Ok(applicationRoles);
        }
    }
}
