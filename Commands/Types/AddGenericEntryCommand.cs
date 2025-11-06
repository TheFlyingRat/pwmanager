using PWMan.CLI;
using PWMan.Core;

namespace PWMan.Commands;

public class AddGenericEntryCommand : AddEntryCommand
{
    public override string Execute(string[] args)
    {
        string name = GetDefaultValidate.GetString("Enter Name: ", "<< untitled >>"); // no need to name generic entries...
        string username = GetDefaultValidate.GetString("Username: ", "");
        string password = GetDefaultValidate.GetString("Password: ", ""); // TODO: hide password input
        string url = GetDefaultValidate.GetString("URL: ", "");
        string notes = GetDefaultValidate.GetString("Notes: ", "");

        Entry newEntry = new Entry(name)
        {
            Username = username,
            Password = password,
            Notes = notes,
            Url = url
        };

        Vault.Instance.AddEntry(newEntry);
        return $"Entry '{name}' added successfully.";
    }
}