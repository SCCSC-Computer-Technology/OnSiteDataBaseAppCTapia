using Microsoft.AspNetCore.Identity;

namespace OnSiteApp.Services;


public static class PasswordHelper
{
    private static readonly PasswordHasher<string> Hasher = new PasswordHasher<string>();

    // Turn a password into a secure hash for storage.
    public static string Hash(string password)
    {
        return Hasher.HashPassword("user", password);
    }

    // Check whether a password matches its saved hash.
    public static bool Check(string hash, string password)
    {
        return Hasher.VerifyHashedPassword("user", hash, password) != PasswordVerificationResult.Failed;
    }
}
