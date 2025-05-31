namespace AppCollection.Helpers;
/// <summary>
/// Provides utility methods for hashing and verifying passwords using the BCrypt algorithm.
/// </summary>
public static class PasswordHelper
{
    /// <summary>
    /// Returns a hashed version of a plaintext password.
    /// </summary>
    public static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    /// <summary>
    /// Checks if the given plaintext password matches a hashed password.
    /// </summary>
    public static bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}