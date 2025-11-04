namespace PWMan.Core;

public class SecureNote : Entry
{
    public SecureNote(string title, string content, string notes = "") : base(title, notes)
    {
        Content = content;
        EntryType = EntryType.SecureNote;
    }

    public string Content { get; set; }
}