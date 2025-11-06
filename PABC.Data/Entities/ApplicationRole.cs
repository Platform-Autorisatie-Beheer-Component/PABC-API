
namespace PABC.Data.Entities
{
    public class ApplicationRole
    {
        public required Guid Id { get; init; }
        
        [Json.Schema.Generation.MaxLength(PabcDbContext.MaxLengthForIndexProperties)]
        [System.ComponentModel.DataAnnotations.MaxLength(PabcDbContext.MaxLengthForIndexProperties)]
        public required string Name { get; set; }

        public required Guid ApplicationId { get; set; }

        [Json.Schema.Generation.JsonExclude]
        public Application Application { get; private init; } = null!;
    }
}
