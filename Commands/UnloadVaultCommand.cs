// stub to delete

using PWMan.Core;

namespace PWMan.Commands;

public class UnloadVaultCommand : Command
{
    public UnloadVaultCommand() : base("unload", "Unloads any loaded vault and frees memory") { }

    public override string Execute(string[] args)
    {
        if (!Vault.Instance.IsLocked)
        {
            Vault.Instance.Lock();
        }

        Vault.Instance.Destroy();

        return "Successfully unloaded the vault.";
    }
}