using PWMan.Core;

namespace PWMan.Commands;

public class SaveVaultCommand : Command
{
    public SaveVaultCommand() : base("save", "Saves the current vault to disk.") { }
    
    public override string Execute(string[] args)
    {
        if (!Vault.Instance.IsLocked)
        {
            return "No unlocked vault to save..";
        }

        Vault.Instance.SaveAll();
        return "Vault saved successfully.";
    }
}