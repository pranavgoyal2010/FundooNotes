using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RepositoryLayer.Interface;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RepositoryLayer.Service;

public class AuthService : IAuthService
{
    //private readonly AppDbContext _appDbContext;
    private readonly IConfiguration _configuration;
    public AuthService(IConfiguration configuration)
    {
        //_appDbContext = appDbContext;
        _configuration = configuration;
    }

    public string GenerateJwtToken(int UserId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:SecretKey"]);

        // Ensure the key size is at least 256 bits (32 bytes)
        if (key.Length < 32)
        {
            throw new ArgumentException("JWT secret key must be at least 256 bits (32 bytes)");
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.NameIdentifier, UserId.ToString()), // Include user ID as a claim
                                                                      // Add more claims if needed
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
