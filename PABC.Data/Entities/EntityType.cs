namespace PABC.Data.Entities
{
    public class EntityType
    {
        public required Guid Id { get; init; }
        public required string EntityTypeId { get; init; }
        public required string Type { get; init; }
        public required string Name { get; init; }
        public Uri? Uri { get; init; }
    }
}
