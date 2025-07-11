using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyPersonalDiary.DataAccess.Repository;

namespace MyPersonalDiary.Application.Services;

public class AccountCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1);

    public AccountCleanupService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (IServiceScope scope = _scopeFactory.CreateScope())
            {
                UserRepository userRepository = scope.ServiceProvider.GetRequiredService<UserRepository>();
                DiaryEntryRepository diaryEntryRepository = scope.ServiceProvider.GetRequiredService<DiaryEntryRepository>();

                var usersToDelete = await userRepository.GetUsersToDeletePermanentlyAsync();

                foreach (var user in usersToDelete)
                {
                    await diaryEntryRepository.DeleteAllForUserAsync(user.Id);

                    await userRepository.DeletePermanentlyAsync(user.Id);
                }
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }
    }
}