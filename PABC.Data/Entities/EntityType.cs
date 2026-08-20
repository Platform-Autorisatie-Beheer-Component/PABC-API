namespace PABC.Data.Entities
{
    public class EntityType
    {
        public required Guid Id { get; init; }
        public required string EntityTypeId { get; set; }
        public required string Type { get; set; }
        public required string Name { get; set; }
        public Uri? Uri { get; set; }
    }
}
