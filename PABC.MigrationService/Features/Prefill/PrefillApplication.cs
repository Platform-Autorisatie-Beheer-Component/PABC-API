namespace PABC.MigrationService.Features.Prefill;

public class PrefillApplication
{
    public required string Name { get; init; }
    public required IReadOnlyList<string> Roles { get; init; }
}
