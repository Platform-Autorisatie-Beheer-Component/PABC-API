namespace PABC.MigrationService.Features.DatabaseInitialization;

using Json.Schema.Generation;
using PABC.Data;

public record DataSet
{
    public required IReadOnlyList<ApplicationRole> ApplicationRoles { get; init; }
    public required IReadOnlyList<FunctionRole> FunctionalRoles { get; init; }
    public required IReadOnlyList<EntityType> EntityTypes { get; init; }
    public required IReadOnlyList<Domain> Domains { get; init; }
    public required IReadOnlyList<Mapping> Mappings { get; init; }

    public record ApplicationRole
    {
        public required Guid Id { get; init; }

        [MaxLength(PabcDbContext.MaxLengthForIndexProperties)]
        public required string Application { get; init; }

        [MaxLength(PabcDbContext.MaxLengthForIndexProperties)]
        public required string Name { get; init; }
    }

    public record EntityType
    {
        public required Guid Id { get; init; }

        [MaxLength(PabcDbContext.MaxLengthForIndexProperties)]
        public required string EntityTypeId { get; init; }

        [MaxLength(PabcDbContext.MaxLengthForIndexProperties)]
        public required string Type { get; init; }

        [MaxLength(PabcDbContext.MaxLengthForIndexProperties)]
        public required string Name { get; init; }

        public string? Description { get; init; }

        public Uri? Uri { get; init; }
    }

    public record Domain
    {
        public required Guid Id { get; init; }

        [MaxLength(PabcDbContext.MaxLengthForIndexProperties)]
        public required string Name { get; init; }

        [MaxLength(PabcDbContext.MaxLengthForIndexProperties)]
        public required string Description { get; init; }

        public required IReadOnlyList<Guid> EntityTypes { get; init; }
    }

    public record FunctionRole
    {
        public required Guid Id { get; init; }

        [MaxLength(PabcDbContext.MaxLengthForIndexProperties)]
        public required string Name { get; init; }
    }

    public record Mapping
    {
        public required Guid Id { get; init; }

        public required Guid FunctionalRole { get; init; }

        public required Guid Domain { get; init; }

        public required Guid ApplicationRole { get; init; }
    }
};

