using System.Text;
using PWMan.CLI;
using PWMan.Core;

namespace PWMan.Commands;

public class UnlockVaultCommand : Command
{
    public UnlockVaultCommand() : base("unlock", "Unlocks the password vault.") { }

    public override string Execute(string[] args)
    {
        if (!Vault.Instance.IsLocked)
        {
            return "Vault is already unlocked.";
        }

        string password = GetDefaultValidate.GetPasswordRequired("Enter password: ");

        try
        {
            Vault.Instance.Unlock(password);
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