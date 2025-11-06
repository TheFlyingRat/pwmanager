using System.Text;
using PWMan.Core;

namespace PWMan.Commands;

public class GetEntryCommand : Command
{
    public GetEntryCommand() : base("get", "Get details of a particular entry by its ID") { }

    public override string Execute(string[] args)
    {
        if (args.Length < 1) { return $"Usage: {base.Name} <id>"; }

        Guid guid = Guid.Parse(args[1]);

        Entry? e = Vault.Instance.GetEntry(guid);

        if (e == null)
        {
            return "Couldn't get an entry with ID: " + args[1];
        }

        // okay so we know the entry with the given ID does exist in the vault
        // pretty print!

        // no polymorphic design due to out of scope and time!

        StringBuilder sb = new StringBuilder();

        sb.AppendLine("Entry: " + e.Title + $"({e.EntryType})");
        sb.AppendLine("Created at: " + e.UpdatedAt);
        sb.AppendLine("========================");
        sb.AppendLine("Username: " + e.Username);
        sb.AppendLine("Password: " + e.Password);
        sb.AppendLine("========================");
        sb.AppendLine("URL: " + e.Url);
        sb.AppendLine("Notes: " + e.Notes);

        return sb.ToString();
    }
}