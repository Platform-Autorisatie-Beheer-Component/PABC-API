using PABC.Data;

namespace PABC.Server.Features.FunctionalRoles
{
    public class FunctionalRoleUpsertModel
    {
        [Json.Schema.Generation.MaxLength(PabcDbContext.MaxLengthForIndexProperties)]
        [System.ComponentModel.DataAnnotations.MaxLength(PabcDbContext.MaxLengthForIndexProperties)]
        public required string Name { get; init; }
    }
}
