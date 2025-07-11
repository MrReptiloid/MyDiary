using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using MyPersonalDiary.Application.Services;
using MyPersonalDiary.Contracts;
using MyPersonalDiary.Core.Contracts;

namespace MyPersonalDiary.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController: ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }
    
    [Route("register")]
    [HttpPost]
    public async Task<ActionResult> Register([FromBody]RegisterUserRequest request)
    {
        Result registerResult = await _userService.Register(request);

        return registerResult.IsSuccess ? Ok() : BadRequest();
    }
    
    [Route("login")]  
    [HttpPost]
    public async Task<ActionResult> Login(LoginUserRequest request)
    {
        string token  = await _userService.Login(request.UserName, request.Password);
        
        HttpContext.Response.Cookies.Append("tasty-cookies", token);
        
        return Ok(token);
    }

    [Route("verify")]
    [HttpGet]
    public async Task<IActionResult> Verify()
    {
        string? tokenValue = HttpContext.Request.Cookies["tasty-cookies"];
        
        if (string.IsNullOrEmpty(tokenValue))
        {
            return Unauthorized("Token is missing");
        }
        
        var result = await _userService.Verify(tokenValue);

        return result.IsSuccess ? Ok() : Unauthorized();
    }
    
    [HttpDelete]
    public async Task<IActionResult> SoftDeleteAccount(DeleteAccountRequest request)
    {
        string? tokenValue = HttpContext.Request.Cookies["tasty-cookies"];
    
        if (string.IsNullOrEmpty(tokenValue))
        {
            return Unauthorized("Token is missing");
        }

        var result = await _userService.SoftDeleteAccountAsync(tokenValue, request.Password);

        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }
    
    [HttpPost("restore")]
    public async Task<IActionResult> RestoreAccount([FromBody] RestoreAccountRequest request)
    {
        var result = await _userService.RestoreAccountAsync(request.UserName, request.Password);

        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }
    
}