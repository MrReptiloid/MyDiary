using Microsoft.EntityFrameworkCore;
using MyPersonalDiary.Core.Models;

namespace MyPersonalDiary.DataAccess.Repository;

public class UserRepository 
{
    private readonly MyPersonalDiaryDbContext _context;

    public UserRepository(MyPersonalDiaryDbContext context)
    {
        _context = context;
    }

    public async Task Create(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task<User?> GetByUserName(string userName)
    {
        List<User> users = await _context.Users
            .AsNoTracking()
            .Where(u => !u.IsDeleted)
            .ToListAsync();

        return users.FirstOrDefault(u => u.UserName == userName);
    }
    
    public async Task<User?> GetByIdAsync(Guid id)
    {
        User? user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
        
        return user;
    }

    public async Task<User?> GetByUsernameAsync(string email)
    {
        User? user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserName == email);
        
        return user;
    }
    
    public async Task<IEnumerable<User>> GetUsersToDeletePermanentlyAsync()
    {
        return await _context.Users
            .Where(u => u.IsDeleted && u.DeletionDate <= DateTime.UtcNow)
            .ToListAsync();
    }
    
    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }
    
    public async Task DeletePermanentlyAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}