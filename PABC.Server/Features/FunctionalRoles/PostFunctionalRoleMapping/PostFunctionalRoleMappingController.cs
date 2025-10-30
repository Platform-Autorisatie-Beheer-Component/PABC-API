using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PABC.Data;
using PABC.Data.Entities;

namespace PABC.Server.Features.FunctionalRoles.PostFunctionalRoleMapping
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("/api/v1/functional-roles/{functionalRoleId}/mappings")]
    public class PostFunctionalRoleMappingController(PabcDbContext db) : Controller
    {
        [HttpPost]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, MediaTypeNames.Application.ProblemJson)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict, MediaTypeNames.Application.ProblemJson)]
        [ProducesResponseType<Mapping>(StatusCodes.Status201Created, MediaTypeNames.Application.Json)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.ProblemJson)]
        public async Task<IActionResult> PostFunctionalRoleMapping(Guid functionalRoleId, [FromBody] MappingCreateModel model, CancellationToken token = default)
        {
            try
            {
                var functionalRole = await db.FunctionalRoles.FindAsync([functionalRoleId], token);

                if (functionalRole == null)
                {
                    return NotFound(new ProblemDetails
                    {
                        Detail = "Functionele rol niet gevonden",
                        Status = StatusCodes.Status404NotFound
                    });
                }

                var applicationRole = await db.ApplicationRoles.FindAsync([model.ApplicationRoleId], token);

                if (applicationRole == null)
                {
                    return NotFound(new ProblemDetails
                    {
                        Detail = "Applicatierol niet gevonden",
                        Status = StatusCodes.Status404NotFound
                    });
                }

                // If not IsAllEntityTypes, domain must be provided and must exist
                if (!model.IsAllEntityTypes)
                {
                    if (model.DomainId == null)
                    {
                        return BadRequest(new ProblemDetails
                        {
                            Detail = "Domein is verplicht",
                            Status = StatusCodes.Status400BadRequest
                        });
                    }

                    var domain = await db.Domains.FindAsync([model.DomainId.Value], token);

                    if (domain == null)
                    {
                        return NotFound(new ProblemDetails
                        {
                            Detail = "Domein niet gevonden",
                            Status = StatusCodes.Status404NotFound
                        });
                    }
                }

                var mapping = new Mapping
                {

                    Id = Guid.NewGuid(),
                    FunctionalRoleId = functionalRoleId,
                    ApplicationRoleId = model.ApplicationRoleId,
                    DomainId = model.DomainId,
                    IsAllEntityTypes = model.IsAllEntityTypes
                };

                db.Mappings.Add(mapping);

                await db.SaveChangesAsync(token);

                return StatusCode(201, mapping);
            }
            catch (DbUpdateException ex) when (ex.IsDuplicateException())
            {
                return Conflict(new ProblemDetails
                {
                    Detail = "Deze mapping bestaat al",
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

    public class MappingCreateModel
    {
        public required Guid ApplicationRoleId { get; init; }
        public Guid? DomainId { get; init; }
        public required bool IsAllEntityTypes { get; init; }
    }
}
