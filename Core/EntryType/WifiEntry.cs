namespace PWMan.Core;

public class WifiEntry : Entry
{
    public WifiEntry(string title, string ssid, SecurityType securityType, string password = "", string notes = "") : base(title, notes)
    {
        Ssid = ssid;
        Password = password; // can be empty for open networks
        SecurityType = securityType;
        EntryType = EntryType.Wifi;
    }

    public string Ssid { get; set; }
    public SecurityType SecurityType { get; set; }
}