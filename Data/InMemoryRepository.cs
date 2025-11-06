// the point of this is so that we can do rapid development without having
// a persistent database since there really is no need for that

using PWMan.Core;
using PWMan.Core.Encryption;
using PWMan.Core.KeyDerivation;

namespace PWMan.Data;

// well i mean i could just store json data.. how else would i manage to serialize the entire vault list<entry> obj?

public class InMemoryRepository : IEntryRepository
{
    private string _data = ""; // b64 string of the encrypted data
    private VaultMetadata _metadata = new VaultMetadata();
    private readonly IEncryptionStrategy _encryption;

    public InMemoryRepository(IEncryptionStrategy enc)
    {
        _encryption = enc;
    }

    public void Create(VaultMetadata metadata, string password, IKeyDerivation KDF)
    {
        _metadata = metadata;
        string json = EntrySerializer.Serialize(new List<Entry>());
        _data = _encryption.Encrypt(json, password, KDF);
    }

    public VaultMetadata ReadMetadata()
    {
        return _metadata;
    }

    public void SaveVault(List<Entry> entries, VaultMetadata metadata, string password, IKeyDerivation KDF)
    {
        _metadata = metadata;
        string json = EntrySerializer.Serialize(entries);
        _data = _encryption.Encrypt(json, password, KDF);
    }

    public List<Entry> LoadVault(string password, IKeyDerivation KDF)
    {
        if (string.IsNullOrEmpty(_data))
        {
            return new List<Entry>();
        }


        string decrypted = _encryption.Decrypt(_data, password, KDF);

        List<Entry> readEntries = EntrySerializer.DeserializeEntries(decrypted) ?? throw new UnauthorizedAccessException("Failed to parse JSON! Maybe the password is wrong.");
        return readEntries;
    }
}