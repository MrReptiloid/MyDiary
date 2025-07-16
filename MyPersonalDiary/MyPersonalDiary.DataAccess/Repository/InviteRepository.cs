using Microsoft.EntityFrameworkCore;
using MyPersonalDiary.Core.Models;

namespace MyPersonalDiary.DataAccess.Repository;

public class InviteRepository : RepositoryBase<Invite>
{
    public InviteRepository(MyPersonalDiaryDbContext dbContext) : base(dbContext)
    {
        
    }
    
    public async Task MarkAsUsedAsync(string code, Guid userId)
    {
        var invite = await _dbContext.Invites
            .FirstOrDefaultAsync(i => i.Code == code && !i.IsUsed);

        if (invite == null) return;

        invite.IsUsed = true;
        invite.UsedById = userId;

        await _dbContext.SaveChangesAsync();
    }
    
    public async Task<bool> IsInviteValidAsync(string code)
    {
        var now = DateTime.UtcNow;
        return await _dbContext.Invites
            .AnyAsync(i => i.Code == code && !i.IsUsed);
    }
}