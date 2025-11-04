using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PABC.Data;
using PABC.Data.Entities;

namespace PABC.Server.Features.Applications.GetApplications
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("/api/v1/applications")]
    public class GetApplicationsController(PabcDbContext db) : Controller
    {
        [HttpGet]
        [ProducesResponseType<List<Application>>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
        public async Task<IActionResult> GetApplications(CancellationToken token = default)
        {
            var applications = await db.Applications
                .OrderBy(d => d.Name)
                .ToListAsync(token);

            return Ok(applications);
        }
    }
}
