using PWMan.Core;

namespace PWMan.Commands;

public class AddGenericEntryCommand : AddEntryCommand
{
    public override string Name { get; protected set; } = "new";

    public override string Execute(string[] args)
    {
        if (Vault.Instance.IsLocked)
        {
            return "Vault is locked. Please unlock it first.";
        }

        Console.Write("Entry Name: ");
        string name = Console.ReadLine() ?? "";

        Console.Write("Username: ");
        string username = Console.ReadLine() ?? "";

        Console.Write("Password: ");
        string password = Console.ReadLine() ?? "";
        // TODO: hide password input

        Console.Write("URL: ");
        string url = Console.ReadLine() ?? "";

        Console.Write("Notes: ");
        string notes = Console.ReadLine() ?? "";

        Entry newEntry = new Entry(name)
        {
            Username = username,
            Password = password,
            Notes = notes,
            Url = url
        };

        SaveEntry(newEntry);
        return $"Entry '{name}' added successfully.";
    }
}