using PWMan.CLI;
using PWMan.Core;
using PWMan.Core.Setup;

namespace PWMan.Commands;

public class NewVaultCommand : Command
{
    public NewVaultCommand() : base("create", "Creates a new vault if it doesn't exist already") { }

    public override string Execute(string[] args)
    {
        if (!Vault.Instance.IsLocked)
        {
            return "Wait! You already have a vault unlocked! Please lock first.";
        }

        var o = new NewVaultOptions();

        // TODO: config file to determine what valid types are instead of having to hardcode these

        o.Password = GetDefaultValidate.GetStringRequired("Enter a new vault password: ");

        o.Encryption = GetDefaultValidate.ValidateString("Select encryption method (aes, caesar) [aes]: ", ["aes", "caesar"], "aes");

        o.Kdf = GetDefaultValidate.ValidateString("Select KDF (argon2, pbkdf2) [argon2]: ", ["argon2", "pkdf2"], "argon2");

        if (o.Kdf == "argon2")
        {
            // gets slow with higher than like 12 on laptops
            o.Iterations = GetDefaultValidate.ValidateInt("Select iteration count (3-1024) [3]: ", 3, 1024, 3);
        }
        else// if (kdf == "pbkdf2")
        {
            // safe ish
            o.Iterations = GetDefaultValidate.ValidateInt("Select iteration count (100,000-1,000,000) [350,000]: ", 100_000, 1_000_000, 350_000);
        } 

        o.SaveType = GetDefaultValidate.ValidateString("Select save type (json) [json]: ", ["json"], "json");

        o.SaveFile = GetDefaultValidate.GetString("Choose output file name [vault.json]: ", "vault.json");



        // factory
        var encStrategy = VaultWiring.BuildEncryption(o);
        var kdfStrategy = VaultWiring.BuildKdf(o);
        var repo        = VaultWiring.BuildRepository(o, encStrategy);
        

        // metadata
        VaultWiring.ApplyMetadata(Vault.Instance._metadata, o);
        VaultWiring.WireVault(encStrategy, kdfStrategy, repo, o.Password);

        // now tell repository to create - polymorphed
        Vault.Instance.Repository.Create();

        // unlock it with the same runtime password that was provided - Create() called encrypt, unlock calls decrypt again
        Vault.Instance.Unlock(o.Password);

        return "Vault has been created!";
    }
}