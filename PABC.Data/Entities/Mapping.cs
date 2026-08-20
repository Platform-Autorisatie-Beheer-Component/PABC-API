namespace PABC.Data.Entities
{
    public class Mapping
    {
        public required Guid Id { get; init; }
        public required Guid FunctionalRoleId { get; init; }
        public Guid? DomainId { get; init; }
        public required Guid ApplicationRoleId { get; init; }
        public bool IsAllEntityTypes { get; init; } = false;

        public FunctionalRole FunctionalRole { get; private init; } = null!;
        public Domain? Domain { get; private init; }
        public ApplicationRole ApplicationRole { get; private init; } = null!;
    }
}
