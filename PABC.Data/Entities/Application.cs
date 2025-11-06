
namespace PABC.Data.Entities
{
    public class Application
    {
        public required Guid Id { get; init; }
        
        [Json.Schema.Generation.MaxLength(PabcDbContext.MaxLengthForIndexProperties)]
        [System.ComponentModel.DataAnnotations.MaxLength(PabcDbContext.MaxLengthForIndexProperties)]
        public required string Name { get; set; }
    }
}
