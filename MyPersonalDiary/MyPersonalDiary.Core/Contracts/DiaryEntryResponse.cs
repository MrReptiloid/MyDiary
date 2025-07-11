namespace MyPersonalDiary.Core.Contracts;

public class DiaryEntryResponse
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? ImagePath { get; set; }
    public DateTime CreatedAt { get; set; }
}