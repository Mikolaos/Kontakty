using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Kontakty.Interfaces;
using Kontakty.Models;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Kontakty.Service;


public class TokenService : ITokenService
{

    private readonly IConfiguration _configuration;


    private readonly SymmetricSecurityKey _key;

    /// Provides functionality for creating JWT tokens.
    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
    }

    public string CreateToken(AppUser appUser)
    {
        var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, appUser.Email),
                new Claim(JwtRegisteredClaimNames.GivenName, appUser.UserName)
            };
        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(7),
            SigningCredentials = creds,
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"]
        };
        // Create JWT security token handler and generate token
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        // Serialize token to a string and return
        return tokenHandler.WriteToken(token);
    }
}