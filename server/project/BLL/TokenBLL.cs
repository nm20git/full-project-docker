using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using project.BLL.Interfaces;
using project.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Logging;

public class TokenBLL : ITokenBLL
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<TokenBLL> _logger;

    public TokenBLL(IConfiguration configuration, ILogger<TokenBLL> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public string CreateToken(User user)
    {
        _logger.LogInformation("BLL: Creating JWT token for UserId: {UserId}, Email: {Email}",
            user.Id, user.Email);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])
        );

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expireMinutes = int.Parse(_configuration["Jwt:ExpireMinutes"]);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expireMinutes),
            signingCredentials: creds
        );

        _logger.LogInformation("BLL: JWT token created successfully for UserId: {UserId}. ExpireMinutes: {ExpireMinutes}",
            user.Id, expireMinutes);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
