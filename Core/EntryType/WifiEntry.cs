namespace PWMan.Core;

public class WifiEntry : Entry
{
    public WifiEntry(string title, string ssid, string password, SecurityType securityType, string notes = "") : base(title, notes)
    {
        Ssid = ssid;
        Password = password;
        SecurityType = securityType;
        EntryType = EntryType.Wifi;
    }

    public string Ssid { get; set; }
    public SecurityType SecurityType { get; set; }
}