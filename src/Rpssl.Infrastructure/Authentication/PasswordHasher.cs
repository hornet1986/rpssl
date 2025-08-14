using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Rpssl.Application.Abstractions;

namespace Rpssl.Infrastructure.Authentication;

internal sealed class PasswordHasher : IPasswordHasher
{
    private const int Iterations = 150_000; // PBKDF2 iterations
    private const int SaltSize = 16;
    private const int KeySize = 32;

    public string Hash(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] key = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA256, Iterations, KeySize);
        return $"v1.{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";
    }

    public bool Verify(string password, string passwordHash)
    {
        try
        {
            string[] parts = passwordHash.Split('.', StringSplitOptions.RemoveEmptyEntries);
            if (parts is not ["v1", _, _, _])
            {
                return false;
            }

            int iterationCount = int.Parse(parts[1]);
            byte[] salt = Convert.FromBase64String(parts[2]);
            byte[] expected = Convert.FromBase64String(parts[3]);
            byte[] actual = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA256, iterationCount, expected.Length);
            return CryptographicOperations.FixedTimeEquals(actual, expected);
        }
        catch { return false; }
    }
}
