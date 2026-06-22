using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Abstraction;
using Domain.Entities;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services.Authentication;

public class JwtProvider(JwtOptions jwtOptions) : IJwtProvider
{
    public string Create(Guid id, string email)
    {
        var descriptor = new SecurityTokenDescriptor()
        {
            Expires = DateTime.UtcNow.AddMinutes(jwtOptions.LifeTime),
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                new Claim(ClaimTypes.Email, email),
            }),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
                SecurityAlgorithms.HmacSha256)
        };

        var handler = new JwtSecurityTokenHandler();
        var securityToken = handler.CreateToken(descriptor);
        return handler.WriteToken(securityToken);
    }

}
