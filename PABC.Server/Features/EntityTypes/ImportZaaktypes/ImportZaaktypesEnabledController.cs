using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using PABC.Server.ZgwZaakregister;

namespace PABC.Server.Features.EntityTypes.ImportZaaktypes
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("/api/v1/entity-types/import-zaaktypes")]
    public class ImportZaaktypesEnabledController(ZgwZaakregisterOptions options) : Controller
    {
        [HttpGet("enabled")]
        [ProducesResponseType<ImportZaaktypesEnabledResponse>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
        public IActionResult GetEnabled()
        {
            return Ok(new ImportZaaktypesEnabledResponse { Enabled = options.Enabled });
        }
    }

    public class ImportZaaktypesEnabledResponse
    {
        public bool Enabled { get; init; }
    }
}
