namespace PABC.MigrationService.Features.DatabaseInitialization;

using PABC.Data.Entities;

public record DataSet
{
    public required IReadOnlyList<ApplicationRole> ApplicationRoles { get; init; }
    public required IReadOnlyList<FunctionalRole> FunctionalRoles { get; init; }
    public required IReadOnlyList<EntityType> EntityTypes { get; init; }
    public required IReadOnlyList<DataSetDomain> Domains { get; init; }
    public required IReadOnlyList<Mapping> Mappings { get; init; }
};

public class DataSetDomain : Domain
{
    public required IReadOnlyList<Guid> EntityTypeIds { get; init; }
}

