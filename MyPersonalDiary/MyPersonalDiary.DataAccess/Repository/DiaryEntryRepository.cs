using Microsoft.EntityFrameworkCore;
using MyPersonalDiary.Core.Models;

namespace MyPersonalDiary.DataAccess.Repository;

public class DiaryEntryRepository
{
    private readonly MyPersonalDiaryDbContext _dbContext;

    public DiaryEntryRepository(MyPersonalDiaryDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task CreateAsync(DiaryEntry diaryEntry)
    {
        await _dbContext.DiaryEntries.AddAsync(diaryEntry);
        await _dbContext.SaveChangesAsync();
    }
    
    public async Task<(List<DiaryEntry> Entries, int TotalCount)> GetPaginatedEntriesAsync(
        Guid userId,
        int pageNumber,
        int pageSize,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        var query = _dbContext.DiaryEntries
            .Where(entry => entry.UserId == userId);

        if (startDate.HasValue)
            query = query.Where(entry => entry.CreatedAt >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(entry => entry.CreatedAt <= endDate.Value);

        int totalCount = await query.CountAsync();

        var entries = await query
            .OrderByDescending(entry => entry.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (entries, totalCount);
    }
    
    public async Task<DiaryEntry?> GetByIdAsync(Guid id)
    {
        return await _dbContext.DiaryEntries
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task DeleteAsync(Guid id)
    {
        var entry = await _dbContext.DiaryEntries.FindAsync(id);
        if (entry != null)
        {
            _dbContext.DiaryEntries.Remove(entry);
            await _dbContext.SaveChangesAsync();
        }
    }
    
    public async Task UpdateAsync(DiaryEntry entry)
    {
        _dbContext.DiaryEntries.Update(entry);
        await _dbContext.SaveChangesAsync();
    }
    
    public async Task DeleteAllForUserAsync(Guid userId)
    {
        var entries = await _dbContext.DiaryEntries
            .Where(e => e.UserId == userId)
            .ToListAsync();
    
        if (entries.Any())
        {
            _dbContext.DiaryEntries.RemoveRange(entries);
            await _dbContext.SaveChangesAsync();
        }
    }
}