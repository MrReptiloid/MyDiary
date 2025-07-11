using System.ComponentModel.DataAnnotations;

namespace MyPersonalDiary.Contracts;

public record LoginUserRequest(
    [Required] string UserName, 
    [Required] string Password);