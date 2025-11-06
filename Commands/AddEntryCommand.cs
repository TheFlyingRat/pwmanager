using PWMan.Core;

namespace PWMan.Commands;

public class AddEntryCommand : Command
{
    readonly static string[] options = Enum.GetNames(typeof(EntryType));

    public AddEntryCommand() : base("add", $"Adds a new entry to the vault. Optional parameter [{string.Join("|", options)}]") { }
    public override string Execute(string[] args)
    {
        string selectedType = args.Length > 1 ? args[1] : "Generic";

        EntryType entryType = Enum.Parse<EntryType>(selectedType, true);

        switch (entryType)
        {
            case EntryType.Wifi:
                return new AddWifiEntryCommand().Execute(args);
            case EntryType.SecureNote:
                return new AddSecureNoteEntryCommand().Execute(args);
            case EntryType.Generic:
                return new AddGenericEntryCommand().Execute(args);
            default:
                Console.WriteLine($"Defaulting to generic entry! Usage: {base.Name} [{string.Join("|", options)}]");
                return new AddGenericEntryCommand().Execute(args);
        }
    }
}