using System.Security.Cryptography;

namespace BankingApp.Security;
public static class PasswordHasher
{
    private const string Prefix = "PBKDF2$";
    private const int SaltSize = 16;        
    private const int HashSize = 32;          
    private const int Iterations = 100000;    

    public static string Hash(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, HashSize);
        return Prefix + Convert.ToBase64String(salt) + "$" + Convert.ToBase64String(hash);
    }

    public static bool Verify(string password, string stored)
    {
        if (!IsHashed(stored)) return password == stored;

        string[] parts = stored.Split('$');                
        byte[] salt = Convert.FromBase64String(parts[1]);
        byte[] expected = Convert.FromBase64String(parts[2]);

        byte[] actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, HashSize);
        return CryptographicOperations.FixedTimeEquals(actual, expected);
    }

    public static bool IsHashed(string stored) => stored.StartsWith(Prefix);
}