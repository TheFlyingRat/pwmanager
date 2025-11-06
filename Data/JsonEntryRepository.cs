using System.Text.Json;
using System.Text.Json.Serialization;
using PWMan.Core;
using PWMan.Core.Encryption;

namespace PWMan.Data;

public class JsonEntryRepository : IEntryRepository
{
    private readonly string _filePath;
    private readonly IEncryptionStrategy? _encryption;

    private class VaultEnvelope
    {
        public VaultMetadata? Metadata { get; set; }
        public string Entries { get; set; } = "";
    }


    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        // store enums as strings
        Converters = { new JsonStringEnumConverter(allowIntegerValues: true) }
    };

    public JsonEntryRepository(string filePath, IEncryptionStrategy? encryption)
    {
        _filePath = filePath;
        _encryption = encryption;
    }


    // api


    public VaultMetadata ReadMetadata()
    {
        Log.Debug("Reading existing vault metadata...");
        if (!File.Exists(_filePath)) { throw new FileNotFoundException("Vault file not found."); };

        string json = File.ReadAllText(_filePath);

        Log.Debug("Unpacking metadata...");
        VaultEnvelope env = JsonSerializer.Deserialize<VaultEnvelope>(json, JsonOptions) ?? throw new InvalidDataException("Malformed metadata in vault file.");

        if (env.Metadata == null) { throw new InvalidDataException("Missing metadata."); };

        env.Metadata.SaveFile = _filePath;
        return env.Metadata;
    }

    public void LoadVault()
    {
        if (!File.Exists(_filePath)) { throw new FileNotFoundException("Vault doesn't exist."); }

        if (_encryption == null) { throw new InvalidOperationException("Encryption not configured."); } // only required for decrypting, not reading metadata

        Log.Debug("Loading vault...");
        string json = File.ReadAllText(_filePath);

        VaultEnvelope env = JsonSerializer.Deserialize<VaultEnvelope>(json, JsonOptions) ?? throw new InvalidDataException("Malformed vault file.");
        if (env.Metadata == null) { throw new InvalidDataException("Missing metadata."); }
        
        Log.Debug("Setting vault metadata...");
        Vault.Instance._metadata = env.Metadata;

        string entriesJson;
        try
        {
            entriesJson = _encryption.Decrypt(env.Entries, Vault.Instance.RuntimePassword);
        }
        catch
        {
            throw new UnauthorizedAccessException("Failed to decrypt! Wrong password.");
        }

        Log.Debug("Deserializing decrypted entries...");
        Vault.Instance._entries = DeserializeEntries(entriesJson);
    }

    public void SaveVault()
    {
        Persist(Vault.Instance.ListEntries());
    }







    public Entry? GetEntry(Guid id)
    {
        var entries = Vault.Instance.ListEntries();
        return entries.Find(e => e.Id == id); // lambda
    }

    public void Create()
    {
        Persist(new List<Entry>()); // save to disk
        Vault.Instance._entries = new List<Entry>(); // initialise empty
    }






    // writes to the save file
    private void Persist(List<Entry> entries)
    {
        if (_encryption == null) { throw new InvalidOperationException("Encryption not configured."); }

        Log.Debug("Serializing entries...");
        string entriesJson = JsonSerializer.Serialize(entries, JsonOptions);
        string encrypted = _encryption.Encrypt(entriesJson, Vault.Instance.RuntimePassword);

        Log.Debug("Saving to file...");
        var env = new VaultEnvelope
        {
            Metadata = Vault.Instance._metadata,
            Entries = encrypted
        };

        var fileJson = JsonSerializer.Serialize(env, JsonOptions);
        File.WriteAllText(_filePath, fileJson);
    }

    // types of entry
    private List<Entry> DeserializeEntries(string json)
    {
        var elements = JsonSerializer.Deserialize<List<JsonElement>>(json, JsonOptions);
        var entries = new List<Entry>();

        foreach (var element in elements)
        {
            // Get EntryType as a string
            string typeName = element.GetProperty("EntryType").GetString();
            EntryType entryType = Enum.Parse<EntryType>(typeName); // convert type enum name to the enum value

            Entry entry;

            switch (entryType)
            {
                case EntryType.SecureNote:
                    entry = JsonSerializer.Deserialize<SecureNote>(element.GetRawText(), JsonOptions);
                    break;

                case EntryType.Wifi:
                    entry = JsonSerializer.Deserialize<WifiEntry>(element.GetRawText(), JsonOptions);
                    break;

                default:
                    entry = JsonSerializer.Deserialize<Entry>(element.GetRawText(), JsonOptions);
                    break;
            }

            entries.Add(entry);
        }

        return entries;
    }
}
