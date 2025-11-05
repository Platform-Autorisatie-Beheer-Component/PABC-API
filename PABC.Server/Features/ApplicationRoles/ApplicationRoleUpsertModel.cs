using PABC.Data;

namespace PABC.Server.Features.ApplicationRoles
{
    public class ApplicationRoleUpsertModel
    {
        [System.ComponentModel.DataAnnotations.MaxLength(PabcDbContext.MaxLengthForIndexProperties)]
        public required string Name { get; set; }

        public required Guid ApplicationId { get; set; }
    }
}
