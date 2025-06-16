namespace PABC.Data.Entities
{
    public class Mapping
    {
        public required Guid Id { get; init; }
        public Guid FunctionalRoleId { get; private init; }
        public required FunctionalRole FunctionalRole { get; init; }
        public Guid DomainId { get; private init; }
        public required Domain Domain { get; init; }
        public Guid ApplicationRoleId { get; private init; }
        public required ApplicationRole ApplicationRole { get; init; }
    }
}
