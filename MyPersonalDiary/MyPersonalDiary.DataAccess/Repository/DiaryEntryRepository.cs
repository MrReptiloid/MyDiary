using Microsoft.EntityFrameworkCore;
using MyPersonalDiary.Core.Models;

namespace MyPersonalDiary.DataAccess.Repository;

public class DiaryEntryRepository : RepositoryBase<DiaryEntry>
{
    public DiaryEntryRepository(MyPersonalDiaryDbContext dbContext) : base(dbContext)
    {
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

        var orderedQuery = query.OrderByDescending(entry => entry.CreatedAt);

        var totalCount = await orderedQuery.CountAsync();
    
        var entries = await orderedQuery
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (entries, totalCount);
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