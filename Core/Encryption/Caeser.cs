// caesar encryption is one of the oldest methods of encryption
// it requires an offset (the key) and works by "shifting" the alphabet
// down by this offset such that a --> d (offset of d) and so on. 

namespace PWMan.Core.Encryption;

public class Caesar : IEncryptionStrategy
{
    public string Encrypt(string plain, string key)
    {
        return new string(plain.Select(c => (char)(c + 3)).ToArray());
    }

    public string Decrypt(string cipher, string key)
    {
        return new string(cipher.Select(c => (char)(c - 3)).ToArray());
    }
}
