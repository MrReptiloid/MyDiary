using System.ComponentModel.DataAnnotations;
using MyPersonalDiary.Core.Models;

namespace MyPersonalDiary.Contracts;

public record RegisterUserRequest(
    [Required] string UserName, 
    [Required] string Password,
    [Required] string Email,
    [Required] UserRole UserRole,
    [Required] string InviteCode,
    [Required] string VerifiedCaptcha);