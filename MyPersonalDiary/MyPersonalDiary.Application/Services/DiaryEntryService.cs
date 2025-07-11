using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Http;
using MyPersonalDiary.Application.Interfaces;
using MyPersonalDiary.Core.Contracts;
using MyPersonalDiary.Core.Models;
using MyPersonalDiary.DataAccess.Repository;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace MyPersonalDiary.Application.Services;

public class DiaryEntryService
{
    private readonly DiaryEntryRepository _diaryEntryRepository;
    private readonly AesEncryptionService _encryptionService;
    private readonly IJwtProvider _jwtProvider;
    private const int MaxFileSizeInBytes = 10 * 1024 * 1024;

    public DiaryEntryService(
        DiaryEntryRepository diaryEntryRepository,
        AesEncryptionService encryptionService,
        IJwtProvider jwtProvider)
    {
        _diaryEntryRepository = diaryEntryRepository;
        _encryptionService = encryptionService;
        _jwtProvider = jwtProvider;
    }

    public async Task<Result<Unit, string>> CreateDiaryEntryAsync(
        string tokenValue,
        string content,
        IFormFile? imageFile)
    {
        string? imagePath = null;
        if (imageFile != null && imageFile.Length > 0)
        {
            imagePath = await SaveImageFileAsync(imageFile);
        }
            
        JwtSecurityToken token = _jwtProvider.ToToken(tokenValue);

        string userIdString = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value!;
        
        var encryptedContent = _encryptionService.Encrypt(content);
        
        var diaryEntry = new DiaryEntry
        {
            Id = Guid.NewGuid(),
            UserId = Guid.Parse(userIdString),
            EncryptedContent = encryptedContent,
            ImagePath = imagePath
        };

        await _diaryEntryRepository.CreateAsync(diaryEntry);
        
        return Result.Success<Unit, string>(Unit.Value);
    }

    private async Task<string> SaveImageFileAsync(IFormFile imageFile)
    {
        string fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
        string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads");

        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        string filePath = Path.Combine(uploadsFolder, fileName);

        if (imageFile.Length > MaxFileSizeInBytes)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                await imageFile.CopyToAsync(memoryStream);
                memoryStream.Position = 0;
                
                using (Image image = Image.Load(memoryStream))
                {
                    JpegEncoder encoder = new JpegEncoder
                    {
                        Quality = 70
                    };
                    
                    if (imageFile.Length > 2 * MaxFileSizeInBytes)
                    {
                        int maxDimension = Math.Max(image.Width, image.Height);
                        float scaleFactor = 1920f / maxDimension;
                        
                        image.Mutate(x => x.Resize((int)(image.Width * scaleFactor), (int)(image.Height * scaleFactor)));
                    }
                    
                    image.Save(filePath, encoder);
                }
                
                return filePath;
            }
        }
        else
        {
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }
            
            return filePath;
        }
    }

    public async Task<Result<PagedResponse<DiaryEntryResponse>, string>> GetDiaryEntriesAsync(
        string tokenValue,
        int pageNumber,
        int pageSize,
        DateTime? startDate,
        DateTime? endDate,
        string? searchTerm)
    {
        try
        {
            JwtSecurityToken token = _jwtProvider.ToToken(tokenValue);
            string userIdString = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value!;
            Guid userId = Guid.Parse(userIdString);

            (List<DiaryEntry> entries, int totalCount) = await _diaryEntryRepository.GetPaginatedEntriesAsync(
                userId, pageNumber, pageSize, startDate, endDate);

            List<DiaryEntryResponse> decryptedEntries = new List<DiaryEntryResponse>();
            foreach (var entry in entries)
            {
                string decryptedContent = _encryptionService.Decrypt(entry.EncryptedContent);

                if (!string.IsNullOrEmpty(searchTerm) &&
                    !decryptedContent.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    continue;

                decryptedEntries.Add(new DiaryEntryResponse
                {
                    Id = entry.Id,
                    Content = decryptedContent,
                    ImagePath = entry.ImagePath != null ? Path.GetFileName(entry.ImagePath) : null,
                    CreatedAt = entry.CreatedAt
                });
            }

            PagedResponse<DiaryEntryResponse> response = new()
            {
                Items = decryptedEntries,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            return Result.Success<PagedResponse<DiaryEntryResponse>, string>(response);
        }
        catch (Exception ex)
        {
            return Result.Failure<PagedResponse<DiaryEntryResponse>, string>(ex.Message);
        }
    }
    
    public async Task<Result<Unit, string>> UpdateDiaryEntryAsync(
        string tokenValue,
        Guid diaryEntryId,
        string content,
        IFormFile? image)
    {
        try
        {
            JwtSecurityToken token = _jwtProvider.ToToken(tokenValue);
            string userIdString = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value!;
            Guid userId = Guid.Parse(userIdString);

            DiaryEntry? entry = await _diaryEntryRepository.GetByIdAsync(diaryEntryId);

            if (entry == null)
                return Result.Failure<Unit, string>("Diary entry not found.");

            if (entry.UserId != userId)
                return Result.Failure<Unit, string>("You don't have permission to modify this diary entry.");

            entry.EncryptedContent = _encryptionService.Encrypt(content);
            
            if (image != null && image.Length > 0)
            {
                if (!string.IsNullOrEmpty(entry.ImagePath) && File.Exists(entry.ImagePath))
                {
                    File.Delete(entry.ImagePath);
                }

                string filePath = await SaveImageFileAsync(image);
                entry.ImagePath = filePath;
            }

            await _diaryEntryRepository.UpdateAsync(entry);
            return Result.Success<Unit, string>(Unit.Value);
        }
        catch (Exception ex)
        {
            return Result.Failure<Unit, string>($"Error updating diary entry: {ex.Message}");
        }
    }
    
    public async Task<Result<Unit, string>> DeleteDiaryEntryAsync(string tokenValue, Guid diaryEntryId)
    {
        try
        {
            JwtSecurityToken token = _jwtProvider.ToToken(tokenValue);
            string userIdString = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value!;
            Guid userId = Guid.Parse(userIdString);
        
            DiaryEntry? entry = await _diaryEntryRepository.GetByIdAsync(diaryEntryId);

            if (entry == null)
            {
                return Result.Failure<Unit, string>("Diary entry not found.");
            }

            if ((DateTime.UtcNow - entry.CreatedAt).TotalDays > 2)
            {
                return Result.Failure<Unit, string>("Entries older than 2 days cannot be deleted.");
            }

            if (entry.UserId != userId)
            {
                return Result.Failure<Unit, string>("You don't have permission to delete this diary entry.");
            }
            
            if (!string.IsNullOrEmpty(entry.ImagePath) && File.Exists(entry.ImagePath))
            {
                File.Delete(entry.ImagePath);
            }
        
            await _diaryEntryRepository.DeleteAsync(diaryEntryId);
        
            return Result.Success<Unit, string>(Unit.Value);
        }
        catch (Exception ex)
        {
            return Result.Failure<Unit, string>(ex.Message);
        }
    }
    
    
}