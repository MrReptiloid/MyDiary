using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.EntityFrameworkCore;
using MyPersonalDiary.Application;
using MyPersonalDiary.Application.Interfaces;
using MyPersonalDiary.Application.Services;
using MyPersonalDiary.Core.Models;
using MyPersonalDiary.DataAccess;
using MyPersonalDiary.DataAccess.Repository;
using MyPersonalDiary.Extensions;
using MyPersonalDiary.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(nameof(JwtOptions)));
builder.Services.Configure<EncryptionSettings>(builder.Configuration.GetSection(nameof(EncryptionSettings)));    

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApiAuthentication(builder.Configuration);

builder.Services.AddDbContext<MyPersonalDiaryDbContext>(
    options =>
    {
        options.UseNpgsql(
            builder.Configuration.GetConnectionString(nameof(MyPersonalDiaryDbContext)));
    });

builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<InviteRepository>();
builder.Services.AddScoped<DiaryEntryRepository>();
builder.Services.AddSingleton<CaptchaInMemoryRepository>();

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<InviteService>();
builder.Services.AddScoped<AesEncryptionService>();
builder.Services.AddScoped<DiaryEntryService>();
builder.Services.AddScoped<CaptchaService>();

builder.Services.AddScoped<IJwtProvider, JwtProvider>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddHostedService<AccountCleanupService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(x =>
{
    x.WithOrigins("http://localhost:8080");
    x.WithMethods().AllowAnyMethod();
    x.WithHeaders().AllowAnyHeader();
    x.AllowCredentials();
});

app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.None,
    HttpOnly = HttpOnlyPolicy.None,
    Secure = CookieSecurePolicy.Always
});

app.UseHttpsRedirection();
app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();

app.Run();