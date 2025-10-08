using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PABC.Data;

namespace PABC.Server.Features.Domains.DeleteDomain
{
    [ApiController]
    [Route("/api/v1/domains")]
    public class DeleteDomainController(PabcDbContext db) : Controller
    {
        [HttpDelete("{id}")]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, MediaTypeNames.Application.ProblemJson)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteDomain(Guid id, CancellationToken token = default)
        {
            var domain = await db.Domains.FindAsync([id], token);

            if (domain == null)
            {
                return NotFound(new ProblemDetails
                {
                    Detail = "Domein niet gevonden",
                    Status = StatusCodes.Status404NotFound
                });
            }

            await db.Domains.Where(d => d.Id == id).ExecuteDeleteAsync(token);

            return NoContent();
        }
    }
}
