using System.Security.Cryptography;

namespace BankingApi.Security;

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string storedHash);
}


public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;        // 128-bit salt
    private const int HashSize = 32;        // 256-bit hash
    private const int Iterations = 100_000;
    private const string Prefix = "PBKDF2-SHA256";

    public string Hash(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, HashSize);
        return $"{Prefix}${Iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    public bool Verify(string password, string storedHash)
    {
        string[] parts = storedHash.Split('$');
        if (parts.Length != 4 || parts[0] != Prefix) return false;

        int iterations = int.Parse(parts[1]);
        byte[] salt = Convert.FromBase64String(parts[2]);
        byte[] expected = Convert.FromBase64String(parts[3]);

        byte[] actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, expected.Length);

        return CryptographicOperations.FixedTimeEquals(actual, expected);
    }
}
