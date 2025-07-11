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

        return registerResult.IsSuccess 
            ? Ok() 
            : BadRequest(new { error = registerResult.Error });
    }

    [Route("login")]
    [HttpPost]
    public async Task<ActionResult> Login(LoginUserRequest request)
    {
        try
        {
            string token = await _userService.Login(request.UserName, request.Password);
            HttpContext.Response.Cookies.Append("tasty-cookies", token);
            return Ok(token);
        }
        catch (Exception ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
    }

    [Route("verify")]
    [HttpGet]
    public async Task<IActionResult> Verify()
    {
        string? tokenValue = HttpContext.Request.Cookies["tasty-cookies"];

        if (string.IsNullOrEmpty(tokenValue))
        {
            return Unauthorized(new { error = "Token is missing" });
        }

        var result = await _userService.Verify(tokenValue);

        return result.IsSuccess ? Ok() : Unauthorized(new { error = result.Error });
    }

    [HttpDelete]
    public async Task<IActionResult> SoftDeleteAccount(DeleteAccountRequest request)
    {
        string? tokenValue = HttpContext.Request.Cookies["tasty-cookies"];

        if (string.IsNullOrEmpty(tokenValue))
        {
            return Unauthorized(new { error = "Token is missing" });
        }

        var result = await _userService.SoftDeleteAccountAsync(tokenValue, request.Password);

        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error });
    }

    [HttpPost("restore")]
    public async Task<IActionResult> RestoreAccount([FromBody] RestoreAccountRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest(new { error = "Username and password are required" });
        }
        
        var result = await _userService.RestoreAccountAsync(request.UserName, request.Password);

        return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error });
    }

}