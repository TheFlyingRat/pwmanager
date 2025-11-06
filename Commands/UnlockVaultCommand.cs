using System.Text;
using PWMan.Core;

namespace PWMan.Commands;

public class UnlockVaultCommand : Command
{
    public UnlockVaultCommand() : base("unlock", "Unlocks the password vault.") { }

    public override string Execute(string[] args)
    {
        if (args.Length < 2)
        {
            return "Usage: unlock <master password>";
        }

        if (!Vault.Exists)
        {
            // can't confirm vault exists
            return "No vault found to unlock. Did you load?";
        }

        if (!Vault.Instance.IsLocked)
        {
            return "Vault is already unlocked.";
        }

        try
        {
            Vault.Instance.Unlock(args[1]);
            return "Successfully unlocked!";
        }
        catch (InvalidOperationException ex)
        {
            return ex.Message;
        }
        catch (UnauthorizedAccessException)
        {
            return "Incorrect master password.";
        }
    }
}