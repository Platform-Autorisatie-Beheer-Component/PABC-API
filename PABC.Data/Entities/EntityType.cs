namespace PABC.Data.Entities
{
    public class EntityType
    {
        public required Guid Id { get; init; }

        [Json.Schema.Generation.MaxLength(PabcDbContext.MaxLengthForIndexProperties)]
        public required string EntityTypeId { get; init; }

        [Json.Schema.Generation.MaxLength(PabcDbContext.MaxLengthForIndexProperties)]

        public required string Type { get; init; }

        [Json.Schema.Generation.MaxLength(PabcDbContext.MaxLengthForIndexProperties)]

        public required string Name { get; init; }

        public Uri? Uri { get; init; }
    }
}
