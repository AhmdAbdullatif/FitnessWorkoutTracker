using Application.Abstraction;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services.Authentication;

public class PasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<object> _hasher = new();

    public string HashPassword(string password)
    {
        return _hasher.HashPassword(null!, password);
    }

    public bool VerifyPassword(string hashedPassword, string enteredPassword)
    {
        var result = _hasher.VerifyHashedPassword(null!, hashedPassword, enteredPassword);
        return result == PasswordVerificationResult.Success;
    }
}
