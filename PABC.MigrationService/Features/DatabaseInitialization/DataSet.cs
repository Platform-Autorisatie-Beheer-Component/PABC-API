namespace PABC.MigrationService.Features.DatabaseInitialization;

using PABC.Data.Entities;

public record DataSet
{
    public required IReadOnlyCollection<Application> Applications { get; init; }
    public required IReadOnlyCollection<ApplicationRole> ApplicationRoles { get; init; }
    public required IReadOnlyCollection<FunctionalRole> FunctionalRoles { get; init; }
    public required IReadOnlyCollection<EntityType> EntityTypes { get; init; }
    public required IReadOnlyCollection<Domain> Domains { get; init; }
    public required IReadOnlyCollection<Mapping> Mappings { get; init; }
};

