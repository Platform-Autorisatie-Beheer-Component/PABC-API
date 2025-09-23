namespace PABC.Data.Entities
{
    public class Domain
    {
        public required Guid Id { get; init; }

        [Json.Schema.Generation.MaxLength(PabcDbContext.MaxLengthForIndexProperties)]
        [System.ComponentModel.DataAnnotations.MaxLength(PabcDbContext.MaxLengthForIndexProperties)]
        public required string Name { get; set; }

        [Json.Schema.Generation.MaxLength(PabcDbContext.MaxLengthForIndexProperties)]
        [System.ComponentModel.DataAnnotations.MaxLength(PabcDbContext.MaxLengthForIndexProperties)]
        public required string Description { get; set; }

        [Json.Schema.Generation.JsonExclude]
        public List<EntityType> EntityTypes { get; private init; } = [];
    }
}
