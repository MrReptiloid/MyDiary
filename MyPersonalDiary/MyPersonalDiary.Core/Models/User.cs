namespace MyPersonalDiary.Core.Models;

public class User
{
    public Guid Id { get; set; }
    public string UserName { get; set; } 
    public string PasswordHash { get; set; }
    public string Email { get; set; }
    public UserRole Role { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletionDate { get; set; }
}