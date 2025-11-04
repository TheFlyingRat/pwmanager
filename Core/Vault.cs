// what this does is store instance state whether the vault is actually
// unlocked or locked right now as well as a decrypted-password cache
// so we don't need to constantly decrypt if we're within the same
// password manager session.
using System.Text;
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
        Repository = new JsonEntryRepository("vault.json");
        Encryption = new Caesar();
        KDF = new Argon2Derivation();
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
        }

        return target == hash;
    }

    // locks vault and clears self cache to prevent previously decrypted passwords remaining in mem
    public void Lock()
    {
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

    public static bool Exists { get { return _instance != null; } }
}
