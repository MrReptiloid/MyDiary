using Microsoft.Extensions.Configuration;

namespace MyPersonalDiary.DataAccess.Repository;

public class CaptchaInMemoryRepository
{
    private readonly Dictionary<string, (string FolderName, DateTime CreatedAt)> _captchaDictionary = new();
    private readonly List<string> _verified = new();
    
    public void MemorizeAnswer(string trueCaptchaId, string folderName)
    {
        _captchaDictionary[trueCaptchaId] = (folderName, DateTime.Now);
    }

    public string? VerifyAnswer(string answer)
    {
        bool isCorrect = _captchaDictionary.ContainsKey(answer);

        if (isCorrect)
        {
            _captchaDictionary.Remove(answer);
            _verified.Add(answer);
            return answer;
        }
        else
        {
            return null;
        }
    }

    public bool CheckVerified(string captcha)
    {
        return _verified.Contains(captcha);
    }
}