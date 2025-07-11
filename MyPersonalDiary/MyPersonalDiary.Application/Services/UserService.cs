using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CSharpFunctionalExtensions;
using MyPersonalDiary.Application.Interfaces;
using MyPersonalDiary.Contracts;
using MyPersonalDiary.Core.Models;
using MyPersonalDiary.DataAccess.Repository;


namespace MyPersonalDiary.Application.Services;

public class UserService
{
    private readonly UserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;
    private readonly InviteRepository _inviteRepository;
    private readonly CaptchaService _captchaService;

    public UserService(
        UserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtProvider jwtProvider,
        InviteRepository inviteRepository,
        CaptchaService captchaService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
        _inviteRepository = inviteRepository;
        _captchaService = captchaService;
    }
    
    public async Task<Result> Register(RegisterUserRequest request)
    {
        bool isValidInvite = await _inviteRepository.IsInviteValidAsync(request.InviteCode);
        if (!isValidInvite)
        {
            return Result.Failure("Invalid invite code");
        }

        if (!_captchaService.CheckVerified(request.VerifiedCaptcha))
        {
            return Result.Failure("Captcha is invalid or not verified");
        }
        
        string hashedPassword = _passwordHasher.Generate(request.Password);

        User user = new ()
        {
            Id = Guid.NewGuid(),
            UserName = request.UserName,
            PasswordHash = hashedPassword,
            Email = request.Email,
            Role = request.UserRole,
        };

        await _inviteRepository.MarkAsUsedAsync(request.InviteCode, user.Id);
        
        await _userRepository.Create(user);
        
        return Result.Success();
    }

    public async Task<string> Login(string userName, string password)
    {
        var user = await _userRepository.GetByUserName(userName);
        if (user == null || user.IsDeleted)
            throw new Exception($"Failed to login: {userName}");

        var result = _passwordHasher.Verify(password, user.PasswordHash);

        if (!result)
            throw new Exception($"Failed to login: {userName}");

        var token = _jwtProvider.GenerateToken(user);

        return token;
    }

    public async Task<Result<string, string>> Verify(string tokenValue)
    {
        if (string.IsNullOrEmpty(tokenValue))
        {
            return Result.Failure<string, string>("Token is null or empty");
        }
            
        JwtSecurityToken token = _jwtProvider.ToToken(tokenValue);

        string? userName = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

        if (string.IsNullOrEmpty(userName))
            return Result.Failure<string, string>("Token does not contain a valid user name");

        User? user = await _userRepository.GetByUserName(userName);
        if (user == null || user.IsDeleted)
        {
            return Result.Failure<string, string>("User not found or deleted");
        }

        return Result.Success<string, string>(tokenValue);
    }
    
    public async Task<Result<Unit, string>> SoftDeleteAccountAsync(string tokenValue, string password)
    {
        try
        {
            JwtSecurityToken token = _jwtProvider.ToToken(tokenValue);
            string userIdString = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value!;
            Guid userId = Guid.Parse(userIdString);

            User? user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return Result.Failure<Unit, string>("User not found");
            }

            if (!_passwordHasher.Verify(password, user.PasswordHash))
            {
                return Result.Failure<Unit, string>("Invalid password");
            }

            user.IsDeleted = true;
            user.DeletionDate = DateTime.UtcNow.AddDays(2);

            await _userRepository.UpdateAsync(user);

            return Result.Success<Unit, string>(Unit.Value);
        }
        catch (Exception ex)
        {
            return Result.Failure<Unit, string>(ex.Message);
        }
    }
    
    public async Task<Result<Unit, string>> RestoreAccountAsync(string userName, string password)
    {
        try
        {
            var user = await _userRepository.GetByUsernameAsync(userName);
            if (user == null)
                return Result.Failure<Unit, string>("User not found");

            if (!user.IsDeleted)
                return Result.Failure<Unit, string>("Account is not marked for deletion");

            bool isPasswordValid = _passwordHasher.Verify(password, user.PasswordHash);
            if (!isPasswordValid)
                return Result.Failure<Unit, string>("Invalid password");

            user.IsDeleted = false;
            user.DeletionDate = null;

            await _userRepository.UpdateAsync(user);

            return Result.Success<Unit, string>(Unit.Value);
        }
        catch (Exception ex)
        {
            return Result.Failure<Unit, string>(ex.Message);
        }
    }
}