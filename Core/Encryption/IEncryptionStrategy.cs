// the encryption strategy (design pattern!!) will require two methods
// anything that implements encryption strategy requires both encrypt and decrypt 

namespace PWMan.Core.Encryption;

public interface IEncryptionStrategy
{
    string Encrypt(string plain, string key);
    string Decrypt(string cipher, string key);
}
