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
        string tokenValue = HttpContext.Request.Cookies["tasty-cookies"]!;

        Result<Unit, string> result = await _diaryEntryService.CreateDiaryEntryAsync(
            tokenValue,
            request.Content,
            request.Image);

        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [HttpGet]
    public async Task<IActionResult> GetDiaryEntries([FromQuery] GetDiaryEntriesRequest request)
    {
        string tokenValue = HttpContext.Request.Cookies["tasty-cookies"]!;

        Result<PagedResponse<DiaryEntryResponse>, string> result = await _diaryEntryService.GetDiaryEntriesAsync(
            tokenValue,
            request.PageNumber,
            request.PageSize,
            request.StartDate,
            request.EndDate,
            request.SearchTerm);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDiaryEntry(Guid id)
    {
        string tokenValue = HttpContext.Request.Cookies["tasty-cookies"]!;
    
        Result<Unit, string> result = await _diaryEntryService.DeleteDiaryEntryAsync(tokenValue, id);
    
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDiaryEntry(Guid id, [FromForm] UpdateDiaryEntryRequest request)
    {
        string tokenValue = HttpContext.Request.Cookies["tasty-cookies"]!;

        Result<Unit, string> result = await _diaryEntryService.UpdateDiaryEntryAsync(
            tokenValue,
            id,
            request.Content,
            request.Image);

        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }
}