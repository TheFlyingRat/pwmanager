// what this does is store instance state whether the vault is actually
// unlocked or locked right now as well as a decrypted-password cache
// so we don't need to constantly decrypt if we're within the same
// password manager session.
using PWMan.Core.Encryption;
using PWMan.Core.KeyDerivation;
using PWMan.Data;

namespace PWMan.Core;

public class Vault
{
    // singleton pattern, we only have one instance
    private static Vault? _instance = new Vault(); // eagerly create the instance
    private List<Entry> _entries = new List<Entry>();
    public static IEntryRepository? Repository { get; private set; }
    public static IEncryptionStrategy? Encryption { get; private set; }
    public static IKeyDerivation? KDF { get; private set; }

    // cant create a vault from outside of the actual class as constructor is private
    private Vault()
    {
        Encryption = new Caesar();
        KDF = new Argon2Derivation();
        Repository = new JsonEntryRepository("vault.json", Encryption, "joey");
        // TODO: check if repository comes back as not existing, and if so, create a new one with new keyfile
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
    public bool IsLocked { get; private set; } = true; // default to locked state only allow unlocking by self masterkey
    public string? MasterKey { get; private set; } // cached masterkey when unlocked (kdfed)


    // unlock vault with a given hash
    public bool Unlock(string hash)
    {
        // TODO: determine password kdf so we know what derivation to use here target is stored in keyfile for now but will be in repo.
        string target = KeyFile.ReadFromFile("keyfile.txt");

        if (target == hash) // replace with actual check
        {
            MasterKey = hash; // cache it on the vault instance
            IsLocked = false;

            // load entries from repository into memory
            _entries = Repository.GetAllEntries();
            Console.WriteLine($"Loaded {_entries.Count} entries from repository.");
            return true;
        }

        return false;
    }

    // locks vault and clears self cache to prevent previously decrypted passwords remaining in mem
    public void Lock()
    {
        if (IsLocked)
        {
            throw new InvalidOperationException("Vault is already locked.");
        }
        MasterKey = null;
        IsLocked = true;

        // clear entries from memory as well 
        Repository.SaveAllEntries(_entries); // save before clearing
        _entries.Clear();
    }

    public void AddEntry(Entry entry)
    {
        if (IsLocked)
        {
            throw new InvalidOperationException("Vault is locked. Cannot add entry.");
        }

        _entries.Add(entry);
        Repository.SaveEntry(entry);
    }

    public Entry GetEntry(Guid id)
    {
        if (IsLocked)
        {
            throw new InvalidOperationException("Vault is locked. Cannot get entry.");
        }

        return _entries.FirstOrDefault(e => e.Id == id) ?? throw new KeyNotFoundException($"Entry with ID '{id}' not found.");
    }

    public Entry SearchEntries(string title)
    {
        if (IsLocked)
        {
            throw new InvalidOperationException("Vault is locked. Cannot search entries.");
        }

        return _entries.FirstOrDefault(e => e.Title.Equals(title.ToLower())) ?? throw new KeyNotFoundException($"Entry with title '{title}' not found.");
    }

    public List<Entry> ListEntries()
    {
        if (IsLocked)
        {
            throw new InvalidOperationException("Vault is locked. Cannot get entries.");
        }

        return _entries;
    }

    public static bool Exists { get { return _instance != null; } }
}
