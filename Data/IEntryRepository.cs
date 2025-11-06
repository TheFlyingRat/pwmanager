using PWMan.Core;
using PWMan.Core.KeyDerivation;

namespace PWMan.Data;

public interface IEntryRepository
{
    void Create(VaultMetadata metadata, string password, IKeyDerivation KDF);
    List<Entry> LoadVault(string password, IKeyDerivation KDF);
    void SaveVault(List<Entry> entries, VaultMetadata metadata, string password, IKeyDerivation KDF);
    VaultMetadata ReadMetadata();
}
