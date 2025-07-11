namespace MyPersonalDiary.Core.Models;

public class DiaryEntry
{
    public Guid Id { get; set; } 
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; } 
    public string? ImagePath { get; set; }
    public string EncryptedContent { get; set; } 
}
