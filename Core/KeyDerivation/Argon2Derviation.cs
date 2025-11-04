// https://mojoauth.com/hashing/argon2-in-c/
// https://stackoverflow.com/questions/73914715/sha-512-and-or-argon2id-hashing-salt-for-dovecot-in-c-sharp

using Konscious.Security.Cryptography;
using System.Text;

namespace PWMan.Core.KeyDerivation;

public class Argon2Derivation : IKeyDerivation
{

    private int Iterations = 3; // Number of iterations for Argon2
    private int MemorySize = 1024 * 64; // 64 MB
    private int DegreeOfParallelism = 2; // Number of parallel threads
    private int KeySize = 32; // match sha256


    public byte[] DeriveKey(string password, byte[] salt)
    {
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
