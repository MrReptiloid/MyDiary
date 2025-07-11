using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace MyPersonalDiary.Core.Contracts;

public record CreateDiaryEntityRequest
(
    [Required]
    [MaxLength(500)]
    string Content,
    IFormFile? Image
);