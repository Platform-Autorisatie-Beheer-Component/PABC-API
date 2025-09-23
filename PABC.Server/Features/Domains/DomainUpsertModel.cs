using Json.Schema.Generation;
using System.ComponentModel.DataAnnotations;
using PABC.Data;

namespace PABC.Server.Features.Domains
{
    public class DomainUpsertModel
    {
        [Json.Schema.Generation.MaxLength(PabcDbContext.MaxLengthForIndexProperties)]
        [System.ComponentModel.DataAnnotations.MaxLength(PabcDbContext.MaxLengthForIndexProperties)]
        public required string Name { get; init; }

        [Json.Schema.Generation.MaxLength(PabcDbContext.MaxLengthForIndexProperties)]
        [System.ComponentModel.DataAnnotations.MaxLength(PabcDbContext.MaxLengthForIndexProperties)]
        public required string Description { get; init; }
    }
}
