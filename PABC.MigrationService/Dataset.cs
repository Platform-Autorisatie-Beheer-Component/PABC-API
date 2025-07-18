namespace PABC.MigrationService;

using Json.Schema;
using Json.Schema.Generation;
using Json.Schema.Serialization;
using PABC.Data;
using static DataSet;

public static class Schemas
{ 
  public static readonly JsonSchema Dataset = JsonSchema.FromFile("dataset.schema.json");
}

[JsonSchema(typeof(Schemas), nameof(Schemas.Dataset))]
internal record DataSet(
    IReadOnlyList<ApplicationRole> ApplicationRoles,
    IReadOnlyList<FunctionRole> FunctionalRoles,
    IReadOnlyList<EntityType> EntityTypes,
    IReadOnlyList<Domain> Domains,
    IReadOnlyList<Mapping> Mappings
)
{

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

