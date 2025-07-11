using CSharpFunctionalExtensions;
using Microsoft.Extensions.Configuration;
using MyPersonalDiary.Core.Models;
using MyPersonalDiary.DataAccess.Repository;

namespace MyPersonalDiary.Application.Services;

public class CaptchaService
{
    private readonly CaptchaInMemoryRepository _captchaRepository;
    
    public CaptchaService(CaptchaInMemoryRepository captchaRepository)
    {
        _captchaRepository = captchaRepository;
    }
    
    public async Task<(List<string> links, string folder)> GenerateCaptchaAsync()
    {
        string sourceDir = Path.Combine("wwwroot", "CaptchaImages");
        string tempDir = "wwwroot/CaptchaTemp";

        if (!Directory.Exists(tempDir))
        {
            Directory.CreateDirectory(tempDir);
        }

        Guid folderGuid = Guid.NewGuid();
        string folderPath = Path.Combine(tempDir, folderGuid.ToString());

        Directory.CreateDirectory(folderPath);

        string[] imageFiles = Directory.GetFiles(sourceDir);
        List<string> newFileNames = new List<string>();
        string dogImageNewName = null;

        foreach (string imageFile in imageFiles)
        {
            string originalFileName = Path.GetFileName(imageFile).ToLower();
            string extension = Path.GetExtension(imageFile);
            string newFileName = Guid.NewGuid().ToString() + extension;
            string destFile = Path.Combine(folderPath, newFileName);

            if (originalFileName == "dog.jpeg")
            {
                _captchaRepository.MemorizeAnswer(newFileName.Split('.')[0], folderGuid.ToString());
            }

            File.Copy(imageFile, destFile, true);
            newFileNames.Add(newFileName);
        }

        newFileNames.Select(name => 
            Path.Combine(
                "CaptchaTemp",
                folderGuid.ToString(),
                name))
            .Order();
        
        return (newFileNames, folderGuid.ToString());
    }
    
    public async Task<Result<string, string>> VerifyCaptchaAsync(string answer)
    {
        string? verified = _captchaRepository.VerifyAnswer(answer);

        if (verified == null)
        {
            return Result.Failure<string, string>("Captcha verification failed");
        }
        
        return Result.Success<string, string>(verified);
    }
    
    public bool CheckVerified(string captcha)
    {
        return _captchaRepository.CheckVerified(captcha);
    }
}