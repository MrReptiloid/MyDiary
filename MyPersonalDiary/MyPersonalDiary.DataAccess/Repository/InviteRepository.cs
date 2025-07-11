using Microsoft.EntityFrameworkCore;
using MyPersonalDiary.Core.Models;

namespace MyPersonalDiary.DataAccess.Repository;

public class InviteRepository
{
    private readonly MyPersonalDiaryDbContext _dbContext;
    
    public InviteRepository(MyPersonalDiaryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreateInviteAsync(Invite invite)
    {
        _dbContext.Invites.Add(invite);
        await _dbContext.SaveChangesAsync();
    }
    
    public async Task<bool> MarkAsUsedAsync(string code, Guid userId)
    {
        var invite = await _dbContext.Invites
            .FirstOrDefaultAsync(i => i.Code == code && !i.IsUsed);

        if (invite == null)
            return false;

        invite.IsUsed = true;
        invite.UsedById = userId;

        await _dbContext.SaveChangesAsync();
        return true;
    }
    
    public async Task<bool> IsInviteValidAsync(string code)
    {
        var now = DateTime.UtcNow;
        return await _dbContext.Invites
            .AnyAsync(i => i.Code == code && !i.IsUsed);
    }
}