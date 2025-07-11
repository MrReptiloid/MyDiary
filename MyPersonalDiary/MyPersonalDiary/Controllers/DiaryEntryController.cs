using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyPersonalDiary.Application.Services;
using MyPersonalDiary.Contracts;
using MyPersonalDiary.Core.Contracts;
using MyPersonalDiary.Core.Models;

namespace MyPersonalDiary.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class DiaryEntryController : ControllerBase
{
    private readonly DiaryEntryService _diaryEntryService;

    public DiaryEntryController(DiaryEntryService diaryEntryService)
    {
        _diaryEntryService = diaryEntryService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateDiaryEntry([FromForm] CreateDiaryEntityRequest request)
    {
        string? tokenValue = HttpContext.Request.Cookies["tasty-cookies"];
        
        if (string.IsNullOrEmpty(tokenValue))
        {
            return Unauthorized(new { error = "Authentication token is missing" });
        }

        Result<Unit, string> result = await _diaryEntryService.CreateDiaryEntryAsync(
            tokenValue,
            request.Content,
            request.Image);

        return result.IsSuccess 
            ? Ok() 
            : BadRequest(new { error = result.Error });
    }

    [HttpGet]
    public async Task<IActionResult> GetDiaryEntries([FromQuery] GetDiaryEntriesRequest request)
    {
        string? tokenValue = HttpContext.Request.Cookies["tasty-cookies"];
        
        if (string.IsNullOrEmpty(tokenValue))
        {
            return Unauthorized(new { error = "Authentication token is missing" });
        }

        Result<PagedResponse<DiaryEntryResponse>, string> result = await _diaryEntryService.GetDiaryEntriesAsync(
            tokenValue,
            request.PageNumber,
            request.PageSize,
            request.StartDate,
            request.EndDate,
            request.SearchTerm);

        return result.IsSuccess 
            ? Ok(result.Value) 
            : BadRequest(new { error = result.Error });
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDiaryEntry(Guid id)
    {
        string? tokenValue = HttpContext.Request.Cookies["tasty-cookies"];
    
        if (string.IsNullOrEmpty(tokenValue))
        {
            return Unauthorized(new { error = "Authentication token is missing" });
        }
    
        Result<Unit, string> result = await _diaryEntryService.DeleteDiaryEntryAsync(tokenValue, id);
    
        return result.IsSuccess 
            ? Ok() 
            : result.Error.Contains("permission") 
                ? Forbid() 
                : BadRequest(new { error = result.Error });
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDiaryEntry(Guid id, [FromForm] UpdateDiaryEntryRequest request)
    {
        if (id == Guid.Empty)
        {
            return BadRequest(new { error = "Invalid diary entry ID" });
        }
        
        string? tokenValue = HttpContext.Request.Cookies["tasty-cookies"];

        if (string.IsNullOrEmpty(tokenValue))
        {
            return Unauthorized(new { error = "Authentication token is missing" });
        }

        if (string.IsNullOrWhiteSpace(request.Content))
        {
            return BadRequest(new { error = "Content cannot be empty" });
        }

        Result<Unit, string> result = await _diaryEntryService.UpdateDiaryEntryAsync(
            tokenValue,
            id,
            request.Content,
            request.Image);

        return result.IsSuccess 
            ? Ok() 
            : result.Error.Contains("permission") 
                ? Forbid() 
                : BadRequest(new { error = result.Error });
    }
}