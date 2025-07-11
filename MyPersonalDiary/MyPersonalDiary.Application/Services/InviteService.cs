using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MyPersonalDiary.Application.Interfaces;
using MyPersonalDiary.Core.Models;
using MyPersonalDiary.DataAccess.Repository;

namespace MyPersonalDiary.Application.Services;

public class InviteService
{
    private readonly IJwtProvider _jwtProvider;
    private readonly InviteRepository _inviteRepository;

    public InviteService(IJwtProvider jwtProvider, InviteRepository inviteRepository)
    {
        _jwtProvider = jwtProvider;
        _inviteRepository = inviteRepository;
    }
    
    public async Task<string> GenerateInviteAsync(string creatorTokenValue)
    {
        JwtSecurityToken token = _jwtProvider.ToToken(creatorTokenValue);
        
        string inviteCode = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
    
        string creatorId = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value!;
        var invite = new Invite
        {
            Id = Guid.NewGuid(),
            Code = inviteCode,
            CreatorId = creatorId,
            CreatedAt = DateTime.UtcNow,
            IsUsed = false
        };
        
        await _inviteRepository.CreateInviteAsync(invite);
    
        return inviteCode;
    }
}