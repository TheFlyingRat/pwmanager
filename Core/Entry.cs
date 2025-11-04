// entry

namespace PWMan.Core;

public enum EntryType
{
    Generic,
    Wifi,
    SecureNote
}

public class Entry : IdentifiableObject
{
    public Entry(string title, string username = "", string password = "", string notes = "", string url = "")
    {
        Id = Guid.NewGuid();
        Title = title;
        Username = username;
        Password = password;
        Notes = notes;
        Url = url;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        EntryType = EntryType.Generic;
    }

    public string Title { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Notes { get; set; }
    public string Url { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public EntryType EntryType { get; set; }
}
