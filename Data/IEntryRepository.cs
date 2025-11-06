using PWMan.Core;

namespace PWMan.Data;

public interface IEntryRepository
{
    Entry? GetEntry(Guid id);
    void Create();
    void LoadVault();
    void SaveVault();
    VaultMetadata ReadMetadata();
}
