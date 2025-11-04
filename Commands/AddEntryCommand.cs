using PWMan.Core;

namespace PWMan.Commands;

public class AddEntryCommand : Command
{
    public override string Name { get; protected set; } = "new";

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
        Vault.Repository.SaveEntry(entry);
    }
}