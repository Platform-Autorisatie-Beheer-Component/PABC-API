using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PABC.Data;
using PABC.Data.Entities;

namespace PABC.Server.Features.FunctionalRoles.GetFunctionalRoles
{
    [ApiController]
    [Route("/api/v1/functional-roles")]
    public class GetFunctionalRolesController(PabcDbContext db) : Controller
    {
        [HttpGet]
        [ProducesResponseType<List<FunctionalRole>>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
        public async Task<IActionResult> GetFunctionalRoles(CancellationToken token = default)
        {
            var functionalRoles = await db.FunctionalRoles.ToListAsync(token);

            return Ok(functionalRoles);
        }
    }
}
