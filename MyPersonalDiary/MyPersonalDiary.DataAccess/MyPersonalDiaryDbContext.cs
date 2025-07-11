using Microsoft.EntityFrameworkCore;
using MyPersonalDiary.Core.Models;

namespace MyPersonalDiary.DataAccess;

public class MyPersonalDiaryDbContext : DbContext
{
    public MyPersonalDiaryDbContext(DbContextOptions<MyPersonalDiaryDbContext> options)
        : base(options)
    {
        
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Invite> Invites { get; set; }
    public DbSet<DiaryEntry> DiaryEntries { get; set; }
}