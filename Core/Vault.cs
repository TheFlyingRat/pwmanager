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
    private static Vault? _instance;
    private List<Entry> _entries = new List<Entry>();
    private static IEntryRepository? _repo;
    private static IEncryptionStrategy? _enc;
    private static IKeyDerivation? _kdf; 

    // cant create a vault from outside of the actual class as constructor is private
    private Vault()
    {
        _repo = new JsonEntryRepository("vault.json");
        _enc = new Caesar();
        _kdf = new Argon2Derivation();
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
    public byte[]? MasterKey { get; private set; } // cached masterkey when unlocked (kdfed)




    // unlock vault with a given masterkey
    public void Unlock(string masterKey)
    {
 
        string target = KeyFile.ReadFromFile("keyfile.txt");

        byte[] salt = Convert.FromBase64String(target.Split('?')[1]);

        byte[] key = _kdf.DeriveKey(masterKey ?? "", salt); // derive the key to cache

        string hash = Convert.ToBase64String(key) + "?" + Convert.ToBase64String(salt);

        Console.WriteLine("Full hash is: " + hash);


        // TODO: determine password kdf so we know what derivation to use here

        if (target == hash) // replace with actual check
        {
            MasterKey = key; // cache it on the vault instance
            IsLocked = false;
            Console.WriteLine("Unlocked.");
        }
        else
        {
            Console.WriteLine("Rejected.");
        }
    }

    // locks vault and clears self cache to prevent previously decrypted passwords remaining in mem
    public void Lock()
    {
        MasterKey = null;
        IsLocked = true;

        // clear entries from memory as well 
        _repo.SaveAllEntries(_entries); // save before clearing
        _entries.Clear();
    }
}
