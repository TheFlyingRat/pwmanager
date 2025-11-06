using PWMan.Core.Encryption;
using PWMan.Core.KeyDerivation;
using PWMan.Data;

namespace PWMan.Core;

public class Vault
{
    // singleton pattern, we only have one instance
    private static Vault? _instance; // = new Vault(); // eagerly create the instance


    // determinable and required (entries can be empty)
    public bool IsLocked { get; private set; } = true;
    public static bool Exists { get { return _instance != null; } } // static because only one globally
    public List<Entry> _entries;


     // determinable at load and on creation
    public VaultMetadata? _metadata;
    public IEntryRepository? Repository { get; set; }
    public IEncryptionStrategy? Encryption { get; set; }
    public IKeyDerivation? KDF { get; set; }
    public string? RuntimePassword { get; set; }

    // cant create a vault from outside of the actual class as constructor is private
    private Vault()
    {
        _entries = new List<Entry>();
        _metadata = new VaultMetadata();
    }

    // accessor for our singleton instance
    public static Vault Instance
    {
        get
        {
            // create a new vault if the vault instance doesn't exist
            if (_instance == null)
            {
                _instance = new Vault();
            }
            return _instance;
        }
    }


    public void Destroy()
    {
        _instance = null;
    }

    // unlock vault with a given password
    public bool Unlock(string password)
    {
        if (Repository == null) { throw new InvalidOperationException("No repository."); }

        Log.Debug("Attempting to unlock vault...");

        RuntimePassword = password; // set runtimepassword to the given password - it'll be used for encryption
        try
        {
            _entries = Repository.LoadVault(RuntimePassword, KDF); // does the decrypting
            IsLocked = false;
            return true;
        }
        catch (UnauthorizedAccessException ex)
        {
            RuntimePassword = null;
            _metadata = new VaultMetadata(); // prevent stale metadata
            throw new UnauthorizedAccessException(ex.Message);
        }
        catch (Exception ex) // TODO: determinable exception types
        {
            RuntimePassword = null;
            _metadata = new VaultMetadata();
            throw new InvalidDataException(ex.Message);
        }
    }

    // locks vault and clears self cache to prevent previously decrypted passwords remaining in mem
    public void Lock()
    {
        if (IsLocked) { throw new InvalidOperationException("Vault is already locked."); }

        Log.Debug("Attempting to lock vault...");

        SaveAll();
        RuntimePassword = null;
        _entries.Clear(); // clear entries from memory as well after saving
        IsLocked = true;
    }





    // CRUD
    public void AddEntry(Entry entry)
    {
        if (IsLocked) { throw new InvalidOperationException("Vault is locked. Cannot add entry."); }

        _entries.Add(entry);
        SaveAll();
    }

    public Entry? GetEntry(Guid id)
    {
        if (IsLocked) { throw new InvalidOperationException("Vault is locked. Cannot get entry."); }

        return _entries.Find(e => e.Id == id);
    }

    public void UpdateEntry(Guid id, Entry entry)
    {
        if (IsLocked) { throw new InvalidOperationException("Vault is locked. Cannot update entry."); }

        int index = _entries.FindIndex(e => e.Id == id);
        if (index == -1) { throw new KeyNotFoundException($"Entry with ID {id} not found."); }

        _entries[index] = entry; // update in existing list
        SaveAll();
    }

    public void DeleteEntry(Guid id)
    {
        if (IsLocked) { throw new InvalidOperationException("Vault is locked. Cannot delete entry."); }

        _entries.RemoveAll(entry => entry.Id == id); // should only be one but can use lambda
        SaveAll();
    }







    public List<Entry> SearchEntries(string query)
    {
        if (IsLocked) { throw new InvalidOperationException("Vault is locked. Cannot search entries."); }

        return _entries
            .Where(entry => entry.Title.ToLower().Contains(query)) // fuzzy search using contains (will match single letters though which is fine i suppose???)
            .ToList();
    }

    public List<Entry> ListEntries()
    {
        if (IsLocked) { throw new InvalidOperationException("Vault is locked. Cannot get entries."); }

        return _entries;
    }

    public void SaveAll()
    {
        if (IsLocked) { throw new InvalidOperationException("Vault is locked. Cannot save entries."); }

        Repository.SaveVault(_entries, _metadata, RuntimePassword, KDF); // utilises vault's mem
    }
}
