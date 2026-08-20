using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PABC.Data;
using PABC.Data.Entities;
using PABC.Server.ZgwZaakregister;

namespace PABC.Server.Features.EntityTypes.ImportZaaktypes
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("/api/v1/entity-types/import-zaaktypes")]
    public class ImportZaaktypesController(PabcDbContext db, IZgwCatalogiClient zgwClient) : Controller
    {
        private const string ZaaktypeType = "ZAAKTYPE";

        [HttpPost]
        [ProducesResponseType<ImportZaaktypesResponse>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.ProblemJson)]
        public async Task<IActionResult> ImportZaaktypes(CancellationToken token = default)
        {
            try
            {
                var zaaktypeOmschrijvingen = await zgwClient.GetZaaktypeOmschrijvingen(token);

                var existingZaaktypes = await db.EntityTypes
                    .Where(e => e.Type == ZaaktypeType) // matching Type case-insensitive (nl_case_insensitive collation)
                    .Select(e => e.EntityTypeId)
                    .ToListAsync(token);

                // matching EntityTypeId to omschrijving case-sensitive, so we use a HashSet for lookups
                var existingSet = new HashSet<string>(existingZaaktypes);

                var created = new List<string>();
                var skipped = new List<string>();

                foreach (var omschrijving in zaaktypeOmschrijvingen)
                {
                    if (existingSet.Contains(omschrijving))
                    {
                        skipped.Add(omschrijving);
                        continue;
                    }

                    db.EntityTypes.Add(new EntityType
                    {
                        Id = Guid.NewGuid(),
                        Type = ZaaktypeType,
                        EntityTypeId = omschrijving,
                        Name = omschrijving,
                        Uri = null
                    });

                    created.Add(omschrijving);
                }

                await db.SaveChangesAsync(token);

                var zaaktypeSet = new HashSet<string>(zaaktypeOmschrijvingen);

                var stale = existingSet
                    .Where(id => !zaaktypeSet.Contains(id))
                    .ToList();

                return Ok(new ImportZaaktypesResponse
                {
                    Created = created,
                    Skipped = skipped,
                    Stale = stale
                });
            }
            catch (Exception ex) when (ex is HttpRequestException or InvalidOperationException)
            {
                return StatusCode(500, new ProblemDetails
                {
                    Detail = $"Fout bij het ophalen van zaaktypes uit het zaakregister: {ex.Message}",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }
    }

    public class ImportZaaktypesResponse
    {
        public required List<string> Created { get; init; }
        public required List<string> Skipped { get; init; }
        public required List<string> Stale { get; init; }
    }
}
