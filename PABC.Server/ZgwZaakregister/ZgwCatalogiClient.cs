using System.Text.Json.Serialization;

namespace PABC.Server.ZgwZaakregister;

public interface IZgwCatalogiClient
{
    /// <summary>
    /// Fetches all published zaaktypes from the configured catalogus,
    /// deduplicated by omschrijving (which is the unique zaaktype identifier).
    /// </summary>
    Task<IReadOnlyList<string>> GetZaaktypeOmschrijvingen(CancellationToken token = default);
}

public class ZgwCatalogiClient(HttpClient httpClient, ZgwZaakregisterOptions options, ILogger<ZgwCatalogiClient> logger) : IZgwCatalogiClient
{
    public async Task<IReadOnlyList<string>> GetZaaktypeOmschrijvingen(CancellationToken token = default)
    {
        var catalogusUrl = await GetCatalogusUrl(token);
        var zaaktypes = await GetAllZaaktypes(catalogusUrl, token);

        return zaaktypes
            .Select(z => z.Omschrijving)
            .Distinct()
            .ToList();
    }

    private async Task<string> GetCatalogusUrl(CancellationToken token)
    {
        var response = await httpClient.GetAsync(
            $"catalogussen?domein={Uri.EscapeDataString(options.CatalogusDomein)}",
            HttpCompletionOption.ResponseHeadersRead,
            token);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ZgwPaginatedResult<ZgwCatalogus>>(cancellationToken: token)
            ?? throw new InvalidOperationException("Ongeldig antwoord van ZGW Catalogi API bij ophalen catalogus");

        var catalogus = result.Results.FirstOrDefault()
            ?? throw new InvalidOperationException(
                $"Geen catalogus gevonden met domein '{options.CatalogusDomein}'");

        logger.LogInformation("Catalogus gevonden: {CatalogusUrl}", catalogus.Url);

        return catalogus.Url;
    }

    private async Task<List<ZgwZaaktype>> GetAllZaaktypes(string catalogusUrl, CancellationToken token)
    {
        var allZaaktypes = new List<ZgwZaaktype>();
        string? nextUrl = $"zaaktypen?catalogus={Uri.EscapeDataString(catalogusUrl)}&status=definitief";

        while (nextUrl is not null)
        {
            var response = await httpClient.GetAsync(nextUrl, HttpCompletionOption.ResponseHeadersRead, token);
            response.EnsureSuccessStatusCode();

            var page = await response.Content.ReadFromJsonAsync<ZgwPaginatedResult<ZgwZaaktype>>(cancellationToken: token)
                ?? throw new InvalidOperationException("Ongeldig antwoord van ZGW Catalogi API bij ophalen zaaktypes");

            allZaaktypes.AddRange(page.Results);

            nextUrl = GetRelativeUrl(page.Next);
        }

        logger.LogInformation("Totaal {Count} zaaktype versies opgehaald uit zaakregister", allZaaktypes.Count);

        return allZaaktypes;
    }

    /// <summary>
    /// The ZGW API returns absolute URLs for pagination.
    /// Convert to relative so they go through our configured HttpClient base address.
    /// </summary>
    private static string? GetRelativeUrl(string? absoluteUrl)
    {
        if (string.IsNullOrEmpty(absoluteUrl)) return null;
        if (!Uri.TryCreate(absoluteUrl, UriKind.Absolute, out var uri)) return absoluteUrl;
        return uri.PathAndQuery;
    }
}

internal record ZgwPaginatedResult<T>
{
    public int Count { get; init; }
    public string? Next { get; init; }
    public string? Previous { get; init; }
    public required IReadOnlyList<T> Results { get; init; }
}

internal record ZgwCatalogus
{
    public required string Url { get; init; }
    public required string Domein { get; init; }
}

internal record ZgwZaaktype
{
    public required string Omschrijving { get; init; }

    [JsonPropertyName("concept")]
    public bool IsConcept { get; init; }
}
