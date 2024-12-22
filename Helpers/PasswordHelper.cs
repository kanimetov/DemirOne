using Microsoft.AspNetCore.Identity;

namespace Demir.Helpers;
public class PasswordHelper
{
    private readonly PasswordHasher<object> passwordHasher;

    public PasswordHelper()
    {
        passwordHasher = new PasswordHasher<object>();
    }

    public string HashPassword(string password)
    {
        return passwordHasher.HashPassword(null, password);
    }

    public bool VerifyPassword(string hashedPassword, string password)
    {
        var result = passwordHasher.VerifyHashedPassword(null, hashedPassword, password);
        return result == PasswordVerificationResult.Success;
    }
}