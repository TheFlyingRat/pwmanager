using System.Security.Cryptography;

namespace PWMan.Core.KeyDerivation;

public class Pbkdf2Derivation : IKeyDerivation
{
    private int Iterations = 10000; // Number of iterations for PBKDF2
    private int KeySize = 32; // match sha256

    public byte[] DeriveKey(string password, byte[] salt)
    {
        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
        return pbkdf2.GetBytes(KeySize);
    }
}
