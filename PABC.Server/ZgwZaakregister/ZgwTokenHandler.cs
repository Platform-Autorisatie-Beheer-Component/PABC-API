using System.Net.Http.Headers;

namespace PABC.Server.ZgwZaakregister;

/// <summary>
/// DelegatingHandler that adds a ZGW JWT Bearer token to each outgoing request.
/// </summary>
public class ZgwTokenHandler(ZgwTokenProvider tokenProvider) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = tokenProvider.GenerateToken();
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return base.SendAsync(request, cancellationToken);
    }
}
