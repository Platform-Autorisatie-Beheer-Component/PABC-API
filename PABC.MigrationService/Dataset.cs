namespace PABC.MigrationService;
using static DataSet;

internal record DataSet(
    IReadOnlyList<ApplicationRole> ApplicationRoles,
    IReadOnlyList<FunctionRole> FunctionalRoles,
    IReadOnlyList<EntityType> EntityTypes,
    IReadOnlyList<Domain> Domains,
    IReadOnlyList<Mapping> Mappings
)
{
    public record ApplicationRole(Guid Id, string Application, string Name);
    public record EntityType(Guid Id, string EntityTypeId, string Type, string Name, string? Description, Uri? Uri);
    public record Domain(Guid Id, string Name, string Description, IReadOnlyList<Guid> EntityTypes);
    public record FunctionRole(Guid Id, string Name);
    public record Mapping(Guid Id, Guid FunctionalRole, Guid Domain, Guid ApplicationRole);
};

