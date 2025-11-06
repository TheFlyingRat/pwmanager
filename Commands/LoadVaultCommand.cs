using PWMan.CLI;
using PWMan.Core;
using PWMan.Core.Encryption;
using PWMan.Core.KeyDerivation;
using PWMan.Data;

namespace PWMan.Commands;

public class LoadVaultCommand : Command
{
    readonly static string[] options = Enum.GetNames(typeof(RepositoryType));

    public LoadVaultCommand() : base("load", "Loads an existing vault") { }

    public override string Execute(string[] args)
    {
        // validate args
        if (args.Length < 3) { return $"Usage: {base.Name} <{string.Join("|", options)}> <path-to-vault>"; }

        RepositoryType repoType;
        try
        {
            repoType = Enum.Parse<RepositoryType>(args[1].Trim(), ignoreCase: true);
        }
        catch 
        {
            return $"Unknown repository type: {args[1].Trim()}";
        }

        string path = args[2].Trim().ToLower();

        // we cant use memory repositories because they're deleted. 
        if (repoType == RepositoryType.Memory) { return "In-Memory repositories are destroyed after unloading!"; }

        // 1. create a probe for metadata - plaintext, so encryption is undetermined
        IEntryRepository probe = CreateRepoWithoutEncryption(repoType, path);
        
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
        metadata.SaveFile ??= path; // update metadata of the vault to the current file name
                                    // TODO: this can be a save as feature probably, maybe rename vault

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
        string password = GetDefaultValidate.GetPasswordRequired("Enter password: "); // creation of vault prevents empty passwords anyways


        // 4. attempt to unlock (quality of life rather than just running "unlock" again)
        try
        {
            Vault.Instance.Unlock(password);
            return $"Loaded vault! {Vault.Instance._entries.Count} entries ready.";
        }
        catch (UnauthorizedAccessException)
        {
            Vault.Instance.RuntimePassword = null; // clear on failure
            Vault.Instance.Destroy();
            return "Incorrect master password";
        }
    }






    private static IEntryRepository CreateRepoWithoutEncryption(RepositoryType type, string path)
    {
        switch (type)
        {
            default:
            case RepositoryType.Json:
                return new JsonEntryRepository(path); // null encryption - only used for retrieval of metadata
            // case "sqlite": // TODO
               // return null;
        }
    }

    private static IEntryRepository CreateRepoWithEncryption(RepositoryType type, string path, IEncryptionStrategy enc)
    {
        switch (type)
        {
            default:
            case RepositoryType.Json:
                return new JsonEntryRepository(path, enc); // actual repo object to be used on the vault instance
            // case "sqlite": 
            //     return null; // TODO
        }
    }
    
    private static IKeyDerivation BuildKdf(VaultMetadata meta)
    {
        DerivationType method  = meta.DerivationMethod; // default
        int keySize = (meta.KeySize == 16 || meta.KeySize == 24 || meta.KeySize == 32) ? meta.KeySize : 32;

        switch (method)
        {
            case DerivationType.Argon2:
            default:
                return new Argon2Derivation(iterations: meta.Iterations, keySize: keySize, degreeOfParallelism: meta.Parallelism, memorySize: meta.MemorySize);
            case DerivationType.PBKDF2:
                return new Pbkdf2Derivation(iterations: meta.Iterations, keySize: keySize);
        }
    }

    private static IEncryptionStrategy BuildEncryption(VaultMetadata meta)
    {
        EncryptionType method = meta.EncryptionMethod; // enums use the first value as default https://stackoverflow.com/questions/4967656/what-is-the-default-value-for-enum-variable

        switch (method)
        {
            case EncryptionType.Caesar:
                return new Caesar();
            case EncryptionType.AES:
            default:
                return new AES(saltSize: meta.SaltSize);
        }
    }
}
