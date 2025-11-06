using PWMan.CLI;
using PWMan.Core;

namespace PWMan.Commands;

public class AddSecureNoteEntryCommand : AddEntryCommand
{
    public override string Execute(string[] args)
    {
        if (Vault.Instance.IsLocked)
        {
            return "Vault is locked. Please unlock it first.";
        }

        string title = GetDefaultValidate.GetStringRequired("Note Title: ");
        string content = GetDefaultValidate.GetString("Note Content: ", "");

        SecureNote newEntry = new SecureNote(title, content);

        Vault.Instance.AddEntry(newEntry);
        return $"Secure note '{title}' added successfully.";
    }
}