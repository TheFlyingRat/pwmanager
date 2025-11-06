using System.Text;
using PWMan.Core;

namespace PWMan.Commands;

public class DeleteEntryCommand : Command
{
    public DeleteEntryCommand() : base("delete", "Deletes an entry by its identifier") { }

    public override string Execute(string[] args)
    {
        if (Vault.Instance.IsLocked)
        {
            return "Vault is locked. Please unlock it first.";
        }

        // determine if we're deleting one, or to show delete menu
        if (args.Length < 2)
        {
            List<Entry> entries = Vault.Instance.ListEntries();

            StringBuilder output = new StringBuilder();
            output.AppendLine($"Entry IDs shown: ");

            foreach (Entry entry in entries)
            {
                output.AppendLine($"- {entry.Title} (ID: {entry.Id})");
            }

            output.AppendLine("Usage: delete (id)");

            return output.ToString();
        }



        Guid id = Guid.Parse(args[1]);
        if (Vault.Instance.GetEntry(id) != null)
        {
            Vault.Instance.DeleteEntry(id);
            return "Deleted!";
        } else
        {
            return $"Couldn't find an id of {args[1]}";
        }
    }
}
