using System.Text;
using PWMan.Core;

namespace PWMan.Commands;

public class ListEntriesCommand : Command
{
    public override string Name { get; protected set; } = "list";

    public override string Execute(string[] args)
    {
        var helpRequested = base.Execute(args);
        if (helpRequested != "") { return helpRequested; }

        try
        {
            var entries = Vault.Instance.ListEntries();

            if (entries.Count == 0)
            {
                return "No entries found in the vault.";
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Entries in vault:");
            foreach (var entry in entries)
            {
                if (args.Length > 1 && args[1] == "details")
                {
                    sb.AppendLine($"- {entry.Title} (Username: {entry.Username}, Password: {entry.Password}, URL: {entry.Url}, Notes: {entry.Notes})");
                }
                else
                {
                    sb.AppendLine($"- {entry.Title} (Username: {entry.Username})");
                }
            }
            return sb.ToString();
        }
        catch (InvalidOperationException ex)
        {
            return ex.Message;
        }
    }


}