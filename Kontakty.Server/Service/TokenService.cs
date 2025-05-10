using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Kontakty.Interfaces;
using Kontakty.Models;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Kontakty.Service;

/// <summary>
/// Service responsible for generating JSON Web Tokens (JWT) for user authentication.
/// </summary>
public class TokenService : ITokenService
{
    /// <summary>
    /// Represents the configuration settings used within the application.
    /// </summary>
    /// <remarks>
    /// This variable is used to retrieve application-specific configuration data,
    /// such as values related to JWT authentication, including keys, issuers, and audiences.
    /// It provides a mechanism to access configuration values from various sources supported by the IConfiguration interface.
    /// </remarks>
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Represents the symmetric security key used for signing JSON Web Tokens (JWT).
    /// The key is derived from a configuration setting and is used to ensure the integrity and authenticity of tokens.
    /// </summary>
    private readonly SymmetricSecurityKey _key;

    /// Provides functionality for creating JWT tokens.
    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
    }

    /// <summary>
    /// Generates a JSON Web Token (JWT) for the specified user.
    /// </summary>
    /// <param name="appUser">The user for whom the token is being created.</param>
    /// <returns>A JWT string representing the user's authentication token.</returns>
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