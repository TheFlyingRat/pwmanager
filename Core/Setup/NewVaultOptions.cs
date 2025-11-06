using PWMan.CLI;

namespace PWMan.Core.Setup
{
    public class NewVaultOptions // sets some defaults
    {
        public EncryptionType Encryption { get; set; } = EncryptionType.AES;
        public DerivationType Kdf { get; set; } = DerivationType.Argon2;
        public int Iterations { get; set; } = 0;
        public int KeySize { get; set; } = 32;
        public int SaltSize { get; set; } = 16;
        public RepositoryType RepositoryType { get; set; } = RepositoryType.Json;
        public string SaveFile { get; set; } = "vault.dat";


        // for validation only
        public int Argon2IterationsMin { get; set; } = 3;
        public int Argon2Iterations { get; set; } = 3;
        public int Argon2IterationsMax { get; set; } = 1024;
        public int Pbkdf2IterationsMin { get; set; } = 100_000;
        public int Pbkdf2Iterations { get; set; } = 300_000;
        public int Pbkdf2IterationsMax { get; set; } = 100_000_000;


        // default configuration
        public int Argon2MemorySize { get; set; } = 1024 * 64;
        public int Argon2Parallelism { get; set; } = 8;
    }
}