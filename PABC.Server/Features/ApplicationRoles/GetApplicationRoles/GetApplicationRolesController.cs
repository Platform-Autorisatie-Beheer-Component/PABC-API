using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PABC.Data;
using PABC.Data.Entities;

namespace PABC.Server.Features.ApplicationRoles.GetApplicationRoles
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("/api/v1/application-roles")]
    public class GetApplicationRolesController(PabcDbContext db) : Controller
    {
        [HttpGet]
        [ProducesResponseType<List<ApplicationRole>>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
        public async Task<IActionResult> GetApplicationRoles(CancellationToken token = default)
        {
            var applicationRoles = await db.ApplicationRoles
                .Include(ar => ar.Application)
                .OrderBy(ar => ar.Application.Name)
                .ThenBy(ar => ar.Name)
                .Select(ar => new ApplicationRoleResponse
                {
                    Id = ar.Id,
                    Name = ar.Name,
                    Application = ar.Application.Name,
                    ApplicationId = ar.ApplicationId
                })
                .ToListAsync(token);

            return Ok(applicationRoles);
        }
    }
}
