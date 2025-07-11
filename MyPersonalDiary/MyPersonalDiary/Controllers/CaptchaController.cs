using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using MyPersonalDiary.Application.Services;
using MyPersonalDiary.Core.Contracts;

namespace MyPersonalDiary.Controllers;

[ApiController]
[Route("[controller]")]
public class CaptchaController : ControllerBase
{
    private readonly CaptchaService _captchaService;
    
    public CaptchaController(CaptchaService captchaService)
    {
        _captchaService = captchaService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCaptcha()
    {
         (List<string> links, string folder) =
             await _captchaService.GenerateCaptchaAsync();
         
        return Ok(new { Links = links, Folder = folder });
    }

    [HttpPost]
    public async Task<IActionResult> Verify(VerifyCaptchaRequest request)
    {
        Result<string, string> verifyResult = await _captchaService.VerifyCaptchaAsync(request.Answer);
    
        return verifyResult.IsSuccess
            ? Ok(new { Captcha = verifyResult.Value })
            : BadRequest("Captcha verification failed");
    }
}