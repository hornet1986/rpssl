namespace Rpssl.Application.Abstractions;

public interface IPasswordHasher
{
    /// <summary>
    /// Produces a hashed representation of the provided plaintext password.
    /// </summary>
    /// <param name="password">The plaintext password to hash.</param>
    /// <returns>A hashed password string containing all data required for later verification.</returns>
    string Hash(string password);

    /// <summary>
    /// Verifies that a plaintext password matches the stored hash.
    /// </summary>
    /// <param name="password">The plaintext password provided by the user.</param>
    /// <param name="passwordHash">The previously stored hash (including salt & parameters).</param>
    /// <returns><c>true</c> if the password is valid; otherwise <c>false</c>.</returns>
    bool Verify(string password, string passwordHash);
}
