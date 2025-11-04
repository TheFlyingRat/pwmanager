using System.Text.Json;
using PWMan.Core;
using PWMan.Core.Encryption;

namespace PWMan.Data;

public class JsonEntryRepository : IEntryRepository
{
    private readonly string _filePath;
    private readonly IEncryptionStrategy _encryption;

    public JsonEntryRepository(string filePath, IEncryptionStrategy encryption)
    {
        _filePath = filePath;
        _encryption = encryption;
    }

    public List<Entry> GetAllEntries()
    {
        if (!File.Exists(_filePath))
        {
            return new List<Entry>();
        }

        try
        {
            var json = _encryption.Decrypt(File.ReadAllText(_filePath), "not used in caesar!");
            return JsonSerializer.Deserialize<List<Entry>>(json) ?? new List<Entry>();
        } catch (System.Text.Json.JsonException) {
            // TODO (valid path but no data): handle case where file is corrupted or not valid json after decryption
            return new List<Entry>();
        }
    }

    public void SaveEntry(Entry entry) // used for edits as well
    {
        var entries = GetAllEntries();
        var existingEntryIndex = entries.FindIndex(e => e.Id == entry.Id); // lambda

        if (existingEntryIndex >= 0)
        {
            entries[existingEntryIndex] = entry; // Update existing entry
        }
        else
        {
            entries.Add(entry); // Add new entry
        }
        var json = JsonSerializer.Serialize(entries);
        var encryptedJson = _encryption.Encrypt(json, "not used in caesar!");
        File.WriteAllText(_filePath, encryptedJson);
    }

    public Entry? GetEntry(Guid id)
    {
        var entries = GetAllEntries();
        return entries.Find(e => e.Id == id); // lambda
    }

    public void DeleteEntry(Guid id)
    {
        var entries = GetAllEntries();
        entries.RemoveAll(e => e.Id == id); // lambda

        var json = JsonSerializer.Serialize(entries);
        var encryptedJson = _encryption.Encrypt(json, "not used in caesar!");
        File.WriteAllText(_filePath, encryptedJson);
    }

    public void SaveAllEntries(List<Entry> entries)
    {
        var json = JsonSerializer.Serialize(entries);
        var encryptedJson = _encryption.Encrypt(json, "not used in caesar!");
        File.WriteAllText(_filePath, encryptedJson);
    }

    public void Create()
    {
        if (!File.Exists(_filePath))
        {
            var emptyList = new List<Entry>();
            var json = JsonSerializer.Serialize(emptyList);
            var encryptedJson = _encryption.Encrypt(json, "not used in caesar!");
            File.WriteAllText(_filePath, encryptedJson);
        }
    }
}
