namespace MyPersonalDiary.Core.Models;

public class Invite
{
    public Guid Id { get; init; }
    public string Code { get; init; }
    public string CreatorId { get; init; }
    public DateTime CreatedAt { get; init; }
    public bool IsUsed { get; set; }
    public Guid UsedById { get; set; }
}