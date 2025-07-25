﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CSharpFunctionalExtensions;
using MyPersonalDiary.Contracts;
using MyPersonalDiary.Core.Interfaces;
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
        try
        {
            if (string.IsNullOrWhiteSpace(request.UserName))
                return Result.Failure("Username cannot be empty");
                
            if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
                return Result.Failure("Password must be at least 6 characters long");
            
            if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains("@"))
                return Result.Failure("Invalid email format");
                
            var existingUser = _userRepository.GetByUserName(request.UserName);
            if (existingUser != null)
                return Result.Failure("Username already exists");

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
            
            await _userRepository.CreateAsync(user);
            
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Registration failed: {ex.Message}");
        }
    }

    public string Login(string userName, string password)
    {
        try
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentException("Username cannot be empty");
                
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Password cannot be empty");
                
            var user = _userRepository.GetByUserName(userName);
            if (user == null)
                throw new Exception("Invalid username or password");
                
            if (user.IsDeleted)
                throw new Exception("This account has been deleted");

            var result = _passwordHasher.Verify(password, user.PasswordHash);

            if (!result)
                throw new Exception("Invalid username or password");

            var token = _jwtProvider.GenerateToken(user);

            return token;
        }
        catch (Exception ex) when (ex is not ArgumentException)
        {
            // Rethrow but with generic message for security
            throw new Exception("Authentication failed");
        }
    }

    public async Task<Result<string, string>> Verify(string tokenValue)
    {
        try
        {
            if (string.IsNullOrEmpty(tokenValue))
            {
                return Result.Failure<string, string>("Token is null or empty");
            }
                
            JwtSecurityToken token = _jwtProvider.ToToken(tokenValue);
            if (token.ValidTo < DateTime.UtcNow)
            {
                return Result.Failure<string, string>("Token has expired");
            }

            string? userName = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(userName))
                return Result.Failure<string, string>("Token does not contain a valid user name");

            User user = _userRepository.GetByUserName(userName);
            if (user.IsDeleted)
            {
                return Result.Failure<string, string>("User not found or deleted");
            }

            return Result.Success<string, string>(tokenValue);
        }
        catch (Exception ex)
        {
            return Result.Failure<string, string>($"Token validation failed: {ex.Message}");
        }
    }
    
    public async Task<Result<Unit, string>> SoftDeleteAccountAsync(string tokenValue, string password)
    {
        try
        {
            if (string.IsNullOrEmpty(tokenValue))
                return Result.Failure<Unit, string>("Token cannot be empty");
                
            if (string.IsNullOrEmpty(password))
                return Result.Failure<Unit, string>("Password cannot be empty");
                
            JwtSecurityToken token = _jwtProvider.ToToken(tokenValue);
            string userIdString = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value!;
            
            if (string.IsNullOrEmpty(userIdString))
                return Result.Failure<Unit, string>("Invalid token format");
                
            Guid userId;
            if (!Guid.TryParse(userIdString, out userId))
                return Result.Failure<Unit, string>("Invalid user ID in token");

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
            return Result.Failure<Unit, string>($"Account deletion failed: {ex.Message}");
        }
    }
    
    public async Task<Result<Unit, string>> RestoreAccountAsync(string userName, string password)
    {
        try
        {
            if (string.IsNullOrEmpty(userName))
                return Result.Failure<Unit, string>("Username cannot be empty");
                
            if (string.IsNullOrEmpty(password))
                return Result.Failure<Unit, string>("Password cannot be empty");
                
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
            return Result.Failure<Unit, string>($"Account restoration failed: {ex.Message}");
        }
    }
}