using System.Security.Cryptography;
using System.Text;

namespace PWMan.Core.Encryption
{
    public class AES : IEncryptionStrategy
    {
        // aes-gcm defaults
        private int SaltSize { get; }
        private int NonceSize { get; }
        private int TagSize { get; }

        public AES(int saltSize = 16, int nonceSize = 12, int tagSize = 16)
        {
            SaltSize = saltSize;
            NonceSize = nonceSize;
            TagSize = tagSize;
        }

        public string Encrypt(string plaintext, string password)
        {
            Log.Debug("Encrypting...");
            
            // 1) fresh salt per encryption
            var salt = Salt.GenerateSalt(SaltSize);

            // 2) derive key from password + salt
            var key = Vault.Instance.KDF.DeriveKey(password, salt);

            // 3) fresh nonce per encryption
            var nonce = Salt.GenerateSalt(NonceSize);

            // 4) start encoding
            var pt = Encoding.UTF8.GetBytes(plaintext);
            var ct = new byte[pt.Length];
            var tag = new byte[TagSize];

            var aes = new AesGcm(key, TagSize); // utilise derived key and tagsize for integrity

            Log.Debug("Running AES encrypt...");
            aes.Encrypt(nonce, pt, ct, tag); // encrypt

            // 5) pack [salt|nonce|tag|ciphertext] to a base64 string
            Log.Debug("Packing...");
            var output = new byte[SaltSize + NonceSize + TagSize + ct.Length];
            Buffer.BlockCopy(salt,  0, output, 0,                         SaltSize);
            Buffer.BlockCopy(nonce, 0, output, SaltSize,                  NonceSize);
            Buffer.BlockCopy(tag,   0, output, SaltSize + NonceSize,      TagSize);
            Buffer.BlockCopy(ct,    0, output, SaltSize + NonceSize + TagSize, ct.Length);

            return Convert.ToBase64String(output);
        }

        public string Decrypt(string ciphertext, string password)
        {
            var input = Convert.FromBase64String(ciphertext);

            // sanity check first (there needs to be some data beyond the end of the byte lengths)
            Log.Debug("Checking ciphertext length...");
            if (input.Length < SaltSize + NonceSize + TagSize) {
                throw new CryptographicException("Ciphertext too short.");
            };

            // 1) unpack [salt|nonce|tag|ciphertext]
            Log.Debug("Unpacking...");
            var salt  = new byte[SaltSize];
            var nonce = new byte[NonceSize];
            var tag   = new byte[TagSize];

            // derive from the byte array
            Buffer.BlockCopy(input, 0,                         salt,  0, SaltSize); 
            Buffer.BlockCopy(input, SaltSize,                  nonce, 0, NonceSize);
            Buffer.BlockCopy(input, SaltSize + NonceSize,      tag,   0, TagSize);

            // ciphertext length is the total length of the incoming encrypted byte length subtracting the size of salt, nonce and tag 
            var ctLen = input.Length - (SaltSize + NonceSize + TagSize);
            var ct = new byte[ctLen];
            Buffer.BlockCopy(input,     SaltSize + NonceSize + TagSize,     ct, 0, ctLen); // extract the actual encrypted data

            // 2) re-derive key
            var key = Vault.Instance.KDF.DeriveKey(password, salt);

            // 3) decrypt
            var pt = new byte[ct.Length]; // plaintext same size as ciphertext
            try
            {
                var aes = new AesGcm(key, TagSize);
                Log.Debug("Running AES decrypt...");
                aes.Decrypt(nonce, ct, tag, pt); // try decrypt
            }
            catch (CryptographicException)
            {
                // wrong password
                throw new UnauthorizedAccessException("Decryption failed.");
            }
            return Encoding.UTF8.GetString(pt); // decrypted data
        }
    }
}
