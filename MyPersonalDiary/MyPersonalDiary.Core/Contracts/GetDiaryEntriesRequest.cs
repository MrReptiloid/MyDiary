namespace MyPersonalDiary.Contracts;

public class GetDiaryEntriesRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 5;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? SearchTerm { get; set; }
}