using PWMan.Core;

namespace PWMan.Commands;

public class LockVaultCommand : Command
{
    public override string Name { get; protected set; } = "lock";
    public override string Execute(string[] args)
    {
        var helpRequested = base.Execute(args);
        if (helpRequested != "") { return helpRequested; }

        Vault.Instance.Lock();
        return "Vault locked successfully.";
    }

    public override string GetHelp()
    {
        return $"Usage: {Name}\nLocks the password vault.";
    }
}