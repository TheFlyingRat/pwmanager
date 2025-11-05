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

        Console.Write("Note Title: ");
        string title = Console.ReadLine() ?? "";

        Console.Write("Note Content: ");
        string content = Console.ReadLine() ?? "";

        SecureNote newEntry = new SecureNote(title, content);

        SaveEntry(newEntry);
        return $"Secure note '{title}' added successfully.";
    }
}