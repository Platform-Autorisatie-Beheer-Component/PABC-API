using Json.Schema.Generation;

namespace PABC.Data.Entities
{
    public class Mapping
    {
        public required Guid Id { get; init; }
        public required Guid FunctionalRoleId { get; init; }
        public required Guid DomainId { get; init; }
        public required Guid ApplicationRoleId { get; init; }

        [JsonExclude]
        public FunctionalRole FunctionalRole { get; private init; } = null!;

        [Json.Schema.Generation.JsonExclude]
        public Domain Domain { get; private init; } = null!;
        
        [Json.Schema.Generation.JsonExclude]
        public ApplicationRole ApplicationRole { get; private init; } = null!;
    }
}
