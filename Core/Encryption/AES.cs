using PWMan.Core.KeyDerivation;
using System.Security.Cryptography;
using System.Text;

namespace PWMan.Core.Encryption
{
    public class AES : IEncryptionStrategy
    {
        // aes-gcm standards
        private int SaltSize { get; set; }
        private int NonceSize { get; set; }
        private int TagSize { get; set; }
        private readonly Pbkdf2Derivation KDF;

        public AES(int saltSize = 16, int nonceSize = 12, int tagSize = 16, int KDFIterations = 100_000, int KDFKeySize = 32)
        {
            SaltSize = saltSize;
            NonceSize = nonceSize;
            TagSize = tagSize;
            KDF = new Pbkdf2Derivation(KDFIterations, KDFKeySize);
        }
        

        public string Encrypt(string plaintext, string password)
        {
            // 1. derive key from password + random salt
            byte[] salt = new byte[SaltSize];
            RandomNumberGenerator.Fill(salt);
            byte[] key = KDF.DeriveKey(password, salt);

            // 2. make random nonce (randomness)
            byte[] nonce = new byte[NonceSize];
            RandomNumberGenerator.Fill(nonce);

            // 3. encrypt
            byte[] plainBytes = Encoding.UTF8.GetBytes(plaintext); // convert data to bytes
            byte[] cipherBytes = new byte[plainBytes.Length];
            byte[] tag = new byte[TagSize];
            var aes = new AesGcm(key, TagSize);
            aes.Encrypt(nonce, plainBytes, cipherBytes, tag); // encrypt the plainBytes into cipherBytes with tag for integrity, nonce for randomness


            // 4. package to a base64 string: [salt | nonce | tag | ciphertext]

            // ============== offset the blocks correctly into the byte array (AI helped with this idea)
            byte[] output = new byte[SaltSize + NonceSize + TagSize + cipherBytes.Length];
            Buffer.BlockCopy(salt,        0, output, 0,                              SaltSize);
            Buffer.BlockCopy(nonce,       0, output, SaltSize,                       NonceSize);
            Buffer.BlockCopy(tag,         0, output, SaltSize + NonceSize,           TagSize);
            Buffer.BlockCopy(cipherBytes, 0, output, SaltSize + NonceSize + TagSize, cipherBytes.Length);

            // salt start offset 0, into the output, 0 bytes from the start of the destination array, for the length of SaltSize

            return Convert.ToBase64String(output);

            // ==============
        }

        public string Decrypt(string ciphertext, string password)
        {

            // 1. unpack base64: [salt | nonce | tag | ciphertext] (assumes the encryption used same size for decryption - shouldn't change.)
            // TODO: config file!!
            byte[] input = Convert.FromBase64String(ciphertext);
            byte[] salt  = new byte[SaltSize];
            byte[] nonce = new byte[NonceSize];
            byte[] tag = new byte[TagSize];

            // unpack from input to respective byte arrays from the offset as the second param
            // TODO: something to talk about in presentation 
            Buffer.BlockCopy(input, 0,                         salt,  0, SaltSize);
            Buffer.BlockCopy(input, SaltSize,                  nonce, 0, NonceSize);
            Buffer.BlockCopy(input, SaltSize + NonceSize,      tag,   0, TagSize);

            // calculate message length
            int cipherLen = input.Length - (SaltSize + NonceSize + TagSize);
            byte[] cipherBytes = new byte[cipherLen];

            // copy input starting from the cipher offset to cipherBytes length into cypherBytes array
            Buffer.BlockCopy(input,   SaltSize + NonceSize + TagSize,   cipherBytes, 0, cipherLen);

            // 2. re-derive key for decryption
            byte[] key = KDF.DeriveKey(password, salt);

            // 3. try decrypt
            byte[] plainBytes = new byte[cipherBytes.Length]; // decrypted data will be same length as cipher
            var aes = new AesGcm(key, TagSize);
            aes.Decrypt(nonce, cipherBytes, tag, plainBytes);

            return Encoding.UTF8.GetString(plainBytes);
        }
    }
}
