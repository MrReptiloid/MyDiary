using Microsoft.EntityFrameworkCore;
using MyPersonalDiary.Core.Models;

namespace MyPersonalDiary.DataAccess.Repository;

public class UserRepository : RepositoryBase<User>
{

    public UserRepository(MyPersonalDiaryDbContext context): base(context)
    {
    }
    
    public User GetByUserName(string userName)
    {
        return _dbContext.Users
            .Where(u => !u.IsDeleted)
            .FirstOrDefault(u => u.UserName == userName)!;
    }
    
    public async Task<User?> GetByUsernameAsync(string email)
    {
        User? user = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserName == email);
        
        return user;
    }
    
    public async Task<IEnumerable<User>> GetUsersToDeletePermanentlyAsync()
    {
        return await _dbContext.Users
            .Where(u => u.IsDeleted && u.DeletionDate <= DateTime.UtcNow)
            .ToListAsync();
    }
    
    
    public async Task DeletePermanentlyAsync(Guid userId)
    {
        var user = await _dbContext.Users.FindAsync(userId);
        if (user != null)
        {
            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
        }
    }
}