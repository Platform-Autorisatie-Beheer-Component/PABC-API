using System.Text.Json;

namespace PABC.MigrationService.Features.Prefill;

public interface IPrefillParser
{
    Task<IReadOnlyList<PrefillApplication>> Parse(Stream stream, CancellationToken cancellationToken);
}

public class PrefillParser : IPrefillParser
{
    private static readonly JsonSerializerOptions s_options = new(JsonSerializerDefaults.Web);

    public async Task<IReadOnlyList<PrefillApplication>> Parse(Stream stream, CancellationToken cancellationToken)
    {
        var result = await JsonSerializer.DeserializeAsync<List<PrefillApplication>>(stream, s_options, cancellationToken);
        return result ?? [];
    }
}
