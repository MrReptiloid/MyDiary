using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyPersonalDiary.Application.Services;

namespace MyPersonalDiary.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(Roles = "Admin")]
public class InviteController : ControllerBase
{
    private readonly InviteService _inviteService;
    public InviteController(InviteService inviteService)
    {
        _inviteService = inviteService;
    }

    [HttpGet]
    public async Task<IActionResult> GenerateInvite()
    { 
        string tokenValue = HttpContext.Request.Cookies["tasty-cookies"]!;

        string inviteCode = await _inviteService.GenerateInviteAsync(tokenValue);
        
        return Ok(new {InviteCode = inviteCode});
    }
}