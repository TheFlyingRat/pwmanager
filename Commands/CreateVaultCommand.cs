using PWMan.CLI;
using PWMan.Core;
using PWMan.Core.Setup;

namespace PWMan.Commands;

public class CreateVaultCommand : Command
{
    public CreateVaultCommand() : base("create", "Creates a new vault if it doesn't exist already") { }

    public override string Execute(string[] args)
    {
        var o = new NewVaultOptions();

        // TODO: config file to determine what valid types are instead of having to hardcode these

        o.Password = GetDefaultValidate.GetStringRequired("Enter a new vault password: ");

        o.Encryption = GetDefaultValidate.ValidateEncryption("Select encryption method", o.Encryption);

        o.Kdf = GetDefaultValidate.ValidateDerivation("Select KDF", o.Kdf);

        if (o.Kdf == DerivationType.argon2)
        {
            // gets slow with higher than like 12 on laptops
            o.Iterations = GetDefaultValidate.ValidateInt($"Select iteration count ({o.Argon2IterationsMin}-{o.Argon2IterationsMax}) [{o.Argon2Iterations}]: ", o.Argon2IterationsMin, o.Argon2IterationsMax, o.Argon2Iterations); // TODO config for recommended values
        }
        else// if (kdf == "pbkdf2")
        {
            // safe ish
            o.Iterations = GetDefaultValidate.ValidateInt($"Select iteration count ({o.Pbkdf2IterationsMin}-{o.Pbkdf2IterationsMax}) [{o.Iterations}]: ", o.Pbkdf2IterationsMin, o.Pbkdf2IterationsMax, o.Iterations);
        } 

        o.RepositoryType = GetDefaultValidate.ValidateRepositoryType("Select save type", o.RepositoryType);

        if (o.RepositoryType != RepositoryType.memory)
        {
            o.SaveFile = GetDefaultValidate.GetString($"Choose output file name [{o.SaveFile}]: ", o.SaveFile);

            if (File.Exists(o.SaveFile))
            {
                return "Vault already exists! Not implemented: overriding"; // TODO low priority
            }
        } else
        {
            o.SaveFile = "memory";
        }


        Log.Debug("Building encryption, kfc and repository...");

        // factory
        var encStrategy = VaultWiring.BuildEncryption(o);
        var kdfStrategy = VaultWiring.BuildKdf(o);
        var repo        = VaultWiring.BuildRepository(o, encStrategy);

        Log.Debug("Setting vault metadata and setting instance data...");

        // metadata
        VaultWiring.ApplyMetadata(Vault.Instance._metadata, o);
        VaultWiring.WireVault(encStrategy, kdfStrategy, repo, o.Password);

        Log.Debug("Generating a repository...");

        // now tell repository to create - polymorphed
        Vault.Instance.Repository.Create(Vault.Instance._metadata, o.Password, kdfStrategy);

        // unlock it with the same runtime password that was provided - Create() called encrypt, unlock calls decrypt again
        Vault.Instance.Unlock(o.Password);

        return "Vault has been created!";
    }
}