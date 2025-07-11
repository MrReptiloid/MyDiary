
using Microsoft.AspNetCore.Http;

namespace MyPersonalDiary.Core.Contracts;

public class UpdateDiaryEntryRequest
{
    public string Content { get; set; }
    public IFormFile? Image { get; set; }
}