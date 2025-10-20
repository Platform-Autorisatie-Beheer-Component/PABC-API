using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PABC.Data;
using PABC.Data.Entities;

namespace PABC.Server.Features.Domains.PutDomain
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("/api/v1/domains")]
    public class PutDomainController(PabcDbContext db) : Controller
    {
        [HttpPut("{id}")]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.ProblemJson)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, MediaTypeNames.Application.ProblemJson)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict, MediaTypeNames.Application.ProblemJson)]
        [ProducesResponseType<Domain>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.ProblemJson)]
        public async Task<IActionResult> PutDomain(Guid id, [FromBody] DomainUpsertModel model, CancellationToken token = default)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            try
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

                domain.Name = model.Name;
                domain.Description = model.Description;

                db.Domains.Update(domain);
                
                await db.SaveChangesAsync(token);

                return Ok(domain);
            }
            catch (DbUpdateException ex) when (ex.IsDuplicateException())
            {
                return Conflict(new ProblemDetails
                {
                    Detail = "Domeinnaam bestaat al",
                    Status = StatusCodes.Status409Conflict
                });
            }
            catch
            {
                return StatusCode(500, new ProblemDetails
                {
                    Detail = "Internal Server Error",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }
    }
}
