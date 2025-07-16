using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace MyPersonalDiary.DataAccess.Repository;

public class RepositoryBase<T> where T : class
{
    protected readonly MyPersonalDiaryDbContext _dbContext;
    
    public RepositoryBase(MyPersonalDiaryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Set<T>().FindAsync(id);
    }
    
    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbContext.Set<T>().ToListAsync();
    }

    public IEnumerable<T> GetAsync(Func<T, bool> predicate)
    {
        return  _dbContext.Set<T>().Where(predicate);
    }

    public async Task CreateAsync(T entity)
    {
        await _dbContext.Set<T>().AddAsync(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        _dbContext.Entry(entity).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        T? entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}