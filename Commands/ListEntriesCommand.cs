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
                    switch (entry.EntryType)
                    {
                        case EntryType.Wifi:
                            var wifiEntry = (WifiEntry)entry;
                            sb.AppendLine($"- {entry.Title} (SSID: {wifiEntry.Ssid}, Security: {wifiEntry.SecurityType}, Username: {entry.Username}, Password: {entry.Password}, URL: {entry.Url}, Notes: {entry.Notes}, CreatedAt: {entry.CreatedAt}, UpdatedAt: {entry.UpdatedAt})");
                            break;
                        case EntryType.SecureNote:
                            var noteEntry = (SecureNote)entry;
                            sb.AppendLine($"- {entry.Title} (Content: {noteEntry.Content}, Username: {entry.Username}, Password: {entry.Password}, URL: {entry.Url}, Notes: {entry.Notes}, CreatedAt: {entry.CreatedAt}, UpdatedAt: {entry.UpdatedAt})");
                            break;
                        case EntryType.Generic:
                            sb.AppendLine($"- {entry.Title} (Username: {entry.Username}, Password: {entry.Password}, URL: {entry.Url}, Notes: {entry.Notes}, CreatedAt: {entry.CreatedAt}, UpdatedAt: {entry.UpdatedAt})");
                            break;
                    }
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

    public override string GetHelp()
    {
        return "Lists all entries in the vault. Use 'list details' to show detailed information.";
    }

}