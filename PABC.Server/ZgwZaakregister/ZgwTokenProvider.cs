using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace PABC.Server.ZgwZaakregister;

/// <summary>
/// Generates short-lived HS256 JWT tokens for ZGW API authentication.
/// See: https://open-zaak.readthedocs.io/en/latest/client-development/authentication.html
/// </summary>
public class ZgwTokenProvider(ZgwZaakregisterOptions options)
{
    public string GenerateToken()
    {
        var now = DateTime.UtcNow;
        // One minute leeway to account for clock differences between machines
        var issuedAt = now.AddMinutes(-1);

        var claims = new Dictionary<string, object>
        {
            { "client_id", options.ClientId },
            { "iss", options.ClientId },
            { "user_id", "PABC" },
            { "user_representation", "PABC" }
        };

        var key = Encoding.UTF8.GetBytes(options.ClientSecret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            IssuedAt = issuedAt,
            NotBefore = issuedAt,
            Expires = now.AddHours(1),
            Claims = claims,
            Subject = new ClaimsIdentity(),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
