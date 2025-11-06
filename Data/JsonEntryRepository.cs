using System.Text.Json;
using System.Text.Json.Serialization;
using PWMan.Core;
using PWMan.Core.Encryption;
using PWMan.Core.KeyDerivation;

namespace PWMan.Data;

public class JsonEntryRepository : IEntryRepository
{
    private readonly string _filePath;
    private readonly IEncryptionStrategy _encryption;

    private class VaultEnvelope
    {
        public VaultMetadata Metadata { get; set; } = new VaultMetadata();
        public string Entries { get; set; } = "";
    }


    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        // store enums as strings
        Converters = { new JsonStringEnumConverter(allowIntegerValues: true) }
    };

    public JsonEntryRepository(string filePath, IEncryptionStrategy encryption)
    {
        _filePath = filePath;
        _encryption = encryption;
    }

    // ctor for probe
    public JsonEntryRepository(string filePath) : this(filePath, new AES()) { }


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

    public List<Entry> LoadVault(string password, IKeyDerivation KDF)
    {
        if (!File.Exists(_filePath)) { throw new FileNotFoundException("Vault doesn't exist."); }

        Log.Debug("Loading vault...");
        string json = File.ReadAllText(_filePath);

        VaultEnvelope env = JsonSerializer.Deserialize<VaultEnvelope>(json, JsonOptions) ?? throw new InvalidDataException("Malformed vault file.");
        if (env.Metadata == null) { throw new InvalidDataException("Missing metadata."); }

        string entriesJson;
        try
        {
            entriesJson = _encryption.Decrypt(env.Entries, password, KDF);
        }
        catch
        {
            throw new UnauthorizedAccessException("Failed to decrypt! Wrong password.");
        }

        Log.Debug("Deserializing decrypted entries...");

        List<Entry> readEntries = EntrySerializer.DeserializeEntries(entriesJson) ?? throw new UnauthorizedAccessException("Failed to parse JSON! Maybe the password is wrong.");
        return readEntries;
    }

    public void SaveVault(List<Entry> entries, VaultMetadata metadata, string password, IKeyDerivation KDF)
    {
        string entriesJson = EntrySerializer.Serialize(entries);
        string encrypted = _encryption.Encrypt(entriesJson, password, KDF);

        var env = new VaultEnvelope
        {
            Metadata = metadata,
            Entries = encrypted
        };

        File.WriteAllText(_filePath, JsonSerializer.Serialize(env, JsonOptions));
    }

    public void Create(VaultMetadata metadata, string password, IKeyDerivation KDF)
    {
        SaveVault(new List<Entry>(), metadata, password, KDF);
    }
}
