namespace PABC.Data.Entities
{
    public class FunctionalRole
    {
        public required Guid Id { get; set; }

        [Json.Schema.Generation.MaxLength(PabcDbContext.MaxLengthForIndexProperties)]
        public required string Name { get; init; }
    }
}
