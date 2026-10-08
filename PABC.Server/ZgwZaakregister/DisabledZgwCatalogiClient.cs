namespace PABC.Server.ZgwZaakregister;

/// <summary>
/// Registered when the ZGW Zaakregister integration is disabled.
/// Throws on use so the controller returns a meaningful error.
/// </summary>
public class DisabledZgwCatalogiClient : IZgwCatalogiClient
{
    public Task<IReadOnlyList<string>> GetZaaktypeOmschrijvingen(CancellationToken token = default)
    {
        throw new InvalidOperationException("ZGW Zaakregister-integratie is niet ingeschakeld.");
    }
}
