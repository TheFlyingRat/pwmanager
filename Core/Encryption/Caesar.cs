// caesar encryption is one of the oldest methods of encryption
// it requires an offset (the key) and works by "shifting" the alphabet
// down by this offset such that a --> d (offset of d) and so on. 

using PWMan.Core.KeyDerivation;

namespace PWMan.Core.Encryption;

public class Caesar : IEncryptionStrategy
{
    public string Encrypt(string plain, string key, IKeyDerivation KDF)
    {
        // sum of bytes
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(key);
        int offset = bytes.Sum(b => b); // equal to a for loop

        return new string(plain.Select(c => (char)(c + offset)).ToArray());
    }

    public string Decrypt(string cipher, string key, IKeyDerivation KDF)
    {
        // sum of bytes
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(key);
        int offset = bytes.Sum(b => b); // equal to a for loop

        return new string(cipher.Select(c => (char)(c - offset)).ToArray());
    }
}
