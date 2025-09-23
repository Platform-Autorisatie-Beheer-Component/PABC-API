using System.Net.Mime;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PABC.Data;
using PABC.Data.Entities;

namespace PABC.Server.Features.Domains.PostDomain
{
    //[ApiController]
    //[Route("/api/v1/domains")]
    //public class PutDomainController(PabcDbContext db) : Controller
    //{
    //    [HttpPut("{id}")]
    //}


    [ApiController]
    [Route("/api/v1/domains")]
    public class PutDomainController(PabcDbContext db) : Controller
    {
        [HttpPut("{id}")]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.ProblemJson)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, MediaTypeNames.Application.ProblemJson)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict, MediaTypeNames.Application.ProblemJson)]
        [ProducesResponseType<Domain>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.ProblemJson)]
        public async Task<IActionResult> PostDomain(Guid id, [FromBody] DomainUpsertModel model, CancellationToken token = default)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            try
            {
                var domain = await db.Domains.FindAsync(id, token);

                if (domain == null)
                {
                    return NotFound(new ProblemDetails
                    {
                        Title = "Domain Not Found",
                        Status = StatusCodes.Status404NotFound
                    });
                }


                var duplicateDomain = await db.Domains.FirstOrDefaultAsync(d =>
                    d.Id != id && d.Name.ToLower() == model.Name.ToLower(), token);

                if (duplicateDomain != null)
                {
                    return Conflict(new ProblemDetails
                    {
                        Title = "Duplicate Domain Name",
                        Status = StatusCodes.Status500InternalServerError
                    });
                }

                domain.Name = model.Name;
                domain.Description = model.Description;

                db.Domains.Update(domain);
                
                await db.SaveChangesAsync(token);

                return Ok(domain);
            }
            catch
            {
                return StatusCode(500, new ProblemDetails
                {
                    Title = "Internal Server Error",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }
    }
}
