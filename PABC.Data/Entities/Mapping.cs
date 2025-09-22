using Json.Schema.Generation;

namespace PABC.Data.Entities
{
    [If(nameof(IsAllEntityTypes), true, nameof(IsAllEntityTypes))]
    public class Mapping
    {
        public required Guid Id { get; init; }
        public required Guid FunctionalRoleId { get; init; }

        [Json.Schema.Generation.Const(null, ConditionGroup = nameof(IsAllEntityTypes))]
        public Guid? DomainId { get; init; }

        public required Guid ApplicationRoleId { get; init; }
        public bool IsAllEntityTypes { get; init; } = false;

        [JsonExclude]
        public FunctionalRole FunctionalRole { get; private init; } = null!;

        [Json.Schema.Generation.JsonExclude]
        public Domain? Domain { get; private init; }

        [Json.Schema.Generation.JsonExclude]
        public ApplicationRole ApplicationRole { get; private init; } = null!;
    }
}
