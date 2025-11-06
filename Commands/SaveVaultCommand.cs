using PWMan.Core;

namespace PWMan.Commands;

public class SaveVaultCommand : Command
{
    public SaveVaultCommand() : base("save", "Saves the current vault to disk.") { }
    
    public override string Execute(string[] args)
    {
        Vault.Instance.SaveAll();
        return "Vault saved successfully.";
    }
}