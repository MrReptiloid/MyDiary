namespace MyPersonalDiary.Core.Contracts;

public record RestoreAccountRequest(
    string UserName,
    string Password
);
