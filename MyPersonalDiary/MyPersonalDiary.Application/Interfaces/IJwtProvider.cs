using System.IdentityModel.Tokens.Jwt;
using MyPersonalDiary.Core.Models;

namespace MyPersonalDiary.Application.Interfaces;

public interface IJwtProvider
{
    string GenerateToken(User user);
    JwtSecurityToken ToToken(string tokenValue);
}