using System.Text;
using PWMan.Core;

namespace PWMan.Commands;

public class SearchEntriesCommand : Command
{
    public SearchEntriesCommand() : base("search", "Searches for entries in the vault by title.") { }

    public override string Execute(string[] args)
    {
        if (args.Length < 2)
        {
            return "Usage: search <query>";
        }

        string query = string.Join(' ', args[1..]).ToLower(); // combine all args after command name as query

        // search all entries title for the case insensitive query
        List<Entry> results = Vault.Instance.SearchEntries(query);


        if (results.Count == 0) { return "No entries found matching the query."; }


        // generate a result string based on results we received from the vault
        StringBuilder output = new StringBuilder();
        output.AppendLine($"Found {results.Count} entries:");
    
        foreach (Entry entry in results)
        {
            output.AppendLine($"- {entry.Title} (Username: {entry.Username})");
        }

        return output.ToString();
    }
}