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

        List<string> sortedLinks = newFileNames.OrderBy(f => f.GetHashCode()).ToList();
        
        return (sortedLinks, folderGuid.ToString());
    }
    
    public async Task<Result<string, string>> VerifyCaptchaAsync(string answer)
    {
        (string? verifiedId, string? folderId) = _captchaRepository.VerifyAnswer(answer);

        if (verifiedId == null || folderId == null)
        {
            return Result.Failure<string, string>("Captcha answer is incorrect or not found");
        }
        
        CleanupTempFiles(folderId);
        
        return Result.Success<string, string>(verifiedId);
    }
    
    public bool CheckVerified(string captcha)
    {
        return _captchaRepository.CheckVerified(captcha);
    }
    
    private void CleanupTempFiles(string folderGuid)
    {
        try
        {
            string tempFolderPath = Path.Combine("wwwroot", "CaptchaTemp", folderGuid);
            if (Directory.Exists(tempFolderPath))
            {
                Directory.Delete(tempFolderPath, true);
            }
        }
        catch 
        {
            Console.WriteLine("For now, we'll silently continue as cleanup failure shouldn't break the flow");
        }
    }
}