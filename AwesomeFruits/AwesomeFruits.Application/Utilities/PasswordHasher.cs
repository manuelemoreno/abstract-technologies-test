using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace AwesomeFruits.Application.Utilities;

public class PasswordHasher
{
    public static (string hash, byte[] salt) HashPassword(string password)
    {
        // Generate a 128-bit salt using a secure PRNG
        var salt = new byte[128 / 8];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        // Derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
        var hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password,
            salt,
            KeyDerivationPrf.HMACSHA256,
            100000,
            256 / 8));

        return (hashed, salt);
    }

    public static bool VerifyPassword(string hashedPassword, string password, byte[] salt)
    {
        // Derive the same key given the same salt and password
        var verificationHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password,
            salt,
            KeyDerivationPrf.HMACSHA256,
            100000,
            256 / 8));

        return hashedPassword == verificationHash;
    }
}