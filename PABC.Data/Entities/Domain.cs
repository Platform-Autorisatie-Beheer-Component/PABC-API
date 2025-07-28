using Json.Schema.Generation;

namespace PABC.Data.Entities
{
    public class Domain
    {
        public required Guid Id { get; init; }

        [Json.Schema.Generation.MaxLength(PabcDbContext.MaxLengthForIndexProperties)]
        public required string Name { get; init; }

        [Json.Schema.Generation.MaxLength(PabcDbContext.MaxLengthForIndexProperties)]
        public required string Description { get; init; }

        [Json.Schema.Generation.JsonExclude]
        public List<EntityType> EntityTypes { get; private init; } = [];
    }
}
