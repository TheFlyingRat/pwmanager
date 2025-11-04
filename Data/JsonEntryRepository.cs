using System.Text.Json;
using PWMan.Core;
using PWMan.Core.Encryption;

namespace PWMan.Data;

public class JsonEntryRepository : IEntryRepository
{
    private readonly string _filePath;
    private readonly IEncryptionStrategy _encryption;
    private readonly string _key;

    public JsonEntryRepository(string filePath, IEncryptionStrategy encryption, string key)
    {
        _filePath = filePath;
        _encryption = encryption;
        _key = key;
    }

    private bool Exists()
    {
        if (!File.Exists(_filePath))
        {
            Create();
            return false;
        }
        return true;
    }

    public List<Entry> GetAllEntries()
    {
        if (!Exists())
        {
            return new List<Entry>();
        }

        try
        {
            var json = _encryption.Decrypt(File.ReadAllText(_filePath), _key);
            return JsonSerializer.Deserialize<List<Entry>>(json) ?? new List<Entry>();
        } catch (JsonException) {
            // TODO (valid path but no data): handle case where file is corrupted or not valid json after decryption
            // also bad decryption key could lead to this
            Console.WriteLine("Failed to decrypt! Empty vault returned.");
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
        var encryptedJson = _encryption.Encrypt(json, _key);
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
        var encryptedJson = _encryption.Encrypt(json, _key);
        File.WriteAllText(_filePath, encryptedJson);
    }

    public void SaveAllEntries(List<Entry> entries)
    {
        var json = JsonSerializer.Serialize(entries);
        var encryptedJson = _encryption.Encrypt(json, _key);
        File.WriteAllText(_filePath, encryptedJson);
    }

    public void Create()
    {
        var emptyList = new List<Entry>();
        var json = JsonSerializer.Serialize(emptyList);
        var encryptedJson = _encryption.Encrypt(json, _key);
        File.WriteAllText(_filePath, encryptedJson);
    }
}
