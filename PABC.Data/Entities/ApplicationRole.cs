
namespace PABC.Data.Entities
{
    public class ApplicationRole
    {
        public required Guid Id { get; init; }
        public required string Name { get; set; }
        public required Guid ApplicationId { get; set; }
        public Application Application { get; private init; } = null!;
    }
}
