namespace PABC.Data.Entities
{
    public class Domain
    {
        public required Guid Id { get; init; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public List<EntityType> EntityTypes { get; private init; } = [];
    }
}
