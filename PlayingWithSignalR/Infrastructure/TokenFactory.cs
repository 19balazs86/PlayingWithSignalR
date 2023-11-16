using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PlayingWithSignalR.Infrastructure;

public static class TokenFactory
{
    private static readonly string _issuer   = "http://localhost:5000";
    private static readonly string _audience = "http://localhost:5000";

    private static SecurityKey _securityKeySymm = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey_WhichIsEnoughLong@345"));

    private static readonly SigningCredentials _signingCredential =
        new SigningCredentials(_securityKeySymm, SecurityAlgorithms.HmacSha256);

    public static readonly TokenValidationParameters TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer           = true,
        ValidateAudience         = true,
        ValidateLifetime         = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer      = _issuer,
        ValidAudience    = _audience,
        IssuerSigningKey = _securityKeySymm
    };

    public static string CreateToken(IEnumerable<Claim> claims)
    {
        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject            = new ClaimsIdentity(claims),
            Issuer             = _issuer,
            Audience           = _audience,
            Expires            = DateTime.Now.AddDays(1),
            SigningCredentials = _signingCredential
        };

        SecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
