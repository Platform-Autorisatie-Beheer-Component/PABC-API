namespace PABC.Data.Entities
{
    public class ApplicationRole
    {
        public required Guid Id { get; set; }
        public required string Name { get; init; }
        public required string Application { get; init; }
    }
}
