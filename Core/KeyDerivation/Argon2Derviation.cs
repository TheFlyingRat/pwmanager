// https://mojoauth.com/hashing/argon2-in-c/
// https://stackoverflow.com/questions/73914715/sha-512-and-or-argon2id-hashing-salt-for-dovecot-in-c-sharp

using Konscious.Security.Cryptography;
using System.Text;

namespace PWMan.Core.KeyDerivation;

public class Argon2Derivation : IKeyDerivation
{

    private int Iterations { get; }
    private int MemorySize { get; }
    private int DegreeOfParallelism { get; }
    private int KeySize { get; }

    public Argon2Derivation(int iterations = 3, int memorySize = 1024 * 64, int degreeOfParallelism = 2, int keySize = 32)
    {
        Iterations = iterations;
        MemorySize = memorySize;
        DegreeOfParallelism = degreeOfParallelism;
        KeySize = keySize;
    }

    public byte[] DeriveKey(string password, byte[] salt)
    {
        Log.Debug("Deriving Argon2id key...");
        Log.Debug("" + Iterations);
        var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = salt,
            Iterations = Iterations,
            MemorySize = MemorySize,
            DegreeOfParallelism = DegreeOfParallelism
        };

        byte[] hash = argon2.GetBytes(KeySize);
        return hash;
    }
}
