using PWMan.Core;

namespace PWMan.Commands;

public class AddEntryCommand : Command
{
    public AddEntryCommand() : base("add", "Adds a new entry to the vault. Usage: add (optional: entryType)") { }
    public override string Execute(string[] args)
    {
        if (Vault.Instance.IsLocked)
        {
            return "Vault is locked. Please unlock it first.";
        }

        // test entry type
        if (args.Length > 1)
        {
            string entryType = args[1].ToLower();
            if (entryType == "wifi")
            {
                return new AddWifiEntryCommand().Execute(args);
            }
            else if (entryType == "securenote")
            {
                return new AddSecureNoteEntryCommand().Execute(args);
            }
            else
            {
                return $"Unknown entry type '{entryType}'.";
            }
        }
        else
        {
            return new AddGenericEntryCommand().Execute(args);
        }
    }
    
    protected static void SaveEntry(Entry entry)
    {
        Vault.Instance.AddEntry(entry);
    }
}