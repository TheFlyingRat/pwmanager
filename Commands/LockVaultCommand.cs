using PWMan.Core;

namespace PWMan.Commands;

public class LockVaultCommand : Command
{
    public LockVaultCommand() : base("lock", "Locks the password vault.") { }
    public override string Execute(string[] args)
    {
        Vault.Instance.Lock();
        return "Vault locked successfully.";
    }
}