using PWMan.Core.Encryption;
using PWMan.Core.KeyDerivation;
using PWMan.Data;

namespace PWMan.Core.Setup
{
    public static class VaultWiring
    {

        // helper method to determine encryption type to use based on options
        public static IEncryptionStrategy BuildEncryption(NewVaultOptions options)
        {
            switch (options.Encryption)
            {
                case EncryptionType.caesar:
                    return new Caesar();
                case EncryptionType.aes:
                default:
                    return new AES(saltSize: options.SaltSize);
            }
        }

        // helper to determine kdf based on options
        public static IKeyDerivation BuildKdf(NewVaultOptions options)
        {
            switch (options.Kdf)
            {
                case DerivationType.argon2:
                    return new Argon2Derivation(iterations: options.Iterations, keySize: options.KeySize);
                case DerivationType.pbkdf2:
                default:
                    return new Pbkdf2Derivation(iterations: options.Iterations, keySize: options.KeySize);
            }
        }

        // helper to determine repo type based on options
        public static IEntryRepository BuildRepository(NewVaultOptions options, IEncryptionStrategy enc)
        {
            switch (options.RepositoryType)
            {
                case RepositoryType.memory:
                    return new InMemoryRepository(enc);
                case RepositoryType.json:
                default:
                    return new JsonEntryRepository(options.SaveFile, enc);
            }
        }

        // helper to save the temporary options data to the metadata of the vault
        public static void ApplyMetadata(VaultMetadata meta, NewVaultOptions options)
        {
            meta.EncryptionMethod = options.Encryption;
            meta.DerivationMethod = options.Kdf;
            meta.Iterations = options.Iterations;
            meta.KeySize = options.KeySize;
            meta.SaltSize = options.SaltSize;
            meta.RepositoryType = options.RepositoryType;
            meta.SaveFile = options.SaveFile;
        }

        // helper to generate the vault (not instantiated until Vault.Instance is referenced because singleton)
        public static void WireVault(IEncryptionStrategy enc, IKeyDerivation kdf, IEntryRepository repo, string password)
        {
            Vault.Instance.Encryption = enc;
            Vault.Instance.KDF = kdf;
            Vault.Instance.Repository = repo;
            Vault.Instance.RuntimePassword = password;
        }
    }
}