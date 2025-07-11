using System.Collections.Concurrent;
using System.Timers;

namespace MyPersonalDiary.DataAccess.Repository;

public class CaptchaInMemoryRepository : IDisposable
{
    private readonly ConcurrentDictionary<string, (string FolderName, DateTime CreatedAt)> _captchaDictionary = new();
    private readonly ConcurrentBag<string> _verified = new();
    private readonly System.Timers.Timer _cleanupTimer;

    public CaptchaInMemoryRepository()
    {
        _cleanupTimer = new System.Timers.Timer(TimeSpan.FromMinutes(1).TotalMilliseconds);
        _cleanupTimer.Elapsed += CleanupExpiredEntries;
        _cleanupTimer.AutoReset = true;
        _cleanupTimer.Start();
    }

    public void MemorizeAnswer(string trueCaptchaId, string folderName)
    {
        _captchaDictionary[trueCaptchaId] = (folderName, DateTime.Now);
    }

    public (string? verifiedId, string? folderId) VerifyAnswer(string answer)
    {
        if (_captchaDictionary.TryGetValue(answer, out var captchaInfo))
        {
            _verified.Add(answer);
            return (answer, captchaInfo.FolderName);
        }

        return (null, null);
    }

    public bool CheckVerified(string captcha)
    {
        return _verified.Contains(captcha);
    }

    private void CleanupExpiredEntries(object? sender, ElapsedEventArgs e)
    {
        DateTime cutoffTime = DateTime.Now.AddMinutes(-5);
        List<string> expiredKeys = new List<string>();

        foreach (var kvp in _captchaDictionary)
        {
            if (kvp.Value.CreatedAt < cutoffTime)
            {
                expiredKeys.Add(kvp.Key);
            }
        }

        foreach (string key in expiredKeys)
        {
            _captchaDictionary.TryRemove(key, out _);
        }
    }

    public void Dispose()
    {
        _cleanupTimer?.Stop();
        _cleanupTimer?.Dispose();
    }
}