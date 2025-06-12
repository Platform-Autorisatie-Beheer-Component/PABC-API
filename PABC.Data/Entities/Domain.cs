namespace PABC.Data.Entities
{
    public class Domain
    {
        public required Guid Id { get; init; }
        public required string Name { get; init; }
        public required string Description { get; init; }
        public required List<EntityType> EntityTypes { get; init; }
    }
}
