using PWMan.Core;

namespace PWMan.Commands;

public class AddEntryCommand : Command
{
    public AddEntryCommand() : base("add", "Adds a new entry to the vault. Optional parameter entryType") { }
    public override string Execute(string[] args)
    {
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
}