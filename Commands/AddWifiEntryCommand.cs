using PWMan.Commands;

namespace PWMan.Core;

public class AddWifiEntryCommand : AddEntryCommand
{
    public override string Name { get; protected set; } = "new wifi";

    public override string Execute(string[] args)
    {
        if (Vault.Instance.IsLocked)
        {
            return "Vault is locked. Please unlock it first.";
        }

        Console.Write("Entry Name: ");
        string name = Console.ReadLine() ?? "";

        Console.Write("SSID: ");
        string ssid = Console.ReadLine() ?? "";

        Console.Write("Password: ");
        string password = Console.ReadLine() ?? "";

        Console.Write("Security Type (WPA, WPA2, WPA3, WEP, Open, Other): ");
        string security = Console.ReadLine() ?? "";

        Enum.TryParse(security.ToLower(), out SecurityType securityType); // https://stackoverflow.com/questions/16100/convert-a-string-to-an-enum-in-c-sharp
        
        Console.Write("Notes: ");
        string notes = Console.ReadLine() ?? "";

        WifiEntry newEntry = new WifiEntry(name, ssid, securityType)
        {
            Password = password,
            Notes = notes
        };

        SaveEntry(newEntry);
        return $"WiFi Entry '{name}' added successfully.";
    }
}