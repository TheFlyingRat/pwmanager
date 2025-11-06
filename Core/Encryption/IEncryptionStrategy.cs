// the encryption strategy (design pattern!!) will require two methods
// anything that implements encryption strategy requires both encrypt and decrypt 

using PWMan.Core.KeyDerivation;

namespace PWMan.Core.Encryption;

public interface IEncryptionStrategy
{
    string Encrypt(string plain, string key, IKeyDerivation KDF);
    string Decrypt(string cipher, string key, IKeyDerivation KDF);
}
