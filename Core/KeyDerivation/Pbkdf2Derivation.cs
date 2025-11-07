using System.Security.Cryptography;

namespace PWMan.Core.KeyDerivation;

public class Pbkdf2Derivation : IKeyDerivation
{
    private int Iterations { get; }
    private int KeySize { get; }

    public Pbkdf2Derivation(int iterations = 100_000, int keySize = 32)
    {
        Iterations = iterations;
        KeySize = keySize;
    }

    public byte[] DeriveKey(string password, byte[] salt)
    {
        Log.Debug("Deriving Pbkdf2 key...");
        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
        return pbkdf2.GetBytes(KeySize);
    }
}
