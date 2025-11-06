using PWMan.CLI;
using PWMan.Core;
using PWMan.Core.Encryption;
using PWMan.Core.KeyDerivation;
using PWMan.Data;

namespace PWMan.Commands;

public class LoadVaultCommand : Command
{
    public LoadVaultCommand() : base("load", "Loads an existing vault") { }

    public override string Execute(string[] args)
    {

        // validate args
        if (args.Length < 3) { return "Usage: load <repository-type> <path-to-vault>"; }


        // test if theres a currently unlocked vault before we allow loading another one
        if (!Vault.Instance.IsLocked)
        {
            return "Wait! You already have a vault unlocked! Please lock first.";
        }

        string repoType = args[1].Trim().ToLower();
        string path = args[2].Trim().ToLower();


        // 1. create a probe for metadata - plaintext, so encryption is undetermined
        IEntryRepository probe = CreateRepoWithoutEncryption(repoType, path);
        if (probe == null) { return $"Unknown repository type: {repoType}"; }

        
        VaultMetadata metadata;
        try
        {
            metadata = probe.ReadMetadata();
            if (metadata == null) { return "Invalid or missing vault metadata."; };
        }
        catch (Exception ex)
        {
            return $"Failed to read vault metadata: {ex.Message}";
        }
        metadata.SaveFile ??= path; // use the one from the metadata of the repository, otherwise the one user entered
                                    // TODO: this can be a save as feature probably


        // 2. we know the metadata, so based on it, we can try to create the kdf, encryption and repository type
        Log.Debug("Trying to build kdf, encryption and repository");
        IKeyDerivation kdf = BuildKdf(metadata);
        IEncryptionStrategy enc = BuildEncryption(metadata);
        IEntryRepository repo = CreateRepoWithEncryption(repoType, path, enc);
        
        Vault.Instance._metadata = metadata;
        Vault.Instance.Repository = repo;
        Vault.Instance.Encryption = enc;
        Vault.Instance.KDF = kdf;


        // 3. utilising helper, we can prompt for the vault password - this will fail if the metadata we retrieved was modified externally
        string password = GetDefaultValidate.GetStringRequired("Enter vault password: "); // creation of vault prevents empty passwords anyways


        // 4. attempt to unlock
        try
        {
            Vault.Instance.Unlock(password);
            return $"Loaded vault! {Vault.Instance._entries.Count} entries ready.";
        }
        catch (UnauthorizedAccessException)
        {
            Vault.Instance.RuntimePassword = null; // clear on failure
            return "Incorrect master password";
        }
    }






    private static IEntryRepository? CreateRepoWithoutEncryption(string type, string path)
    {
        switch (type)
        {
            case "json":
                return new JsonEntryRepository(path, null); // null encryption - only used for retrieval of metadata
            case "sqlite":
                return null; // TODO
            default:
                return null;
        }
    }

    private static IEntryRepository? CreateRepoWithEncryption(string type, string path, IEncryptionStrategy enc)
    {
        switch (type)
        {
            case "json":   
                return new JsonEntryRepository(path, enc); // actual repo object to be used on the vault instance
            case "sqlite": 
                return null; // TODO
            default:       
                return null;
        }
    }
    
    private static IKeyDerivation BuildKdf(VaultMetadata meta)
    {
        string method  = (meta.DerivationMethod ?? "pbkdf2").ToLower(); // default
        int iterations = meta.Iterations > 0 ? meta.Iterations : 100_000; // if the metadata shows argon2, but no iteration count, itll run with 100,000 iterations and lag
        int keySize    = (meta.KeySize == 16 || meta.KeySize == 24 || meta.KeySize == 32) ? meta.KeySize : 32; // verify for aes

        switch (method)
        {
            case "argon2":
                return new Argon2Derivation(iterations: iterations, keySize: keySize);
            case "pbkdf2":
            default:
                return new Pbkdf2Derivation(iterations: iterations, keySize: keySize);
        }
    }

    private static IEncryptionStrategy BuildEncryption(VaultMetadata meta)
    {
        string method = (meta.EncryptionMethod ?? "aes").ToLower(); // default
        int saltSize = meta.SaltSize > 0 ? meta.SaltSize : 16;

        switch (method)
        {
            case "caesar":
                return new Caesar();
            case "aes":
            default:
                return new AES(saltSize: saltSize);
        }
    }
}
