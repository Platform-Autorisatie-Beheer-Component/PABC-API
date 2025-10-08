namespace PABC.Data.Entities
{
    public class FunctionalRole
    {
        public Guid Id { get; set; }

        [Json.Schema.Generation.MaxLength(PabcDbContext.MaxLengthForIndexProperties)]
        [System.ComponentModel.DataAnnotations.MaxLength(PabcDbContext.MaxLengthForIndexProperties)]
        public required string Name { get; set; }
    }
}
