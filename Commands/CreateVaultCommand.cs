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

        string password = GetDefaultValidate.GetPasswordRequired("Enter password: ");

        o.Encryption = GetDefaultValidate.ValidateEncryption("Select encryption method", o.Encryption);

        o.Kdf = GetDefaultValidate.ValidateDerivation("Select KDF", o.Kdf);

        if (o.Kdf == DerivationType.Argon2)
        {
            // gets slow with higher than like 12 on laptops
            o.Iterations = GetDefaultValidate.ValidateInt($"Select iteration count ({o.Argon2IterationsMin}-{o.Argon2IterationsMax}) [{o.Argon2Iterations}]: ", o.Argon2IterationsMin, o.Argon2IterationsMax, o.Argon2Iterations); // TODO config for recommended values
        }
        else// if (kdf == "pbkdf2")
        {
            // safe ish
            o.Iterations = GetDefaultValidate.ValidateInt($"Select iteration count ({o.Pbkdf2IterationsMin}-{o.Pbkdf2IterationsMax}) [{o.Iterations}]: ", o.Pbkdf2IterationsMin, o.Pbkdf2IterationsMax, o.Iterations);
        }

        o.KeySize = Convert.ToInt32(
            GetDefaultValidate.ValidateString("Enter key size (16/24/32) [32]: ", ["16", "24", "32"], "32")
        );

        // TODO: argon2 mem and parallelism are hardcoded in NewVaultOptions. make this configurable??

        o.RepositoryType = GetDefaultValidate.ValidateRepositoryType("Select save type", o.RepositoryType);

        if (o.RepositoryType != RepositoryType.Memory)
        {
            o.SaveFile = GetDefaultValidate.GetString($"Choose output file name [{o.SaveFile}]: ", o.SaveFile);

            if (File.Exists(o.SaveFile))
            {
                return "Vault already exists! Not implemented: overriding"; // TODO low priority
            }
        } else
        {
            o.SaveFile = "memory"; // just to keep the system happy
        }


        Log.Debug("Building encryption, kfc and repository...");

        // factory
        var encStrategy = VaultWiring.BuildEncryption(o);
        var kdfStrategy = VaultWiring.BuildKdf(o);
        var repo        = VaultWiring.BuildRepository(o, encStrategy);

        Log.Debug("Setting vault metadata and setting instance data...");

        // metadata
        VaultWiring.ApplyMetadata(Vault.Instance._metadata, o);
        VaultWiring.WireVault(encStrategy, kdfStrategy, repo, password);

        Log.Debug("Generating a repository...");

        // now tell repository to create - polymorphed
        Vault.Instance.Repository!.Create(Vault.Instance._metadata,password, kdfStrategy); // forgive null because we know WireVault assigned a repository

        // unlock it with the same runtime password that was provided - Create() called encrypt, unlock calls decrypt again
        Vault.Instance.Unlock(password);

        return "Vault has been created!";
    }
}