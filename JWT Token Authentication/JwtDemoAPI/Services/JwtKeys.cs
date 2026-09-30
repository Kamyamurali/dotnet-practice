using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace JwtDemoAPI.Services;

public class JwtKeys
{
    public SymmetricSecurityKey HmacKey { get; }
    public RsaSecurityKey RsaSigningKey { get; }
    public RsaSecurityKey RsaEncryptionKey { get; }

    public JwtKeys(string secret)
    {
        HmacKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)) { KeyId = "hs256-key" };
        RsaSigningKey = new RsaSecurityKey(RSA.Create(2048)) { KeyId = "rs256-key" };
        RsaEncryptionKey = new RsaSecurityKey(RSA.Create(2048)) { KeyId = "jwe-key" };
    }
}