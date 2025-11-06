using PWMan.CLI;
using PWMan.Commands;

namespace PWMan.Core;

public class AddWifiEntryCommand : AddEntryCommand
{
    public override string Execute(string[] args)
    {
        string name = GetDefaultValidate.GetStringRequired("Entry Name: ");
        string ssid = GetDefaultValidate.GetStringRequired("SSID: ");
        string password = PasswordReader.Get("Enter password: ");
        string security = GetDefaultValidate.GetString("Security Type (WPA, WPA2, WPA3, WEP, Open, Other): ", "other");
        string notes = GetDefaultValidate.GetString("Notes: ", "");
        
        Enum.TryParse(security.ToLower(), out SecurityType securityType); // https://stackoverflow.com/questions/16100/convert-a-string-to-an-enum-in-c-sharp

        WifiEntry newEntry = new WifiEntry(name, ssid, securityType)
        {
            Password = password,
            Notes = notes
        };

        Vault.Instance.AddEntry(newEntry);
        return $"WiFi Entry '{name}' added successfully.";
    }
}