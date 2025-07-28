
namespace PABC.Data.Entities
{
    public class ApplicationRole
    {
        public required Guid Id { get; set; }
        
        [Json.Schema.Generation.MaxLength(PabcDbContext.MaxLengthForIndexProperties)]
        public required string Name { get; init; }

        [Json.Schema.Generation.MaxLength(PabcDbContext.MaxLengthForIndexProperties)]
        public required string Application { get; init; }
    }
}
